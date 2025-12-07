"use client";

import { useState, useEffect } from "react";
import { Package, Search, Filter, Loader2, AlertCircle, Eye, X } from "lucide-react";
import { OrderService, UserService } from "@/services/api";
import { Order, OrderStatus, User } from "@/types";
import { extractErrorMessage } from "@/lib/errors";
import { formatCurrency } from "@/lib/utils";
import StatusBadge from "@/components/StatusBadge";

export default function AdminOrdersPage() {
  const [orders, setOrders] = useState<Order[]>([]);
  const [filteredOrders, setFilteredOrders] = useState<Order[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [searchTerm, setSearchTerm] = useState("");
  const [statusFilter, setStatusFilter] = useState<string>("all");
  const [selectedOrder, setSelectedOrder] = useState<Order | null>(null);
  const [userMap, setUserMap] = useState<Record<string, User>>({});

  useEffect(() => {
    const fetchOrders = async () => {
      try {
        setLoading(true);
        setError(null);
        const [ordersData, usersData] = await Promise.all([
          OrderService.getAllOrders(),
          UserService.getAllUsers().catch(() => []),
        ]);
        const map = (usersData || []).reduce<Record<string, User>>((acc, user) => {
          acc[user.id] = user;
          return acc;
        }, {});
        setUserMap(map);
        setOrders(ordersData);
        setFilteredOrders(ordersData);
      } catch (err: unknown) {
        setError(extractErrorMessage(err, "Failed to load orders"));
      } finally {
        setLoading(false);
      }
    };

    fetchOrders();
  }, []);

  useEffect(() => {
    let filtered = orders;

    // Search filter
    if (searchTerm) {
      filtered = filtered.filter(
        (order) =>
          order.id.toLowerCase().includes(searchTerm.toLowerCase()) ||
          order.userId.toLowerCase().includes(searchTerm.toLowerCase())
      );
    }

    // Status filter
    if (statusFilter !== "all") {
      filtered = filtered.filter((order) => order.status.toString() === statusFilter);
    }

    setFilteredOrders(filtered);
  }, [searchTerm, statusFilter, orders]);

  const getStatusCount = (status: string) => {
    if (status === "all") return orders.length;
    return orders.filter((o) => o.status.toString() === status).length;
  };

  const getUserLabel = (userId: string) => {
    const user = userMap[userId];
    if (user?.email) return user.email;
    return `${userId.slice(0, 8)}...`;
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
            <Package className="h-8 w-8 text-cyan-400" />
            <h1 className="text-4xl font-black">Order Management</h1>
          </div>
          <p className="text-slate-400">View and manage all customer orders</p>
        </div>

        {/* Filters */}
        <div className="bg-slate-800/50 rounded-xl p-6 mb-6 space-y-4">
          <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
            {/* Search */}
            <div className="relative">
              <Search className="absolute left-3 top-1/2 transform -translate-y-1/2 h-5 w-5 text-slate-400" />
              <input
                type="text"
                placeholder="Search by Order ID or User ID..."
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
                <option value="0">Pending ({getStatusCount("0")})</option>
                <option value="1">Confirmed ({getStatusCount("1")})</option>
                <option value="2">Shipped ({getStatusCount("2")})</option>
                <option value="3">Delivered ({getStatusCount("3")})</option>
                <option value="4">Cancelled ({getStatusCount("4")})</option>
              </select>
            </div>
          </div>
        </div>

        {/* Orders Table */}
        <div className="bg-slate-800/50 rounded-xl overflow-hidden">
          <div className="overflow-x-auto">
            <table className="w-full">
              <thead className="bg-slate-900/80 border-b border-slate-700">
                <tr>
                  <th className="px-6 py-4 text-left text-xs font-semibold text-slate-300 uppercase tracking-wider">
                    Order ID
                  </th>
                  <th className="px-6 py-4 text-left text-xs font-semibold text-slate-300 uppercase tracking-wider">
                    User
                  </th>
                  <th className="px-6 py-4 text-left text-xs font-semibold text-slate-300 uppercase tracking-wider">
                    Status
                  </th>
                  <th className="px-6 py-4 text-left text-xs font-semibold text-slate-300 uppercase tracking-wider">
                    Total
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
                {filteredOrders.length === 0 ? (
                  <tr>
                    <td colSpan={6} className="px-6 py-12 text-center text-slate-400">
                      No orders found
                    </td>
                  </tr>
                ) : (
                  filteredOrders.map((order) => (
                    <tr key={order.id} className="hover:bg-slate-700/30 transition-colors">
                      <td className="px-6 py-4 text-sm font-mono text-cyan-300">
                        {order.id.slice(0, 8)}...
                      </td>
                      <td className="px-6 py-4 text-sm font-semibold text-slate-200">
                        {getUserLabel(order.userId)}
                      </td>
                      <td className="px-6 py-4">
                        <StatusBadge status={order.status as OrderStatus} type="order" />
                      </td>
                      <td className="px-6 py-4 text-sm font-semibold text-white">
                        {formatCurrency(order.totalAmount || 0)}
                      </td>
                      <td className="px-6 py-4 text-sm text-slate-300">
                        {order.createdDate
                          ? new Date(order.createdDate).toLocaleDateString()
                          : "N/A"}
                      </td>
                      <td className="px-6 py-4">
                        <button
                          onClick={() => setSelectedOrder(order)}
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

      {/* Order Detail Modal */}
      {selectedOrder && (
        <div className="fixed inset-0 bg-black/60 flex items-center justify-center p-4 z-50">
          <div className="bg-slate-800 rounded-2xl max-w-2xl w-full max-h-[90vh] overflow-y-auto">
            <div className="sticky top-0 bg-slate-900 px-6 py-4 border-b border-slate-700 flex items-center justify-between">
              <h2 className="text-2xl font-bold">Order Details</h2>
              <button
                onClick={() => setSelectedOrder(null)}
                className="p-2 hover:bg-slate-700 rounded-lg transition-colors"
              >
                <X className="h-5 w-5" />
              </button>
            </div>

            <div className="p-6 space-y-6">
              {/* Order Info */}
              <div className="grid grid-cols-2 gap-4">
                <div>
                  <p className="text-sm text-slate-400">Order ID</p>
                  <p className="font-mono text-cyan-300">{selectedOrder.id}</p>
                </div>
                <div>
                  <p className="text-sm text-slate-400">User</p>
                  <p className="font-mono text-slate-200">
                    {getUserLabel(selectedOrder.userId)}
                  </p>
                </div>
                <div>
                  <p className="text-sm text-slate-400">Status</p>
                  <div className="mt-1">
                    <StatusBadge status={selectedOrder.status as OrderStatus} type="order" />
                  </div>
                </div>
                <div>
                  <p className="text-sm text-slate-400">Total Amount</p>
                  <p className="text-xl font-bold text-cyan-300">
                    {formatCurrency(selectedOrder.totalAmount || 0)}
                  </p>
                </div>
              </div>

              {/* Items */}
              <div>
                <h3 className="font-bold text-lg mb-3">Order Items</h3>
                <div className="space-y-2">
                  {selectedOrder.items.map((item, idx) => (
                    <div
                      key={idx}
                      className="bg-slate-900 rounded-lg p-4 flex justify-between items-center"
                    >
                      <div>
                        <p className="font-semibold">{item.productName}</p>
                        <p className="text-sm text-slate-400">Quantity: {item.quantity}</p>
                      </div>
                      <p className="font-bold text-cyan-300">
                        {formatCurrency(item.price * item.quantity)}
                      </p>
                    </div>
                  ))}
                </div>
              </div>

              {/* Shipping Address */}
              {selectedOrder.shippingAddress && (
                <div>
                  <h3 className="font-bold text-lg mb-2">Shipping Address</h3>
                  <div className="bg-slate-900 rounded-lg p-4 text-sm">
                    <p>{selectedOrder.shippingAddress.fullName}</p>
                    <p className="text-slate-300">{selectedOrder.shippingAddress.addressLine}</p>
                    <p className="text-slate-300">
                      {selectedOrder.shippingAddress.city}, {selectedOrder.shippingAddress.state}{" "}
                      {selectedOrder.shippingAddress.zipCode}
                    </p>
                    <p className="text-slate-300">{selectedOrder.shippingAddress.country}</p>
                  </div>
                </div>
              )}
            </div>
          </div>
        </div>
      )}
    </div>
  );
}
