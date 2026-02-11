# BuildCore - Kullanım Kılavuzu

Bu dokümantasyon, BuildCore projesindeki tüm domain'lerin nasıl kullanılacağını detaylı olarak açıklar.

## İçindekiler

1. [Genel Bakış](#genel-bakış)
2. [HumanResources Domain](#humanresources-domain)
3. [WorkflowEngine Domain](#workflowengine-domain)
4. [ApprovalManagement Domain](#approvalmanagement-domain)
5. [Notification Domain](#notification-domain)
6. [Entegrasyon Senaryoları](#entegrasyon-senaryoları)

---

## Genel Bakış

BuildCore, Clean Architecture prensiplerine uygun olarak geliştirilmiş bir İnsan Kaynakları Yönetim Sistemidir. Proje aşağıdaki domain'lerden oluşur:

- **HumanResources**: Kullanıcı ve rol yönetimi
- **WorkflowEngine**: İş akışı motoru
- **ApprovalManagement**: Onay yönetimi sistemi
- **Notification**: Bildirim sistemi

Her domain kendi veritabanına sahiptir ve bağımsız olarak çalışabilir.

---

## HumanResources Domain

### Amaç
Kullanıcı ve rol yönetimi, kimlik doğrulama ve yetkilendirme işlemlerini yönetir.

### Temel Kavramlar

#### Kullanıcı (User)
- Sistemdeki her kullanıcı bir `User` entity'si ile temsil edilir
- Email ve şifre ile kimlik doğrulama yapılır
- Şifreler BCrypt ile hash'lenir

#### Rol (Role)
- Kullanıcılara atanabilen yetki grupları
- Örnek: Admin, HR Manager, HR User

#### UserRole
- Kullanıcı ve rol arasındaki many-to-many ilişki

### Kullanım Senaryosu

#### 1. Kullanıcı Kaydı (Register)

**WebApp:**
```
1. /Auth/Register sayfasına git
2. Formu doldur:
   - Ad, Soyad
   - Email
   - Şifre
   - Telefon (opsiyonel)
   - Departman (opsiyonel)
   - Pozisyon (opsiyonel)
3. "Kayıt Ol" butonuna tıkla
4. Başarılı olursa Login sayfasına yönlendirilirsin
```

**API:**
```http
POST /api/auth/register
Content-Type: application/json

{
  "firstName": "Ahmet",
  "lastName": "Yılmaz",
  "email": "ahmet@example.com",
  "password": "SecurePass123!",
  "confirmPassword": "SecurePass123!",
  "phoneNumber": "+905551234567",
  "department": "IT",
  "position": "Developer"
}
```

#### 2. Giriş Yapma (Login)

**WebApp:**
```
1. /Auth/Login sayfasına git
2. Email ve şifre gir
3. "Giriş Yap" butonuna tıkla
4. Başarılı olursa ana sayfaya yönlendirilirsin
5. Sol sidebar menü görünür
```

**API:**
```http
POST /api/auth/login
Content-Type: application/json

{
  "email": "ahmet@example.com",
  "password": "SecurePass123!"
}

Response:
{
  "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "tokenType": "Bearer",
  "expiresAt": "2026-02-12T10:00:00Z",
  "user": {
    "id": "guid",
    "firstName": "Ahmet",
    "lastName": "Yılmaz",
    "email": "ahmet@example.com",
    ...
  }
}
```

#### 3. Kullanıcı Listeleme

**WebApp:**
```
1. Sol menüden "Kullanıcılar" sekmesine tıkla
2. Tüm kullanıcılar listelenir
3. Her kullanıcı için Detay, Düzenle, Sil butonları görünür
```

**API:**
```http
GET /api/users
Authorization: Bearer {token}

Response: [
  {
    "id": "guid",
    "firstName": "Ahmet",
    "lastName": "Yılmaz",
    "email": "ahmet@example.com",
    "roles": [...]
  },
  ...
]
```

#### 4. Rol Atama

**API:**
```http
POST /api/users/{userId}/roles
Authorization: Bearer {token}
Content-Type: application/json

{
  "roleId": "role-guid"
}
```

### Veritabanı Yapısı

- **Users**: Kullanıcı bilgileri
- **Roles**: Rol tanımları
- **UserRoles**: Kullanıcı-Rol ilişkileri

---

## WorkflowEngine Domain

### Amaç
İş akışı tanımları oluşturma ve bu tanımlara göre iş akışı instance'ları çalıştırma.

### Temel Kavramlar

#### WorkflowDefinition (İş Akışı Tanımı)
- Bir iş akışının şablonu
- Adımlar (Steps) ve geçişler (Transitions) içerir
- Örnek: "İzin Talebi Onay Süreci"

#### WorkflowStep (İş Akışı Adımı)
- İş akışındaki her bir adım
- Sıralı (Sequential) veya paralel (Parallel) olabilir
- Her adımın bir tipi vardır (Start, Task, Approval, End)

#### Transition (Geçiş)
- Adımlar arasındaki geçiş kuralları
- Koşullu geçişler tanımlanabilir

#### WorkflowInstance (İş Akışı Örneği)
- Bir iş akışı tanımından oluşturulan çalışan örnek
- Durumu takip edilir (Pending, InProgress, Completed, Cancelled)

### Kullanım Senaryosu: İzin Talebi İş Akışı

#### 1. İş Akışı Tanımı Oluşturma

**WebApp:**
```
1. Sol menüden "İş Akışları" > "İş Akışı Tanımları" sekmesine tıkla
2. "Yeni İş Akışı Ekle" butonuna tıkla
3. Formu doldur:
   - Ad: "İzin Talebi Onay Süreci"
   - Açıklama: "Çalışan izin taleplerinin onay süreci"
4. "Kaydet" butonuna tıkla
```

**API:**
```http
POST /api/workflowdefinitions
Authorization: Bearer {token}
Content-Type: application/json

{
  "name": "İzin Talebi Onay Süreci",
  "description": "Çalışan izin taleplerinin onay süreci"
}
```

**Not:** Şu anda WebApp'te sadece temel bilgiler girilebilir. Adımlar ve geçişler API üzerinden eklenmelidir.

#### 2. İş Akışı Instance Başlatma

**WebApp:**
```
1. Sol menüden "İş Akışları" > "İş Akışı Örnekleri" sekmesine tıkla
2. "Yeni İş Akışı Başlat" butonuna tıkla
3. Formu doldur:
   - İş Akışı Tanımı ID: {workflow-definition-id}
   - Başlatan Kullanıcı ID: {user-id}
   - Bağlam Verisi (JSON): {"leaveType": "Annual", "days": 5}
4. "Başlat" butonuna tıkla
```

**API:**
```http
POST /api/workflowinstances
Authorization: Bearer {token}
Content-Type: application/json

{
  "workflowDefinitionId": "workflow-definition-guid",
  "initiatorUserId": "user-guid",
  "contextData": "{\"leaveType\": \"Annual\", \"days\": 5}"
}
```

#### 3. İş Akışı Adımını Tamamlama

**API:**
```http
POST /api/workflowinstances/{instanceId}/steps/{stepId}/complete
Authorization: Bearer {token}
Query Parameters:
  - userId: "user-guid"
```

#### 4. İş Akışı İptal Etme

**API:**
```http
POST /api/workflowinstances/{instanceId}/cancel
Authorization: Bearer {token}
Query Parameters:
  - userId: "user-guid"
```

### Veritabanı Yapısı

- **WorkflowDefinitions**: İş akışı tanımları
- **WorkflowSteps**: İş akışı adımları
- **Transitions**: Adımlar arası geçişler
- **WorkflowInstances**: Çalışan iş akışı örnekleri
- **WorkflowInstanceHistory**: İş akışı geçmişi

### Örnek İş Akışı Senaryosu

```
1. Çalışan izin talebi oluşturur
   → WorkflowInstance başlatılır (Status: Pending)

2. İlk adım: Doğrudan Müdür Onayı
   → Adım tamamlanır (Status: InProgress)

3. İkinci adım: İK Onayı
   → Adım tamamlanır

4. Son adım: Onaylandı
   → WorkflowInstance tamamlanır (Status: Completed)
```

---

## ApprovalManagement Domain

### Amaç
Onay kuralları tanımlama ve onay talepleri yönetme.

### Temel Kavramlar

#### ApprovalRule (Onay Kuralı)
- Hangi durumlarda kimlerin onay vermesi gerektiğini tanımlar
- Örnek: "5 günden fazla izin için Müdür + İK onayı gerekir"

#### ApprovalChain (Onay Zinciri)
- Onay sırasını tanımlar
- Her zincirde bir sıra (Order) vardır
- Onaylayıcı tipi: Role, User, Manager

#### ApprovalRequest (Onay Talebi)
- Bir onay kuralına göre oluşturulan talep
- Durum: Pending, Approved, Rejected, Cancelled, Escalated

#### ApprovalDecision (Onay Kararı)
- Her onaylayıcının verdiği karar
- Status: Approved, Rejected

### Kullanım Senaryosu: İzin Onay Süreci

#### 1. Onay Kuralı Oluşturma

**WebApp:**
```
1. Sol menüden "Onaylar" > "Onay Kuralları" sekmesine tıkla
2. "Yeni Onay Kuralı Ekle" butonuna tıkla
3. Formu doldur:
   - Ad: "Yıllık İzin Onay Kuralı"
   - Tip: "Leave"
   - Açıklama: "5 günden fazla izin için çift onay gerekir"
4. "Kaydet" butonuna tıkla
```

**API:**
```http
POST /api/approvalrules
Authorization: Bearer {token}
Content-Type: application/json

{
  "name": "Yıllık İzin Onay Kuralı",
  "ruleType": "Leave",
  "description": "5 günden fazla izin için çift onay gerekir",
  "condition": "{\"days\": {\"$gt\": 5}}"
}
```

**Not:** Onay zincirleri (ApprovalChain) şu anda API üzerinden eklenmelidir.

#### 2. Onay Talebi Oluşturma

**WebApp:**
```
1. Sol menüden "Onaylar" > "Onay Talepleri" sekmesine tıkla
2. "Yeni Onay Talebi" butonuna tıkla
3. Formu doldur:
   - Onay Kuralı ID: {approval-rule-id}
   - Talep Tipi: "Leave"
   - Talep Eden Kullanıcı ID: {user-id}
4. "Kaydet" butonuna tıkla
```

**API:**
```http
POST /api/approvalrequests
Authorization: Bearer {token}
Content-Type: application/json

{
  "approvalRuleId": "approval-rule-guid",
  "requestType": "Leave",
  "requesterUserId": "user-guid",
  "requestData": "{\"leaveType\": \"Annual\", \"days\": 7, \"startDate\": \"2026-02-15\"}"
}
```

#### 3. Onay Talebini Onaylama

**WebApp:**
```
1. "Onay Talepleri" listesinden bir talebe tıkla
2. "Detay" sayfasında "Onayla" butonuna tıkla
3. Notlar ekle (opsiyonel)
4. Onayla
```

**API:**
```http
POST /api/approvalrequests/{requestId}/approve
Authorization: Bearer {token}
Query Parameters:
  - approverUserId: "user-guid"
Content-Type: application/json

"Onay notları buraya"
```

#### 4. Onay Talebini Reddetme

**API:**
```http
POST /api/approvalrequests/{requestId}/reject
Authorization: Bearer {token}
Query Parameters:
  - rejectorUserId: "user-guid"
Content-Type: application/json

"Red nedeni buraya"
```

#### 5. Bekleyen Onayları Görüntüleme

**API:**
```http
GET /api/approvalrequests/pending?userId={user-id}&role={role-name}
Authorization: Bearer {token}
```

### Veritabanı Yapısı

- **ApprovalRules**: Onay kuralları
- **ApprovalChains**: Onay zincirleri
- **ApprovalRequests**: Onay talepleri
- **ApprovalDecisions**: Onay kararları

### Örnek Onay Senaryosu

```
1. Çalışan 7 günlük izin talebi oluşturur
   → ApprovalRequest oluşturulur (Status: Pending)

2. İlk Onay: Doğrudan Müdür
   → Müdür onaylar
   → ApprovalDecision kaydedilir (Status: Approved)

3. İkinci Onay: İK Müdürü
   → İK Müdürü onaylar
   → ApprovalDecision kaydedilir (Status: Approved)

4. Tüm gerekli onaylar tamamlandı
   → ApprovalRequest durumu: Approved
```

---

## Notification Domain

### Amaç
Bildirim şablonları oluşturma ve bildirim gönderme.

### Temel Kavramlar

#### NotificationTemplate (Bildirim Şablonu)
- Tekrar kullanılabilir bildirim şablonları
- Tip: Email, SMS, Push, InApp

#### Notification (Bildirim)
- Gönderilen bildirimler
- Durum: Pending, Sent, Delivered, Failed

### Kullanım Senaryosu: İzin Onay Bildirimi

#### 1. Bildirim Şablonu Oluşturma

**WebApp:**
```
1. Sol menüden "Bildirimler" > "Bildirim Şablonları" sekmesine tıkla
2. "Yeni Şablon Ekle" butonuna tıkla
3. Formu doldur:
   - Ad: "İzin Onay Bildirimi"
   - Tip: Email
   - Konu: "İzin Talebiniz Onaylandı"
   - İçerik: "Sayın {userName}, {days} günlük izin talebiniz onaylanmıştır."
4. "Kaydet" butonuna tıkla
```

**API:**
```http
POST /api/notificationtemplates
Authorization: Bearer {token}
Content-Type: application/json

{
  "name": "İzin Onay Bildirimi",
  "templateType": "Email",
  "subject": "İzin Talebiniz Onaylandı",
  "body": "Sayın {userName}, {days} günlük izin talebiniz onaylanmıştır."
}
```

#### 2. Bildirim Gönderme

**WebApp:**
```
1. Sol menüden "Bildirimler" > "Bildirimler" sekmesine tıkla
2. "Yeni Bildirim Gönder" butonuna tıkla
3. Formu doldur:
   - Bildirim Tipi: Email
   - Alıcı Kullanıcı ID: {user-id}
   - Alıcı Email: user@example.com
   - Konu: "İzin Talebiniz Onaylandı"
   - İçerik: "Sayın Ahmet Yılmaz, 7 günlük izin talebiniz onaylanmıştır."
   - Şablon ID: {template-id} (opsiyonel)
4. "Gönder" butonuna tıkla
```

**API:**
```http
POST /api/notifications
Authorization: Bearer {token}
Content-Type: application/json

{
  "notificationType": "Email",
  "recipientUserId": "user-guid",
  "recipientEmail": "user@example.com",
  "subject": "İzin Talebiniz Onaylandı",
  "body": "Sayın Ahmet Yılmaz, 7 günlük izin talebiniz onaylanmıştır.",
  "templateId": "template-guid" // opsiyonel
}
```

#### 3. Bildirim Durumunu Kontrol Etme

**API:**
```http
GET /api/notifications/{notificationId}
Authorization: Bearer {token}

Response:
{
  "id": "guid",
  "status": "Sent",
  "sentAt": "2026-02-11T10:00:00Z",
  "deliveredAt": "2026-02-11T10:00:05Z",
  ...
}
```

#### 4. Kullanıcının Bildirimlerini Görüntüleme

**API:**
```http
GET /api/notifications/recipient/{userId}
Authorization: Bearer {token}
```

### Veritabanı Yapısı

- **NotificationTemplates**: Bildirim şablonları
- **Notifications**: Gönderilen bildirimler

### Bildirim Tipleri

- **Email**: E-posta bildirimi
- **SMS**: SMS bildirimi
- **Push**: Push notification
- **InApp**: Uygulama içi bildirim

---

## Entegrasyon Senaryoları

### Senaryo 1: İzin Talebi ve Onay Süreci

Bu senaryo, tüm domain'lerin birlikte nasıl çalıştığını gösterir:

```
1. Kullanıcı Girişi (HumanResources)
   → Kullanıcı sisteme giriş yapar

2. İzin Talebi Oluşturma (ApprovalManagement)
   → Kullanıcı izin talebi oluşturur
   → ApprovalRequest oluşturulur

3. İş Akışı Başlatma (WorkflowEngine)
   → ApprovalRequest oluşturulduğunda WorkflowInstance başlatılır
   → İlk adım: "Müdür Onayı Bekleniyor"

4. Onay İşlemi (ApprovalManagement)
   → Müdür onaylar
   → ApprovalDecision kaydedilir
   → WorkflowInstance bir sonraki adıma geçer

5. Bildirim Gönderme (Notification)
   → Onaylandığında kullanıcıya bildirim gönderilir
   → Email bildirimi oluşturulur

6. İş Akışı Tamamlama (WorkflowEngine)
   → Tüm adımlar tamamlandığında WorkflowInstance Completed olur
```

### Senaryo 2: Otomatik Bildirim Gönderme

```
1. ApprovalRequest oluşturulur
   → Domain Event: ApprovalRequestCreated

2. Event Handler (Notification Domain)
   → Onaylayıcılara bildirim gönderilir
   → "Yeni onay talebi var" bildirimi

3. Onay kararı verilir
   → Domain Event: ApprovalRequestApproved/Rejected

4. Event Handler (Notification Domain)
   → Talep sahibine bildirim gönderilir
   → "Talebiniz onaylandı/reddedildi" bildirimi
```

### Senaryo 3: İş Akışı ile Onay Entegrasyonu

```
1. WorkflowInstance başlatılır
   → İlk adım: "Onay Talebi Oluştur"

2. ApprovalRequest otomatik oluşturulur
   → WorkflowInstance'dan gelen veriler kullanılır

3. Onay süreci tamamlanır
   → ApprovalRequest durumu: Approved

4. WorkflowInstance bir sonraki adıma geçer
   → "Onaylandı" adımına geçiş yapılır

5. İş akışı tamamlanır
   → WorkflowInstance durumu: Completed
```

---

## API Endpoint Özeti

### HumanResources

- `POST /api/auth/register` - Kullanıcı kaydı
- `POST /api/auth/login` - Giriş yapma
- `GET /api/users` - Kullanıcı listesi
- `GET /api/users/{id}` - Kullanıcı detayı
- `POST /api/users` - Kullanıcı oluşturma
- `PUT /api/users/{id}` - Kullanıcı güncelleme
- `DELETE /api/users/{id}` - Kullanıcı silme
- `GET /api/roles` - Rol listesi
- `POST /api/users/{userId}/roles` - Rol atama

### WorkflowEngine

- `GET /api/workflowdefinitions` - İş akışı tanımları listesi
- `GET /api/workflowdefinitions/{id}` - İş akışı tanımı detayı
- `POST /api/workflowdefinitions` - İş akışı tanımı oluşturma
- `PUT /api/workflowdefinitions/{id}` - İş akışı tanımı güncelleme
- `DELETE /api/workflowdefinitions/{id}` - İş akışı tanımı silme
- `POST /api/workflowinstances` - İş akışı instance başlatma
- `GET /api/workflowinstances/{id}` - İş akışı instance detayı
- `GET /api/workflowinstances/definition/{definitionId}` - Tanıma göre instance'lar
- `POST /api/workflowinstances/{id}/steps/{stepId}/complete` - Adım tamamlama
- `POST /api/workflowinstances/{id}/cancel` - İş akışı iptal etme

### ApprovalManagement

- `GET /api/approvalrules` - Onay kuralları listesi
- `GET /api/approvalrules/{id}` - Onay kuralı detayı
- `POST /api/approvalrules` - Onay kuralı oluşturma
- `PUT /api/approvalrules/{id}` - Onay kuralı güncelleme
- `DELETE /api/approvalrules/{id}` - Onay kuralı silme
- `POST /api/approvalrequests` - Onay talebi oluşturma
- `GET /api/approvalrequests/{id}` - Onay talebi detayı
- `GET /api/approvalrequests/requester/{userId}` - Kullanıcının talepleri
- `GET /api/approvalrequests/pending` - Bekleyen talepler
- `POST /api/approvalrequests/{id}/approve` - Onay talebini onaylama
- `POST /api/approvalrequests/{id}/reject` - Onay talebini reddetme
- `POST /api/approvalrequests/{id}/cancel` - Onay talebini iptal etme

### Notification

- `GET /api/notificationtemplates` - Bildirim şablonları listesi
- `GET /api/notificationtemplates/{id}` - Bildirim şablonu detayı
- `POST /api/notificationtemplates` - Bildirim şablonu oluşturma
- `PUT /api/notificationtemplates/{id}` - Bildirim şablonu güncelleme
- `DELETE /api/notificationtemplates/{id}` - Bildirim şablonu silme
- `POST /api/notifications` - Bildirim gönderme
- `GET /api/notifications/{id}` - Bildirim detayı
- `GET /api/notifications/recipient/{userId}` - Kullanıcının bildirimleri
- `GET /api/notifications/pending` - Bekleyen bildirimler
- `POST /api/notifications/{id}/delivered` - Bildirimi teslim edildi olarak işaretleme

---

## WebApp Kullanımı

### Giriş Yapmadan

- Sadece Ana Sayfa ve Login/Register sayfalarına erişilebilir
- Menüler görünmez

### Giriş Yaptıktan Sonra

- Sol sidebar menü görünür
- Tüm modüllere erişilebilir
- Aktif sayfa vurgulanır

### Menü Yapısı

```
Ana Sayfa
Kullanıcılar
├── Kullanıcı Listesi
├── Yeni Kullanıcı Ekle
├── Kullanıcı Düzenle
└── Kullanıcı Sil

İş Akışları
├── İş Akışı Tanımları
│   ├── Tanım Listesi
│   ├── Yeni Tanım Ekle
│   ├── Tanım Düzenle
│   └── Tanım Sil
└── İş Akışı Örnekleri
    ├── Örnek Listesi
    ├── Yeni Örnek Başlat
    └── Örnek Detayı

Onaylar
├── Onay Kuralları
│   ├── Kural Listesi
│   ├── Yeni Kural Ekle
│   ├── Kural Düzenle
│   └── Kural Sil
└── Onay Talepleri
    ├── Talep Listesi
    ├── Yeni Talep Oluştur
    └── Talep Detayı

Bildirimler
├── Bildirim Şablonları
│   ├── Şablon Listesi
│   ├── Yeni Şablon Ekle
│   ├── Şablon Düzenle
│   └── Şablon Sil
└── Bildirimler
    ├── Bildirim Listesi
    ├── Yeni Bildirim Gönder
    └── Bildirim Detayı
```

---

## Veritabanı Bağlantıları

Tüm domain'ler aynı SQL Server instance'ını kullanır ancak farklı veritabanlarına sahiptir:

- **BuildCoreHumanResources**: HumanResources domain
- **BuildCoreWorkflowEngine**: WorkflowEngine domain
- **BuildCoreApprovalManagement**: ApprovalManagement domain
- **BuildCoreNotification**: Notification domain

Connection String örneği:
```
Server=.\\SQLEXPRESS2022;Database=BuildCoreHumanResources;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True
```

---

## Örnek Kullanım Akışları

### Akış 1: Yeni Kullanıcı ve İzin Talebi

```
1. Admin olarak giriş yap
2. "Kullanıcılar" > "Yeni Kullanıcı Ekle"
3. Kullanıcı bilgilerini gir ve kaydet
4. Kullanıcı kendi hesabıyla giriş yapar
5. "Onaylar" > "Onay Talepleri" > "Yeni Onay Talebi"
6. İzin talebi oluştur
7. Müdür olarak giriş yap
8. "Onaylar" > "Onay Talepleri" > Bekleyen talepleri gör
9. Talebi onayla
10. Kullanıcıya bildirim gönderilir
```

### Akış 2: İş Akışı ile Entegre Onay

```
1. "İş Akışları" > "İş Akışı Tanımları" > Yeni tanım oluştur
2. "İş Akışları" > "İş Akışı Örnekleri" > Yeni örnek başlat
3. İş akışı otomatik olarak onay talebi oluşturur
4. Onaylayıcılar bildirim alır
5. Onaylar tamamlandığında iş akışı devam eder
6. İş akışı tamamlandığında sonuç bildirimi gönderilir
```

---

## Notlar

1. **Domain Event'ler**: Şu anda domain event'ler tanımlanmış ancak handler'lar henüz implement edilmemiştir. MassTransit entegrasyonu ile event-driven architecture tamamlanacaktır.

2. **Onay Zincirleri**: Onay zincirleri şu anda API üzerinden eklenmelidir. WebApp'te yakında eklenecektir.

3. **İş Akışı Adımları**: İş akışı adımları ve geçişler şu anda API üzerinden yönetilmelidir. WebApp'te yakında eklenecektir.

4. **Bildirim Gönderimi**: Şu anda bildirimler kaydedilir ancak gerçek gönderim (Email, SMS) henüz implement edilmemiştir. Bu özellik yakında eklenecektir.

5. **Authorization**: Tüm API endpoint'leri JWT token gerektirir. WebApp'te session tabanlı kimlik doğrulama kullanılır.

---

## Sorun Giderme

### Kullanıcı listede görünmüyor
- UserConfiguration'da HasQueryFilter kontrol edin
- UserRepository'de soft delete filtresi kontrol edin
- Veritabanında IsDeleted = 0 olup olmadığını kontrol edin

### Migration hataları
- Connection string'i kontrol edin
- SQL Server'ın çalıştığından emin olun
- Veritabanının oluşturulduğunu kontrol edin

### Login olmadan menülere erişim
- RequireLoginAttribute'un controller'lara eklendiğini kontrol edin
- Session'ın çalıştığından emin olun

---

## İletişim ve Destek

Sorularınız için GitHub Issues kullanabilirsiniz.
