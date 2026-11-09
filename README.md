# WebFashionDesignerGirl

Dijital gardırop, kombin tasarımı ve moda paylaşımını bir araya getiren bir web uygulaması. Kullanıcıların kendi stillerini oluşturması, kombinlerini paylaşması ve toplulukla etkileşim kurması amaçlanır.

> **Durum:** Proje geliştirme aşamasındadır; henüz tamamlanmamıştır. Depoda aşağıdaki özelliklerin kodları bulunur, ancak tüm akışların sorunsuz çalıştığı doğrulanmış değildir.

## Mevcut içerik

- **Hesap ve profil:** Kayıt, giriş, profil görseli ve kullanıcı bilgileri.
- **Dijital gardırop:** Kullanıcıya ait kıyafetler ve stil puanlarıyla yeni parçalar edinme.
- **Kombin stüdyosu:** Gardıroptaki parçalarla kombin hazırlama, görselini kaydetme ve isim/açıklama ekleme.
- **Lookbook ve topluluk:** Kombinleri görüntüleme, beğenme ve yorum yapma.
- **Stil puanları:** Kombinler ve etkileşimlerle ilişkili puan mekanizması.
- **Yönetim:** Kullanıcıları görüntüleme ve kullanıcılara kıyafet hediye etme işlemleri.

## Tamamlandığında hedeflenen deneyim

Kullanıcı kayıt olup profilini oluşturabilmeli, dijital gardırobundaki parçaları stüdyoda birleştirerek kombinini kaydedebilmeli ve toplulukta paylaşabilmelidir. Diğer kullanıcıların kombinlerini keşfedip beğenebilmeli ve yorumlayabilmelidir. Kazandığı stil puanlarıyla gardırobunu genişletebilmeli; yönetim bölümü kullanıcı ve içerik yönetimini desteklemelidir.

Bu hedef için sayfa ve yönlendirme eksiklerinin giderilmesi, puan/gardırop akışlarının tutarlı hale getirilmesi, giriş ve yetkilendirme güvenliğinin geliştirilmesi, veritabanı kurulumunun tamamlanması ve uçtan uca testler yapılması gerekir. Bu bölüm tamamlanmış özellik garantisi değil, projenin hedefini açıklar.

## Teknolojiler ve yerel kurulum

**ASP.NET Core MVC · C# · .NET 8 · Entity Framework Core 8 · SQL Server / LocalDB · Razor · HTML/CSS/JavaScript**

`WebFashionDesignerGirl.sln` dosyasını Visual Studio ile açıp NuGet paketlerini geri yükleyin. `WebFashionDesignerGirl/appsettings.json` içindeki `DefaultConnection` değerini kendi SQL Server ortamınıza göre düzenleyin.

Veritabanı kayıtları bu depoya dahil değildir. Migration dosyaları bulunsa da mevcut migration'ların boş bir veritabanını eksiksiz oluşturduğu doğrulanmamıştır; uygulamayı çalıştırmadan önce şema ve başlangıç kıyafet verileri kontrol edilmelidir.
