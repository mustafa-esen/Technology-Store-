"use client";

import React from "react";

export default function CartPage() {
  return (
    <div className="min-h-screen flex items-center justify-center bg-black overflow-hidden relative">
      <div className="absolute inset-0 bg-[url('https://media.giphy.com/media/26tOZ42Mg6pbTUPVS/giphy.gif')] opacity-10 bg-cover bg-center pointer-events-none"></div>

      <div className="relative z-10 text-center p-8 border-4 border-red-600 border-dashed rounded-3xl bg-black/80 backdrop-blur-sm transform rotate-[-2deg] hover:rotate-2 transition-transform duration-300">
        <h1 className="text-4xl md:text-7xl font-black text-transparent bg-clip-text bg-gradient-to-br from-yellow-400 via-red-500 to-purple-600 animate-pulse tracking-tighter leading-tight drop-shadow-2xl">
          MUSTİYİ GÖTTEN
          <br />
          SİKEYİM
        </h1>
        <p className="mt-6 text-xl md:text-3xl font-mono text-cyan-300 font-bold tracking-widest animate-bounce">
          BU DA BENİM SERSERİ
          <br />
          SERBEST STİLİM
        </p>
        <div className="mt-8 text-6xl animate-spin inline-block">
          🖕
        </div>
      </div>
    </div>
  );
}
