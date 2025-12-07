"use client";

import { useEffect, useMemo, useState } from "react";
import { Plus, Edit, Trash2, RefreshCcw } from "lucide-react";

import { ProductService, CategoryService } from "@/services/api";
import { Product, Category } from "@/types";
import { extractErrorMessage } from "@/lib/errors";
import { formatCurrency } from "@/lib/utils";

const emptyProduct: Partial<Product> = {
  name: "",
  price: 0,
  stock: 0,
  brand: "",
  categoryId: "",
  description: "",
  imageUrl: "",
};

export default function AdminProductsPage() {
  const [products, setProducts] = useState<Product[]>([]);
  const [categories, setCategories] = useState<Category[]>([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [form, setForm] = useState<Partial<Product>>(emptyProduct);
  const [editingId, setEditingId] = useState<string | null>(null);
  const [saving, setSaving] = useState(false);

  const categoryOptions = useMemo(
    () => categories.map((c) => ({ value: c.id, label: c.name })),
    [categories]
  );

  const load = async () => {
    setLoading(true);
    setError(null);
    try {
      const [prod, cats] = await Promise.all([
        ProductService.getAll(),
        CategoryService.getAll(),
      ]);
      setProducts(prod);
      setCategories(cats);
    } catch (err: unknown) {
      setError(extractErrorMessage(err, "Ürünler yüklenemedi."));
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    void load();
  }, []);

  const resetForm = () => {
    setForm(emptyProduct);
    setEditingId(null);
  };

  const startEdit = (p: Product) => {
    setEditingId(p.id);
    setForm({
      name: p.name,
      price: Number(p.price ?? 0),
      stock: (p as any).stock ?? 0,
      brand: p.brand,
      categoryId: (p as any).categoryId ?? (p as any).category?.id,
      description: p.description,
      imageUrl: (p as any).imageUrl ?? "",
    });
  };

  const handleSave = async () => {
    if (!form.name?.trim()) {
      setError("Ürün adı gerekli.");
      return;
    }
    if (!form.categoryId) {
      setError("Kategori seçmeniz gerekiyor.");
      return;
    }

    const payload: Partial<Product> = {
      name: form.name.trim(),
      price: Number(form.price ?? 0),
      stock: Number((form as any).stock ?? 0),
      brand: form.brand?.trim(),
      categoryId: form.categoryId,
      description: form.description?.trim(),
      imageUrl: (form as any).imageUrl?.trim(),
    };

    setSaving(true);
    setError(null);
    try {
      if (editingId) {
        await ProductService.update(editingId, payload);
      } else {
        await ProductService.create(payload);
      }
      resetForm();
      await load();
    } catch (err: unknown) {
      setError(extractErrorMessage(err, "Kaydedilemedi."));
    } finally {
      setSaving(false);
    }
  };

  const handleDelete = async (id: string) => {
    setSaving(true);
    setError(null);
    try {
      await ProductService.delete(id);
      await load();
    } catch (err: unknown) {
      setError(extractErrorMessage(err, "Silme işlemi başarısız."));
    } finally {
      setSaving(false);
    }
  };

  return (
    <div className="max-w-6xl mx-auto px-4 sm:px-6 lg:px-8 py-10 text-white">
      <div className="flex items-center justify-between mb-6">
        <div>
          <h1 className="text-3xl font-bold">Ürün Yönetimi</h1>
          <p className="text-sm text-slate-300">
            Ürün ekle, düzenle, sil ve stoklarını yönet.
          </p>
        </div>
        <button
          onClick={load}
          disabled={loading}
          className="inline-flex items-center gap-2 px-4 py-2 border border-white/10 rounded-md hover:border-cyan-400 transition disabled:opacity-50"
        >
          <RefreshCcw className="h-4 w-4" />
          Yenile
        </button>
      </div>

      {error && <div className="mb-4 text-red-400">{error}</div>}

      <div className="grid grid-cols-1 lg:grid-cols-3 gap-6">
        <div className="lg:col-span-1">
          <div className="border border-white/10 rounded-2xl bg-slate-900/70 p-4 shadow">
            <div className="flex items-center gap-2 mb-4">
              <Plus className="h-5 w-5 text-cyan-300" />
              <h2 className="text-xl font-semibold">
                {editingId ? "Ürünü düzenle" : "Yeni ürün"}
              </h2>
            </div>
            <div className="space-y-3">
              <label className="space-y-1 block text-sm">
                <span className="text-slate-300">Ürün adı</span>
                <input
                  value={form.name || ""}
                  onChange={(e) => setForm({ ...form, name: e.target.value })}
                  placeholder="Örn: MacBook Air M2"
                  className="w-full px-3 py-2 rounded-md border border-white/10 bg-slate-800 text-white text-sm"
                />
              </label>

              <div className="grid grid-cols-2 gap-3">
                <label className="space-y-1 block text-sm">
                  <span className="text-slate-300">Fiyat (₺)</span>
                  <input
                    type="number"
                    value={form.price ?? 0}
                    onChange={(e) =>
                      setForm({ ...form, price: Number(e.target.value) })
                    }
                    className="w-full px-3 py-2 rounded-md border border-white/10 bg-slate-800 text-white text-sm"
                  />
                </label>
                <label className="space-y-1 block text-sm">
                  <span className="text-slate-300">Stok adedi</span>
                  <input
                    type="number"
                    value={(form as any).stock ?? 0}
                    onChange={(e) =>
                      setForm({ ...form, stock: Number(e.target.value) })
                    }
                    className="w-full px-3 py-2 rounded-md border border-white/10 bg-slate-800 text-white text-sm"
                  />
                </label>
              </div>

              <label className="space-y-1 block text-sm">
                <span className="text-slate-300">Marka</span>
                <input
                  value={form.brand || ""}
                  onChange={(e) => setForm({ ...form, brand: e.target.value })}
                  placeholder="Örn: Apple"
                  className="w-full px-3 py-2 rounded-md border border-white/10 bg-slate-800 text-white text-sm"
                />
              </label>

              <label className="space-y-1 block text-sm">
                <span className="text-slate-300">Kategori</span>
                <select
                  value={(form as any).categoryId || ""}
                  onChange={(e) => setForm({ ...form, categoryId: e.target.value })}
                  className="w-full px-3 py-2 rounded-md border border-white/10 bg-slate-800 text-white text-sm"
                >
                  <option value="">Kategori seç</option>
                  {categoryOptions.map((opt) => (
                    <option key={opt.value} value={opt.value}>
                      {opt.label}
                    </option>
                  ))}
                </select>
              </label>

              <label className="space-y-1 block text-sm">
                <span className="text-slate-300">Açıklama</span>
                <textarea
                  value={form.description || ""}
                  onChange={(e) =>
                    setForm({ ...form, description: e.target.value })
                  }
                  placeholder="Ürün özellikleri, boyut, malzeme vb."
                  className="w-full px-3 py-2 rounded-md border border-white/10 bg-slate-800 text-white text-sm"
                  rows={3}
                />
              </label>

              <label className="space-y-1 block text-sm">
                <span className="text-slate-300">Görsel URL</span>
                <textarea
                  value={(form as any).imageUrl || ""}
                  onChange={(e) => setForm({ ...form, imageUrl: e.target.value })}
                  placeholder="https://..."
                  className="w-full px-3 py-2 rounded-md border border-white/10 bg-slate-800 text-white text-sm"
                  rows={2}
                />
              </label>

              <div className="flex gap-2">
                <button
                  onClick={handleSave}
                  disabled={saving}
                  className="flex-1 px-4 py-2 bg-cyan-500 hover:bg-cyan-400 text-slate-900 font-semibold rounded-md transition disabled:opacity-50"
                >
                  {editingId ? "Güncelle" : "Ekle"}
                </button>
                {editingId && (
                  <button
                    onClick={resetForm}
                    className="px-4 py-2 border border-white/20 rounded-md text-sm"
                  >
                    İptal
                  </button>
                )}
              </div>
            </div>
          </div>
        </div>

        <div className="lg:col-span-2">
          <div className="overflow-x-auto border border-white/10 rounded-2xl bg-slate-900/70 shadow">
            <table className="min-w-full divide-y divide-white/10 text-sm">
              <thead className="bg-white/5">
                <tr>
                  <th className="px-4 py-3 text-left">Ad</th>
                  <th className="px-4 py-3 text-left">Kategori</th>
                  <th className="px-4 py-3 text-left">Fiyat</th>
                  <th className="px-4 py-3 text-left">Stok</th>
                  <th className="px-4 py-3 text-left">Marka</th>
                  <th className="px-4 py-3 text-left w-32">Aksiyon</th>
                </tr>
              </thead>
              <tbody className="divide-y divide-white/5">
                {products.map((p) => (
                  <tr key={p.id}>
                    <td className="px-4 py-3">{p.name}</td>
                    <td className="px-4 py-3">
                      {(p as any).categoryName ||
                        (p as any).category?.name ||
                        "—"}
                    </td>
                    <td className="px-4 py-3">
                      {formatCurrency(Number(p.price))}
                    </td>
                    <td className="px-4 py-3">{(p as any).stock ?? 0}</td>
                    <td className="px-4 py-3">{p.brand || "—"}</td>
                    <td className="px-4 py-3">
                      <div className="flex items-center gap-2">
                        <button
                          onClick={() => startEdit(p)}
                          className="p-2 rounded-md bg-white/10 hover:bg-white/20 transition"
                        >
                          <Edit className="h-4 w-4" />
                        </button>
                        <button
                          onClick={() => handleDelete(p.id)}
                          className="p-2 rounded-md bg-red-500/20 hover:bg-red-500/30 text-red-200 transition"
                          disabled={saving}
                        >
                          <Trash2 className="h-4 w-4" />
                        </button>
                      </div>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
            {loading && (
              <div className="p-4 text-center text-slate-300">Yükleniyor...</div>
            )}
          </div>
        </div>
      </div>
    </div>
  );
}
