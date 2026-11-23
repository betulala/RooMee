# RooMee

Öğrenciler için ev arkadaşı / oda ilanı platformu (ASP.NET Core MVC, .NET 10, Entity Framework Core, SQL Server LocalDB).

## Kurulum

Gerekenler:
- Visual Studio 2026 ("ASP.NET ve web geliştirme" iş yüküyle)
- .NET SDK 10.0.404 (`global.json` bu sürümü ister)

Adımlar:
1. Visual Studio → **Clone a repository** → `https://github.com/betulala/RooMee`
2. **Tools → NuGet Package Manager → Package Manager Console** açıp çalıştır:
   ```
   Update-Database
   ```
   Bu, bilgisayarındaki LocalDB'de veritabanını oluşturur. Herkesin veritabanı kendi bilgisayarındadır, veriler paylaşılmaz.
3. **F5** ile çalıştır.

## Çalışma düzeni

`master` her zaman çalışan sürümdür. Kimse doğrudan `master`'a göndermez.

1. Çalışmaya başlamadan önce `master`'ı güncelle (Visual Studio → Git → **Pull**).
2. Yaptığın iş için yeni bir branch aç: Git → **New Branch**. İsim örnekleri: `mesajlasma`, `ilan-arama`, `profil-duzeltme`.
3. Küçük adımlarla commit at, açıklamayı Türkçe ve anlaşılır yaz (örn. "Favorilere ekleme butonu").
4. Branch'ini GitHub'a gönder (**Push**).
5. GitHub'da **Pull Request** aç, ekipten birini reviewer olarak ekle.
6. Onay gelince PR'ı `master`'a birleştir (**Merge**) ve branch'i sil.

### Veritabanı değişiklikleri (migration)

Bir modele alan eklediysen / yeni model eklediysen:
```
Add-Migration KisaAciklama
Update-Database
```
Oluşan `Data/Migrations` dosyalarını da commit'e ekle. Başkasının migration'ını çektikten sonra sen de `Update-Database` çalıştır.

Aynı anda iki kişi migration eklerse çakışma olabilir; migration ekleyeceğin zaman ekibe haber ver.

### Çakışma (conflict) olursa

Visual Studio çakışan dosyaları gösterir. **Merge** ekranında iki tarafı da kontrol edip doğru olanı seç. Emin değilsen o kodu yazan kişiye sor.

## Yapılanlar

- Kayıt / giriş, öğrenci doğrulama, profil, hesap silme
- İlan oluşturma, düzenleme, listeleme, fotoğraf yükleme
- İlan filtreleme ve sayfalama, İlanlarım sayfası
- Favoriler
