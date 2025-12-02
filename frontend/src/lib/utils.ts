import { type ClassValue, clsx } from "clsx";
import { twMerge } from "tailwind-merge";
import { StatusMeta } from "@/types";

export function cn(...inputs: ClassValue[]) {
  return twMerge(clsx(inputs));
}

const ORDER_STATUS_META: Record<string, StatusMeta> = {
  Pending: { label: "Ödeme Bekleniyor", badge: "bg-amber-500/15 text-amber-400", description: "Ödeme henüz tamamlanmadı" },
  PaymentReceived: { label: "Ödeme Alındı", badge: "bg-emerald-500/15 text-emerald-400", description: "Sipariş hazırlanıyor" },
  Processing: { label: "Hazırlanıyor", badge: "bg-sky-500/15 text-sky-400", description: "Depoda hazırlanıyor" },
  Shipped: { label: "Kargoya Verildi", badge: "bg-indigo-500/15 text-indigo-300", description: "Sipariş yolda" },
  Delivered: { label: "Teslim Edildi", badge: "bg-emerald-500/15 text-emerald-300", description: "Sipariş tamamlandı" },
  Cancelled: { label: "İptal Edildi", badge: "bg-slate-500/15 text-slate-300", description: "Sipariş iptal edildi" },
  Failed: { label: "Başarısız", badge: "bg-red-500/15 text-red-400", description: "Ödeme başarısız" },
};

const PAYMENT_STATUS_META: Record<string, StatusMeta> = {
  Pending: { label: "Beklemede", badge: "bg-amber-500/15 text-amber-400", description: "Ödeme başlatılıyor" },
  Processing: { label: "İşleniyor", badge: "bg-blue-500/15 text-blue-300", description: "Banka yanıtı bekleniyor" },
  Success: { label: "Başarılı", badge: "bg-emerald-500/15 text-emerald-300", description: "Ödeme onaylandı" },
  Failed: { label: "Başarısız", badge: "bg-red-500/15 text-red-400", description: "Ödeme reddedildi" },
  Refunded: { label: "İade Edildi", badge: "bg-purple-500/15 text-purple-300", description: "Ödeme iade edildi" },
};

// Backend enum değerlerini string'e çevirir (0 -> "Pending", 1 -> "PaymentReceived" vb.)
const ORDER_STATUS_MAP: Record<number, string> = {
  0: "Pending",
  1: "PaymentReceived",
  2: "Processing",
  3: "Shipped",
  4: "Delivered",
  5: "Cancelled",
  6: "Failed",
};

/**
 * Backend'den gelen order status'u normalize eder
 * Backend hem string hem de number olarak dönebiliyor
 */
export function normalizeOrderStatus(status: string | number | undefined | null): string {
  if (status === undefined || status === null) return "Pending";
  
  // Number ise map'ten çevir
  if (typeof status === "number") {
    return ORDER_STATUS_MAP[status] ?? "Pending";
  }
  
  // String ama sayı formatında ise
  if (/^\d+$/.test(status)) {
    const asNum = Number(status);
    return ORDER_STATUS_MAP[asNum] ?? "Pending";
  }
  
  return status || "Pending";
}

export function getOrderStatusMeta(status?: string): StatusMeta {
  if (!status) return { label: "Bilinmiyor", badge: "bg-slate-500/15 text-slate-300", description: "" };
  return ORDER_STATUS_META[status] ?? { label: status, badge: "bg-slate-500/15 text-slate-300", description: "" };
}

export function getPaymentStatusMeta(status?: string): StatusMeta {
  if (!status) return { label: "Bilinmiyor", badge: "bg-slate-500/15 text-slate-300", description: "" };
  return PAYMENT_STATUS_META[status] ?? { label: status, badge: "bg-slate-500/15 text-slate-300", description: "" };
}

export function formatCurrency(value: number, currency = "TRY") {
  try {
    return new Intl.NumberFormat("tr-TR", {
      style: "currency",
      currency,
      minimumFractionDigits: 2,
    }).format(value);
  } catch {
    return `${value.toFixed(2)} ${currency}`;
  }
}
