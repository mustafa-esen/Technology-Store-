import { AuthResponse, User } from "@/types";

const TOKEN_KEY = "token";
const USER_ID_KEY = "userId";
const USER_EMAIL_KEY = "userEmail";
const REFRESH_TOKEN_KEY = "refreshToken";

// ALWAYS LOGGED IN USER
const HARDCODED_USER = {
  id: "u-demo",
  email: "kaşarmusti@gmail.com",
  firstName: "Kaşar",
  lastName: "Musti",
  roles: ["Customer", "Admin"]
};

export function saveAuthData(response: AuthResponse): void {
  // No-op or log, as we are hardcoded
  console.log("Mock saving auth data", response);
}

export function clearAuthData(): void {
  // No-op in this mode, preventing logout
  console.log("Mock clear auth data (ignored)");
}

export function getToken(): string | null {
  return "mock-always-valid-token";
}

export function getUserId(): string | null {
  return HARDCODED_USER.id;
}

export function getUserEmail(): string | null {
  return HARDCODED_USER.email;
}

export function getRefreshToken(): string | null {
  return "mock-refresh-token";
}

export function isAuthenticated(): boolean {
  return true;
}

export function decodeToken(token: string): Record<string, unknown> | null {
  return {
    sub: HARDCODED_USER.id,
    email: HARDCODED_USER.email,
    role: HARDCODED_USER.roles,
    exp: Date.now() / 1000 + 3600 // Always valid for an hour
  };
}

export function isTokenExpired(token?: string | null): boolean {
  return false; // Never expires
}

export function getCurrentUser(): Partial<User> | null {
  return {
    id: HARDCODED_USER.id,
    email: HARDCODED_USER.email,
    firstName: HARDCODED_USER.firstName,
    lastName: HARDCODED_USER.lastName,
    roles: HARDCODED_USER.roles
  };
}
