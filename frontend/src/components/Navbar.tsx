"use client";

import Link from "next/link";
import { ShoppingCart, Menu, X, User } from "lucide-react";
import { useEffect, useState } from "react";
import { cn } from "@/lib/utils";
import { useLang } from "@/hooks/useLang";

export function Navbar() {
    const [isOpen, setIsOpen] = useState(false);
    const [theme, setTheme] = useState<"dark" | "light">("dark");
    const { lang, setLang } = useLang("en");

    const applyTheme = (nextTheme: "dark" | "light") => {
        setTheme(nextTheme);
        if (typeof document === "undefined") return;
        document.body.classList.remove("theme-dark", "theme-light");
        document.body.classList.add(nextTheme === "dark" ? "theme-dark" : "theme-light");
        localStorage.setItem("theme", nextTheme);
    };

    useEffect(() => {
        if (typeof document === "undefined") return;
        const storedTheme = (localStorage.getItem("theme") as "dark" | "light") || "dark";
        setTheme(storedTheme);
        document.body.classList.remove("theme-dark", "theme-light");
        document.body.classList.add(storedTheme === "dark" ? "theme-dark" : "theme-light");
    }, []);

    const navClass =
        theme === "dark"
            ? "bg-slate-900/80 backdrop-blur border-b border-white/10 text-white"
            : "bg-white/90 backdrop-blur border-b border-slate-200 text-slate-900 shadow-sm";

    const linkClass =
        theme === "dark"
            ? "border-transparent text-slate-200 hover:border-cyan-300 hover:text-cyan-300 inline-flex items-center px-1 pt-1 border-b-2 text-sm font-medium transition-colors"
            : "border-transparent text-slate-800 hover:border-cyan-600 hover:text-cyan-700 inline-flex items-center px-1 pt-1 border-b-2 text-sm font-medium transition-colors";

    return (
        <nav className={cn(navClass, "sticky top-0 z-50")}>
            <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
                <div className="flex justify-between h-16">
                    <div className="flex">
                        <Link href="/" className="flex-shrink-0 flex items-center">
                            <span className="text-2xl font-bold text-cyan-400">TechStore</span>
                        </Link>
                        <div className="hidden sm:ml-6 sm:flex sm:space-x-8">
                            <Link href="/" className={linkClass}>
                                Home
                            </Link>
                            <Link href="/products" className={linkClass}>
                                Products
                            </Link>
                            <Link href="/login" className={linkClass}>
                                Login
                            </Link>
                        </div>
                    </div>
                    <div className="hidden sm:ml-6 sm:flex sm:items-center sm:space-x-4">
                        <div className="flex items-center gap-2 mr-3">
                            <button
                                onClick={() => applyTheme(theme === "dark" ? "light" : "dark")}
                                className={cn(
                                    "px-3 py-1 rounded-full border text-sm transition-colors",
                                    theme === "dark"
                                        ? "border-white/20 text-white hover:border-cyan-300"
                                        : "border-slate-300 text-slate-800 hover:border-cyan-600"
                                )}
                            >
                                {theme === "dark" ? "Light" : "Dark"}
                            </button>
                            <button
                                onClick={() => setLang(lang === "en" ? "tr" : "en")}
                                className={cn(
                                    "px-3 py-1 rounded-full border text-sm transition-colors",
                                    theme === "dark"
                                        ? "border-white/20 text-white hover:border-cyan-300"
                                        : "border-slate-300 text-slate-800 hover:border-cyan-600"
                                )}
                            >
                                {lang.toUpperCase()}
                            </button>
                        </div>
                        <Link
                            href="/cart"
                            className={cn(
                                "p-2 transition-colors relative",
                                theme === "dark" ? "text-slate-200 hover:text-cyan-300" : "text-slate-800 hover:text-cyan-700"
                            )}
                        >
                            <ShoppingCart className="h-6 w-6" />
                        </Link>
                        <Link
                            href="/login"
                            className={cn(
                                "p-2 transition-colors",
                                theme === "dark" ? "text-slate-200 hover:text-cyan-300" : "text-slate-800 hover:text-cyan-700"
                            )}
                        >
                            <User className="h-6 w-6" />
                        </Link>
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
                            <span className="sr-only">Open main menu</span>
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
                            {theme === "dark" ? "Light" : "Dark"}
                        </button>
                            <button
                            onClick={() => setLang(lang === "en" ? "tr" : "en")}
                            className={cn(
                                "px-3 py-1 rounded-full border text-sm",
                                theme === "dark"
                                    ? "border-white/20 text-white hover:border-cyan-300"
                                    : "border-slate-300 text-slate-800 hover:border-cyan-600"
                            )}
                        >
                            {lang.toUpperCase()}
                        </button>
                    </div>
                    <Link
                        href="/"
                        className={cn(
                            "block pl-3 pr-4 py-2 border-l-4 text-base font-medium",
                            theme === "dark"
                                ? "bg-cyan-500/10 border-cyan-400 text-white"
                                : "bg-slate-100 border-cyan-600 text-slate-900"
                        )}
                    >
                        Home
                    </Link>
                    <Link
                        href="/products"
                        className={cn(
                            "border-transparent block pl-3 pr-4 py-2 border-l-4 text-base font-medium",
                            theme === "dark"
                                ? "text-slate-300 hover:bg-slate-800 hover:border-cyan-300 hover:text-white"
                                : "text-slate-800 hover:bg-slate-100 hover:border-cyan-600 hover:text-slate-900"
                        )}
                    >
                        Products
                    </Link>
                    <Link
                        href="/cart"
                        className={cn(
                            "border-transparent block pl-3 pr-4 py-2 border-l-4 text-base font-medium",
                            theme === "dark"
                                ? "text-slate-300 hover:bg-slate-800 hover:border-cyan-300 hover:text-white"
                                : "text-slate-800 hover:bg-slate-100 hover:border-cyan-600 hover:text-slate-900"
                        )}
                    >
                        Cart
                    </Link>
                    <Link
                        href="/login"
                        className={cn(
                            "border-transparent block pl-3 pr-4 py-2 border-l-4 text-base font-medium",
                            theme === "dark"
                                ? "text-slate-300 hover:bg-slate-800 hover:border-cyan-300 hover:text-white"
                                : "text-slate-800 hover:bg-slate-100 hover:border-cyan-600 hover:text-slate-900"
                        )}
                    >
                        Login
                    </Link>
                </div>
            </div>
        </nav>
    );
}
