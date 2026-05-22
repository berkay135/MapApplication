# MapApplication

MapApplication, Başarsoft stajı kapsamında geliştirilen temel bir Coğrafi Bilgi Sistemi (CBS / GIS) uygulamasıdır. Proje; harita üzerinde nokta ve alan verilerinin oluşturulması, görüntülenmesi, güncellenmesi, silinmesi ve PostgreSQL veritabanında WKT (Well-Known Text) formatında saklanması üzerine kuruludur.

## Proje Özeti

Bu uygulama, kullanıcıların web tabanlı bir harita arayüzü üzerinden konumsal verilerle etkileşim kurmasını sağlar. OpenLayers ile harita üzerinde nokta ve poligon çizimleri yapılabilir; çizilen geometriler isimlendirilerek veritabanına kaydedilebilir. Kayıtlı veriler daha sonra harita üzerinde tekrar görüntülenebilir, düzenlenebilir veya silinebilir.

Projenin temel amacı, CBS uygulamalarında kullanılan harita etkileşimleri, geometrik veri yönetimi, WKT formatı, REST API mantığı ve PostgreSQL entegrasyonu gibi konuları pratik bir uygulama üzerinden deneyimlemektir.

## Demo / Kullanım Videosu

[Uygulama kullanım videosu](https://www.youtube.com/watch?v=pLgefW86aZo)

## Özellikler

- OpenStreetMap tabanlı harita görüntüleme
- Harita üzerine nokta ekleme
- Harita üzerine poligon / alan ekleme
- Nokta ve alan verilerini WKT formatında saklama
- Kayıtlı verileri harita üzerinde görüntüleme
- Kayıtlı geometrileri listeleme ve sorgulama
- Nokta ve poligon bilgilerini güncelleme
- Harita üzerinden manuel konum / geometri düzenleme
- Kayıtlı verileri silme
- REST API üzerinden CRUD işlemleri
- Swagger ile API test desteği

## Kullanılan Teknolojiler

### Backend

- ASP.NET Core Web API (.NET 8)
- Entity Framework Core
- PostgreSQL
- Npgsql Entity Framework Core Provider
- Repository Pattern
- Unit of Work Pattern
- Swagger / Swashbuckle

### Frontend

- JavaScript
- Vite
- OpenLayers
- Bootstrap
- jsPanel
- Toastr

## Proje Yapısı

```text
MapApplication/
├── Controllers/
│   └── PointController.cs
├── Data/
│   └── AppDbContext.cs
├── Services/
│   ├── IRepository.cs
│   ├── Repository.cs
│   ├── IPointRepository.cs
│   ├── PointRepository.cs
│   ├── IUnitOfWork.cs
│   └── UnitOfWork.cs
├── Migrations/
├── wwwroot/
│   └── my-app/
│       ├── index.html
│       ├── main.js
│       ├── style.css
│       └── package.json
├── Point.cs
├── Program.cs
├── appsettings.json
└── MapApplication.csproj
```

## Veri Modeli

Uygulamada konumsal veriler `Point` modeli ile temsil edilir.

```csharp
public class Point
{
    public int Id { get; set; }
    public string Wkt { get; set; }
    public string Name { get; set; }
}
```

`Wkt` alanı, nokta veya poligon geometrisini WKT formatında tutar. Örneğin:

```text
POINT(34 34)
POLYGON((10.689 -25.092, 34.595 -20.170, 38.814 -35.639, 13.502 -39.155, 10.689 -25.092))
```

## API Endpointleri

| Metot | Endpoint | Açıklama |
| --- | --- | --- |
| GET | `/api/Point` | Tüm kayıtları getirir |
| GET | `/api/Point/{id}` | Belirli ID'ye sahip kaydı getirir |
| POST | `/api/Point` | Yeni nokta veya alan kaydı ekler |
| PUT | `/api/Point/{id}` | Var olan kaydı günceller |
| DELETE | `/api/Point/Delete/{id}` | Var olan kaydı siler |
| GET | `/api/Point/GetAllJSON` | Harita arayüzü için JSON formatında kayıtları getirir |

## Kurulum

### Gereksinimler

- .NET 8 SDK
- Node.js ve npm
- PostgreSQL
- Entity Framework Core CLI

Entity Framework Core CLI yüklü değilse:

```bash
dotnet tool install --global dotnet-ef
```

### 1. Repoyu Klonlayın

```bash
git clone https://github.com/berkay135/MapApplication.git
cd MapApplication
```

### 2. Backend Bağımlılıklarını Yükleyin

```bash
dotnet restore
```

### 3. PostgreSQL Bağlantısını Ayarlayın

`appsettings.json` içindeki `WebApiDatabase` connection string değerini kendi PostgreSQL ayarlarınıza göre güncelleyin.

```json
"ConnectionStrings": {
  "WebApiDatabase": "Host=localhost; Database=MapApplication; Username=postgres; Password=your_password"
}
```

### 4. Veritabanını Oluşturun

Migration dosyalarını kullanarak veritabanını güncelleyin:

```bash
dotnet ef database update
```

### 5. Backend'i Çalıştırın

```bash
dotnet run
```

Varsayılan geliştirme ortamında Swagger arayüzü şu adresten açılabilir:

```text
https://localhost:7235/swagger
```

### 6. Frontend Bağımlılıklarını Yükleyin

```bash
cd wwwroot/my-app
npm install
```

### 7. Frontend'i Çalıştırın

```bash
npm run start
```

Vite geliştirme sunucusu varsayılan olarak şu adreste çalışır:

```text
http://localhost:5173
```

> Not: Frontend tarafındaki API istekleri geliştirme ortamında `https://localhost:7235` adresine gönderilmektedir. Backend farklı bir portta çalışıyorsa `wwwroot/my-app/main.js` içindeki API URL'leri güncellenmelidir.

## Kullanım

1. Backend uygulamasını başlatın.
2. Frontend uygulamasını Vite ile çalıştırın.
3. Harita arayüzünde:
   - `Add Point` butonu ile haritaya nokta ekleyin.
   - `Add Area` butonu ile poligon / alan çizin.
   - Açılan panelden veriye isim verip kaydedin.
   - Var olan geometriye tıklayarak güncelleme veya silme işlemleri yapın.
   - `Query` butonu ile kayıtlı verileri tablo halinde görüntüleyin.
   - `View` butonu ile seçilen kaydın harita üzerindeki konumuna yakınlaşın.

## Öğrenilen / Uygulanan Konular

- CBS uygulamalarında temel harita işlemleri
- OpenLayers ile nokta ve poligon çizimi
- WKT formatı ile geometrik veri saklama
- ASP.NET Core Web API geliştirme
- PostgreSQL ve Entity Framework Core kullanımı
- Repository ve Unit of Work pattern kullanımı
- RESTful CRUD operasyonları
- Frontend ve backend arasında API iletişimi

## Notlar

Bu proje staj sürecinde geliştirilmiş eğitim ve pratik amaçlı bir çalışmadır. Production ortamı için güvenlik, validasyon, hata yönetimi, environment bazlı configuration ve deployment ayarlarının ayrıca geliştirilmesi önerilir.
