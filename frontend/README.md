# Technology Store Frontend (Next.js 16 + TS)

## Kurulum
```bash
npm install
npm run dev # http://localhost:3000
```
İsteğe bağlı env değişkenleri (root `.env.local`):
```
NEXT_PUBLIC_API_URL=http://localhost:5000/api      # ProductService
NEXT_PUBLIC_IDENTITY_URL=http://localhost:5001/api # IdentityService
```

## Neler Hazır?
- Landing/Home sayfası (hero, kategoriler, öne çıkan ürünler)
- Products listesi: ProductService API'den `/products` ile veri çekiyor, hata halinde mock veri döner.
- Product detail sayfası: mock veri ile tasarım hazır (API bağlamak için hazır).
- Login/Register: form doğrulamaları ve mock token kaydı (`localStorage`) mevcut, gerçek IdentityService entegrasyonu için bekliyor.
- Cart sayfası: client-state, miktar artır/azalt ve özet kartı.
- Ortak layout (Navbar/Footer), Tailwind teması, responsive tasarım.

## Backend ile Entegrasyon
- ProductService uç noktası `/api/products`; `NEXT_PUBLIC_API_URL` ile yönlendirilir.
- IdentityService için login/register çağrıları TODO; `identityApi` hazır (token header ekliyor).
- CORS ApiGateway'de açık; yerelde doğrudan servislere istek atılabilir.
- ProductService şu an InMemory DB ile geliyor, bu yüzden frontend gerçek ürünleri alabilir; DB aktive edilince davranış değişmez.

## Yol Haritası (Frontend)
1) Login/Register akışını IdentityService'e bağla (token saklama, guard'lar, profil).
2) Cart'ı BasketService'e bağla; checkout akışını PaymentService ile tamamlama.
3) Order history & order detail ekranlarını OrderService'den doldurma.
4) Product detail sayfasını gerçek API'ye bağla; varyant/galeri ekleme.
5) Global state (Context/Zustand) ile auth + sepeti yönetme, yüklenme/bildirim bileşenleri.
6) NotificationService çıktıları için in-app banner/toast gösterimi.

## Scriptler
- `npm run dev` — geliştirme
- `npm run build` — production build
- `npm run start` — production server
- `npm run lint` — lint
