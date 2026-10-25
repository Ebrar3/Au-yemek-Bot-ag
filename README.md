# AÜ Yemekhane Asistanı (Aü Bot)

**BLM4581 Ağ Tabanlı Teknolojiler ve Uygulamaları – Dönem Projesi (2026–2027 Güz)**

Ankara Üniversitesi öğrencilerinin ve personelinin aylık yemekhane menülerine hızlıca ulaşmasını, yemekleri puanlamasını ve seçenek/buton tabanlı etkileşimli sohbet botu (Web + Telegram + Mobil API) üzerinden menü sorgulamasını sağlayan ASP.NET Core Razor Pages web portalı.

## Grup Üyeleri

| Ad Soyad | Öğrenci No | GitHub | Lider Olduğu Haftalar |
|---|---|---|---|
| **Yılmaz Emre Gündaş** | 23291244 | [@gundasyemre](https://github.com/gundasyemre) | 1., 2. ve 3. haftalar |
| **Arife Ebrar Üstüner** | 23291207 | [@Ebrar3](https://github.com/Ebrar3) | 4., 5. ve 6. haftalar |
| **Efe Can Seymen** | 23291006 | [@efecanseymen](https://github.com/efecanseymen) | 7., 8. ve 9. haftalar |

## Kullanılan Teknolojiler

- **Web Çatısı:** ASP.NET Core Razor Pages (.NET 9 / .NET 10)
- **Veritabanı:** Microsoft SQL Server (LocalDB) & Entity Framework Core (Code-First, Migration)
- **Kimlik Doğrulama & Roller:** ASP.NET Core Identity (`Yönetici`, `Öğrenci`) + Mobil istemci için JWT
- **Web API & Dokümantasyon:** ASP.NET Core Web API + Swagger UI (`Swashbuckle`)
- **Harici API:** Telegram Bot API (`Telegram.Bot`)
- **Excel İçe Aktarma:** `ClosedXML` (Üniversitenin aylık öğle ve akşam Excel menülerinin otomatik ayrıştırılması)
- **Arayüz:** Bootstrap 5

## Sohbet Botu İşleyişi (Seçenek / Buton Tabanlı)

Sohbet ekranı açıldığında kullanıcı metin yazmak zorunda kalmadan tıklanabilir seçenek butonlarıyla ilerler:
- **Varsayılan Açılış:** Bugünün **Öğle Menüsü** (yemekler, kaloriler ve ortalama puanlar) gösterilir.
- **Seçenekler:**
  - `⭐ Puanlama`: Seçili öğündeki yemeklere 1–5 arası puan verme
  - `🌙 Akşam Yemeği` / `☀️ Öğle Yemeği`: Aynı günün diğer öğününe geçiş
  - `⏭️ Ertesi Günün Öğle Yemeği`: Sonraki günün öğle menüsüne geçiş
  - `⏮️ Önceki Günün Öğle Yemeği`: İleri tarihe geçildikten sonra önceki güne dönüş
  - `🆘 Yardım`: Hata/öneri bildirimi için ekip iletişim e-posta adresinin gösterimi

## Kurulum ve Çalıştırma Adımları

### 1. Depoyu Klonlayın
```bash
git clone https://github.com/Ebrar3/Au-yemek-Bot-ag.git
cd Au-yemek-Bot-ag/YemekBot
```

### 2. Gizli Bilgileri (User Secrets) Tanımlayın
Veritabanı şifresi veya Telegram Bot Token gibi gizli bilgiler depoya eklenmez:
```bash
dotnet user-secrets init
dotnet user-secrets set "Telegram:BotToken" "TELEGRAM_BOT_TOKEN_BURAYA"
```

### 3. Veritabanını Oluşturun (Migration)
*(2. haftada EF Core kurulumu tamamlandığında aktif olacaktır)*
```bash
dotnet ef database update
```

### 4. Uygulamayı Çalıştırın
```bash
dotnet run
```
Tarayıcıdan `https://localhost:5001` (veya konsolda belirtilen `http://localhost:5xxx`) adresine gidin.
