    //Domain event üretilir, SaveChanges commit olur, Interceptor publish eder.
    Persistence = verinin nasıl saklandığı ve geri alındığı

/*
Application der ki:
"Order'ı kaydet"

Persistence der ki:
"Tamam, SQL Server'da şöyle kaydediyorum"
*/

Infrastructure içindeki : Configurations (Fluent API) 

/*
Altın kurallar (senin için birebir)
1️⃣ Domain → Persistence bilmez
2️⃣ Application → Interface bilir
3️⃣ Persistence → EF Core bilir
4️⃣ SaveChanges → tek merkez
5️⃣ Interceptor → cross-cutting
*/

/*
mimari mantra

Persistence veri saklar, kural koymaz.
Interceptor kuralı uygular, karar vermez.
*/


##mini akış 

Program.cs
  ↓
services.AddPersistence()
  ↓
DbContext + Interceptors + Repo + UoW register
  ↓
Application Handler
  ↓
Repository.Add()
  ↓
UnitOfWork.SaveChanges()
  ↓
EF Core Pipeline
  ↓
AuditInterceptor
  ↓
DomainEventInterceptor
  ↓
DB Commit


////
🧠 Mimari Altın Kural (buraya yıldız at ⭐)

Entity davranışı bilir
Audit & zaman Infrastructure bilir
////

Cross-cutting concern → Interceptor

####### 
✅ Visibility (izin)
SharedKernel  ──► (friend) Infrastructure


Sadece internal üyeleri görebilir

Kod kullanmaz

Referans yok

✔️ VAR

Bu, C# dil özelliği. Mimari bağımlılık değil.

InternalsVisibleTo ne yapar, ne yapmaz?
Ne yapar

Infrastructure’a şunu der:

“Benim internal kapımdan girebilirsin”

Ne yapmaz

SharedKernel, Infrastructure’a referans eklemez

Infrastructure kodunu çağırmaz

Compile-time dependency oluşturmaz

📌 Yani SharedKernel Infrastructure’ı “bilmez”,
sadece güvenir.
####### 
