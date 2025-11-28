"use client";

import { ProductService } from "@/services/api";
import { useEffect, useState } from "react";
import { Search, Filter, Grid3x3, List, ShoppingCart } from "lucide-react";
import Link from "next/link";
import { useLang } from "@/hooks/useLang";

interface Product {
    id: string;
    name: string;
    price: number;
    category: string;
    description: string;
    brand?: string;
}

export default function ProductsPage() {
    const { lang } = useLang("en");
    const copy = {
        en: {
            title: "All Products",
            subtitle: "Discover amazing tech at unbeatable prices",
            searchPlaceholder: "Search products...",
            showing: "Showing",
            products: "products",
            noProductsTitle: "No products found",
            noProductsDesc: "Try adjusting your filters or search terms",
            clearFilters: "Clear Filters",
        },
        tr: {
            title: "Tüm Ürünler",
            subtitle: "Harika teknolojileri en iyi fiyatlarla keşfet",
            searchPlaceholder: "Ürün ara...",
            showing: "Gösterilen",
            products: "ürün",
            noProductsTitle: "Ürün bulunamadı",
            noProductsDesc: "Filtreleri veya arama terimlerini değiştirin",
            clearFilters: "Filtreleri temizle",
        },
    } as const;
    const t = copy[lang];
    const [products, setProducts] = useState<Product[]>([]);
    const [loading, setLoading] = useState(true);
    const [search, setSearch] = useState("");
    const [selectedCategory, setSelectedCategory] = useState("All");
    const [sortBy, setSortBy] = useState("featured");
    const [viewMode, setViewMode] = useState<"grid" | "list">("grid");
    const [error, setError] = useState<string | null>(null);

    useEffect(() => {
        const fetchProducts = async () => {
            try {
                setError(null);
                const data = await ProductService.getAll();
                setProducts(data);
            } catch (error) {
                console.error("Error fetching products:", error);
                setError("Failed to load products");
                setProducts([]);
            } finally {
                setLoading(false);
            }
        };

        fetchProducts();
    }, []);

    const categories = ["All", ...Array.from(new Set(products.map((p) => p.category)))];

    let filteredProducts = products.filter((product) => {
        const matchesSearch = product.name.toLowerCase().includes(search.toLowerCase());
        const matchesCategory = selectedCategory === "All" || product.category === selectedCategory;
        return matchesSearch && matchesCategory;
    });

    filteredProducts = [...filteredProducts].sort((a, b) => {
        switch (sortBy) {
            case "price-low":
                return a.price - b.price;
            case "price-high":
                return b.price - a.price;
            case "name":
                return a.name.localeCompare(b.name);
            case "rating":
                return (b.rating || 0) - (a.rating || 0);
            default:
                return 0;
        }
    });

    return (
        <div className="min-h-screen bg-gradient-to-b from-slate-950 via-slate-900 to-slate-950 text-white">
            <div className="bg-gradient-to-r from-blue-600 to-purple-600 text-white py-12">
                <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
                    <h1 className="text-4xl md:text-5xl font-black mb-4">{t.title}</h1>
                    <p className="text-xl text-blue-100">{t.subtitle}</p>
                </div>
            </div>

            <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
                <div className="bg-slate-900/80 backdrop-blur border border-white/10 rounded-xl shadow-sm p-6 mb-8 space-y-4 text-white card-surface">
                    <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
                        <div className="md:col-span-2 relative">
                            <Search className="absolute left-3 top-1/2 transform -translate-y-1/2 text-slate-400 h-5 w-5" />
                            <input
                                type="text"
                                placeholder={t.searchPlaceholder}
                                value={search}
                                onChange={(e) => setSearch(e.target.value)}
                                className="w-full pl-10 pr-4 py-3 border border-white/10 bg-slate-800 text-white rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500"
                            />
                        </div>

                        <div className="flex items-center gap-2">
                            <Filter className="text-slate-400 h-5 w-5" />
                            <select
                                value={selectedCategory}
                                onChange={(e) => setSelectedCategory(e.target.value)}
                                className="flex-1 px-4 py-3 border border-white/10 bg-slate-800 text-white rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500"
                            >
                                {categories.map((category) => (
                                    <option key={category} value={category}>{category}</option>
                                ))}
                            </select>
                        </div>
                    </div>

                    <div className="flex items-center justify-between pt-4 border-t border-white/10">
                        <p className="text-slate-200">
                            {t.showing} <span className="font-semibold text-white">{filteredProducts.length}</span> {t.products}
                        </p>
                        <div className="flex gap-2">
                            <button
                                onClick={() => setViewMode("grid")}
                                className={`p-2 rounded-lg transition-colors ${viewMode === "grid" ? "bg-blue-600 text-white" : "bg-slate-800 text-slate-300 border border-white/10"}`}
                            >
                                <Grid3x3 className="h-5 w-5" />
                            </button>
                            <button
                                onClick={() => setViewMode("list")}
                                className={`p-2 rounded-lg transition-colors ${viewMode === "list" ? "bg-blue-600 text-white" : "bg-slate-800 text-slate-300 border border-white/10"}`}
                            >
                                <List className="h-5 w-5" />
                            </button>
                        </div>
                    </div>
                </div>

                {loading ? (
                    <div className="flex justify-center items-center h-64">
                        <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-blue-600"></div>
                    </div>
                ) : error ? (
                    <div className="text-center py-16">
                        <h3 className="text-2xl font-bold text-gray-900 mb-2">Cannot load products</h3>
                        <p className="text-gray-600 mb-6">{error}</p>
                        <button
                            onClick={() => location.reload()}
                            className="px-6 py-3 bg-blue-600 text-white rounded-lg font-semibold hover:bg-blue-700"
                        >
                            Retry
                        </button>
                    </div>
                ) : viewMode === "grid" ? (
                    <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4 gap-6">
                        {filteredProducts.map((product) => (
                            <Link
                                key={product.id}
                                href={`/products/${product.id}`}
                                className="group bg-slate-900 border border-white/10 rounded-xl shadow-md hover:shadow-2xl transition-all duration-300 overflow-hidden transform hover:-translate-y-2"
                            >
                                <div className="bg-gradient-to-br from-blue-500 to-purple-600 h-56 flex items-center justify-center">
                                    <span className="text-white text-4xl font-bold">{product.brand || "Tech"}</span>
                                </div>
                                <div className="p-5">
                                    <p className="text-xs font-semibold text-blue-400 mb-1">{product.category}</p>
                                    <h3 className="text-lg font-bold text-white mb-2 line-clamp-2 group-hover:text-blue-300">
                                        {product.name}
                                    </h3>
                                    <p className="text-sm text-slate-300 mb-3 line-clamp-2">{product.description}</p>

                                    <div className="flex items-center justify-between">
                                        <span className="text-2xl font-black text-cyan-300">${product.price.toFixed(2)}</span>
                                        <button className="p-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700">
                                            <ShoppingCart className="h-5 w-5" />
                                        </button>
                                    </div>
                                </div>
                            </Link>
                        ))}
                    </div>
                ) : (
                    <div className="space-y-4">
                        {filteredProducts.map((product) => (
                            <Link
                                key={product.id}
                                href={`/products/${product.id}`}
                                className="group bg-slate-900 border border-white/10 rounded-xl shadow-md hover:shadow-xl transition-all overflow-hidden flex"
                            >
                                <div className="bg-gradient-to-br from-blue-500 to-purple-600 w-64 flex items-center justify-center flex-shrink-0">
                                    <span className="text-white text-4xl font-bold">{product.brand || "Tech"}</span>
                                </div>
                                <div className="p-6 flex-grow flex items-center justify-between">
                                    <div className="flex-grow">
                                        <p className="text-sm font-semibold text-blue-400 mb-1">{product.category}</p>
                                        <h3 className="text-2xl font-bold text-white mb-2">{product.name}</h3>
                                        <p className="text-slate-300 mb-3">{product.description}</p>
                                    </div>
                                    <div className="text-right ml-6">
                                        <p className="text-3xl font-black text-cyan-300 mb-4">${product.price.toFixed(2)}</p>
                                        <button className="px-6 py-3 bg-blue-600 text-white rounded-lg font-semibold hover:bg-blue-700 flex items-center gap-2">
                                            <ShoppingCart className="h-5 w-5" />
                                            Add to Cart
                                        </button>
                                    </div>
                                </div>
                            </Link>
                        ))}
                    </div>
                )}

                {!loading && filteredProducts.length === 0 && (
                    <div className="text-center py-16 text-white">
                        <div className="text-6xl mb-4">🔍</div>
                        <h3 className="text-2xl font-bold mb-2">{t.noProductsTitle}</h3>
                        <p className="text-slate-200 mb-6">{t.noProductsDesc}</p>
                        <button
                            onClick={() => {
                                setSearch("");
                                setSelectedCategory("All");
                            }}
                            className="px-6 py-3 bg-blue-600 text-white rounded-lg font-semibold hover:bg-blue-700"
                        >
                            {t.clearFilters}
                        </button>
                    </div>
                )}
            </div>
        </div>
    );
}
