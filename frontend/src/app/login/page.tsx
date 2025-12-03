"use client";

import { useState } from "react";
import Link from "next/link";
import { useRouter } from "next/navigation";
import { LogIn } from "lucide-react";
import { AuthService } from "@/services/api";
import { saveAuthData } from "@/lib/auth";
import { extractErrorMessage } from "@/lib/errors";

const texts = {
  tr: {
    title: "Hesabına giriş yap",
    subtitle: "Ya da yeni bir hesap oluştur",
    email: "E-posta adresi",
    password: "Şifre",
    remember: "Beni hatırla",
    forgot: "Şifreni mi unuttun?",
    submit: "Giriş yap",
    loading: "Giriş yapılıyor...",
    error: "E-posta veya şifre hatalı",
    createLink: "yeni hesap oluştur",
  },
};

export default function LoginPage() {
  const router = useRouter();
  const t = texts.tr;
  const [formData, setFormData] = useState({
    email: "",
    password: "",
  });
  const [error, setError] = useState("");
  const [loading, setLoading] = useState(false);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError("");
    setLoading(true);

    try {
      const res = await AuthService.login(formData);
      console.log("LOGIN RESPONSE:", res);
      console.log("LOGIN RESPONSE KEYS:", Object.keys(res));
      saveAuthData(res);
      console.log("SAVED userId:", localStorage.getItem("userId"));
      router.push("/");
    } catch (err: unknown) {
      setError(extractErrorMessage(err, t.error));
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="min-h-[calc(100vh-200px)] flex items-center justify-center py-12 px-4 sm:px-6 lg:px-8 bg-gradient-to-b from-slate-950 via-slate-900 to-slate-950 text-white">
      <div className="max-w-md w-full space-y-8 card-surface border border-white/10 rounded-2xl p-8 shadow-xl">
        <div>
          <div className="flex justify-center">
            <LogIn className="h-12 w-12 text-cyan-300" />
          </div>
          <h2 className="mt-6 text-center text-3xl font-extrabold">{t.title}</h2>
          <p className="mt-2 text-center text-sm text-slate-300">
            {t.subtitle}{" "}
            <Link href="/register" className="font-medium text-cyan-300 hover:text-cyan-200">
              {t.createLink}
            </Link>
          </p>
        </div>
        <form className="mt-8 space-y-6" onSubmit={handleSubmit}>
          {error && (
            <div className="rounded-md bg-red-500/10 border border-red-500/50 p-4 text-sm text-red-200">
              {error}
            </div>
          )}
          <div className="rounded-md shadow-sm space-y-4">
            <div>
              <label htmlFor="email" className="sr-only">
                {t.email}
              </label>
              <input
                id="email"
                name="email"
                type="email"
                autoComplete="email"
                required
                value={formData.email}
                onChange={(e) => setFormData({ ...formData, email: e.target.value })}
                className="block w-full px-3 py-2 border border-white/10 bg-slate-900/70 placeholder-slate-400 text-white rounded-md focus:outline-none focus:ring-cyan-400 focus:border-cyan-400 sm:text-sm"
                placeholder={t.email}
              />
            </div>
            <div>
              <label htmlFor="password" className="sr-only">
                {t.password}
              </label>
              <input
                id="password"
                name="password"
                type="password"
                autoComplete="current-password"
                required
                value={formData.password}
                onChange={(e) => setFormData({ ...formData, password: e.target.value })}
                className="block w-full px-3 py-2 border border-white/10 bg-slate-900/70 placeholder-slate-400 text-white rounded-md focus:outline-none focus:ring-cyan-400 focus:border-cyan-400 sm:text-sm"
                placeholder={t.password}
              />
            </div>
          </div>

          <div className="flex items-center justify-between text-sm text-slate-300">
            <div className="flex items-center">
              <input
                id="remember-me"
                name="remember-me"
                type="checkbox"
                className="h-4 w-4 text-cyan-400 focus:ring-cyan-400 border-white/20 rounded bg-slate-900"
              />
              <label htmlFor="remember-me" className="ml-2">
                {t.remember}
              </label>
            </div>

            <div>
              <a href="#" className="font-medium text-cyan-300 hover:text-cyan-200">
                {t.forgot}
              </a>
            </div>
          </div>

          <div>
            <button
              type="submit"
              disabled={loading}
              className="w-full flex justify-center py-2 px-4 border border-transparent text-sm font-medium rounded-md text-slate-900 bg-cyan-300 hover:bg-cyan-200 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-cyan-400 disabled:opacity-50 disabled:cursor-not-allowed"
            >
              {loading ? t.loading : t.submit}
            </button>
          </div>
        </form>
      </div>
    </div>
  );
}
