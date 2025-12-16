import { ApiError } from "@/types";
import { AxiosError } from "axios";

/**
 * API hata mesajlarını kullanıcı dostu formata çevirir
 */
export function extractErrorMessage(error: unknown, fallback = "Bir hata oluştu. Lütfen tekrar deneyin."): string {
  if (!error) return fallback;

  // Axios error
  if (isAxiosError(error)) {
    const data = error.response?.data as ApiError | undefined;

    // FluentValidation hataları
    if (data?.errors) {
      const firstErrorKey = Object.keys(data.errors)[0];
      if (firstErrorKey && data.errors[firstErrorKey]?.[0]) {
        return data.errors[firstErrorKey][0];
      }
    }

    // Title mesajı
    if (data?.title) {
      return data.title;
    }

    // Generic message
    if (data?.message) {
      return data.message;
    }

    // HTTP status bazlı mesajlar
    const status = error.response?.status;
    if (status) {
      return getHttpStatusMessage(status);
    }

    // Network hatası
    if (error.code === "ERR_NETWORK") {
      return "Sunucuya bağlanılamadı. İnternet bağlantınızı kontrol edin.";
    }

    if (error.code === "ECONNABORTED") {
      return "İstek zaman aşımına uğradı. Lütfen tekrar deneyin.";
    }
  }

  // Standard Error
  if (error instanceof Error) {
    return error.message || fallback;
  }

  // String error
  if (typeof error === "string") {
    return error;
  }

  return fallback;
}

/**
 * Axios error type guard
 */
function isAxiosError(error: unknown): error is AxiosError {
  return (error as AxiosError)?.isAxiosError === true;
}

/**
 * HTTP status koduna göre Türkçe mesaj döner
 */
function getHttpStatusMessage(status: number): string {
  const messages: Record<number, string> = {
    400: "Geçersiz istek. Lütfen bilgileri kontrol edin.",
    401: "Oturum süreniz dolmuş. Lütfen tekrar giriş yapın.",
    403: "Bu işlem için yetkiniz bulunmuyor.",
    404: "Aradığınız kaynak bulunamadı.",
    409: "Bu kayıt zaten mevcut.",
    422: "Girilen veriler doğrulanamadı.",
    429: "Çok fazla istek gönderildi. Lütfen biraz bekleyin.",
    500: "Sunucu hatası. Lütfen daha sonra tekrar deneyin.",
    502: "Sunucu geçici olarak kullanılamıyor.",
    503: "Servis şu anda kullanılamıyor. Lütfen daha sonra deneyin.",
  };

  return messages[status] || `Hata kodu: ${status}`;
}

/**
 * Form validasyon hatalarını çıkarır
 */
export function extractValidationErrors(error: unknown): Record<string, string> {
  const result: Record<string, string> = {};

  if (isAxiosError(error)) {
    const data = error.response?.data as ApiError | undefined;
    if (data?.errors) {
      for (const [key, messages] of Object.entries(data.errors)) {
        if (messages?.[0]) {
          result[key.toLowerCase()] = messages[0];
        }
      }
    }
  }

  return result;
}

/**
 * 401 hatası mı kontrol eder
 */
export function isUnauthorizedError(error: unknown): boolean {
  if (isAxiosError(error)) {
    return error.response?.status === 401;
  }
  return false;
}

/**
 * Network hatası mı kontrol eder
 */
export function isNetworkError(error: unknown): boolean {
  if (isAxiosError(error)) {
    return error.code === "ERR_NETWORK" || !error.response;
  }
  return false;
}
