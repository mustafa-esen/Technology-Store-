"use client";

import { useEffect, useMemo, useState } from "react";
import Link from "next/link";
import { Trash2, Plus, Minus, ShoppingBag, Loader2, AlertCircle, CheckCircle2 } from "lucide-react";
import { BasketService, OrderService, PaymentService } from "@/services/api";
import { getOrderStatusMeta, getPaymentStatusMeta } from "@/lib/utils";
import { getUserId } from "@/lib/auth";
import { extractErrorMessage } from "@/lib/errors";
import { Basket, ShippingAddress } from "@/types";

const defaultAddress: ShippingAddress = {
  street: "",
  city: "",
  state: "",
  country: "",
  zipCode: "",
};

export default function CartPage() {
  const [basket, setBasket] = useState<Basket | null>(null);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [success, setSuccess] = useState<string | null>(null);
  const [checkoutLoading, setCheckoutLoading] = useState(false);
  const [activeOrderId, setActiveOrderId] = useState<string | null>(null);
  const [orderStatus, setOrderStatus] = useState<string | null>(null);
  const [polling, setPolling] = useState(false);
  const [paymentStatus, setPaymentStatus] = useState<string | null>(null);
  const [useSavedCard, setUseSavedCard] = useState(false);
  const [savedCard, setSavedCard] = useState<{ last4: string; name: string; expiry: string } | null>(null);
  const [cardForm, setCardForm] = useState({ name: "", number: "", expiry: "", cvc: "" });
  const [address, setAddress] = useState<ShippingAddress>(defaultAddress);
  const [mounted, setMounted] = useState(false);

  // Merkezi auth fonksiyonundan userId al
  const userId = useMemo(() => getUserId() || "", []);

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
    } catch (err: unknown) {
      // Sepet yoksa 404 gelebilir, bu durumda boş sepet göster
      const axiosErr = err as { response?: { status?: number } };
      if (axiosErr?.response?.status === 404) {
        setBasket({ userId, items: [] });
      } else {
        setError(extractErrorMessage(err, "Sepet alınamadı. Lütfen tekrar deneyin."));
      }
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    setMounted(true);
    void loadBasket();

    if (typeof window !== "undefined") {
      const stored = localStorage.getItem("savedCard");
      if (stored) {
        try {
          setSavedCard(JSON.parse(stored));
        } catch {
          setSavedCard(null);
        }
      }
    }
  }, [userId]);

  useEffect(() => {
    if (!activeOrderId) return;

    setPolling(true);
    const interval = setInterval(async () => {
      try {
        const order = await OrderService.getOrder(activeOrderId);
        const status = String(order?.status ?? "Pending");
        setOrderStatus(status);

        if (["PaymentReceived", "Processing", "Shipped", "Delivered"].includes(status)) {
          setSuccess(`Ödeme onaylandı. Sipariş durumu: ${getOrderStatusMeta(status).label}`);
          setPolling(false);
          clearInterval(interval);
        } else if (["Failed", "Cancelled"].includes(status)) {
          setError(`Ödeme başarısız oldu. Sipariş durumu: ${getOrderStatusMeta(status).label}`);
          setPolling(false);
          clearInterval(interval);
        }

        // Payment durumunu kontrol et (PaymentService)
        if (userId) {
          const payments = await PaymentService.getPaymentsByUserId(userId);
          const current = payments.find((p) => p.orderId === activeOrderId);
          if (current) {
            const pStatus = String(current.status);
            setPaymentStatus(pStatus);
            const meta = getPaymentStatusMeta(pStatus);
            if (pStatus === "Success") {
              setSuccess(`Ödeme onaylandı. ${meta.label}`);
              setPolling(false);
              clearInterval(interval);
            } else if (pStatus === "Failed") {
              setError(`Ödeme başarısız: ${current.failureReason || meta.label}`);
              setPolling(false);
              clearInterval(interval);
            }
          }
        }
      } catch {
        // Order henüz oluşmamış olabilir, beklemeye devam et
      }
    }, 4000);

    return () => clearInterval(interval);
  }, [activeOrderId, userId]);

  const handleQuantity = async (item: { productId: string; quantity: number }, change: number) => {
    if (!basket || !userId) return;
    const nextQty = Math.max(1, item.quantity + change);
    setLoading(true);
    setError(null);
    try {
      await BasketService.updateItem(userId, item.productId, nextQty);
      await loadBasket();
    } catch (err: unknown) {
      setError(extractErrorMessage(err, "Miktar güncellenemedi."));
    } finally {
      setLoading(false);
    }
  };

  const handleRemove = async (item: { productId: string }) => {
    if (!basket || !userId) return;
    setLoading(true);
    setError(null);
    try {
      await BasketService.removeItem(userId, item.productId);
      await loadBasket();
    } catch (err: unknown) {
      setError(extractErrorMessage(err, "Ürün silinemedi."));
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
    } catch (err: unknown) {
      setError(extractErrorMessage(err, "Sepet temizlenemedi."));
    } finally {
      setLoading(false);
    }
  };

  const handleCheckout = async () => {
    // Step 1: ensure address is filled
    if (!basket || !userId || basket.items.length === 0) return;
    if (!address.street || !address.city || !address.state || !address.country || !address.zipCode) {
      setError("Adres bilgileri eksik. Lütfen tüm alanları doldurun.");
      return;
    }
    // Step 2: ensure payment form is filled
    if (!useSavedCard && (!cardForm.name || !cardForm.number || !cardForm.expiry || !cardForm.cvc)) {
      setError("Kart bilgilerini eksiksiz doldurun.");
      return;
    }

    // Save masked card for future use (frontend only)
    if (!useSavedCard && cardForm.number.length >= 4) {
      const masked = {
        last4: cardForm.number.slice(-4),
        name: cardForm.name,
        expiry: cardForm.expiry,
      };
      localStorage.setItem("savedCard", JSON.stringify(masked));
      setSavedCard(masked);
    }

    setCheckoutLoading(true);
    setError(null);
    setSuccess(null);
    setPaymentStatus("Processing");
    try {
      const payload = {
        userId,
        items: basket.items.map((i) => ({
          productId: i.productId,
          productName: i.productName,
          quantity: i.quantity,
          price: i.price,
        })),
        shippingAddress: address,
        paymentMethod: "CreditCard",
      };
      const createdOrder = await OrderService.createOrder(payload);
      const createdOrderId = createdOrder?.id || null;
      await BasketService.clear(userId);
      setBasket({ userId, items: [] });
      if (createdOrderId) {
        setActiveOrderId(createdOrderId);
        const initialStatus = String(createdOrder?.status ?? "Pending");
        setOrderStatus(initialStatus);
        setSuccess(`Sipariş oluşturuldu. Ödeme işleniyor (#${createdOrderId.slice(0, 6)}…)`);
      } else {
        setSuccess("Sipariş oluşturuldu ve sepet temizlendi.");
      }
    } catch (err: unknown) {
      setError(extractErrorMessage(err, "Sipariş oluşturulamadı."));
    } finally {
      setCheckoutLoading(false);
      setPaymentStatus(null);
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

  if (!mounted) return null;

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
          <div className="flex flex-col">
            <span>{success}</span>
            {activeOrderId && (
              <Link href="/orders" className="text-sm underline text-emerald-200 hover:text-emerald-100">
                Siparişleri görüntüle
              </Link>
            )}
          </div>
        </div>
      )}
      {polling && orderStatus && (
        <div className="mb-4 flex items-center gap-2 text-sky-400 bg-sky-500/10 border border-sky-500/30 rounded-md px-3 py-2">
          <Loader2 className="h-4 w-4 animate-spin" />
          <span>{`Mevcut durum: ${getOrderStatusMeta(orderStatus).label}`}</span>
        </div>
      )}
      {paymentStatus === "Processing" && (
        <div className="mb-4 flex items-center gap-2 text-blue-400 bg-blue-500/10 border border-blue-500/30 rounded-md px-3 py-2">
          <Loader2 className="h-4 w-4 animate-spin" />
          <span>Ödeme işleniyor (Fake gateway)</span>
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
              <div className="space-y-2">
                <h3 className="font-semibold text-foreground">Teslimat Adresi</h3>
                <input
                  value={address.street}
                  onChange={(e) => setAddress({ ...address, street: e.target.value })}
                  placeholder="Sokak / Adres"
                  className="w-full px-3 py-2 rounded-md border border-border bg-background text-foreground text-sm"
                />
                <input
                  value={address.city}
                  onChange={(e) => setAddress({ ...address, city: e.target.value })}
                  placeholder="Şehir"
                  className="w-full px-3 py-2 rounded-md border border-border bg-background text-foreground text-sm"
                />
                <div className="grid grid-cols-2 gap-2">
                  <input
                    value={address.state}
                    onChange={(e) => setAddress({ ...address, state: e.target.value })}
                    placeholder="İl/İlçe"
                    className="w-full px-3 py-2 rounded-md border border-border bg-background text-foreground text-sm"
                  />
                  <input
                    value={address.zipCode}
                    onChange={(e) => setAddress({ ...address, zipCode: e.target.value })}
                    placeholder="Posta Kodu"
                    className="w-full px-3 py-2 rounded-md border border-border bg-background text-sm text-foreground"
                  />
                </div>
                <input
                  value={address.country}
                  onChange={(e) => setAddress({ ...address, country: e.target.value })}
                  placeholder="Ülke"
                  className="w-full px-3 py-2 rounded-md border border-border bg-background text-foreground text-sm"
                />
              </div>
              <div className="space-y-2 pt-2">
                <div className="flex items-center justify-between">
                  <h3 className="font-semibold text-foreground">Ödeme (Fake Gateway)</h3>
                  {savedCard && (
                    <label className="flex items-center gap-2 text-sm text-foreground">
                      <input
                        type="checkbox"
                        checked={useSavedCard}
                        onChange={(e) => setUseSavedCard(e.target.checked)}
                      />
                      <span>{`Kayıtlı kartı kullan (${savedCard.name} •••• ${savedCard.last4})`}</span>
                    </label>
                  )}
                </div>
                {!useSavedCard && (
                  <div className="space-y-2">
                    <input
                      value={cardForm.name}
                      onChange={(e) => setCardForm({ ...cardForm, name: e.target.value })}
                      placeholder="Kart üzerindeki isim"
                      className="w-full px-3 py-2 rounded-md border border-border bg-background text-foreground text-sm"
                    />
                    <input
                      value={cardForm.number}
                      onChange={(e) => setCardForm({ ...cardForm, number: e.target.value })}
                      placeholder="Kart numarası (fake)"
                      className="w-full px-3 py-2 rounded-md border border-border bg-background text-foreground text-sm"
                    />
                    <div className="grid grid-cols-2 gap-2">
                      <input
                        value={cardForm.expiry}
                        onChange={(e) => setCardForm({ ...cardForm, expiry: e.target.value })}
                        placeholder="AA/YY"
                        className="w-full px-3 py-2 rounded-md border border-border bg-background text-foreground text-sm"
                      />
                      <input
                        value={cardForm.cvc}
                        onChange={(e) => setCardForm({ ...cardForm, cvc: e.target.value })}
                        placeholder="CVC"
                        className="w-full px-3 py-2 rounded-md border border-border bg-background text-foreground text-sm"
                      />
                    </div>
                  </div>
                )}
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
