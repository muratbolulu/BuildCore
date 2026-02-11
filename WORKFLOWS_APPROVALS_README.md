# Workflows & Approvals Modülleri

Bu dokümantasyon, BuildCore projesine eklenen **Workflows & Approvals** modüllerinin yapısını ve planını açıklar.

## 📋 Domain Yapısı

### 1. WorkflowEngine Domain

**Amaç:** İş akışı tanımlama ve yürütme motoru

**Katmanlar:**
- `BuildCore.WorkflowEngine.Domain` - Domain entities, value objects, domain events
- `BuildCore.WorkflowEngine.Application` - Use cases, commands, queries, DTOs
- `BuildCore.WorkflowEngine.Infrastructure` - Persistence, external services

**Planlanan Özellikler:**
- Workflow tanımlama (BPMN-like)
- Workflow instance yönetimi
- State machine implementasyonu
- Workflow execution engine
- Workflow history ve audit

**Aggregates (Planlanan):**
- `WorkflowDefinition` (Aggregate Root)
- `WorkflowInstance` (Aggregate Root)

**Value Objects (Planlanan):**
- `WorkflowStep`
- `Transition`
- `WorkflowVariable`

**Domain Events (Planlanan):**
- `WorkflowStarted`
- `WorkflowCompleted`
- `WorkflowCancelled`
- `StepCompleted`

---

### 2. ApprovalManagement Domain

**Amaç:** Onay süreçleri ve onay zinciri yönetimi

**Katmanlar:**
- `BuildCore.ApprovalManagement.Domain` - Domain entities, value objects, domain events
- `BuildCore.ApprovalManagement.Application` - Use cases, commands, queries, DTOs
- `BuildCore.ApprovalManagement.Infrastructure` - Persistence, external services

**Planlanan Özellikler:**
- Onay kuralları motoru
- Onay zinciri builder
- Onay kararları takibi
- Escalation kuralları
- Onay politikaları

**Aggregates (Planlanan):**
- `ApprovalRule` (Aggregate Root)
- `ApprovalRequest` (Aggregate Root)

**Value Objects (Planlanan):**
- `ApprovalChain`
- `ApprovalDecision`
- `ApprovalPolicy`

**Domain Events (Planlanan):**
- `ApprovalRequestCreated`
- `ApprovalRequestApproved`
- `ApprovalRequestRejected`
- `ApprovalRequestEscalated`

---

### 3. Notification Domain

**Amaç:** Çok kanallı bildirim servisi

**Katmanlar:**
- `BuildCore.Notification.Domain` - Domain entities, value objects, domain events
- `BuildCore.Notification.Application` - Use cases, commands, queries, DTOs
- `BuildCore.Notification.Infrastructure` - Persistence, messaging, external services

**Planlanan Özellikler:**
- Bildirim şablonları
- Çok kanallı teslimat (Email, SMS, Push, In-App)
- Bildirim kuyruğu
- Teslimat takibi

**Aggregates (Planlanan):**
- `NotificationTemplate` (Aggregate Root)
- `Notification` (Aggregate Root)

**Value Objects (Planlanan):**
- `NotificationChannel`
- `NotificationContent`

**Domain Events (Planlanan):**
- `NotificationSent`
- `NotificationDelivered`
- `NotificationFailed`

---

## 🏗️ Mimari Yapı

```
WorkflowEngine Domain
    ↓
ApprovalManagement Domain (WorkflowEngine'i kullanır)
    ↓
Notification Domain (Her iki domain'den event'leri dinler)
```

### Domain İlişkileri

- **WorkflowEngine** → Bağımsız domain (core)
- **ApprovalManagement** → WorkflowEngine'i kullanabilir
- **Notification** → Her iki domain'den domain event'leri dinler (loose coupling)

---

## 📁 Proje Yapısı

```
BuildCore/
├── BuildCore.WorkflowEngine.Domain/
│   ├── Entities/
│   ├── DomainEvents/
│   ├── ValueObjects/
│   └── Interfaces/
│
├── BuildCore.WorkflowEngine.Application/
│   ├── Commands/
│   ├── Queries/
│   ├── DTOs/
│   ├── Interfaces/
│   └── UseCases/
│
├── BuildCore.WorkflowEngine.Infrastructure/
│   ├── Persistence/
│   └── ExternalServices/
│
├── BuildCore.ApprovalManagement.Domain/
│   ├── Entities/
│   ├── DomainEvents/
│   ├── ValueObjects/
│   └── Interfaces/
│
├── BuildCore.ApprovalManagement.Application/
│   ├── Commands/
│   ├── Queries/
│   ├── DTOs/
│   ├── Interfaces/
│   └── UseCases/
│
├── BuildCore.ApprovalManagement.Infrastructure/
│   ├── Persistence/
│   └── ExternalServices/
│
├── BuildCore.Notification.Domain/
│   ├── Entities/
│   ├── DomainEvents/
│   ├── ValueObjects/
│   └── Interfaces/
│
├── BuildCore.Notification.Application/
│   ├── Commands/
│   ├── Queries/
│   ├── DTOs/
│   ├── Interfaces/
│   └── UseCases/
│
└── BuildCore.Notification.Infrastructure/
    ├── Persistence/
    ├── Messaging/
    └── ExternalServices/
```

---

## 🔄 İş Akışı Senaryosu

### Senaryo: İzin Talebi Onay Süreci

```
1. User → İzin talebi oluşturur
   ↓
2. ApprovalManagement → ApprovalRequest oluşturur
   ↓
3. ApprovalManagement → ApprovalRequestCreated event yayınlar
   ↓
4. WorkflowEngine → WorkflowInstance başlatır
   ↓
5. WorkflowEngine → İlk onay adımına geçer
   ↓
6. Notification → Onaylayıcıya bildirim gönderir
   ↓
7. Approver → Onay/Red kararı verir
   ↓
8. ApprovalManagement → ApprovalRequestApproved/Rejected event yayınlar
   ↓
9. WorkflowEngine → Sonraki adıma geçer veya tamamlar
   ↓
10. Notification → Kullanıcıya sonuç bildirimi gönderir
```

---

## 🚀 Sonraki Adımlar

### Faz 1: Temel Yapı (Şu an)
- ✅ Domain projeleri oluşturuldu
- ✅ Application projeleri oluşturuldu
- ✅ Infrastructure projeleri oluşturuldu
- ✅ Solution'a eklendi

### Faz 2: Domain Entities (Sonraki)
- [ ] WorkflowEngine domain entities
- [ ] ApprovalManagement domain entities
- [ ] Notification domain entities

### Faz 3: Application Layer
- [ ] Commands ve Queries
- [ ] DTOs
- [ ] Use cases

### Faz 4: Infrastructure
- [ ] DbContext'ler
- [ ] Repository implementasyonları
- [ ] EF Core configurations
- [ ] Migrations

### Faz 5: Integration
- [ ] Domain event handlers
- [ ] MassTransit entegrasyonu
- [ ] API controllers
- [ ] WebApp integration

---

## 📝 Notlar

- Her domain bağımsız olarak geliştirilebilir
- Domain'ler arası iletişim domain event'lerle yapılır (loose coupling)
- Notification domain, diğer domain'lerden bağımsızdır (event-driven)
- WorkflowEngine, ApprovalManagement tarafından kullanılabilir ama zorunlu değildir

---

**Son Güncelleme:** 2026-02-11
