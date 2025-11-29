"use client";

import { useEffect, useMemo, useState } from "react";
import Link from "next/link";
import { Trash2, Plus, Minus, ShoppingBag, Loader2, AlertCircle, CheckCircle2 } from "lucide-react";
import { BasketService, OrderService } from "@/services/api";

interface BasketItem {
  productId: string;
  productName: string;
  price: number;
  quantity: number;
  imageUrl?: string;
}

interface BasketResponse {
  userId: string;
  items: BasketItem[];
}

const dummyAddress = {
  street: "Test Street",
  city: "Istanbul",
  state: "Istanbul",
  country: "TR",
  zipCode: "34000",
};

export default function CartPage() {
  const [basket, setBasket] = useState<BasketResponse | null>(null);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [success, setSuccess] = useState<string | null>(null);
  const [checkoutLoading, setCheckoutLoading] = useState(false);

  // Basit kullanıcı kimliği: token yoksa login yönlendirmesi
  const userId = useMemo(() => {
    if (typeof window === "undefined") return "";
    const tokenUser = localStorage.getItem("userId");
    return tokenUser || "";
  }, []);

  const subtotal = basket?.items.reduce((sum, item) => sum + item.price * item.quantity, 0) ?? 0;
  const tax = subtotal * 0.1;
  const total = subtotal + tax;

  const loadBasket = async () => {
    if (!userId) {
      setBasket(null);
      return;
    }
    setLoading(true);
    setError(null);
    try {
      const res = await BasketService.getBasket(userId);
      setBasket(res);
    } catch (err: any) {
      // Sepet yoksa 404 gelebilir, bu durumda boş sepet göster
      if (err?.response?.status === 404) {
        setBasket({ userId, items: [] });
      } else {
        setError("Sepet alınamadı. Lütfen tekrar deneyin.");
      }
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    void loadBasket();
  }, [userId]);

  const handleQuantity = async (item: BasketItem, change: number) => {
    if (!basket || !userId) return;
    const nextQty = Math.max(1, item.quantity + change);
    setLoading(true);
    setError(null);
    try {
      await BasketService.updateItem(userId, item.productId, nextQty);
      await loadBasket();
    } catch (err: any) {
      setError("Miktar güncellenemedi.");
    } finally {
      setLoading(false);
    }
  };

  const handleRemove = async (item: BasketItem) => {
    if (!basket || !userId) return;
    setLoading(true);
    setError(null);
    try {
      await BasketService.removeItem(userId, item.productId);
      await loadBasket();
    } catch (err: any) {
      setError("Ürün silinemedi.");
    } finally {
      setLoading(false);
    }
  };

  const handleClear = async () => {
    if (!basket || !userId) return;
    setLoading(true);
    setError(null);
    try {
      await BasketService.clear(userId);
      await loadBasket();
    } catch (err: any) {
      setError("Sepet temizlenemedi.");
    } finally {
      setLoading(false);
    }
  };

  const handleCheckout = async () => {
    if (!basket || !userId || basket.items.length === 0) return;
    setCheckoutLoading(true);
    setError(null);
    setSuccess(null);
    try {
      const payload = {
        userId,
        items: basket.items.map((i) => ({
          productId: i.productId,
          productName: i.productName,
          quantity: i.quantity,
          price: i.price,
        })),
        shippingAddress: dummyAddress,
        paymentMethod: "CreditCard",
      };
      await OrderService.createOrder(payload);
      await BasketService.clear(userId);
      setBasket({ userId, items: [] });
      setSuccess("Sipariş oluşturuldu ve sepet temizlendi.");
    } catch (err: any) {
      setError("Sipariş oluşturulamadı.");
    } finally {
      setCheckoutLoading(false);
    }
  };

  if (!userId) {
    return (
      <div className="max-w-3xl mx-auto px-4 py-12 text-center">
        <ShoppingBag className="mx-auto h-16 w-16 text-muted-foreground mb-4" />
        <p className="text-lg text-muted-foreground mb-4">Sepeti görmek için giriş yapmalısın.</p>
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

  if (loading && !basket) {
    return (
      <div className="max-w-3xl mx-auto px-4 py-12 flex items-center justify-center text-muted-foreground">
        <Loader2 className="h-6 w-6 mr-2 animate-spin" />
        Yükleniyor...
      </div>
    );
  }

  return (
    <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
      <h1 className="text-3xl font-extrabold text-foreground mb-6">Alışveriş Sepeti</h1>

      {error && (
        <div className="mb-4 flex items-center gap-2 text-red-500 bg-red-500/10 border border-red-500/30 rounded-md px-3 py-2">
          <AlertCircle className="h-4 w-4" />
          <span>{error}</span>
        </div>
      )}
      {success && (
        <div className="mb-4 flex items-center gap-2 text-emerald-500 bg-emerald-500/10 border border-emerald-500/30 rounded-md px-3 py-2">
          <CheckCircle2 className="h-4 w-4" />
          <span>{success}</span>
        </div>
      )}

      {!basket || basket.items.length === 0 ? (
        <div className="text-center py-12">
          <ShoppingBag className="mx-auto h-16 w-16 text-muted-foreground mb-4" />
          <p className="text-xl text-muted-foreground mb-4">Sepetin boş</p>
          <Link
            href="/products"
            className="inline-block px-6 py-3 bg-primary text-primary-foreground rounded-md hover:bg-primary/90 transition-colors"
          >
            Alışverişe devam et
          </Link>
        </div>
      ) : (
        <div className="grid grid-cols-1 lg:grid-cols-3 gap-8">
          <div className="lg:col-span-2 space-y-4">
            {basket.items.map((item) => (
              <div
                key={item.productId}
                className="bg-card border border-border rounded-lg p-4 flex items-center gap-4"
              >
                <div className="w-24 h-24 bg-muted rounded-md flex items-center justify-center flex-shrink-0">
                  <span className="text-xs text-muted-foreground">Image</span>
                </div>
                <div className="flex-grow">
                  <h3 className="text-lg font-semibold text-foreground">
                    {item.productName ?? "Ürün"}
                  </h3>
                  <p className="text-primary font-bold mt-1">₺{item.price.toFixed(2)}</p>
                </div>
                <div className="flex items-center gap-2">
                  <button
                    onClick={() => handleQuantity(item, -1)}
                    className="p-1 border border-border rounded hover:bg-accent disabled:opacity-50"
                    disabled={loading}
                  >
                    <Minus className="h-4 w-4" />
                  </button>
                  <span className="w-12 text-center font-medium">{item.quantity}</span>
                  <button
                    onClick={() => handleQuantity(item, 1)}
                    className="p-1 border border-border rounded hover:bg-accent disabled:opacity-50"
                    disabled={loading}
                  >
                    <Plus className="h-4 w-4" />
                  </button>
                </div>
                <button
                  onClick={() => handleRemove(item)}
                  className="p-2 text-destructive hover:bg-destructive/10 rounded disabled:opacity-50"
                  disabled={loading}
                >
                  <Trash2 className="h-5 w-5" />
                </button>
              </div>
            ))}
          </div>

          <div className="lg:col-span-1">
            <div className="bg-card border border-border rounded-lg p-6 sticky top-20 space-y-3">
              <h2 className="text-xl font-bold text-foreground">Sipariş Özeti</h2>
              <div className="space-y-3">
                <div className="flex justify-between text-sm">
                  <span className="text-muted-foreground">Ara Toplam</span>
                  <span className="text-foreground font-medium">₺{subtotal.toFixed(2)}</span>
                </div>
                <div className="flex justify-between text-sm">
                  <span className="text-muted-foreground">Vergi (10%)</span>
                  <span className="text-foreground font-medium">₺{tax.toFixed(2)}</span>
                </div>
                <div className="flex justify-between text-sm">
                  <span className="text-muted-foreground">Kargo</span>
                  <span className="text-foreground font-medium">Ücretsiz</span>
                </div>
                <div className="border-t border-border pt-3">
                  <div className="flex justify-between">
                    <span className="text-lg font-bold text-foreground">Toplam</span>
                    <span className="text-lg font-bold text-primary">₺{total.toFixed(2)}</span>
                  </div>
                </div>
              </div>
              <button
                onClick={handleCheckout}
                disabled={checkoutLoading || loading}
                className="w-full py-3 bg-primary text-primary-foreground rounded-md hover:bg-primary/90 transition-colors font-medium disabled:opacity-50"
              >
                {checkoutLoading ? (
                  <span className="inline-flex items-center gap-2">
                    <Loader2 className="h-4 w-4 animate-spin" /> İşleniyor...
                  </span>
                ) : (
                  "Ödemeye Geç"
                )}
              </button>
              <button
                onClick={handleClear}
                disabled={loading}
                className="w-full py-2 border border-border rounded-md hover:bg-accent transition-colors text-sm disabled:opacity-50"
              >
                Sepeti Temizle
              </button>
              <Link
                href="/products"
                className="block text-center mt-2 text-sm text-primary hover:text-primary/80"
              >
                Alışverişe devam et
              </Link>
            </div>
          </div>
        </div>
      )}
    </div>
  );
}
