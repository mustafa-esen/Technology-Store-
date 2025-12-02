import { AuthResponse, User } from "@/types";

const TOKEN_KEY = "token";
const USER_ID_KEY = "userId";
const USER_EMAIL_KEY = "userEmail";
const REFRESH_TOKEN_KEY = "refreshToken";

/**
 * Auth bilgilerini localStorage'a kaydeder
 */
export function saveAuthData(response: AuthResponse): void {
  const token = response.token || response.accessToken;
  const userId = response.userId || response.id || response.user?.id;
  const email = response.user?.email;
  const refreshToken = response.refreshToken;

  if (token) {
    localStorage.setItem(TOKEN_KEY, token);
  }

  if (userId) {
    localStorage.setItem(USER_ID_KEY, userId);
  }

  if (email) {
    localStorage.setItem(USER_EMAIL_KEY, email);
  }

  if (refreshToken) {
    localStorage.setItem(REFRESH_TOKEN_KEY, refreshToken);
  }
}

/**
 * Tüm auth bilgilerini temizler
 */
export function clearAuthData(): void {
  localStorage.removeItem(TOKEN_KEY);
  localStorage.removeItem(USER_ID_KEY);
  localStorage.removeItem(USER_EMAIL_KEY);
  localStorage.removeItem(REFRESH_TOKEN_KEY);
}

/**
 * Access token'ı döner
 */
export function getToken(): string | null {
  if (typeof window === "undefined") return null;
  return localStorage.getItem(TOKEN_KEY);
}

/**
 * User ID'yi döner
 */
export function getUserId(): string | null {
  if (typeof window === "undefined") return null;
  return localStorage.getItem(USER_ID_KEY);
}

/**
 * User email'i döner
 */
export function getUserEmail(): string | null {
  if (typeof window === "undefined") return null;
  return localStorage.getItem(USER_EMAIL_KEY);
}

/**
 * Refresh token'ı döner
 */
export function getRefreshToken(): string | null {
  if (typeof window === "undefined") return null;
  return localStorage.getItem(REFRESH_TOKEN_KEY);
}

/**
 * Kullanıcı giriş yapmış mı kontrol eder
 */
export function isAuthenticated(): boolean {
  return !!getToken();
}

/**
 * JWT token'ı decode eder (basit implementasyon)
 */
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

/**
 * Token'ın expire olup olmadığını kontrol eder
 */
export function isTokenExpired(token?: string | null): boolean {
  const t = token || getToken();
  if (!t) return true;

  const decoded = decodeToken(t);
  if (!decoded || typeof decoded.exp !== "number") return true;

  // 30 saniye buffer ekle
  const expirationTime = decoded.exp * 1000;
  return Date.now() >= expirationTime - 30000;
}

/**
 * Mevcut kullanıcı bilgilerini döner
 */
export function getCurrentUser(): Partial<User> | null {
  const token = getToken();
  const userId = getUserId();
  const email = getUserEmail();

  if (!token || !userId) return null;

  const decoded = decodeToken(token);

  return {
    id: userId,
    email: email || (decoded?.email as string) || undefined,
    roles: decoded?.role
      ? Array.isArray(decoded.role)
        ? decoded.role
        : [decoded.role as string]
      : undefined,
  };
}
