"use client";

import { useEffect, useState } from "react";
import { CategoryService } from "@/services/api";
import { Category } from "@/types";
import { extractErrorMessage } from "@/lib/errors";
import { Plus, Edit, Trash2, RefreshCcw } from "lucide-react";

const emptyCategory: Partial<Category> = {
  name: "",
  description: "",
};

export default function AdminCategoriesPage() {
  const [categories, setCategories] = useState<Category[]>([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [form, setForm] = useState<Partial<Category>>(emptyCategory);
  const [editingId, setEditingId] = useState<string | null>(null);
  const [saving, setSaving] = useState(false);

  const load = async () => {
    setLoading(true);
    setError(null);
    try {
      const cats = await CategoryService.getAll();
      setCategories(cats);
    } catch (err: unknown) {
      setError(extractErrorMessage(err, "Kategoriler yüklenemedi."));
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    void load();
  }, []);

  const resetForm = () => {
    setForm(emptyCategory);
    setEditingId(null);
  };

  const startEdit = (c: Category) => {
    setEditingId(c.id);
    setForm({ name: c.name, description: c.description });
  };

  const handleSave = async () => {
    if (!form.name) {
      setError("Kategori adı gerekli.");
      return;
    }
    setSaving(true);
    setError(null);
    try {
      if (editingId) {
        await CategoryService.update(editingId, form);
      } else {
        await CategoryService.create(form);
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
      await CategoryService.delete(id);
      await load();
    } catch (err: unknown) {
      setError(extractErrorMessage(err, "Silme işlemi başarısız."));
    } finally {
      setSaving(false);
    }
  };

  return (
    <div className="max-w-5xl mx-auto px-4 sm:px-6 lg:px-8 py-10 text-white">
      <div className="flex items-center justify-between mb-6">
        <div>
          <h1 className="text-3xl font-bold">Kategori Yönetimi</h1>
          <p className="text-sm text-slate-300">Kategori ekle, düzenle, sil.</p>
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
              <h2 className="text-xl font-semibold">{editingId ? "Kategoriyi Düzenle" : "Yeni Kategori"}</h2>
            </div>
            <div className="space-y-3">
              <input
                value={form.name || ""}
                onChange={(e) => setForm({ ...form, name: e.target.value })}
                placeholder="Kategori adı"
                className="w-full px-3 py-2 rounded-md border border-white/10 bg-slate-800 text-white text-sm"
              />
              <textarea
                value={form.description || ""}
                onChange={(e) => setForm({ ...form, description: e.target.value })}
                placeholder="Açıklama"
                className="w-full px-3 py-2 rounded-md border border-white/10 bg-slate-800 text-white text-sm"
                rows={3}
              />
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
                  <th className="px-4 py-3 text-left">Açıklama</th>
                  <th className="px-4 py-3 text-left w-28">Aksiyon</th>
                </tr>
              </thead>
              <tbody className="divide-y divide-white/5">
                {categories.map((c) => (
                  <tr key={c.id}>
                    <td className="px-4 py-3">{c.name}</td>
                    <td className="px-4 py-3">{c.description || "—"}</td>
                    <td className="px-4 py-3">
                      <div className="flex items-center gap-2">
                        <button
                          onClick={() => startEdit(c)}
                          className="p-2 rounded-md bg-white/10 hover:bg-white/20 transition"
                        >
                          <Edit className="h-4 w-4" />
                        </button>
                        <button
                          onClick={() => handleDelete(c.id)}
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
            {loading && <div className="p-4 text-center text-slate-300">Yükleniyor...</div>}
          </div>
        </div>
      </div>
    </div>
  );
}
