# Student Information Management Web API

[![.NET 8](https://img.shields.io/badge/.NET-8.0-512BD4?logo=dotnet)](https://dotnet.microsoft.com/)
[![SQL Server](https://img.shields.io/badge/Database-SQL%20Server-CC292B?logo=microsoft-sql-server)](https://www.microsoft.com/sql-server)
[![Azure Deployed](https://img.shields.io/badge/Deployment-Azure%20App%20Service-0078D4?logo=microsoft-azure)](https://student-info-api-anif-2026-d3cjecehg9fhbbfq.centralindia-01.azurewebsites.net/swagger)

A production-ready RESTful backend API built with **ASP.NET Core Web API**, **C#**, **Entity Framework Core**, and **Microsoft SQL Server**. The application implements an N-Tier architecture (`Controllers` -> `Services` -> `Repositories`) featuring role-based access control, cryptographic token verification, server-side data pagination, and automated image lifecycle management with compensating rollback.

🌐 **Live Azure Base URL:** `[https://student-info-api-anif-2026-d3cjecehg9fhbbfq.centralindia-01.azurewebsites.net](https://student-info-api-anif-2026-d3cjecehg9fhbbfq.centralindia-01.azurewebsites.net)`  
🔗 **Interactive Swagger UI:** [Open Live Swagger UI](https://student-info-api-anif-2026-d3cjecehg9fhbbfq.centralindia-01.azurewebsites.net/swagger)

---

## Architecture Overview

The application follows a decoupled N-Tier pattern to separate HTTP contracts, business orchestration, and data persistence:

| Layer | Component | Core Responsibilities |
| :--- | :--- | :--- |
| **1. Presentation** | `API Controllers` | Endpoint routing, DTO model validation, AutoMapper conversion, standardized `ProblemDetails` responses |
| **2. Business Logic** | `Service Layer` | Domain validation, SHA-256 token hashing, PBKDF2 password hashing, MailKit SMTP dispatch, compensating file rollback |
| **3. Data Access** | `Repository Layer` | EF Core `ApplicationDbContext` operations, deferred `IQueryable` execution, server-side pagination, sorting, and filtering |
| **4. Database** | `MS SQL Server` | Relational schema enforcement, Foreign Keys, Unique constraints, and indexed lookups |

---

## Key Engineering & Security Highlights

* **Cryptographic Token Architecture:**
  * Password reset and email verification tokens generated with 256 bits of entropy via `RandomNumberGenerator`.
  * Tokens stored in SQL Server strictly as **SHA-256 hex hashes**, preventing token exposure even if database records are leaked.
  * Anti-enumeration defenses across authentication and password recovery endpoints.
* **Role-Based Access Control (RBAC):**
  * `Admin`: Full CRUD administrative control over student profiles and course catalogs.
  * `Student`: Self-service profile management (`/api/Student/me`) extracting identity claims (`ClaimTypes.NameIdentifier`) directly from the verified JWT payload to prevent ID spoofing.
* **Server-Side Search, Filtering & Pagination:**
  * Dynamic `IQueryable` queries execute sorting, searching, and pagination (`.Skip()`, `.Take()`) directly in SQL Server rather than in application memory.
* **Compensating File Rollback:**
  * Multipart profile picture uploads (`IFormFile`) are validated for MIME type and file size (max 5 MB).
  * Automatically purges newly written image files from disk if downstream database persistence fails, and deletes stale images upon profile photo replacement.
* **Reliability & Logging:**
  * Custom `GlobalExceptionMiddleware` catches unhandled exceptions and logs contextual `TraceIdentifier` data via **Serilog**.
  * Structured Result pattern (`StudentsResultClass<T>` + `ErrorType` enum) avoiding unhandled 500 crashes on expected domain validation failures.

---

## Tech Stack

| Category | Technology |
| :--- | :--- |
| **Framework** | ASP.NET Core Web API (.NET 8) |
| **Language** | C# 12 |
| **ORM & Database** | Entity Framework Core, Microsoft SQL Server |
| **Security & Auth** | JWT Bearer, ASP.NET Core Identity Password Hasher (PBKDF2), SHA-256 |
| **Object Mapping** | AutoMapper |
| **Email Delivery** | MailKit / MimeKit (SMTP) |
| **Logging** | Serilog (Console & Rolling File sinks) |
| **API Documentation** | Swagger / OpenAPI (Swashbuckle) |
| **Cloud Hosting** | Azure App Service (Central India) |

---

## API Endpoints & Live URLs

### 1. Authentication Endpoints

| Method | Endpoint Route | Access | Full Deployed URL |
| :--- | :--- | :--- | :--- |
| `POST` | `/api/Auth/Register` | Public | `[https://student-info-api-anif-2026-d3cjecehg9fhbbfq.centralindia-01.azurewebsites.net/api/Auth/Register](https://student-info-api-anif-2026-d3cjecehg9fhbbfq.centralindia-01.azurewebsites.net/api/Auth/Register)` |
| `POST` | `/api/Auth/Login` | Public | `[https://student-info-api-anif-2026-d3cjecehg9fhbbfq.centralindia-01.azurewebsites.net/api/Auth/Login](https://student-info-api-anif-2026-d3cjecehg9fhbbfq.centralindia-01.azurewebsites.net/api/Auth/Login)` |
| `POST` | `/api/Auth/EmailTokenVerify` | Public | `[https://student-info-api-anif-2026-d3cjecehg9fhbbfq.centralindia-01.azurewebsites.net/api/Auth/EmailTokenVerify](https://student-info-api-anif-2026-d3cjecehg9fhbbfq.centralindia-01.azurewebsites.net/api/Auth/EmailTokenVerify)` |
| `POST` | `/api/Auth/Forgot%20Password` | Public | `[https://student-info-api-anif-2026-d3cjecehg9fhbbfq.centralindia-01.azurewebsites.net/api/Auth/Forgot%20Password](https://student-info-api-anif-2026-d3cjecehg9fhbbfq.centralindia-01.azurewebsites.net/api/Auth/Forgot%20Password)` |
| `POST` | `/api/Auth/Email%20Based%20Password%20Reset` | Public | `[https://student-info-api-anif-2026-d3cjecehg9fhbbfq.centralindia-01.azurewebsites.net/api/Auth/Email%20Based%20Password%20Reset](https://student-info-api-anif-2026-d3cjecehg9fhbbfq.centralindia-01.azurewebsites.net/api/Auth/Email%20Based%20Password%20Reset)` |
| `PATCH` | `/api/Auth/Password%20Change` | Authenticated | `[https://student-info-api-anif-2026-d3cjecehg9fhbbfq.centralindia-01.azurewebsites.net/api/Auth/Password%20Change](https://student-info-api-anif-2026-d3cjecehg9fhbbfq.centralindia-01.azurewebsites.net/api/Auth/Password%20Change)` |
| `POST` | `/api/Auth/UserAccountEmailChange` | Authenticated | `[https://student-info-api-anif-2026-d3cjecehg9fhbbfq.centralindia-01.azurewebsites.net/api/Auth/UserAccountEmailChange](https://student-info-api-anif-2026-d3cjecehg9fhbbfq.centralindia-01.azurewebsites.net/api/Auth/UserAccountEmailChange)` |
| `POST` | `/api/Auth/EmailBasedEmailReset` | Public | `[https://student-info-api-anif-2026-d3cjecehg9fhbbfq.centralindia-01.azurewebsites.net/api/Auth/EmailBasedEmailReset](https://student-info-api-anif-2026-d3cjecehg9fhbbfq.centralindia-01.azurewebsites.net/api/Auth/EmailBasedEmailReset)` |

### 2. Course Endpoints

| Method | Endpoint Route | Access | Full Deployed URL |
| :--- | :--- | :--- | :--- |
| `GET` | `/api/Courses` | Student / Admin | `[https://student-info-api-anif-2026-d3cjecehg9fhbbfq.centralindia-01.azurewebsites.net/api/Courses](https://student-info-api-anif-2026-d3cjecehg9fhbbfq.centralindia-01.azurewebsites.net/api/Courses)` |
| `GET` | `/api/Courses/{getid}` | Student / Admin | `[https://student-info-api-anif-2026-d3cjecehg9fhbbfq.centralindia-01.azurewebsites.net/api/Courses/](https://student-info-api-anif-2026-d3cjecehg9fhbbfq.centralindia-01.azurewebsites.net/api/Courses/){getid}` |
| `POST` | `/api/Courses` | Admin Only | `[https://student-info-api-anif-2026-d3cjecehg9fhbbfq.centralindia-01.azurewebsites.net/api/Courses](https://student-info-api-anif-2026-d3cjecehg9fhbbfq.centralindia-01.azurewebsites.net/api/Courses)` |
| `PATCH` | `/api/Courses/{id}` | Admin Only | `[https://student-info-api-anif-2026-d3cjecehg9fhbbfq.centralindia-01.azurewebsites.net/api/Courses/](https://student-info-api-anif-2026-d3cjecehg9fhbbfq.centralindia-01.azurewebsites.net/api/Courses/){id}` |
| `DELETE` | `/api/Courses/{id}` | Admin Only | `[https://student-info-api-anif-2026-d3cjecehg9fhbbfq.centralindia-01.azurewebsites.net/api/Courses/](https://student-info-api-anif-2026-d3cjecehg9fhbbfq.centralindia-01.azurewebsites.net/api/Courses/){id}` |

### 3. Student Endpoints

| Method | Endpoint Route | Access | Full Deployed URL |
| :--- | :--- | :--- | :--- |
| `POST` | `/api/Student` | Student / Admin | `[https://student-info-api-anif-2026-d3cjecehg9fhbbfq.centralindia-01.azurewebsites.net/api/Student](https://student-info-api-anif-2026-d3cjecehg9fhbbfq.centralindia-01.azurewebsites.net/api/Student)` |
| `GET` | `/api/Student/me` | Student Only | `[https://student-info-api-anif-2026-d3cjecehg9fhbbfq.centralindia-01.azurewebsites.net/api/Student/me](https://student-info-api-anif-2026-d3cjecehg9fhbbfq.centralindia-01.azurewebsites.net/api/Student/me)` |
| `PATCH` | `/api/Student/UpdateMyProfile` | Student Only | `[https://student-info-api-anif-2026-d3cjecehg9fhbbfq.centralindia-01.azurewebsites.net/api/Student/UpdateMyProfile](https://student-info-api-anif-2026-d3cjecehg9fhbbfq.centralindia-01.azurewebsites.net/api/Student/UpdateMyProfile)` |
| `GET` | `/api/Student` | Admin Only | `[https://student-info-api-anif-2026-d3cjecehg9fhbbfq.centralindia-01.azurewebsites.net/api/Student](https://student-info-api-anif-2026-d3cjecehg9fhbbfq.centralindia-01.azurewebsites.net/api/Student)` |
| `GET` | `/api/Student/{getid}` | Admin Only | `[https://student-info-api-anif-2026-d3cjecehg9fhbbfq.centralindia-01.azurewebsites.net/api/Student/](https://student-info-api-anif-2026-d3cjecehg9fhbbfq.centralindia-01.azurewebsites.net/api/Student/){getid}` |
| `PATCH` | `/api/Student/{id}` | Admin Only | `[https://student-info-api-anif-2026-d3cjecehg9fhbbfq.centralindia-01.azurewebsites.net/api/Student/](https://student-info-api-anif-2026-d3cjecehg9fhbbfq.centralindia-01.azurewebsites.net/api/Student/){id}` |
| `DELETE` | `/api/Student/{deleteid}` | Admin Only | `[https://student-info-api-anif-2026-d3cjecehg9fhbbfq.centralindia-01.azurewebsites.net/api/Student/](https://student-info-api-anif-2026-d3cjecehg9fhbbfq.centralindia-01.azurewebsites.net/api/Student/){deleteid}` |

---

## Testing & Request Payloads

### Student Creation (`POST /api/Student` - `multipart/form-data`)

> **Important:** Send `POST /api/Student` and `PATCH` profile updates as **`multipart/form-data`** (not raw JSON) because requests include an `IFormFile` upload.

| Field Name | Required | Type | Validation Rules | Example Value |
| :--- | :--- | :--- | :--- | :--- |
| `Image` | **Yes** | File | `.jpg`, `.jpeg`, `.png` (Max: 5 MB) | Select local image |
| `FirstName` | **Yes** | Text | Student first name | `Ayaan` |
| `LastName` | **Yes** | Text | Student last name | `Khan` |
| `Email` | **Yes** | Text | Unique valid email (Max 100 chars) | `ayaan@example.com` |
| `PhoneNumber` | **Yes** | Text | 10-digit Indian mobile (`^[6-9]\d{9}$`) | `9876543210` |
| `DateofBirth` | **Yes** | Date | `YYYY-MM-DD` (Minimum age: 16) | `2005-04-12` |
| `Gender` | **Yes** | Text | `Male`, `Female`, or `Others` | `Male` |
| `EnrollmentDate` | **Yes** | Date | `YYYY-MM-DD` (After `DateofBirth`) | `2026-08-01` |
| `CourseName` | **Yes** | Text | Must match an existing course name | `MCA` |

---

## Local Development Setup

1. **Clone the repository:**
   `git clone [https://github.com/Md-Rahman15/Students-Info-Api.git](https://github.com/Md-Rahman15/Students-Info-Api.git)`
2. **Configure `appsettings.json`:** Update `StudentConnection`, `JwtSettings`, and `EmailSMTP` credentials.
3. **Apply EF Core Migrations:**
   `dotnet ef database update`
4. **Run the application:**
   `dotnet run`
