"use client";

// Placeholder home: no backend calls, just an animated "31" centerpiece.
import { useEffect } from "react";
import { useRouter } from "next/navigation";

export default function Home() {
  const router = useRouter();

  useEffect(() => {
    const timer = setTimeout(() => {
      router.push("/products");
    }, 2000);

    return () => clearTimeout(timer);
  }, [router]);

  return (
    <div className="min-h-screen bg-gradient-to-br from-slate-950 via-slate-900 to-slate-950 flex items-center justify-center text-white relative overflow-hidden">
      <div className="absolute inset-0">
        <div className="absolute -left-24 -top-24 w-72 h-72 bg-cyan-500/20 blur-3xl rounded-full animate-pulse" />
        <div className="absolute -right-24 top-10 w-80 h-80 bg-purple-500/20 blur-3xl rounded-full animate-pulse" />
        <div className="absolute left-1/3 bottom-0 w-96 h-96 bg-blue-500/10 blur-3xl rounded-full animate-pulse" />
      </div>

      <div className="relative flex flex-col items-center gap-6">
        <div className="relative">
          <div className="absolute inset-0 bg-cyan-400/30 blur-3xl rounded-full animate-ping" />
          <div className="text-[200px] md:text-[300px] font-black tracking-tight bg-gradient-to-r from-cyan-300 via-blue-400 to-purple-400 bg-clip-text text-transparent drop-shadow-[0_0_40px_rgba(34,211,238,0.35)] animate-pulse">
            31
          </div>
        </div>
      </div>
    </div>
  );
}
