"use client";

import { useState, useEffect } from "react";
import { CreditCard, Search, Filter, Loader2, AlertCircle, Eye, X } from "lucide-react";
import { PaymentService } from "@/services/api";
import { Payment, PaymentStatus } from "@/types";
import { extractErrorMessage } from "@/lib/errors";
import { formatCurrency } from "@/lib/utils";
import StatusBadge from "@/components/StatusBadge";

export default function AdminPaymentsPage() {
  const [payments, setPayments] = useState<Payment[]>([]);
  const [filteredPayments, setFilteredPayments] = useState<Payment[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [searchTerm, setSearchTerm] = useState("");
  const [statusFilter, setStatusFilter] = useState<string>("all");
  const [selectedPayment, setSelectedPayment] = useState<Payment | null>(null);

  useEffect(() => {
    const fetchPayments = async () => {
      try {
        setLoading(true);
        setError(null);
        const data = await PaymentService.getAllPayments();
        setPayments(data);
        setFilteredPayments(data);
      } catch (err: unknown) {
        setError(extractErrorMessage(err, "Failed to load payments"));
      } finally {
        setLoading(false);
      }
    };

    fetchPayments();
  }, []);

  useEffect(() => {
    let filtered = payments;

    // Search filter
    if (searchTerm) {
      filtered = filtered.filter(
        (payment) =>
          payment.id.toLowerCase().includes(searchTerm.toLowerCase()) ||
          payment.orderId.toLowerCase().includes(searchTerm.toLowerCase()) ||
          payment.userId.toLowerCase().includes(searchTerm.toLowerCase())
      );
    }

    // Status filter
    if (statusFilter !== "all") {
      filtered = filtered.filter((payment) => payment.status === statusFilter);
    }

    setFilteredPayments(filtered);
  }, [searchTerm, statusFilter, payments]);

  const getStatusCount = (status: string) => {
    if (status === "all") return payments.length;
    return payments.filter((p) => p.status === status).length;
  };

  if (loading) {
    return (
      <div className="min-h-screen flex items-center justify-center bg-gradient-to-b from-slate-950 via-slate-900 to-slate-950">
        <Loader2 className="h-12 w-12 animate-spin text-blue-500" />
      </div>
    );
  }

  if (error) {
    return (
      <div className="min-h-screen flex items-center justify-center bg-gradient-to-b from-slate-950 via-slate-900 to-slate-950">
        <div className="text-center space-y-4">
          <AlertCircle className="h-12 w-12 text-red-500 mx-auto" />
          <p className="text-red-400">{error}</p>
        </div>
      </div>
    );
  }

  return (
    <div className="min-h-screen bg-gradient-to-b from-slate-950 via-slate-900 to-slate-950 text-white py-8">
      <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        {/* Header */}
        <div className="mb-8">
          <div className="flex items-center gap-3 mb-2">
            <CreditCard className="h-8 w-8 text-cyan-400" />
            <h1 className="text-4xl font-black">Payment Management</h1>
          </div>
          <p className="text-slate-400">View and track all payment transactions</p>
        </div>

        {/* Filters */}
        <div className="bg-slate-800/50 rounded-xl p-6 mb-6 space-y-4">
          <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
            {/* Search */}
            <div className="relative">
              <Search className="absolute left-3 top-1/2 transform -translate-y-1/2 h-5 w-5 text-slate-400" />
              <input
                type="text"
                placeholder="Search by Payment ID, Order ID, or User ID..."
                value={searchTerm}
                onChange={(e) => setSearchTerm(e.target.value)}
                className="w-full pl-10 pr-4 py-3 bg-slate-900 border border-slate-700 rounded-lg text-white placeholder-slate-400 focus:outline-none focus:ring-2 focus:ring-blue-500"
              />
            </div>

            {/* Status Filter */}
            <div className="relative">
              <Filter className="absolute left-3 top-1/2 transform -translate-y-1/2 h-5 w-5 text-slate-400" />
              <select
                value={statusFilter}
                onChange={(e) => setStatusFilter(e.target.value)}
                className="w-full pl-10 pr-4 py-3 bg-slate-900 border border-slate-700 rounded-lg text-white focus:outline-none focus:ring-2 focus:ring-blue-500"
              >
                <option value="all">All Status ({getStatusCount("all")})</option>
                <option value="Pending">Pending ({getStatusCount("Pending")})</option>
                <option value="Processing">Processing ({getStatusCount("Processing")})</option>
                <option value="Success">Success ({getStatusCount("Success")})</option>
                <option value="Failed">Failed ({getStatusCount("Failed")})</option>
                <option value="Refunded">Refunded ({getStatusCount("Refunded")})</option>
              </select>
            </div>
          </div>
        </div>

        {/* Payments Table */}
        <div className="bg-slate-800/50 rounded-xl overflow-hidden">
          <div className="overflow-x-auto">
            <table className="w-full">
              <thead className="bg-slate-900/80 border-b border-slate-700">
                <tr>
                  <th className="px-6 py-4 text-left text-xs font-semibold text-slate-300 uppercase tracking-wider">
                    Payment ID
                  </th>
                  <th className="px-6 py-4 text-left text-xs font-semibold text-slate-300 uppercase tracking-wider">
                    Order ID
                  </th>
                  <th className="px-6 py-4 text-left text-xs font-semibold text-slate-300 uppercase tracking-wider">
                    User ID
                  </th>
                  <th className="px-6 py-4 text-left text-xs font-semibold text-slate-300 uppercase tracking-wider">
                    Amount
                  </th>
                  <th className="px-6 py-4 text-left text-xs font-semibold text-slate-300 uppercase tracking-wider">
                    Status
                  </th>
                  <th className="px-6 py-4 text-left text-xs font-semibold text-slate-300 uppercase tracking-wider">
                    Date
                  </th>
                  <th className="px-6 py-4 text-left text-xs font-semibold text-slate-300 uppercase tracking-wider">
                    Actions
                  </th>
                </tr>
              </thead>
              <tbody className="divide-y divide-slate-700">
                {filteredPayments.length === 0 ? (
                  <tr>
                    <td colSpan={7} className="px-6 py-12 text-center text-slate-400">
                      No payments found
                    </td>
                  </tr>
                ) : (
                  filteredPayments.map((payment) => (
                    <tr key={payment.id} className="hover:bg-slate-700/30 transition-colors">
                      <td className="px-6 py-4 text-sm font-mono text-cyan-300">
                        {payment.id.slice(0, 8)}...
                      </td>
                      <td className="px-6 py-4 text-sm font-mono text-slate-300">
                        {payment.orderId.slice(0, 8)}...
                      </td>
                      <td className="px-6 py-4 text-sm font-mono text-slate-300">
                        {payment.userId.slice(0, 8)}...
                      </td>
                      <td className="px-6 py-4 text-sm font-semibold text-white">
                        {formatCurrency(payment.amount)}
                      </td>
                      <td className="px-6 py-4">
                        <StatusBadge status={payment.status as PaymentStatus} type="payment" />
                      </td>
                      <td className="px-6 py-4 text-sm text-slate-300">
                        {payment.createdDate
                          ? new Date(payment.createdDate).toLocaleDateString()
                          : "N/A"}
                      </td>
                      <td className="px-6 py-4">
                        <button
                          onClick={() => setSelectedPayment(payment)}
                          className="flex items-center gap-2 px-3 py-1.5 bg-blue-600 hover:bg-blue-700 rounded-lg text-sm font-medium transition-colors"
                        >
                          <Eye className="h-4 w-4" />
                          View
                        </button>
                      </td>
                    </tr>
                  ))
                )}
              </tbody>
            </table>
          </div>
        </div>
      </div>

      {/* Payment Detail Modal */}
      {selectedPayment && (
        <div className="fixed inset-0 bg-black/60 flex items-center justify-center p-4 z-50">
          <div className="bg-slate-800 rounded-2xl max-w-2xl w-full max-h-[90vh] overflow-y-auto">
            <div className="sticky top-0 bg-slate-900 px-6 py-4 border-b border-slate-700 flex items-center justify-between">
              <h2 className="text-2xl font-bold">Payment Details</h2>
              <button
                onClick={() => setSelectedPayment(null)}
                className="p-2 hover:bg-slate-700 rounded-lg transition-colors"
              >
                <X className="h-5 w-5" />
              </button>
            </div>

            <div className="p-6 space-y-6">
              {/* Payment Info */}
              <div className="grid grid-cols-2 gap-4">
                <div>
                  <p className="text-sm text-slate-400">Payment ID</p>
                  <p className="font-mono text-cyan-300">{selectedPayment.id}</p>
                </div>
                <div>
                  <p className="text-sm text-slate-400">Order ID</p>
                  <p className="font-mono">{selectedPayment.orderId}</p>
                </div>
                <div>
                  <p className="text-sm text-slate-400">User ID</p>
                  <p className="font-mono">{selectedPayment.userId}</p>
                </div>
                <div>
                  <p className="text-sm text-slate-400">Transaction ID</p>
                  <p className="font-mono text-sm">
                    {selectedPayment.transactionId || "N/A"}
                  </p>
                </div>
                <div>
                  <p className="text-sm text-slate-400">Status</p>
                  <div className="mt-1">
                    <StatusBadge
                      status={selectedPayment.status as PaymentStatus}
                      type="payment"
                    />
                  </div>
                </div>
                <div>
                  <p className="text-sm text-slate-400">Amount</p>
                  <p className="text-xl font-bold text-cyan-300">
                    {formatCurrency(selectedPayment.amount)} {selectedPayment.currency}
                  </p>
                </div>
              </div>

              {/* Dates */}
              <div className="grid grid-cols-2 gap-4">
                <div>
                  <p className="text-sm text-slate-400">Created Date</p>
                  <p className="text-sm">
                    {selectedPayment.createdDate
                      ? new Date(selectedPayment.createdDate).toLocaleString()
                      : "N/A"}
                  </p>
                </div>
                <div>
                  <p className="text-sm text-slate-400">Processed Date</p>
                  <p className="text-sm">
                    {selectedPayment.processedDate
                      ? new Date(selectedPayment.processedDate).toLocaleString()
                      : "N/A"}
                  </p>
                </div>
              </div>

              {/* Failure Reason */}
              {selectedPayment.failureReason && (
                <div className="bg-red-500/10 border border-red-500/30 rounded-lg p-4">
                  <p className="text-sm text-slate-400 mb-1">Failure Reason</p>
                  <p className="text-red-400">{selectedPayment.failureReason}</p>
                </div>
              )}
            </div>
          </div>
        </div>
      )}
    </div>
  );
}
