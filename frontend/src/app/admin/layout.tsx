"use client";

import { useEffect, useState } from "react";
import { useRouter } from "next/navigation";
import { getCurrentUser } from "@/lib/auth";

export default function AdminLayout({ children }: { children: React.ReactNode }) {
  const router = useRouter();
  const [checking, setChecking] = useState(true);
  const [allowed, setAllowed] = useState(false);

  useEffect(() => {
    const user = getCurrentUser();
    const isAdmin = user?.roles?.includes("Admin");
    if (!isAdmin) {
      router.replace("/login");
      setAllowed(false);
    } else {
      setAllowed(true);
    }
    setChecking(false);
  }, [router]);

  if (checking) {
    return (
      <div className="min-h-screen flex items-center justify-center text-muted-foreground">
        Kontrol ediliyor...
      </div>
    );
  }

  if (!allowed) return null;

  return <div className="min-h-screen bg-slate-950 text-white">{children}</div>;
}
