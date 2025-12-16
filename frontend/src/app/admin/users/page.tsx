"use client";

import { useState, useEffect } from "react";
import { Users, Search, Filter, Loader2, AlertCircle, Shield, CheckCircle, XCircle } from "lucide-react";
import { UserService } from "@/services/api";
import { User } from "@/types";
import { extractErrorMessage } from "@/lib/errors";

export default function AdminUsersPage() {
  const [users, setUsers] = useState<User[]>([]);
  const [filteredUsers, setFilteredUsers] = useState<User[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [searchTerm, setSearchTerm] = useState("");
  const [roleFilter, setRoleFilter] = useState<string>("all");
  const [statusFilter, setStatusFilter] = useState<string>("all");

  useEffect(() => {
    const fetchUsers = async () => {
      try {
        setLoading(true);
        setError(null);
        const data = await UserService.getAllUsers();
        setUsers(data);
        setFilteredUsers(data);
      } catch (err: unknown) {
        setError(extractErrorMessage(err, "Failed to load users"));
      } finally {
        setLoading(false);
      }
    };

    fetchUsers();
  }, []);

  useEffect(() => {
    let filtered = users;

    // Search filter
    if (searchTerm) {
      filtered = filtered.filter(
        (user) =>
          user.email.toLowerCase().includes(searchTerm.toLowerCase()) ||
          user.firstName?.toLowerCase().includes(searchTerm.toLowerCase()) ||
          user.lastName?.toLowerCase().includes(searchTerm.toLowerCase())
      );
    }

    // Role filter
    if (roleFilter !== "all") {
      filtered = filtered.filter((user) => user.roles?.includes(roleFilter));
    }

    // Status filter
    if (statusFilter === "active") {
      filtered = filtered.filter((user) => user.isActive);
    } else if (statusFilter === "inactive") {
      filtered = filtered.filter((user) => !user.isActive);
    }

    setFilteredUsers(filtered);
  }, [searchTerm, roleFilter, statusFilter, users]);

  const getRoleCount = (role: string) => {
    if (role === "all") return users.length;
    return users.filter((u) => u.roles?.includes(role)).length;
  };

  const getStatusCount = (status: string) => {
    if (status === "all") return users.length;
    if (status === "active") return users.filter((u) => u.isActive).length;
    return users.filter((u) => !u.isActive).length;
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
            <Users className="h-8 w-8 text-cyan-400" />
            <h1 className="text-4xl font-black">User Management</h1>
          </div>
          <p className="text-slate-400">View and manage all registered users</p>
        </div>

        {/* Filters */}
        <div className="bg-slate-800/50 rounded-xl p-6 mb-6 space-y-4">
          <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
            {/* Search */}
            <div className="relative">
              <Search className="absolute left-3 top-1/2 transform -translate-y-1/2 h-5 w-5 text-slate-400" />
              <input
                type="text"
                placeholder="Search by email or name..."
                value={searchTerm}
                onChange={(e) => setSearchTerm(e.target.value)}
                className="w-full pl-10 pr-4 py-3 bg-slate-900 border border-slate-700 rounded-lg text-white placeholder-slate-400 focus:outline-none focus:ring-2 focus:ring-blue-500"
              />
            </div>

            {/* Role Filter */}
            <div className="relative">
              <Filter className="absolute left-3 top-1/2 transform -translate-y-1/2 h-5 w-5 text-slate-400" />
              <select
                value={roleFilter}
                onChange={(e) => setRoleFilter(e.target.value)}
                className="w-full pl-10 pr-4 py-3 bg-slate-900 border border-slate-700 rounded-lg text-white focus:outline-none focus:ring-2 focus:ring-blue-500"
              >
                <option value="all">All Roles ({getRoleCount("all")})</option>
                <option value="Admin">Admin ({getRoleCount("Admin")})</option>
                <option value="Customer">Customer ({getRoleCount("Customer")})</option>
              </select>
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
                <option value="active">Active ({getStatusCount("active")})</option>
                <option value="inactive">Inactive ({getStatusCount("inactive")})</option>
              </select>
            </div>
          </div>
        </div>

        {/* Users Table */}
        <div className="bg-slate-800/50 rounded-xl overflow-hidden">
          <div className="overflow-x-auto">
            <table className="w-full">
              <thead className="bg-slate-900/80 border-b border-slate-700">
                <tr>
                  <th className="px-6 py-4 text-left text-xs font-semibold text-slate-300 uppercase tracking-wider">
                    Email
                  </th>
                  <th className="px-6 py-4 text-left text-xs font-semibold text-slate-300 uppercase tracking-wider">
                    Name
                  </th>
                  <th className="px-6 py-4 text-left text-xs font-semibold text-slate-300 uppercase tracking-wider">
                    Roles
                  </th>
                  <th className="px-6 py-4 text-left text-xs font-semibold text-slate-300 uppercase tracking-wider">
                    Status
                  </th>
                  <th className="px-6 py-4 text-left text-xs font-semibold text-slate-300 uppercase tracking-wider">
                    Email Verified
                  </th>
                  <th className="px-6 py-4 text-left text-xs font-semibold text-slate-300 uppercase tracking-wider">
                    Registered
                  </th>
                </tr>
              </thead>
              <tbody className="divide-y divide-slate-700">
                {filteredUsers.length === 0 ? (
                  <tr>
                    <td colSpan={6} className="px-6 py-12 text-center text-slate-400">
                      No users found
                    </td>
                  </tr>
                ) : (
                  filteredUsers.map((user) => (
                    <tr key={user.id} className="hover:bg-slate-700/30 transition-colors">
                      <td className="px-6 py-4">
                        <div className="flex items-center gap-2">
                          <div className="h-8 w-8 rounded-full bg-gradient-to-br from-blue-500 to-purple-600 flex items-center justify-center text-white font-bold text-sm">
                            {user.email.charAt(0).toUpperCase()}
                          </div>
                          <span className="text-sm text-white">{user.email}</span>
                        </div>
                      </td>
                      <td className="px-6 py-4 text-sm text-slate-300">
                        {user.firstName} {user.lastName}
                      </td>
                      <td className="px-6 py-4">
                        <div className="flex gap-2">
                          {user.roles?.map((role) => (
                            <span
                              key={role}
                              className={`inline-flex items-center gap-1 px-2.5 py-1 rounded-full text-xs font-semibold ${
                                role === "Admin"
                                  ? "bg-purple-500/20 text-purple-300 border border-purple-500/30"
                                  : "bg-blue-500/20 text-blue-300 border border-blue-500/30"
                              }`}
                            >
                              {role === "Admin" && <Shield className="h-3 w-3" />}
                              {role}
                            </span>
                          ))}
                        </div>
                      </td>
                      <td className="px-6 py-4">
                        {user.isActive ? (
                          <span className="inline-flex items-center gap-1 px-2.5 py-1 bg-green-500/20 text-green-300 border border-green-500/30 rounded-full text-xs font-semibold">
                            <CheckCircle className="h-3 w-3" />
                            Active
                          </span>
                        ) : (
                          <span className="inline-flex items-center gap-1 px-2.5 py-1 bg-red-500/20 text-red-300 border border-red-500/30 rounded-full text-xs font-semibold">
                            <XCircle className="h-3 w-3" />
                            Inactive
                          </span>
                        )}
                      </td>
                      <td className="px-6 py-4">
                        {user.emailConfirmed ? (
                          <CheckCircle className="h-5 w-5 text-green-400" />
                        ) : (
                          <XCircle className="h-5 w-5 text-slate-500" />
                        )}
                      </td>
                      <td className="px-6 py-4 text-sm text-slate-300">
                        {user.createdAt
                          ? new Date(user.createdAt).toLocaleDateString()
                          : "N/A"}
                      </td>
                    </tr>
                  ))
                )}
              </tbody>
            </table>
          </div>
        </div>

        {/* Stats */}
        <div className="grid grid-cols-1 md:grid-cols-4 gap-6 mt-8">
          <div className="bg-slate-800/50 rounded-xl p-6">
            <p className="text-sm text-slate-400 mb-1">Total Users</p>
            <p className="text-3xl font-bold text-white">{users.length}</p>
          </div>
          <div className="bg-slate-800/50 rounded-xl p-6">
            <p className="text-sm text-slate-400 mb-1">Active Users</p>
            <p className="text-3xl font-bold text-green-400">
              {users.filter((u) => u.isActive).length}
            </p>
          </div>
          <div className="bg-slate-800/50 rounded-xl p-6">
            <p className="text-sm text-slate-400 mb-1">Admins</p>
            <p className="text-3xl font-bold text-purple-400">
              {users.filter((u) => u.roles?.includes("Admin")).length}
            </p>
          </div>
          <div className="bg-slate-800/50 rounded-xl p-6">
            <p className="text-sm text-slate-400 mb-1">Verified Emails</p>
            <p className="text-3xl font-bold text-cyan-400">
              {users.filter((u) => u.emailConfirmed).length}
            </p>
          </div>
        </div>
      </div>
    </div>
  );
}
