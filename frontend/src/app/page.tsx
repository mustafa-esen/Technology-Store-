"use client";

import Link from "next/link";
import { useEffect, useState } from "react";
import {
  ArrowRight,
  Laptop,
  Smartphone,
  Headphones,
  Watch,
  Monitor,
  Camera,
  Gamepad2,
  Speaker,
  TrendingUp,
  Sparkles,
  ShieldCheck,
  Clock3,
} from "lucide-react";
import { ProductService } from "@/services/api";
import { useLang } from "@/hooks/useLang";

const translations = {
  en: {
    badge: "New season • Fresh tech picks",
    heroTitle1: "Craft your setup with",
    heroTitle2: "precision hardware",
    heroDesc:
      "Laptops, audio, wearables, and gaming gear picked for builders, creators, and players. Modern design, secure checkout, fast delivery.",
    heroCtaPrimary: "Browse products",
    heroCtaSecondary: "Build your setup",
    featureTitle: "Featured Gear",
    featureDesc: "Curated picks, fast shipping",
    categoriesTitle: "Shop by Category",
    categoriesDesc: "Sharp filters and selected collections",
    viewAll: "View all",
    badgeItems: [
      { icon: ShieldCheck, title: "Secure checkout", desc: "3D Secure, PCI compliant" },
      { icon: Clock3, title: "Fast delivery", desc: "Express & tracked shipping" },
      { icon: Sparkles, title: "Fresh drops", desc: "Weekly curated bundles" },
    ],
    categoriesList: [
      { name: "Laptops", icon: Laptop, color: "bg-blue-500", count: "128+" },
      { name: "Phones", icon: Smartphone, color: "bg-purple-500", count: "95+" },
      { name: "Audio", icon: Headphones, color: "bg-pink-500", count: "67+" },
      { name: "Wearables", icon: Watch, color: "bg-green-500", count: "45+" },
      { name: "Monitors", icon: Monitor, color: "bg-orange-500", count: "82+" },
      { name: "Cameras", icon: Camera, color: "bg-red-500", count: "53+" },
      { name: "Gaming", icon: Gamepad2, color: "bg-indigo-500", count: "71+" },
      { name: "Speakers", icon: Speaker, color: "bg-teal-500", count: "39+" },
    ],
  },
  tr: {
    badge: "Yeni sezon • Özenle seçilmiş teknoloji",
    heroTitle1: "Kurulumunu hazırla",
    heroTitle2: "premium donanımla",
    heroDesc:
      "Laptop, ses ürünleri, giyilebilir cihazlar ve oyun ekipmanları. Modern tasarım, güvenli ödeme, hızlı teslimat.",
    heroCtaPrimary: "Ürünleri keşfet",
    heroCtaSecondary: "Kurulum oluştur",
    featureTitle: "Öne Çıkanlar",
    featureDesc: "Kürasyonlu seçimler, hızlı teslimat",
    categoriesTitle: "Kategoriye göre alışveriş",
    categoriesDesc: "Keskin filtreler ve seçili koleksiyonlar",
    viewAll: "Tümünü gör",
    badgeItems: [
      { icon: ShieldCheck, title: "Güvenli ödeme", desc: "3D Secure, PCI uyumlu" },
      { icon: Clock3, title: "Hızlı teslimat", desc: "Express & takipli kargo" },
      { icon: Sparkles, title: "Yeni koleksiyonlar", desc: "Her hafta taze koleksiyon ve paketler" },
    ],
    categoriesList: [
      { name: "Laptoplar", icon: Laptop, color: "bg-blue-500", count: "128+" },
      { name: "Telefonlar", icon: Smartphone, color: "bg-purple-500", count: "95+" },
      { name: "Ses", icon: Headphones, color: "bg-pink-500", count: "67+" },
      { name: "Giyilebilir", icon: Watch, color: "bg-green-500", count: "45+" },
      { name: "Monitörler", icon: Monitor, color: "bg-orange-500", count: "82+" },
      { name: "Kameralar", icon: Camera, color: "bg-red-500", count: "53+" },
      { name: "Gaming", icon: Gamepad2, color: "bg-indigo-500", count: "71+" },
      { name: "Hoparlör", icon: Speaker, color: "bg-teal-500", count: "39+" },
    ],
  },
};

export default function Home() {
  const { lang } = useLang("en");
  const t = translations[lang];
  const [featured, setFeatured] = useState<any[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    const load = async () => {
      try {
        setError(null);
        const data = await ProductService.getAll();
        setFeatured(data.slice(0, 4));
      } catch (err) {
        console.error(err);
        setError("Failed to load featured products");
      } finally {
        setLoading(false);
      }
    };
    load();
  }, []);

  return (
    <div className="flex flex-col min-h-screen bg-gradient-to-b from-slate-950 via-slate-900 to-slate-950 text-white">
      {/* Hero */}
      <section className="relative overflow-hidden">
        <div className="absolute inset-0 bg-[radial-gradient(circle_at_20%_20%,rgba(14,165,233,0.2),transparent_25%),radial-gradient(circle_at_80%_0%,rgba(236,72,153,0.18),transparent_25%)]" />
        <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-16 lg:py-24 relative">
          <div className="grid grid-cols-1 lg:grid-cols-2 gap-12 items-center">
            <div className="space-y-8">
              <div className="inline-flex items-center px-4 py-2 bg-white/5 border border-white/10 backdrop-blur rounded-full">
                <TrendingUp className="h-4 w-4 text-cyan-300 mr-2" />
                <span className="text-sm text-slate-100">{t.badge}</span>
              </div>
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
                  href="/products?category=computers"
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
                    <p className="font-semibold text-white">Fresh drops</p>
                    <p className="text-sm text-slate-200/70">Weekly curated collections and bundles.</p>
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

          <div className="grid grid-cols-2 md:grid-cols-4 lg:grid-cols-8 gap-4">
            {t.categoriesList.map((category, idx) => (
              <Link key={idx} href={`/products?category=${category.name.toLowerCase()}`} className="group">
                <div className="bg-slate-800 rounded-2xl p-6 text-center hover:shadow-xl transition-all duration-300 transform hover:-translate-y-2 border-2 border-transparent hover:border-cyan-300/50">
                  <div className={`${category.color} w-16 h-16 rounded-xl mx-auto mb-4 flex items-center justify-center transform group-hover:rotate-6 transition-transform duration-300`}>
                    <category.icon className="h-8 w-8 text-white" />
                  </div>
                  <h3 className="font-bold text-white text-sm mb-1">{category.name}</h3>
                  <p className="text-xs text-slate-300">{category.count} items</p>
                </div>
              </Link>
            ))}
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
                        {product.brand || "Tech"}
                      </span>
                    </div>
                  </div>

                  <div className="p-6">
                    <h3 className="text-lg font-bold text-white mb-2 group-hover:text-cyan-200 transition-colors">
                      {product.name}
                    </h3>

                    <div className="flex items-baseline gap-2 mb-4">
                      <span className="text-3xl font-black text-cyan-300">${Number(product.price).toFixed(2)}</span>
                    </div>

                    <div className="flex items-center justify-between">
                      <button className="flex-1 py-3 bg-white text-slate-900 rounded-xl font-semibold hover:bg-slate-100 transition-all duration-300 shadow-md hover:shadow-lg transform hover:-translate-y-0.5">
                        Add to Cart
                      </button>
                      <div className="ml-3 text-xs px-3 py-1 rounded-full bg-white/10 border border-white/10 text-slate-200">
                        Fast ship
                      </div>
                    </div>
                  </div>
                </div>
              ))}
            </div>
          )}

          <div className="mt-10 text-center md:hidden">
            <Link href="/products" className="inline-flex items-center text-cyan-300 hover:text-cyan-200 font-bold text-lg">
              View All Products
              <ArrowRight className="ml-2 h-5 w-5" />
            </Link>
          </div>
        </div>
      </section>
    </div>
  );
}
