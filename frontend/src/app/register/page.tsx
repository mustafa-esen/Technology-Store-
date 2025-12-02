"use client";

import { useState } from "react";
import Link from "next/link";
import { useRouter } from "next/navigation";
import { UserPlus } from "lucide-react";
import { AuthService } from "@/services/api";
import { saveAuthData } from "@/lib/auth";
import { extractErrorMessage } from "@/lib/errors";

const texts = {
  tr: {
    title: "Hesap oluştur",
    subtitle: "Zaten hesabın var mı?",
    signIn: "Giriş yap",
    firstName: "Ad",
    lastName: "Soyad",
    email: "E-posta adresi",
    password: "Şifre",
    confirm: "Şifreyi doğrula",
    submit: "Hesap oluştur",
    loading: "Hesap oluşturuluyor...",
    errorMismatch: "Şifreler eşleşmiyor",
    errorShort: "Şifre en az 6 karakter olmalı",
    errorGeneral: "Kayıt başarısız. Lütfen tekrar dene.",
  },
};

export default function RegisterPage() {
  const router = useRouter();
  const t = texts.tr;
  const [formData, setFormData] = useState({
    firstName: "",
    lastName: "",
    email: "",
    password: "",
    confirmPassword: "",
  });
  const [error, setError] = useState("");
  const [loading, setLoading] = useState(false);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError("");

    if (formData.password !== formData.confirmPassword) {
      setError(t.errorMismatch);
      return;
    }

    if (formData.password.length < 6) {
      setError(t.errorShort);
      return;
    }

    setLoading(true);

    try {
      const res = await AuthService.register({
        firstName: formData.firstName.trim(),
        lastName: formData.lastName.trim(),
        email: formData.email,
        password: formData.password,
      });

      // Eğer API token döndürüyorsa direkt giriş yap
      const hasToken = res?.token || res?.accessToken;
      if (hasToken) {
        saveAuthData(res);
        router.push("/");
      } else {
        router.push("/login");
      }
    } catch (err: unknown) {
      setError(extractErrorMessage(err, t.errorGeneral));
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="min-h-[calc(100vh-200px)] flex items-center justify-center py-12 px-4 sm:px-6 lg:px-8 bg-gradient-to-b from-slate-950 via-slate-900 to-slate-950 text-white">
      <div className="max-w-md w-full space-y-8 card-surface border border-white/10 rounded-2xl p-8 shadow-xl">
        <div>
          <div className="flex justify-center">
            <UserPlus className="h-12 w-12 text-cyan-300" />
          </div>
          <h2 className="mt-6 text-center text-3xl font-extrabold">{t.title}</h2>
          <p className="mt-2 text-center text-sm text-slate-300">
            {t.subtitle}{" "}
            <Link href="/login" className="font-medium text-cyan-300 hover:text-cyan-200">
              {t.signIn}
            </Link>
          </p>
        </div>
        <form className="mt-8 space-y-6" onSubmit={handleSubmit}>
          {error && (
            <div className="rounded-md bg-red-500/10 border border-red-500/50 p-4">
              <p className="text-sm text-red-200">{error}</p>
            </div>
          )}
          <div className="rounded-md shadow-sm space-y-4">
            <div className="grid grid-cols-1 md:grid-cols-2 gap-3">
              <div>
                <label htmlFor="firstName" className="sr-only">
                  {t.firstName}
                </label>
                <input
                  id="firstName"
                  name="firstName"
                  type="text"
                  required
                  value={formData.firstName}
                  onChange={(e) => setFormData({ ...formData, firstName: e.target.value })}
                  className="block w-full px-3 py-2 border border-white/10 bg-slate-900/70 placeholder-slate-400 text-white rounded-md focus:outline-none focus:ring-cyan-400 focus:border-cyan-400 sm:text-sm"
                  placeholder={t.firstName}
                />
              </div>
              <div>
                <label htmlFor="lastName" className="sr-only">
                  {t.lastName}
                </label>
                <input
                  id="lastName"
                  name="lastName"
                  type="text"
                  required
                  value={formData.lastName}
                  onChange={(e) => setFormData({ ...formData, lastName: e.target.value })}
                  className="block w-full px-3 py-2 border border-white/10 bg-slate-900/70 placeholder-slate-400 text-white rounded-md focus:outline-none focus:ring-cyan-400 focus:border-cyan-400 sm:text-sm"
                  placeholder={t.lastName}
                />
              </div>
            </div>
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
                autoComplete="new-password"
                required
                value={formData.password}
                onChange={(e) => setFormData({ ...formData, password: e.target.value })}
                className="block w-full px-3 py-2 border border-white/10 bg-slate-900/70 placeholder-slate-400 text-white rounded-md focus:outline-none focus:ring-cyan-400 focus:border-cyan-400 sm:text-sm"
                placeholder={t.password}
              />
            </div>
            <div>
              <label htmlFor="confirm-password" className="sr-only">
                {t.confirm}
              </label>
              <input
                id="confirm-password"
                name="confirm-password"
                type="password"
                autoComplete="new-password"
                required
                value={formData.confirmPassword}
                onChange={(e) => setFormData({ ...formData, confirmPassword: e.target.value })}
                className="block w-full px-3 py-2 border border-white/10 bg-slate-900/70 placeholder-slate-400 text-white rounded-md focus:outline-none focus:ring-cyan-400 focus:border-cyan-400 sm:text-sm"
                placeholder={t.confirm}
              />
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
