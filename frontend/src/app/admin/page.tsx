"use client";

import Link from "next/link";
import { useEffect, useState } from "react";
import { Package, Layers, LayoutGrid } from "lucide-react";
import { ProductService, CategoryService } from "@/services/api";

export default function AdminDashboard() {
  const [productCount, setProductCount] = useState<number | null>(null);
  const [categoryCount, setCategoryCount] = useState<number | null>(null);

  useEffect(() => {
    const load = async () => {
      try {
        const [products, categories] = await Promise.all([
          ProductService.getAll(),
          CategoryService.getAll(),
        ]);
        setProductCount(products.length);
        setCategoryCount(categories.length);
      } catch {
        setProductCount(null);
        setCategoryCount(null);
      }
    };
    load();
  }, []);

  const cards = [
    {
      title: "Ürün Yönetimi",
      desc: "Ürün ekle, düzenle, sil",
      icon: <Package className="h-10 w-10 text-cyan-300" />,
      href: "/admin/products",
      stat: productCount !== null ? `${productCount} ürün` : "—",
    },
    {
      title: "Kategori Yönetimi",
      desc: "Kategorileri yönet",
      icon: <Layers className="h-10 w-10 text-emerald-300" />,
      href: "/admin/categories",
      stat: categoryCount !== null ? `${categoryCount} kategori` : "—",
    },
  ];

  return (
    <div className="max-w-6xl mx-auto px-4 sm:px-6 lg:px-8 py-12 text-white">
      <div className="flex items-center gap-3 mb-8">
        <LayoutGrid className="h-8 w-8 text-cyan-300" />
        <div>
          <h1 className="text-3xl font-bold">Admin Panel</h1>
          <p className="text-sm text-slate-300">Ürün ve kategori yönetimi</p>
        </div>
      </div>

      <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
        {cards.map((card) => (
          <Link
            key={card.title}
            href={card.href}
            className="group rounded-2xl border border-white/10 bg-slate-900/70 p-6 hover:border-cyan-400/60 transition shadow-lg shadow-black/20"
          >
            <div className="flex items-center justify-between mb-4">
              <div className="p-3 rounded-xl bg-white/5 border border-white/10">{card.icon}</div>
              <span className="text-sm text-slate-300">{card.stat}</span>
            </div>
            <h3 className="text-xl font-semibold mb-1 group-hover:text-cyan-200 transition">
              {card.title}
            </h3>
            <p className="text-sm text-slate-300">{card.desc}</p>
          </Link>
        ))}
      </div>
    </div>
  );
}
