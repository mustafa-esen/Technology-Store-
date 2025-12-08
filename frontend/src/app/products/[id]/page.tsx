"use client";

import { useParams } from "next/navigation";
import { useState, useEffect } from "react";
import Link from "next/link";
import { Star, ShoppingCart, Heart, Share2, Truck, Shield, ArrowLeft, Check, ChevronRight, Zap, Award, Loader2, AlertCircle, CheckCircle2 } from "lucide-react";
import { BasketService, ProductService, ReviewService } from "@/services/api";
import { useLang } from "@/hooks/useLang";
import { getUserId } from "@/lib/auth";
import { extractErrorMessage } from "@/lib/errors";
import { formatCurrency } from "@/lib/utils";
import { Product, Review } from "@/types";

// API'den gelen ürün verisi için genişletilmiş tip
interface ProductDetail extends Product {
  stock?: number;
  imageUrl?: string;
}

export default function ProductDetailPage() {
  const { lang } = useLang("en");
  const copy = {
    en: {
      home: "Home",
      products: "Products",
      back: "Back to Products",
      new: "New",
      noReviews: "No reviews yet",
      writeReview: "Write a Review",
      freeShipping: "Free Shipping",
      freeShippingSub: "On orders over ₺50",
      warranty: "2 Year Warranty",
      warrantySub: "Full coverage",
      authentic: "Authentic",
      authenticSub: "100% genuine",
      quantity: "Quantity",
      youMayAlso: "You May Also Like",
      stock: "In Stock",
      outOfStock: "Out of Stock",
    },
    tr: {
      home: "Ana Sayfa",
      products: "Ürünler",
      back: "Ürünlere Dön",
      new: "Yeni",
      noReviews: "Henüz yorum yok",
      writeReview: "Yorum Yaz",
      freeShipping: "Ücretsiz Kargo",
      freeShippingSub: "50₺ üzeri siparişlerde",
      warranty: "2 Yıl Garanti",
      warrantySub: "Tam kapsam",
      authentic: "Orijinal",
      authenticSub: "%100 orijinal",
      quantity: "Adet",
      youMayAlso: "Bunları da sevebilirsiniz",
      stock: "Stokta",
      outOfStock: "Stokta yok",
    },
  } as const;
  const t = copy[lang];
  const params = useParams();
  const productId = params.id as string;
  const [product, setProduct] = useState<ProductDetail | null>(null);
  const [quantity, setQuantity] = useState(1);
  const [activeTab, setActiveTab] = useState<"description" | "specs" | "reviews">("description");
  const [error, setError] = useState<string | null>(null);
  const [adding, setAdding] = useState(false);
  const [info, setInfo] = useState<string | null>(null);
  const [reviews, setReviews] = useState<Review[]>([]);
  const [reviewLoading, setReviewLoading] = useState(false);
  const [reviewError, setReviewError] = useState<string | null>(null);
  const [comment, setComment] = useState("");
  const [rating, setRating] = useState(5);

  // Merkezi auth fonksiyonundan userId al
  const userId = getUserId() || "";

  useEffect(() => {
    const fetchProduct = async () => {
      try {
        setError(null);
        const data = await ProductService.getById(productId);
        if (!data) {
          setError("Product not found");
          return;
        }
        setProduct({
          ...data,
          stock: (data as ProductDetail).stock ?? 0,
          imageUrl: (data as ProductDetail).imageUrl,
        });
      } catch (err: unknown) {
        console.error(err);
        setError(extractErrorMessage(err, "Failed to load product"));
      }
    };
    fetchProduct();
  }, [productId]);

  useEffect(() => {
    const loadReviews = async () => {
      try {
        setReviewError(null);
        setReviewLoading(true);
        const list = await ReviewService.getByProduct(productId);
        setReviews(list);
      } catch (err: unknown) {
        setReviewError(extractErrorMessage(err, lang === "tr" ? "Yorumlar yüklenemedi" : "Could not load reviews"));
      } finally {
        setReviewLoading(false);
      }
    };
    loadReviews();
  }, [productId, lang]);

  const handleAddToCart = async () => {
    setInfo(null);
    setError(null);
    if (!userId || !product) {
      setInfo(lang === "tr" ? "Sepete eklemek için giriş yap" : "Login to add to cart");
      return;
    }
    setAdding(true);
    try {
      await BasketService.addItem(userId, {
        productId,
        productName: product.name,
        price: product.price,
        quantity,
      });
      setInfo(lang === "tr" ? "Ürün sepete eklendi" : "Added to cart");
    } catch (err: unknown) {
      setError(extractErrorMessage(err, lang === "tr" ? "Sepete eklenemedi" : "Could not add to cart"));
    } finally {
      setAdding(false);
    }
  };

  const handleAddReview = async () => {
    if (!userId) {
      setReviewError(lang === "tr" ? "Yorum eklemek için giriş yap" : "Login to add a review");
      return;
    }
    if (!comment.trim()) {
      setReviewError(lang === "tr" ? "Yorum boş olamaz" : "Comment cannot be empty");
      return;
    }
    try {
      setReviewError(null);
      const created = await ReviewService.create({
        productId,
        comment: comment.trim(),
        rating,
        imageUrls: [],
      });
      setReviews((prev) => [created, ...prev]);
      setComment("");
      setRating(5);
    } catch (err: unknown) {
      setReviewError(extractErrorMessage(err, lang === "tr" ? "Yorum eklenemedi" : "Could not add review"));
    }
  };

  if (error) {
    return (
      <div className="min-h-screen flex items-center justify-center bg-gradient-to-b from-slate-950 via-slate-900 to-slate-950 text-white">
        <div className="text-center space-y-4">
          <h2 className="text-2xl font-bold">{error}</h2>
          <Link href="/products" className="text-cyan-300 hover:underline">
            Back to products
          </Link>
        </div>
      </div>
    );
  }

  if (!product) {
    return (
      <div className="min-h-screen flex items-center justify-center bg-gradient-to-b from-slate-950 via-slate-900 to-slate-950">
        <div className="animate-spin rounded-full h-16 w-16 border-b-2 border-blue-500"></div>
      </div>
    );
  }

  const discount = 0;

    return (
        <div className="min-h-screen bg-gradient-to-b from-slate-950 via-slate-900 to-slate-950 text-white">
      {/* Breadcrumb */}
      <div className="bg-slate-900/80 border-b border-white/10">
        <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-4">
          <div className="flex items-center gap-2 text-sm text-slate-200">
                        <Link href="/" className="hover:text-cyan-300">
              {t.home}
            </Link>
            <ChevronRight className="h-4 w-4" />
            <Link href="/products" className="hover:text-cyan-300">
              {t.products}
            </Link>
            <ChevronRight className="h-4 w-4" />
            <span className="text-white font-medium">{product.category}</span>
          </div>
        </div>
      </div>

        <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
        {info && (
          <div className="mb-4 flex items-center gap-2 text-emerald-400 bg-emerald-500/10 border border-emerald-500/30 rounded-md px-3 py-2">
            <CheckCircle2 className="h-4 w-4" />
            <span>{info}</span>
          </div>
        )}
        {error && (
          <div className="mb-4 flex items-center gap-2 text-red-400 bg-red-500/10 border border-red-500/30 rounded-md px-3 py-2">
            <AlertCircle className="h-4 w-4" />
            <span>{error}</span>
          </div>
        )}
        {/* Back Button */}
        <Link
          href="/products"
          className="inline-flex items-center text-cyan-300 hover:text-cyan-200 font-medium mb-6 group"
        >
          <ArrowLeft className="h-4 w-4 mr-2 group-hover:-translate-x-1 transition-transform" />
          {t.back}
        </Link>

                <div className="grid grid-cols-1 lg:grid-cols-2 gap-12">
          {/* Product Image */}
          <div>
            <div className="bg-gradient-to-br from-blue-500 to-purple-600 rounded-2xl p-16 flex items-center justify-center min-h-[500px] relative overflow-hidden group">
              {product.imageUrl ? (
                <img 
                  src={product.imageUrl} 
                  alt={product.name} 
                  className="absolute inset-0 w-full h-full object-cover rounded-2xl" 
                />
              ) : (
                <>
                  <div className="absolute inset-0 bg-black/10"></div>
                  <div className="relative z-10 text-white text-6xl font-bold">{product.brand || "Tech"}</div>
                </>
              )}
              {discount > 0 && (
                <div className="absolute top-6 right-6 bg-red-500 text-white px-4 py-2 rounded-xl font-bold text-lg shadow-xl">
                  {discount}% OFF
                </div>
              )}
            </div>
          </div>

          {/* Product Info */}
          <div className="space-y-6 text-white">
            {/* Brand & Title */}
            <div>
              <p className="text-sm font-medium text-cyan-300 mb-2">{product.brand}</p>
              <h1 className="text-4xl font-black text-white mb-4">{product.name}</h1>

              {/* Rating placeholder */}
              <div className="flex items-center gap-4 mb-4 text-slate-200">
                <div className="flex items-center gap-2">
                  <div className="flex">
                    {[...Array(5)].map((_, i) => (
                      <Star key={i} className="h-5 w-5 text-slate-500" />
                    ))}
                  </div>
              <span className="text-sm font-semibold text-white">{t.new}</span>
            </div>
                <span className="text-sm text-slate-300">({t.noReviews})</span>
              </div>
            </div>

            {/* Price */}
            <div className="border-y border-white/10 py-6">
              <div className="flex items-baseline gap-4 mb-2">
                <span className="text-5xl font-black text-cyan-300">{formatCurrency(product.price)}</span>
              </div>
              <p className="text-sm text-slate-200 mt-2">
                {product.stock && product.stock > 0 ? (
                  <span className="text-green-400 font-semibold flex items-center gap-1">
                    <Check className="h-4 w-4" /> {t.stock} ({product.stock} available)
                  </span>
                ) : (
                  <span className="text-red-400 font-semibold">{t.outOfStock}</span>
                )}
              </p>
            </div>

            {/* Quick Features */}
            <div className="space-y-3">
              <h3 className="font-bold text-white flex items-center gap-2">
                <Zap className="h-5 w-5 text-yellow-500" />
                Key Features
              </h3>
              <ul className="space-y-2">
                <li className="flex items-start gap-2 text-sm text-slate-200">
                  <Check className="h-5 w-5 text-green-400 flex-shrink-0 mt-0.5" />
                  <span>{product.description}</span>
                </li>
              </ul>
            </div>

            {/* Quantity & Add to Cart */}
            <div className="space-y-4 pt-6">
              <div className="flex items-center gap-4">
                <label className="font-semibold text-white">{t.quantity}:</label>
                <div className="flex items-center border border-white/10 rounded-lg">
                  <button
                    onClick={() => setQuantity(Math.max(1, quantity - 1))}
                    className="px-4 py-2 hover:bg-slate-800 transition-colors"
                  >
                    -
                  </button>
                  <span className="px-6 py-2 font-semibold">{quantity}</span>
                  <button
                    onClick={() => setQuantity(Math.max(1, Math.min(product.stock ?? 1, quantity + 1)))}
                    className="px-4 py-2 hover:bg-slate-800 transition-colors"
                  >
                    +
                  </button>
                </div>
              </div>

              <div className="flex gap-4">
                <button
                  onClick={handleAddToCart}
                  disabled={adding}
                  className="flex-1 flex items-center justify-center gap-2 py-4 bg-gradient-to-r from-blue-600 to-blue-700 text-white rounded-xl font-bold text-lg hover:from-blue-700 hover:to-blue-800 transition-all shadow-lg hover:shadow-xl transform hover:-translate-y-0.5 disabled:opacity-50"
                >
                  {adding ? (
                    <>
                      <Loader2 className="h-5 w-5 animate-spin" />
                      {lang === "tr" ? "Ekleniyor..." : "Adding..."}
                    </>
                  ) : (
                    <>
                      <ShoppingCart className="h-6 w-6" />
                      {lang === "tr" ? "Sepete Ekle" : "Add to Cart"}
                    </>
                  )}
                </button>
                <button className="px-6 py-4 border-2 border-white/10 rounded-xl hover:border-red-500 hover:bg-red-500/10 transition-all group">
                  <Heart className="h-6 w-6 text-slate-200 group-hover:text-red-500 group-hover:fill-red-500 transition-all" />
                </button>
                <button className="px-6 py-4 border-2 border-white/10 rounded-xl hover:border-blue-500 hover:bg-blue-500/10 transition-all group">
                  <Share2 className="h-6 w-6 text-slate-200 group-hover:text-blue-500 transition-all" />
                </button>
              </div>
            </div>

            {/* Trust Badges */}
            <div className="grid grid-cols-3 gap-4 pt-6 border-t border-white/10 text-slate-200">
                {[
                { icon: Truck, text: t.freeShipping, sub: t.freeShippingSub },
                { icon: Shield, text: t.warranty, sub: t.warrantySub },
                { icon: Award, text: t.authentic, sub: t.authenticSub },
              ].map((badge, idx) => (
                <div key={idx} className="text-center">
                  <badge.icon className="h-8 w-8 mx-auto mb-2 text-blue-400" />
                  <p className="text-sm font-semibold text-white">{badge.text}</p>
                  <p className="text-xs text-slate-300">{badge.sub}</p>
                </div>
              ))}
            </div>
          </div>
        </div>

        {/* Tabs Section */}
        <div className="mt-16">
          <div className="border-b border-white/10">
            <div className="flex gap-8">
              {[
                { key: "description", label: "Açıklama" },
                { key: "specs", label: "Özellikler" },
                { key: "reviews", label: "Yorumlar" },
              ].map((tab) => (
                <button
                  key={tab.key}
                  onClick={() => setActiveTab(tab.key as any)}
                  className={`pb-4 px-2 font-bold text-lg transition-colors relative ${
                    activeTab === tab.key ? "text-cyan-300 border-b-4 border-cyan-300" : "text-slate-300 hover:text-white"
                  }`}
                >
                  {tab.label}
                </button>
              ))}
            </div>
          </div>

          <div className="py-8 text-slate-200">
            {activeTab === "description" && (
              <div className="prose max-w-none prose-invert">
                <p className="text-lg leading-relaxed mb-6">{product.description}</p>
              </div>
            )}

            {activeTab === "specs" && (
              <div className="grid grid-cols-1 md:grid-cols-2 gap-x-8 gap-y-4">
                <div className="flex justify-between py-3 border-b border-white/10">
                  <span className="font-semibold text-white">Category</span>
                  <span className="text-slate-200">{product.category}</span>
                </div>
                <div className="flex justify-between py-3 border-b border-white/10">
                  <span className="font-semibold text-white">Brand</span>
                  <span className="text-slate-200">{product.brand ?? "N/A"}</span>
                </div>
                <div className="flex justify-between py-3 border-b border-white/10">
                  <span className="font-semibold text-white">Stock</span>
                  <span className="text-slate-200">{product.stock ?? 0}</span>
                </div>
              </div>
            )}

            {activeTab === "reviews" && (
              <div className="space-y-6">
                {reviewError && <p className="text-red-400 text-sm">{reviewError}</p>}
                <div className="bg-slate-900/60 border border-white/10 rounded-xl p-4">
                  <h3 className="font-bold text-lg mb-2">Yorum Ekle</h3>
                  <div className="flex flex-col gap-3">
                    <div className="flex items-center gap-2">
                      <span className="text-sm text-slate-300">Puan:</span>
                      <input
                        type="number"
                        min={1}
                        max={5}
                        value={rating}
                        onChange={(e) => setRating(Math.max(1, Math.min(5, Number(e.target.value))))}
                        className="w-16 bg-slate-800 border border-white/10 rounded px-2 py-1 text-white"
                      />
                      <Star className="h-4 w-4 text-yellow-400" />
                    </div>
                    <textarea
                      value={comment}
                      onChange={(e) => setComment(e.target.value)}
                      placeholder="Ürün deneyimini paylaş..."
                      className="w-full bg-slate-800 border border-white/10 rounded px-3 py-2 text-white"
                      rows={3}
                    />
                    <button
                      onClick={handleAddReview}
                      className="self-end px-4 py-2 bg-blue-600 hover:bg-blue-700 rounded-lg font-semibold text-white disabled:opacity-50"
                      disabled={reviewLoading}
                    >
                      {reviewLoading ? "Gönderiliyor..." : "Yorumu Gönder"}
                    </button>
                  </div>
                </div>

                <div className="space-y-3">
                  <h3 className="font-bold text-lg">Yorumlar</h3>
                  {reviewLoading ? (
                    <div className="text-slate-300 text-sm">Yorumlar yükleniyor...</div>
                  ) : reviews.length === 0 ? (
                    <div className="text-slate-400 text-sm">Henüz yorum yok.</div>
                  ) : (
                    reviews.map((rev) => (
                      <div key={rev.id} className="bg-slate-900/60 border border-white/10 rounded-xl p-4">
                        <div className="flex items-center justify-between mb-2">
                          <div className="flex items-center gap-2 text-yellow-400">
                            {[...Array(5)].map((_, i) => (
                              <Star key={i} className={`h-4 w-4 ${i < rev.rating ? "fill-yellow-400" : "text-slate-600"}`} />
                            ))}
                          </div>
                          <span className="text-xs text-slate-400">
                            {new Date(rev.createdAt).toLocaleDateString("tr-TR")}
                          </span>
                        </div>
                        <p className="text-sm text-slate-200">{rev.comment}</p>
                      </div>
                    ))
                  )}
                </div>
              </div>
            )}
          </div>
        </div>

        {/* Related Products */}
        <div className="mt-16 text-white">
          <h2 className="text-3xl font-black mb-8">{t.youMayAlso}</h2>
          <div className="grid grid-cols-2 md:grid-cols-4 gap-6">
            <p className="text-slate-300">More items coming soon.</p>
          </div>
        </div>
      </div>
    </div>
  );
}
