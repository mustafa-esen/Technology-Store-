"use client";

import { useEffect, useState } from "react";
import Link from "next/link";
import { Loader2, Package, RefreshCcw, ShoppingBag, RotateCcw } from "lucide-react";
import { OrderService } from "@/services/api";
import { formatCurrency, getOrderStatusMeta, normalizeOrderStatus } from "@/lib/utils";
import { getUserId } from "@/lib/auth";
import { extractErrorMessage } from "@/lib/errors";
import { Order } from "@/types";

export default function OrdersPage() {
  const [orders, setOrders] = useState<Order[]>([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [cancelingId, setCancelingId] = useState<string | null>(null);
  const [cancelError, setCancelError] = useState<string | null>(null);
  const userId = getUserId() || "";

  const loadOrders = async () => {
    if (!userId) return;
    setLoading(true);
    setError(null);
    try {
      const data = await OrderService.getUserOrders(userId);
      setOrders(data);
    } catch (err: unknown) {
      setError(extractErrorMessage(err, "Siparişler alınamadı. Lütfen tekrar dene."));
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    void loadOrders();
  }, [userId]);

  const handleCancel = async (orderId: string) => {
    setCancelError(null);
    setCancelingId(orderId);
    try {
      await OrderService.cancelOrder(orderId, "Kullanıcı iptali");
      await loadOrders();
    } catch (err: unknown) {
      setCancelError(extractErrorMessage(err, "İade/iptal işlemi başarısız oldu."));
    } finally {
      setCancelingId(null);
    }
  };

  if (!userId) {
    return (
      <div className="max-w-3xl mx-auto px-4 py-12 text-center">
        <ShoppingBag className="mx-auto h-16 w-16 text-muted-foreground mb-4" />
        <p className="text-lg text-muted-foreground mb-4">Siparişleri görmek için giriş yapmalısın.</p>
        <div className="flex justify-center gap-3">
          <Link
            href="/login"
            className="px-6 py-3 bg-primary text-primary-foreground rounded-md hover:bg-primary/90 transition-colors"
          >
            Giriş Yap
          </Link>
          <Link
            href="/register"
            className="px-6 py-3 border border-border rounded-md hover:bg-accent transition-colors"
          >
            Kayıt Ol
          </Link>
        </div>
      </div>
    );
  }

  return (
    <div className="max-w-6xl mx-auto px-4 sm:px-6 lg:px-8 py-10">
      <div className="flex items-center justify-between mb-6">
        <div>
          <h1 className="text-3xl font-bold text-foreground">Siparişlerim</h1>
          <p className="text-muted-foreground">Son siparişlerini burada takip edebilirsin.</p>
        </div>
        <button
          onClick={loadOrders}
          disabled={loading}
          className="inline-flex items-center gap-2 px-4 py-2 border border-border rounded-md hover:bg-accent transition disabled:opacity-50"
        >
          <RefreshCcw className="h-4 w-4" />
          Yenile
        </button>
      </div>

      {error && <p className="text-red-500 mb-4">{error}</p>}
      {cancelError && <p className="text-red-500 mb-4">{cancelError}</p>}

      {loading ? (
        <div className="flex justify-center py-20">
          <Loader2 className="h-8 w-8 animate-spin text-primary" />
        </div>
      ) : orders.length === 0 ? (
        <div className="text-center py-20 text-muted-foreground">
          <Package className="h-14 w-14 mx-auto mb-4 opacity-50" />
          Henüz siparişin yok.
        </div>
      ) : (
        <div className="space-y-4">
          {orders.map((order) => {
            const statusMeta = getOrderStatusMeta(normalizeOrderStatus(order.status));
            const amount =
              order.totalAmount ?? order.items?.reduce((sum, item) => sum + item.price * item.quantity, 0) ?? 0;
            const created = order.createdDate ? new Date(order.createdDate).toLocaleString() : "-";
            return (
              <div key={order.id} className="border border-border rounded-xl p-4 bg-card shadow-sm">
                <div className="flex flex-col md:flex-row md:items-center md:justify-between gap-3 mb-3">
                  <div>
                    <p className="text-sm text-muted-foreground">Sipariş ID</p>
                    <p className="font-semibold text-foreground">{order.id}</p>
                  </div>
                  <div>
                    <p className="text-sm text-muted-foreground">Oluşturulma</p>
                    <p className="font-semibold text-foreground">{created}</p>
                  </div>
                  <div className="text-right">
                    <p className="text-sm text-muted-foreground">Tutar</p>
                    <p className="font-semibold text-foreground">{formatCurrency(amount)}</p>
                  </div>
                </div>

                <div className="flex flex-wrap items-center gap-3 mb-2">
                  <span className={`px-3 py-1 rounded-full text-xs font-semibold ${statusMeta.badge}`}>
                    {statusMeta.label}
                  </span>
                  {order.shippingAddress?.city && (
                    <span className="text-xs text-muted-foreground">
                      Gönderim: {order.shippingAddress.city}
                      {order.shippingAddress.country ? `, ${order.shippingAddress.country}` : ""}
                    </span>
                  )}
                </div>

                {order.items?.length ? (
                  <div className="mb-3 text-sm text-muted-foreground space-y-1">
                    <p className="font-semibold text-foreground">Ürünler</p>
                    <ul className="list-disc list-inside space-y-1">
                      {order.items.map((item) => (
                        <li key={item.productId} className="text-foreground">
                          {item.productName} — {item.quantity} adet, {formatCurrency(item.price)} / adet
                        </li>
                      ))}
                    </ul>
                  </div>
                ) : null}

                <div className="flex flex-wrap gap-3">
                  <Link
                    href="/products"
                    className="text-sm text-primary hover:text-primary/80 underline-offset-4 hover:underline"
                  >
                    Alışverişe devam et
                  </Link>
                  <Link
                    href={`/payments?orderId=${order.id}`}
                    className="text-sm text-primary hover:text-primary/80 underline-offset-4 hover:underline"
                  >
                    Ödeme detaylarını gör
                  </Link>
                  <button
                    onClick={() => handleCancel(order.id)}
                    disabled={cancelingId === order.id || loading}
                    className="inline-flex items-center gap-2 text-sm px-3 py-2 rounded-md border border-border hover:bg-accent transition disabled:opacity-50"
                  >
                    <RotateCcw className="h-4 w-4" />
                    {cancelingId === order.id ? "İşleniyor..." : "İade / İptal et"}
                  </button>
                </div>
              </div>
            );
          })}
        </div>
      )}
    </div>
  );
}
