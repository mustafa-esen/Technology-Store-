"use client";

import Link from "next/link";
import { Package, ShoppingBag, CreditCard, Users, LayoutGrid, Layers, MessageSquare } from "lucide-react";

export default function AdminDashboard() {
  // Static stats for demo
  const orderCount = 156;
  const paymentCount = 142;
  const userCount = 89;
  const productCount = 12;
  const categoryCount = 6;
  const reviewCount = 45;

  const cards = [
    {
      title: "Ürün Yönetimi",
      desc: "Ürün ekle, düzenle, sil",
      icon: <Package className="h-10 w-10 text-cyan-300" />,
      href: "/admin/products",
      stat: `${productCount} ürün`,
      color: "cyan",
    },
    {
      title: "Kategori Yönetimi",
      desc: "Kategorileri yönet",
      icon: <Layers className="h-10 w-10 text-emerald-300" />,
      href: "/admin/categories",
      stat: `${categoryCount} kategori`,
      color: "emerald",
    },
    {
      title: "Sipariş Yönetimi",
      desc: "Tüm siparişleri görüntüle ve yönet",
      icon: <ShoppingBag className="h-10 w-10 text-cyan-300" />,
      href: "/admin/orders",
      stat: `${orderCount} sipariş`,
      color: "cyan",
    },
    {
      title: "Ödeme Yönetimi",
      desc: "Ödeme işlemlerini takip et",
      icon: <CreditCard className="h-10 w-10 text-emerald-300" />,
      href: "/admin/payments",
      stat: `${paymentCount} ödeme`,
      color: "emerald",
    },
    {
      title: "Kullanıcı Yönetimi",
      desc: "Kullanıcılar ve roller",
      icon: <Users className="h-10 w-10 text-purple-300" />,
      href: "/admin/users",
      stat: `${userCount} kullanıcı`,
      color: "purple",
    },
    {
      title: "Yorum Yönetimi",
      desc: "Ürün yorumlarını incele / sil",
      icon: <MessageSquare className="h-10 w-10 text-amber-300" />,
      href: "/admin/reviews",
      stat: `${reviewCount} yorum`,
      color: "amber",
    },
  ];

  return (
    <div className="max-w-6xl mx-auto px-4 sm:px-6 lg:px-8 py-12 text-white">
      <div className="flex items-center gap-3 mb-8">
        <LayoutGrid className="h-8 w-8 text-cyan-300" />
        <div>
          <h1 className="text-3xl font-bold">Admin Panel</h1>
          <p className="text-sm text-slate-300">Ürün, kategori, sipariş, ödeme ve kullanıcı yönetimi</p>
        </div>
      </div>

      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
        {cards.map((card) => (
          <Link
            key={card.title}
            href={card.href}
            className={`group rounded-2xl border border-white/10 bg-slate-900/70 p-6 hover:border-${card.color}-400/60 transition shadow-lg shadow-black/20`}
          >
            <div className="flex items-center justify-between mb-4">
              <div className="p-3 rounded-xl bg-white/5 border border-white/10">{card.icon}</div>
              <span className="text-sm text-slate-300">{card.stat}</span>
            </div>
            <h3 className={`text-xl font-semibold mb-1 group-hover:text-${card.color}-200 transition`}>
              {card.title}
            </h3>
            <p className="text-sm text-slate-300">{card.desc}</p>
          </Link>
        ))}
      </div>
    </div>
  );
}
