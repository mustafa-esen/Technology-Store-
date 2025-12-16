import { useEffect, useState } from "react";

type Lang = "en" | "tr";

export function useLang(defaultLang: Lang = "en") {
  const [lang, setLang] = useState<Lang>(defaultLang);

  useEffect(() => {
    if (typeof window === "undefined") return;
    const stored = (localStorage.getItem("lang") as Lang) || defaultLang;
    setLang(stored);
    const apply = (value: Lang) => {
      setLang(value);
      document.documentElement.setAttribute("lang", value);
    };
    apply(stored);

    const handler = () => {
      const value = (localStorage.getItem("lang") as Lang) || defaultLang;
      apply(value);
    };
    window.addEventListener("lang-change", handler);
    window.addEventListener("storage", handler);
    return () => {
      window.removeEventListener("lang-change", handler);
      window.removeEventListener("storage", handler);
    };
  }, [defaultLang]);

  const updateLang = (value: Lang) => {
    setLang(value);
    if (typeof window !== "undefined") {
      localStorage.setItem("lang", value);
      document.documentElement.setAttribute("lang", value);
      window.dispatchEvent(new Event("lang-change"));
    }
  };

  return { lang, setLang: updateLang };
}
