"use client";

import { useEffect, useState } from "react";
import { Trash2, MessageSquare, Loader2, AlertCircle } from "lucide-react";
import { ReviewService, UserService } from "@/services/api";
import { Review, User } from "@/types";
import { extractErrorMessage } from "@/lib/errors";

export default function AdminReviewsPage() {
  const [reviews, setReviews] = useState<Review[]>([]);
  const [users, setUsers] = useState<Record<string, User>>({});
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [deletingId, setDeletingId] = useState<string | null>(null);

  const load = async () => {
    try {
      setLoading(true);
      setError(null);
      const [rev, userList] = await Promise.all([
        ReviewService.getAll(),
        UserService.getAllUsers().catch(() => []),
      ]);
      const map = (userList || []).reduce<Record<string, User>>((acc, u) => {
        acc[u.id] = u;
        return acc;
      }, {});
      setUsers(map);
      setReviews(rev);
    } catch (err: unknown) {
      setError(extractErrorMessage(err, "Yorumlar yüklenemedi (backend desteği yok olabilir)"));
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    void load();
  }, []);

  const handleDelete = async (id: string) => {
    setDeletingId(id);
    try {
      await ReviewService.delete(id);
      setReviews((prev) => prev.filter((r) => r.id !== id));
    } catch (err: unknown) {
      setError(extractErrorMessage(err, "Yorum silinemedi"));
    } finally {
      setDeletingId(null);
    }
  };

  const userLabel = (userId: string) => users[userId]?.email || `${userId.slice(0, 8)}...`;

  if (loading) {
    return (
      <div className="min-h-screen flex items-center justify-center text-slate-300">
        <Loader2 className="h-8 w-8 animate-spin" />
      </div>
    );
  }

  return (
    <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-10 text-white">
      <div className="flex items-center gap-3 mb-6">
        <MessageSquare className="h-8 w-8 text-amber-300" />
        <div>
          <h1 className="text-3xl font-bold">Yorum Yönetimi</h1>
          <p className="text-sm text-slate-300">Ürün yorumlarını görüntüle ve sil</p>
        </div>
      </div>

      {error && (
        <div className="mb-4 flex items-center gap-2 text-red-400 bg-red-500/10 border border-red-500/30 rounded-md px-3 py-2">
          <AlertCircle className="h-4 w-4" />
          <span>{error}</span>
        </div>
      )}

      <div className="space-y-3">
        {reviews.length === 0 ? (
          <p className="text-slate-300">Hiç yorum yok.</p>
        ) : (
          reviews.map((rev) => (
            <div key={rev.id} className="bg-slate-900/70 border border-white/10 rounded-xl p-4 flex flex-col gap-2">
              <div className="flex items-center justify-between">
                <div className="text-sm text-slate-300">
                  <span className="font-semibold text-white">{userLabel(rev.userId)}</span> •{" "}
                  <span className="text-slate-400">{new Date(rev.createdAt).toLocaleString("tr-TR")}</span>
                </div>
                <div className="flex items-center gap-2">
                  <span className="text-amber-300 font-semibold">{rev.rating}/5</span>
                  <button
                    onClick={() => handleDelete(rev.id)}
                    disabled={deletingId === rev.id}
                    className="p-2 rounded-lg bg-red-500/20 hover:bg-red-500/30 text-red-200 disabled:opacity-50"
                  >
                    <Trash2 className="h-4 w-4" />
                  </button>
                </div>
              </div>
              <p className="text-slate-100 text-sm">{rev.comment}</p>
            </div>
          ))
        )}
      </div>
    </div>
  );
}
