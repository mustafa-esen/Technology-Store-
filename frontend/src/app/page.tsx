"use client";

import Link from "next/link";
import { useEffect, useState } from "react";
import {
  ArrowRight,
  Laptop,
  Smartphone,
  Headphones,
  Watch,
  Sparkles,
  ShieldCheck,
  Clock3,
} from "lucide-react";
import { ProductService, CategoryService, BasketService } from "@/services/api";
import { getUserId } from "@/lib/auth";
import { extractErrorMessage } from "@/lib/errors";
import { Product, Category } from "@/types";

const copy = {
  badge: "Yeni sezon · Özenle seçilmiş teknoloji",
  heroTitle1: "Kurulumunu hazırla",
  heroTitle2: "premium donanımla",
  heroDesc:
    "Laptop, ses ürünleri, giyilebilir cihazlar ve oyun ekipmanları. Modern tasarım, güvenli ödeme, hızlı teslimat.",
  heroCtaPrimary: "Ürünleri keşfet",
  heroCtaSecondary: "Kurulum oluştur",
  featureTitle: "Öne Çıkanlar",
  featureDesc: "Seçilmiş ürünler, hızlı teslimat",
  categoriesTitle: "Kategoriye göre alışveriş",
  categoriesDesc: "Keskin filtreler ve seçili koleksiyonlar",
  viewAll: "Tümünü gör",
  badgeItems: [
    { icon: ShieldCheck, title: "Güvenli ödeme", desc: "3D Secure, PCI uyumlu" },
    { icon: Clock3, title: "Hızlı teslimat", desc: "Express & takipli kargo" },
    { icon: Sparkles, title: "Yeni koleksiyonlar", desc: "Her hafta taze koleksiyonlar" },
  ],
};

export default function Home() {
  const t = copy;
  const [featured, setFeatured] = useState<Product[]>([]);
  const [categories, setCategories] = useState<Category[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [categoriesError, setCategoriesError] = useState<string | null>(null);
  const [addingId, setAddingId] = useState<string | null>(null);
  const [actionInfo, setActionInfo] = useState<string | null>(null);
  const [actionError, setActionError] = useState<string | null>(null);

  // Merkezi auth fonksiyonundan userId al
  const userId = getUserId() || "";

  useEffect(() => {
    const load = async () => {
      try {
        setError(null);
        const data = await ProductService.getAll();
        setFeatured(data.slice(0, 4));
        const cats = await CategoryService.getAll();
        setCategories(Array.isArray(cats) ? cats.slice(0, 8) : []);
      } catch (err: unknown) {
        console.error(err);
        setError(extractErrorMessage(err, "Öne çıkan ürünler yüklenemedi"));
        setCategoriesError("Kategoriler yüklenemedi");
      } finally {
        setLoading(false);
      }
    };
    load();
  }, []);

  const handleAddToCart = async (product: Product) => {
    setActionInfo(null);
    setActionError(null);
    if (!userId) {
      setActionInfo("Sepete eklemek için giriş yap");
      return;
    }
    setAddingId(product.id);
    try {
      await BasketService.addItem(userId, {
        productId: product.id,
        productName: product.name,
        price: Number(product.price ?? 0),
        quantity: 1,
      });
      setActionInfo("Ürün sepete eklendi");
    } catch (err: unknown) {
      setActionError(extractErrorMessage(err, "Sepete eklenemedi"));
    } finally {
      setAddingId(null);
    }
  };

  return (
    <div className="flex flex-col min-h-screen bg-gradient-to-b from-slate-950 via-slate-900 to-slate-950 text-white">
      {/* Hero */}
      <section className="relative overflow-hidden">
        <div className="absolute inset-0 bg-[radial-gradient(circle_at_20%_20%,rgba(14,165,233,0.2),transparent_25%),radial-gradient(circle_at_80%_0%,rgba(236,72,153,0.18),transparent_25%)]" />
        <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-16 lg:py-24 relative">
          <div className="grid grid-cols-1 lg:grid-cols-2 gap-12 items-center">
            <div className="space-y-8">
              <div className="inline-flex items-center px-4 py-2 bg-white/5 border border-white/10 backdrop-blur rounded-full">
                <Sparkles className="h-4 w-4 text-cyan-300 mr-2" />
                <span className="text-sm text-slate-100">{t.badge}</span>
              </div>
              {actionInfo && (
                <div className="rounded-md bg-emerald-500/10 border border-emerald-500/40 text-emerald-200 px-4 py-2 text-sm">
                  {actionInfo}
                </div>
              )}
              {actionError && (
                <div className="rounded-md bg-red-500/10 border border-red-500/40 text-red-200 px-4 py-2 text-sm">
                  {actionError}
                </div>
              )}
              <div className="space-y-4">
                <h1 className="text-5xl md:text-6xl font-black leading-tight">
                  {t.heroTitle1}
                  <span className="block bg-gradient-to-r from-cyan-300 via-blue-400 to-indigo-400 text-transparent bg-clip-text">
                    {t.heroTitle2}
                  </span>
                </h1>
                <p className="text-lg md:text-xl text-slate-200/80 max-w-2xl">{t.heroDesc}</p>
              </div>
              <div className="flex flex-col sm:flex-row gap-4">
                <Link
                  href="/products"
                  className="group inline-flex items-center justify-center px-8 py-4 text-lg font-semibold rounded-xl bg-white text-slate-900 hover:bg-slate-100 transition-all shadow-lg shadow-cyan-500/20"
                >
                  {t.heroCtaPrimary}
                  <ArrowRight className="ml-2 h-5 w-5 group-hover:translate-x-1 transition-transform" />
                </Link>
                <Link
                  href="/products?category=Laptops"
                  className="inline-flex items-center justify-center px-8 py-4 text-lg font-semibold rounded-xl border border-white/20 hover:border-cyan-300/60 hover:text-cyan-200 transition-colors"
                >
                  {t.heroCtaSecondary}
                </Link>
              </div>
              <div className="grid grid-cols-1 sm:grid-cols-3 gap-4">
                {t.badgeItems.map((item, idx) => (
                  <div key={idx} className="rounded-2xl border border-white/10 bg-white/5 p-4 flex gap-3 items-start">
                    <item.icon className="h-5 w-5 text-cyan-300 mt-0.5" />
                    <div>
                      <p className="font-semibold text-white">{item.title}</p>
                      <p className="text-sm text-slate-200/70">{item.desc}</p>
                    </div>
                  </div>
                ))}
              </div>
            </div>

            <div className="relative">
              <div className="absolute -inset-6 bg-gradient-to-br from-cyan-500/20 via-blue-500/10 to-purple-500/20 blur-3xl" />
              <div className="relative rounded-3xl border border-white/10 bg-white/5 backdrop-blur-xl p-8 shadow-2xl shadow-blue-900/40">
                <div className="grid grid-cols-2 gap-4">
                  {[Laptop, Smartphone, Headphones, Watch].map((Icon, idx) => (
                    <div
                      key={idx}
                      className="rounded-2xl bg-white/5 border border-white/10 p-6 flex items-center justify-center hover:border-cyan-300/60 transition"
                    >
                      <Icon className="h-14 w-14 text-cyan-200" />
                    </div>
                  ))}
                </div>
                <div className="mt-6 rounded-2xl border border-white/10 bg-slate-900/60 p-4 flex items-center gap-3">
                  <Sparkles className="h-5 w-5 text-amber-300" />
                  <div>
                    <p className="font-semibold text-white">Yeni koleksiyonlar</p>
                    <p className="text-sm text-slate-200/70">Her hafta taze paketler ve fırsatlar.</p>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>
      </section>

      {/* Categories */}
      <section className="py-16 bg-slate-900 text-white">
        <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
          <div className="text-center mb-10">
            <h2 className="text-3xl md:text-4xl font-black mb-3">{t.categoriesTitle}</h2>
            <p className="text-lg text-slate-200/80">{t.categoriesDesc}</p>
          </div>

          {categoriesError && (
            <p className="text-center text-sm text-red-400 mb-4">{categoriesError}</p>
          )}
          <div className="grid grid-cols-2 md:grid-cols-4 lg:grid-cols-8 gap-4">
            {(categories.length ? categories : []).map((category, idx) => {
              const colors = [
                "bg-blue-500",
                "bg-purple-500",
                "bg-pink-500",
                "bg-green-500",
                "bg-orange-500",
                "bg-red-500",
                "bg-indigo-500",
                "bg-teal-500",
              ];
              const color = colors[idx % colors.length];
              const name = category.name || "Kategori";
              const desc = category.description || "";
              return (
                <Link key={category.id ?? idx} href={`/products?category=${encodeURIComponent(name)}`} className="group">
                  <div className="bg-slate-800 rounded-2xl p-6 text-center hover:shadow-xl transition-all duration-300 transform hover:-translate-y-2 border-2 border-transparent hover:border-cyan-300/50">
                    <div
                      className={`${color} w-16 h-16 rounded-xl mx-auto mb-4 flex items-center justify-center transform group-hover:rotate-6 transition-transform duration-300`}
                    >
                      <Laptop className="h-8 w-8 text-white" />
                    </div>
                    <h3 className="font-bold text-white text-sm mb-1">{name}</h3>
                    <p className="text-xs text-slate-300 line-clamp-2">{desc || t.categoriesDesc}</p>
                  </div>
                </Link>
              );
            })}
          </div>
        </div>
      </section>

      {/* Featured */}
      <section className="py-16 bg-slate-900 text-white">
        <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
          <div className="flex items-center justify-between mb-10">
            <div>
              <h2 className="text-4xl md:text-5xl font-black mb-2">{t.featureTitle}</h2>
              <p className="text-lg text-slate-200/70">{t.featureDesc}</p>
            </div>
            <Link
              href="/products"
              className="hidden md:inline-flex items-center text-cyan-300 hover:text-cyan-200 font-bold text-lg group"
            >
              {t.viewAll}
              <ArrowRight className="ml-2 h-5 w-5 group-hover:translate-x-1 transition-transform" />
            </Link>
          </div>

          {loading ? (
            <div className="flex justify-center items-center h-40">
              <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-cyan-400"></div>
            </div>
          ) : error ? (
            <div className="text-center text-slate-200">{error}</div>
          ) : (
            <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6">
              {featured.map((product, idx) => (
                <div
                  key={product.id ?? idx}
                  className="group bg-slate-900 rounded-2xl border border-white/10 hover:border-cyan-300/40 transition-all duration-300 overflow-hidden transform hover:-translate-y-2 shadow-xl shadow-cyan-900/30"
                >
                  <div className="relative">
                    <div className="bg-gradient-to-br from-blue-500 to-purple-600 p-10 flex items-center justify-center h-64 relative overflow-hidden">
                      <div className="absolute inset-0 bg-black/10"></div>
                      <span className="text-5xl font-black text-white relative z-10">
                        {product.brand || "Marka"}
                      </span>
                    </div>
                  </div>

                  <div className="p-6">
                    <h3 className="text-lg font-bold text-white mb-2 group-hover:text-cyan-200 transition-colors">
                      {product.name}
                    </h3>

                    <div className="flex items-baseline gap-2 mb-4">
                      <span className="text-3xl font-black text-cyan-300">₺{Number(product.price).toFixed(2)}</span>
                    </div>

                    <div className="flex items-center justify-between">
                      <button
                        onClick={() => handleAddToCart(product)}
                        disabled={addingId === product.id}
                        className="flex-1 py-3 bg-white text-slate-900 rounded-xl font-semibold hover:bg-slate-100 transition-all duration-300 shadow-md hover:shadow-lg transform hover:-translate-y-0.5 disabled:opacity-50"
                      >
                        Sepete Ekle
                      </button>
                      <div className="ml-3 text-xs px-3 py-1 rounded-full bg-white/10 border border-white/10 text-slate-200">
                        Hızlı kargo
                      </div>
                    </div>
                  </div>
                </div>
              ))}
            </div>
          )}

          <div className="mt-10 text-center md:hidden">
            <Link href="/products" className="inline-flex items-center text-cyan-300 hover:text-cyan-200 font-bold text-lg">
              Tüm Ürünleri Gör
              <ArrowRight className="ml-2 h-5 w-5" />
            </Link>
          </div>
        </div>
      </section>
    </div>
  );
}
