# BuildCore

BuildCore, **Clean Architecture**, **Domain Driven Design (DDD)**, **CQRS** ve **Event-Driven Architecture** prensiplerine uygun olarak geliştirilmiş modüler bir .NET 8.0 uygulamasıdır.

## 📋 İçindekiler

- [Genel Bakış](#genel-bakış)
- [Mimari](#mimari)
- [Proje Yapısı](#proje-yapısı)
- [Teknolojiler](#teknolojiler)
- [Kurulum](#kurulum)
- [Kullanım](#kullanım)
- [Gelecek Özellikler](#gelecek-özellikler)
- [Mimari Prensipler](#mimari-prensipler)

## 🎯 Genel Bakış

BuildCore, modüler yapıda tasarlanmış bir kurumsal uygulama çerçevesidir. Her modül bağımsız olarak geliştirilebilir ve test edilebilir. Şu anda **Human Resources** modülü implementasyonu tamamlanmıştır.

### Özellikler

- ✅ **Clean Architecture** - Katmanlar arası bağımlılık yönetimi
- ✅ **Domain Driven Design** - İş mantığının domain katmanında merkezileştirilmesi
- ✅ **CQRS** - Command Query Responsibility Segregation (MediatR ile)
- ✅ **Event-Driven Architecture** - Domain Events ve Event Handlers
- ✅ **Repository Pattern** - Veri erişim soyutlaması
- ✅ **Unit of Work Pattern** - Transaction yönetimi
- ✅ **JWT Authentication** - Token tabanlı kimlik doğrulama
- ✅ **Role-Based Authorization** - Rol tabanlı yetkilendirme
- ✅ **Audit Logging** - Otomatik audit kayıtları
- ✅ **Soft Delete** - Yumuşak silme desteği
- 🔄 **MassTransit** - Mesaj kuyruğu entegrasyonu (planlanıyor)
- 🔄 **Event Bus** - Event-driven iletişim (planlanıyor)

## 🏗️ Mimari

### Katmanlar

```
┌─────────────────────────────────────────┐
│         Presentation Layer              │
│  ┌──────────────┐  ┌──────────────┐     │
│  │  BuildCore   │  │  BuildCore   │     │
│  │     .Api     │  │   .WebApp    │     │
│  └──────────────┘  └──────────────┘     │
└─────────────────────────────────────────┘
                  ↓
┌─────────────────────────────────────────┐
│      Application Layer                  │
│  BuildCore.HumanResources.Application   │
│  - Commands / Queries                   │
│  - DTOs                                 │
│  - Use Cases                            │
│  - Interfaces                           │
└─────────────────────────────────────────┘
                  ↓
┌─────────────────────────────────────────┐
│         Domain Layer                    │
│  BuildCore.HumanResources.Domain        │
│  - Entities                             │
│  - Domain Events                        │
│  - Value Objects                        │
│  - Aggregates                           │
│  - Domain Interfaces                    │
└─────────────────────────────────────────┘
                  
┌─────────────────────────────────────────┐
│      Infrastructure Layer               │
│  BuildCore.HumanResources.Infrastructure│
│  - Persistence (EF Core)                │
│  - Authentication (JWT)                 │
│  - External Services                    │
│  - Event Publishing                     │
└─────────────────────────────────────────┘
                  
┌─────────────────────────────────────────┐
│       Shared Kernel                     │
│  BuildCore.SharedKernel                 │
│  - Base Entities                        │
│  - Common Interfaces                    │
│  - Domain Events Base                   │
└─────────────────────────────────────────┘
```

### Bağımlılık Yönü

```
Presentation → Application → Domain
                                      
             Infrastructure → Application

                                 SharedKernel
```

**Altın Kural:** Bağımlılıklar her zaman içe doğru (Domain'e doğru) akar. Domain katmanı hiçbir katmana bağımlı değildir.Isterse SharedKernel kullanabilir.

## 📁 Proje Yapısı

```
BuildCore/
├── BuildCore.Api/                         # REST API (Web API)
│   ├── Controllers/                       # API Controllers
│   ├── Authorization/                     # Custom Authorization Handlers
│   └── Program.cs                         # API Startup
│
├── BuildCore.WebApp/                      # MVC Web Application
│   ├── Controllers/                       # MVC Controllers
│   ├── Views/                             # Razor Views
│   └── Program.cs                         # WebApp Startup
│
├── BuildCore.HumanResources.Application/  # Application Layer
│   ├── Commands/                          # CQRS Commands
│   ├── Queries/                           # CQRS Queries
│   ├── DTOs/                              # Data Transfer Objects
│   ├── Interfaces/                        # Application Interfaces
│   └── UseCases/                          # Application Services
│
├── BuildCore.HumanResources.Domain/       # Domain Layer
│   ├── Entities/                          # Domain Entities
│   ├── DomainEvents/                      # Domain Events (planlanıyor)
│   ├── Aggregates/                        # Aggregates (planlanıyor)
│   ├── ValueObjects/                      # Value Objects (planlanıyor)
│   └── Interfaces/                        # Domain Interfaces
│
├── BuildCore.HumanResources.Infrastructure/ # Infrastructure Layer
│   ├── Persistence/                       # Data Access
│   │   ├── Configurations/                # EF Core Configurations
│   │   ├── Repositories/                  # Repository Implementations
│   │   ├── Interceptors/                  # EF Core Interceptors
│   │   │   ├── AuditInterceptor           # Audit Logging
│   │   │   ├── DomainEventInterceptor     # Domain Event Publishing
│   │   │   └── SoftDeleteInterceptor      # Soft Delete
│   │   ├── Migrations/                    # Database Migrations
│   │   └── Seed/                          # Database Seeding
│   ├── Authentication/                    # JWT Authentication
│   └── Common/                            # Infrastructure Services
│
└── BuildCore.SharedKernel/                # Shared Kernel
    ├── Entities/                          # Base Entities
    └── Interfaces/                        # Common Interfaces
```

## 🛠️ Teknolojiler

### Mevcut Teknolojiler

- **.NET 8.0** - Framework
- **ASP.NET Core** - Web Framework
- **Entity Framework Core 8.0** - ORM
- **SQL Server** - Database
- **JWT Bearer Authentication** - Authentication
- **BCrypt.Net-Next** - Password Hashing
- **MediatR** - CQRS Pattern Implementation
- **Swagger/OpenAPI** - API Documentation
- **Bootstrap 5** - Frontend Framework

### Planlanan Teknolojiler

- **MassTransit** - Message Queue (RabbitMQ/Azure Service Bus)
- **Redis** - Caching
- **Serilog** - Advanced Logging
- **FluentValidation** - Validation
- **AutoMapper** - Object Mapping
- **xUnit** - Unit Testing
- **Moq** - Mocking Framework
- **Docker** - Containerization
- **Kubernetes** - Orchestration

## 🚀 Kurulum

### Gereksinimler

- .NET 8.0 SDK
- SQL Server Express 2022 (veya SQL Server)
- Visual Studio 2022 / VS Code / Rider

### Adımlar

1. **Repository'yi klonlayın:**
   ```bash
   git clone <repository-url>
   cd BuildCore
   ```

2. **Connection String'i yapılandırın:**
   
   `BuildCore.Api/appsettings.json` ve `BuildCore.WebApp/appsettings.json` dosyalarında connection string'i güncelleyin:
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Server=.\\SQLEXPRESS2022;Database=BuildCoreHumanResources;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True"
     }
   }
   ```

3. **Paketleri yükleyin:**
   ```bash
   dotnet restore
   ```

4. **Database Migration'ları uygulayın:**
   ```bash
   cd BuildCore.Api
   dotnet ef database update --project ../BuildCore.HumanResources.Infrastructure
   ```

5. **Uygulamayı çalıştırın:**
   
   **API:**
   ```bash
   cd BuildCore.Api
   dotnet run
   ```
   
   **WebApp:**
   ```bash
   cd BuildCore.WebApp
   dotnet run
   ```

### Seed Verileri

Uygulama ilk çalıştığında otomatik olarak seed verileri eklenir:

- **Roller:**
  - Admin
  - HR Manager
  - HR User
  - Employee
  - Viewer

- **Test Kullanıcısı:**
  - Email: `admin@buildcore.com`
  - Password: `Admin123!`
  - Role: Admin

## 📖 Kullanım

### API Endpoints

#### Authentication
- `POST /api/auth/login` - Kullanıcı girişi
- `POST /api/auth/register` - Yeni kullanıcı kaydı

#### Users
- `GET /api/users` - Tüm kullanıcıları listele
- `GET /api/users/{id}` - Kullanıcı detayı
- `POST /api/users` - Yeni kullanıcı oluştur
- `PUT /api/users/{id}` - Kullanıcı güncelle
- `DELETE /api/users/{id}` - Kullanıcı sil

#### Roles
- `GET /api/roles` - Tüm rolleri listele
- `GET /api/roles/{id}` - Rol detayı
- `POST /api/roles` - Yeni rol oluştur
- `PUT /api/roles/{id}` - Rol güncelle
- `DELETE /api/roles/{id}` - Rol sil
- `POST /api/roles/assign` - Kullanıcıya rol ata

### Swagger UI

API dokümantasyonu için Swagger UI kullanılabilir:
```
http://localhost:5000/swagger
```

### WebApp Kullanımı

1. **Giriş Yap:**
   - URL: `http://localhost:5232/Auth/Login`
   - Email: `admin@buildcore.com`
   - Password: `Admin123!`

2. **Kullanıcı Yönetimi:**
   - Giriş yaptıktan sonra "Kullanıcılar" menüsünden kullanıcıları görüntüleyebilir ve yönetebilirsiniz.

## 🔮 Gelecek Özellikler

### Event-Driven Architecture

- **MassTransit Entegrasyonu**
  - RabbitMQ veya Azure Service Bus desteği
  - Event publishing ve consuming
  - Saga pattern implementasyonu
  - Outbox pattern ile transaction güvenliği

- **Domain Events**
  - Domain katmanında event tanımlamaları
  - Event handlers implementasyonu
  - Event sourcing desteği (opsiyonel)

### CQRS Geliştirmeleri

- **MediatR Tam Entegrasyonu**
  - Tüm use case'lerin Command/Query pattern'e dönüştürülmesi
  - Handler'ların ayrılması
  - Validation pipeline'ı

- **Read/Write Separation**
  - Read model için ayrı database (opsiyonel)
  - Read model projection'ları
  - CQRS optimizasyonları

### Domain Driven Design

- **Aggregates**
  - Aggregate root'ların tanımlanması
  - Aggregate boundary'lerinin belirlenmesi
  - Consistency garantileri

- **Value Objects**
  - Primitive obsession'ın önlenmesi
  - Immutable value object'ler
  - Validation logic'inin value object'lere taşınması

- **Domain Services**
  - Complex business logic için domain service'ler
  - Domain event'lerin yönetimi

### Diğer Özellikler

- **Caching**
  - Redis entegrasyonu
  - Distributed caching
  - Cache invalidation stratejileri

- **Logging & Monitoring**
  - Serilog entegrasyonu
  - Structured logging
  - Application Insights / ELK Stack

- **Testing**
  - Unit testler (xUnit)
  - Integration testler
  - E2E testler

- **Performance**
  - Query optimization
  - Database indexing
  - Response caching

- **Security**
  - Rate limiting
  - API versioning
  - OAuth 2.0 / OpenID Connect

## 🏛️ Mimari Prensipler

### Clean Architecture Prensipleri

1. **Dependency Rule**
   - Bağımlılıklar her zaman içe doğru akar
   - Domain katmanı hiçbir katmana bağımlı değildir
   - Infrastructure, Application ve Domain'e bağımlıdır

2. **Separation of Concerns**
   - Her katman kendi sorumluluğuna odaklanır
   - Business logic Domain katmanındadır
   - Infrastructure detayları dış katmanlardan gizlenir

3. **Dependency Inversion**
   - Üst katmanlar alt katmanlara değil, abstraction'lara bağımlıdır
   - Interface'ler Application katmanında tanımlanır
   - Implementation'lar Infrastructure katmanındadır

### Domain Driven Design Prensipleri

1. **Ubiquitous Language**
   - Domain uzmanlarıyla ortak dil kullanımı
   - Kod ve dokümantasyonda tutarlı terminoloji

2. **Bounded Contexts**
   - Her modül kendi bounded context'ine sahiptir
   - Modüller arası iletişim event'lerle yapılır

3. **Aggregates**
   - Aggregate root'lar consistency boundary'lerini belirler
   - Aggregate içindeki entity'ler aggregate root üzerinden erişilir

### CQRS Prensipleri

1. **Command/Query Separation**
   - Command'lar veri değiştirir (write)
   - Query'ler veri okur (read)
   - Her biri kendi optimizasyonuna sahiptir

2. **MediatR Pattern**
   - Request/Response pattern
   - Handler'ların merkezi yönetimi
   - Pipeline behavior'ları

### Event-Driven Architecture Prensipleri

1. **Domain Events**
   - Domain'deki önemli olaylar event olarak yayınlanır
   - Event'ler loose coupling sağlar
   - Event handlers asenkron çalışabilir

2. **Event Sourcing** (Opsiyonel)
   - State değişiklikleri event sequence olarak saklanır
   - Event replay ile state reconstruction
   - Audit trail için ideal

## 🔄 İş Akışı (Flow)

### Command İşleme Akışı

```
Controller (API/WebApp)
    ↓
Command/Query (MediatR Request)
    ↓
Handler (Application Layer)
    ↓
Repository (Infrastructure)
    ↓
UnitOfWork.SaveChanges()
    ↓
EF Core Pipeline
    ↓
AuditInterceptor (CreatedBy, UpdatedBy)
    ↓
DomainEventInterceptor (Event Publishing)
    ↓
SoftDeleteInterceptor (for IsDeleted vb.)
    ↓
Database Commit
    ↓
Domain Events Published (MassTransit)
    ↓
Event Handlers (Async Processing)
```

### Mimari Altın Kurallar

1. **Domain → Persistence bilmez**
   - Domain katmanı veritabanı detaylarından haberdar değildir

2. **Application → Interface bilir**
   - Application katmanı sadece interface'leri bilir, implementation'ları değil

3. **Persistence → EF Core bilir**
   - Infrastructure katmanı EF Core detaylarını bilir ve yönetir

4. **SaveChanges → Tek merkez**
   - Tüm veri değişiklikleri UnitOfWork üzerinden yapılır

5. **Interceptor → Cross-cutting**
   - Audit, Domain Events, Soft Delete gibi cross-cutting concern'ler interceptor'larla yönetilir

### Visibility (InternalsVisibleTo)

```
SharedKernel ──► (friend) Infrastructure
```

- `SharedKernel` içindeki `internal` üyeler `Infrastructure` tarafından erişilebilir
- Bu bir compile-time dependency değil, sadece visibility iznidir
- `SharedKernel`, `Infrastructure`'ı bilmez, sadece güvenir

## 📝 Notlar

- **Entity Davranışı:** Entity'ler kendi business logic'lerini içerir
- **Audit & Zaman:** Infrastructure katmanı audit ve zaman yönetiminden sorumludur
- **Cross-cutting Concerns:** Interceptor'lar ile merkezi yönetim
- **Modüler Yapı:** Her modül bağımsız olarak geliştirilebilir ve test edilebilir

## 📄 Lisans

Bu proje [lisans bilgisi] altında lisanslanmıştır.

## 👥 Katkıda Bulunanlar

- Murat Bolulu

## 📞 İletişim 0541 574 87 16

---

**Not:** Bu proje sürekli geliştirilmektedir. Yeni özellikler ve iyileştirmeler için [issue tracker]'ı takip edebilirsiniz.
