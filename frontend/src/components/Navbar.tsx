"use client";

import Link from "next/link";
import { useEffect, useState } from "react";
import { ShoppingCart, Menu, X, LogOut } from "lucide-react";
import { cn } from "@/lib/utils";
import { isAuthenticated, getUserEmail, clearAuthData, getCurrentUser } from "@/lib/auth";

export function Navbar() {
  const [isOpen, setIsOpen] = useState(false);
  const [theme, setTheme] = useState<"dark" | "light">("dark");
  const [isAuthed, setIsAuthed] = useState(false);
  const [userEmail, setUserEmail] = useState<string | null>(null);
  const [isAdmin, setIsAdmin] = useState(false);

  const applyTheme = (next: "dark" | "light") => {
    setTheme(next);
    if (typeof document === "undefined") return;
    document.body.classList.remove("theme-dark", "theme-light");
    document.body.classList.add(next === "dark" ? "theme-dark" : "theme-light");
    document.documentElement.classList.toggle("dark", next === "dark");
    localStorage.setItem("theme", next);
  };

  useEffect(() => {
    if (typeof document === "undefined") return;
    const storedTheme = (localStorage.getItem("theme") as "dark" | "light") || "dark";
    applyTheme(storedTheme);

    setIsAuthed(isAuthenticated());
    setUserEmail(getUserEmail());
    const current = getCurrentUser();
    setIsAdmin(current?.roles?.includes("Admin") ?? false);
  }, []);

  const handleLogout = () => {
    clearAuthData();
    setIsAuthed(false);
    setUserEmail(null);
    window.location.href = "/";
  };

  const navClass =
    theme === "dark"
      ? "bg-slate-900/80 backdrop-blur border-b border-white/10 text-white"
      : "bg-white/90 backdrop-blur border-b border-slate-200 text-slate-900 shadow-sm";

  const linkClass =
    theme === "dark"
      ? "border-transparent text-slate-200 hover:border-cyan-300 hover:text-cyan-300 inline-flex items-center px-1 pt-1 border-b-2 text-sm font-medium transition-colors"
      : "border-transparent text-slate-800 hover:border-cyan-600 hover:text-cyan-700 inline-flex items-center px-1 pt-1 border-b-2 text-sm font-medium transition-colors";

  const mobileLinkClass = (active = false) =>
    cn(
      "border-transparent block pl-3 pr-4 py-2 border-l-4 text-base font-medium",
      theme === "dark"
        ? active
          ? "bg-cyan-500/10 border-cyan-400 text-white"
          : "text-slate-300 hover:bg-slate-800 hover:border-cyan-300 hover:text-white"
        : active
          ? "bg-slate-100 border-cyan-600 text-slate-900"
          : "text-slate-800 hover:bg-slate-100 hover:border-cyan-600 hover:text-slate-900"
    );

  return (
    <nav className={cn(navClass, "sticky top-0 z-50")}>
      <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        <div className="flex justify-between h-16">
          <div className="flex">
            <Link href="/" className="flex-shrink-0 flex items-center">
              <span className="text-2xl font-bold text-cyan-400">Tech Store</span>
            </Link>
            <div className="hidden sm:ml-6 sm:flex sm:space-x-8">
              <Link href="/" className={linkClass}>
                Anasayfa
              </Link>
              <Link href="/products" className={linkClass}>
                Ürünler
              </Link>
              {isAdmin && (
                <Link href="/admin" className={linkClass}>
                  Admin Panel
                </Link>
              )}
              {isAuthed && (
                <>
                  <Link href="/orders" className={linkClass}>
                    Siparişler
                  </Link>
                  <Link href="/payments" className={linkClass}>
                    Ödemeler
                  </Link>
                </>
              )}
              {isAuthed ? (
                <span className={linkClass}>{userEmail ?? "Hesap"}</span>
              ) : (
                <Link href="/login" className={linkClass}>
                  Giriş Yap
                </Link>
              )}
            </div>
          </div>
          <div className="hidden sm:ml-6 sm:flex sm:items-center sm:space-x-4">
            <button
              onClick={() => applyTheme(theme === "dark" ? "light" : "dark")}
              className={cn(
                "px-3 py-1 rounded-full border text-sm transition-colors",
                theme === "dark"
                  ? "border-white/20 text-white hover:border-cyan-300"
                  : "border-slate-300 text-slate-800 hover:border-cyan-600"
              )}
            >
              {theme === "dark" ? "Açık" : "Koyu"}
            </button>
            <Link
              href="/cart"
              className={cn(
                "p-2 transition-colors relative",
                theme === "dark" ? "text-slate-200 hover:text-cyan-300" : "text-slate-800 hover:text-cyan-700"
              )}
            >
              <ShoppingCart className="h-6 w-6" />
            </Link>
            {isAuthed ? (
              <button
                onClick={handleLogout}
                className={cn(
                  "p-2 transition-colors",
                  theme === "dark" ? "text-slate-200 hover:text-red-300" : "text-slate-800 hover:text-red-700"
                )}
                title="Çıkış Yap"
              >
                <LogOut className="h-6 w-6" />
              </button>
            ) : (
              <Link
                href="/login"
                className={cn(
                  "p-2 transition-colors",
                  theme === "dark" ? "text-slate-200 hover:text-cyan-300" : "text-slate-800 hover:text-cyan-700"
                )}
              >
                Giriş Yap
              </Link>
            )}
          </div>
          <div className="-mr-2 flex items-center sm:hidden">
            <button
              onClick={() => setIsOpen(!isOpen)}
              className={cn(
                "inline-flex items-center justify-center p-2 rounded-md focus:outline-none focus:ring-2 focus:ring-inset",
                theme === "dark"
                  ? "text-slate-200 hover:text-cyan-300 hover:bg-slate-800 focus:ring-cyan-300"
                  : "text-slate-800 hover:text-cyan-700 hover:bg-slate-100 focus:ring-cyan-600"
              )}
            >
              <span className="sr-only">Menüyü aç</span>
              {isOpen ? <X className="block h-6 w-6" /> : <Menu className="block h-6 w-6" />}
            </button>
          </div>
        </div>
      </div>

      <div
        className={cn(
          "sm:hidden",
          isOpen ? "block" : "hidden",
          theme === "dark" ? "bg-slate-900 text-white" : "bg-white text-slate-900"
        )}
      >
        <div className="pt-2 pb-3 space-y-1">
          <div className="flex items-center gap-2 px-3 pb-2">
            <button
              onClick={() => applyTheme(theme === "dark" ? "light" : "dark")}
              className={cn(
                "px-3 py-1 rounded-full border text-sm",
                theme === "dark"
                  ? "border-white/20 text-white hover:border-cyan-300"
                  : "border-slate-300 text-slate-800 hover:border-cyan-600"
              )}
            >
              {theme === "dark" ? "Açık" : "Koyu"}
            </button>
          </div>
          <Link href="/" className={mobileLinkClass(true)}>
            Anasayfa
          </Link>
          <Link href="/products" className={mobileLinkClass()}>
            Ürünler
          </Link>
          <Link href="/cart" className={mobileLinkClass()}>
            Sepet
          </Link>
          {isAdmin && (
            <Link href="/admin" className={mobileLinkClass()}>
              Admin Panel
            </Link>
          )}
          {isAuthed && (
            <>
              <Link href="/orders" className={mobileLinkClass()}>
                Siparişler
              </Link>
              <Link href="/payments" className={mobileLinkClass()}>
                Ödemeler
              </Link>
            </>
          )}
          {isAuthed ? (
            <button onClick={handleLogout} className={mobileLinkClass()}>
              Çıkış Yap
            </button>
          ) : (
            <Link href="/login" className={mobileLinkClass()}>
              Giriş Yap
            </Link>
          )}
        </div>
      </div>
    </nav>
  );
}
