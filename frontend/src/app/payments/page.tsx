"use client";

import { useEffect, useState } from "react";
import Link from "next/link";
import { useSearchParams } from "next/navigation";
import { CreditCard, Loader2, Receipt, RefreshCcw } from "lucide-react";
import { PaymentService } from "@/services/api";
import { formatCurrency, getPaymentStatusMeta } from "@/lib/utils";
import { getUserId } from "@/lib/auth";
import { extractErrorMessage } from "@/lib/errors";
import { Payment } from "@/types";

export default function PaymentsPage() {
  const params = useSearchParams();
  const filterOrderId = params.get("orderId");
  const [payments, setPayments] = useState<Payment[]>([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const userId = getUserId() || "";

  const loadPayments = async () => {
    if (!userId) return;
    setLoading(true);
    setError(null);
    try {
      const data = await PaymentService.getPaymentsByUserId(userId);
      let list = data;
      if (filterOrderId) {
        list = list.filter((payment) => payment.orderId === filterOrderId);
      }
      setPayments(list);
    } catch (err: unknown) {
      setError(extractErrorMessage(err, "Ödemeler alınamadı."));
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    void loadPayments();
  }, [userId, filterOrderId]);

  if (!userId) {
    return (
      <div className="max-w-3xl mx-auto px-4 py-12 text-center">
        <CreditCard className="mx-auto h-16 w-16 text-muted-foreground mb-4" />
        <p className="text-lg text-muted-foreground mb-4">Ödemeleri görmek için giriş yapmalısın.</p>
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
    <div className="max-w-5xl mx-auto px-4 sm:px-6 lg:px-8 py-10">
      <div className="flex items-center justify-between mb-6">
        <div>
          <h1 className="text-3xl font-bold text-foreground">Ödeme Geçmişi</h1>
          <p className="text-muted-foreground">
            {filterOrderId ? `#${filterOrderId} nolu sipariş için işlemler` : "Tüm ödeme kayıtların"}
          </p>
        </div>
        <button
          onClick={loadPayments}
          disabled={loading}
          className="inline-flex items-center gap-2 px-4 py-2 border border-border rounded-md hover:bg-accent transition disabled:opacity-50"
        >
          <RefreshCcw className="h-4 w-4" />
          Yenile
        </button>
      </div>

      {error && <p className="text-red-500 mb-4">{error}</p>}

      {loading ? (
        <div className="flex justify-center py-16">
          <Loader2 className="h-8 w-8 animate-spin text-primary" />
        </div>
      ) : payments.length === 0 ? (
        <div className="text-center py-20 text-muted-foreground">
          <Receipt className="h-14 w-14 mx-auto mb-4 opacity-50" />
          Henüz ödeme kaydı bulunamadı.
        </div>
      ) : (
        <div className="space-y-4">
          {payments.map((payment) => {
            const statusMeta = getPaymentStatusMeta(payment.status);
            const processed = payment.processedDate || payment.createdDate;
            return (
              <div key={payment.id} className="border border-border rounded-xl p-4 bg-card shadow-sm">
                <div className="flex flex-col md:flex-row md:items-center md:justify-between gap-3 mb-3">
                  <div>
                    <p className="text-sm text-muted-foreground">Ödeme ID</p>
                    <p className="font-semibold">{payment.id}</p>
                  </div>
                  <div className="text-right">
                    <p className="text-sm text-muted-foreground">Sipariş</p>
                    <p className="font-semibold">{payment.orderId}</p>
                  </div>
                </div>

                <div className="flex flex-wrap items-center gap-3 mb-2">
                  <span className={`px-3 py-1 rounded-full text-xs font-semibold ${statusMeta.badge}`}>
                    {statusMeta.label}
                  </span>
                  <span className="text-sm text-muted-foreground">
                    {processed ? new Date(processed).toLocaleString() : "-"}
                  </span>
                  <span className="text-xs text-muted-foreground">Durum: {statusMeta.description}</span>
                </div>

                <div className="flex flex-col sm:flex-row sm:items-center sm:justify-between">
                  <p className="text-lg font-semibold">{formatCurrency(payment.amount, payment.currency)}</p>
                  <div className="text-sm text-muted-foreground">
                    {payment.transactionId ? `Txn: ${payment.transactionId}` : "İşlem kimliği yok"}
                  </div>
                </div>

                {payment.failureReason && (
                  <p className="text-sm text-red-400 mt-2">Hata: {payment.failureReason}</p>
                )}
              </div>
            );
          })}
        </div>
      )}
    </div>
  );
}
