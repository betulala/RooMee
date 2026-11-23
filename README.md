# RooMee

Öğrenciler için ev arkadaşı / oda ilanı platformu (ASP.NET Core MVC, .NET 10, Entity Framework Core, SQL Server LocalDB).

## Kurulum

Gerekenler:
- Visual Studio 2026 ("ASP.NET ve web geliştirme" iş yüküyle)
- .NET SDK 10.0.404 (`global.json` bu sürümü ister)

Adımlar:
1. Visual Studio → **Clone a repository** → `https://github.com/betulala/RooMee`
2. **F5** ile çalıştır. Uygulama açılırken veritabanını (LocalDB) kendisi oluşturur ve bekleyen tüm veritabanı değişikliklerini (migration) otomatik uygular; `Update-Database` çalıştırmana gerek yoktur. Herkesin veritabanı kendi bilgisayarındadır, veriler paylaşılmaz.

### Test verisi (isteğe bağlı)

Boş veritabanında denemek için 15 örnek kullanıcı (doldurulmuş ev arkadaşı profilleriyle) ve 15 ilan eklenebilir. Uygulama kapalıyken proje klasöründe:
```
dotnet run --project RooMee -- --test-verisi
```
Test hesaplarının e-postaları `ad@roomee.test` biçimindedir (ör. `mert@roomee.test`, `selin@roomee.test`); şifreleri `Data/TestVerisi.cs` dosyasında yazıyor. Komut tekrar çalıştırılırsa aynı kullanıcıları ikinci kez eklemez.

### Yönetici (şikayet yönetimi)

Şikayetleri inceleyen kullanıcıların "Yonetici" rolü olmalı. Bir hesabı yönetici yapmak için (uygulama kapalıyken):
```
dotnet run --project RooMee -- --yonetici-yap ornek@mail.com
```
Sonra o hesapla çıkış yapıp tekrar giriş yapınca menüde **Şikayetler** görünür. Test verisi yüklendiyse `yonetici@roomee.test` hesabı zaten yöneticidir.

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
- Favoriler, mesajlaşma
- Şikayet sistemi (ilan / kullanıcı bildirme) ve yönetici inceleme sayfası
- E-posta bildirimleri (yeni mesaj, şikayet sonucu kaldırılan ilan); profilden kapatılabilir
- Kayıtlı aramalar: uyan yeni ilan eklenince e-posta uyarısı
- Ev arkadaşı değerlendirmesi (karşılıklı mesajlaşanlar 1-5 yıldız ve yorum bırakabilir)
- Sayfa yenilemeden mesajlaşma (yeni mesajlar ve okundu bilgisi kendiliğinden gelir)
- Ana sayfada giriş yapan kişiye uyumlu ev arkadaşı ve ilan önerileri
- İlanlar için harita görünümü (Leaflet + OpenStreetMap, şehir bazında)

> Not: `.test` uzantılı e-posta adreslerine (test hesapları) e-posta gönderilmez; SMTP ayarı yapılmamışsa e-postalar sadece günlüğe yazılır.
- Hakkımızda ve KVKK aydınlatma metni
- Ev Arkadaşları: profil listesi, gelişmiş filtreler ve uyum yüzdesi
  (tasarım ve filtre mantığı ToplumaHizmetVatanaHizmettir reposundaki `kullanicilar.html` prototipinden uyarlandı)
