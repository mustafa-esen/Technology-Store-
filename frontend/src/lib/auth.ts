import { AuthResponse, User } from "@/types";

const TOKEN_KEY = "token";
const USER_ID_KEY = "userId";
const USER_EMAIL_KEY = "userEmail";
const REFRESH_TOKEN_KEY = "refreshToken";

export function saveAuthData(response: AuthResponse): void {
  if (typeof window === "undefined") return;

  const token = response.token || response.accessToken;
  let userId = response.userId || response.id || response.user?.id;
  const email = response.user?.email;
  const refreshToken = response.refreshToken;

  // Decode token for nameidentifier if backend sadece JWT döndürdüyse
  if (!userId && token) {
    const decoded = decodeToken(token) as any;
    userId =
      decoded?.sub ||
      decoded?.nameid ||
      decoded?.["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier"] ||
      decoded?.["http://schemas.microsoft.com/identity/claims/objectidentifier"];
  }

  if (token) localStorage.setItem(TOKEN_KEY, token);
  if (userId) localStorage.setItem(USER_ID_KEY, userId);
  if (email) localStorage.setItem(USER_EMAIL_KEY, email);
  if (refreshToken) localStorage.setItem(REFRESH_TOKEN_KEY, refreshToken);
}

export function clearAuthData(): void {
  if (typeof window === "undefined") return;
  localStorage.removeItem(TOKEN_KEY);
  localStorage.removeItem(USER_ID_KEY);
  localStorage.removeItem(USER_EMAIL_KEY);
  localStorage.removeItem(REFRESH_TOKEN_KEY);
}

export function getToken(): string | null {
  if (typeof window === "undefined") return null;
  return localStorage.getItem(TOKEN_KEY);
}

export function getUserId(): string | null {
  if (typeof window === "undefined") return null;
  return localStorage.getItem(USER_ID_KEY);
}

export function getUserEmail(): string | null {
  if (typeof window === "undefined") return null;
  return localStorage.getItem(USER_EMAIL_KEY);
}

export function getRefreshToken(): string | null {
  if (typeof window === "undefined") return null;
  return localStorage.getItem(REFRESH_TOKEN_KEY);
}

export function isAuthenticated(): boolean {
  return !!getToken();
}

export function decodeToken(token: string): Record<string, unknown> | null {
  try {
    const base64Url = token.split(".")[1];
    if (!base64Url) return null;

    const base64 = base64Url.replace(/-/g, "+").replace(/_/g, "/");
    const jsonPayload = decodeURIComponent(
      atob(base64)
        .split("")
        .map((c) => "%" + ("00" + c.charCodeAt(0).toString(16)).slice(-2))
        .join("")
    );

    return JSON.parse(jsonPayload);
  } catch {
    return null;
  }
}

export function isTokenExpired(token?: string | null): boolean {
  const t = token || getToken();
  if (!t) return true;

  const decoded = decodeToken(t) as any;
  if (!decoded || typeof decoded.exp !== "number") return true;

  const expirationTime = decoded.exp * 1000;
  return Date.now() >= expirationTime - 30000;
}

export function getCurrentUser(): Partial<User> | null {
  const token = getToken();
  let userId = getUserId();
  const email = getUserEmail();

  if (!token || !userId) return null;

  const decoded = decodeToken(token) as any;
  if (!userId) {
    userId =
      decoded?.sub ||
      decoded?.nameid ||
      decoded?.["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier"] ||
      decoded?.["http://schemas.microsoft.com/identity/claims/objectidentifier"];
  }
  const roleClaim =
    decoded?.role ||
    decoded?.["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"] ||
    decoded?.["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/role"];
  const roles = roleClaim
    ? Array.isArray(roleClaim)
      ? roleClaim
      : [roleClaim as string]
    : undefined;

  return {
    id: userId,
    email: email || (decoded?.email as string) || undefined,
    roles,
  };
}
