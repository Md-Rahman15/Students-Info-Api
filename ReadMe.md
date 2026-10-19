# Student Information Management Web API

[![.NET 8](https://img.shields.io/badge/.NET-8.0-512BD4?logo=dotnet)](https://dotnet.microsoft.com/)
[![SQL Server](https://img.shields.io/badge/Database-SQL%20Server-CC292B?logo=microsoft-sql-server)](https://www.microsoft.com/sql-server)
[![Azure Deployed](https://img.shields.io/badge/Deployment-Azure%20App%20Service-0078D4?logo=microsoft-azure)](https://YOUR-APP-NAME.azurewebsites.net/swagger)

A production-style RESTful API built with **ASP.NET Core Web API**, **C#**, **Entity Framework Core**, and **SQL Server**. The system implements an N-Tier architecture (`Controllers` -> `Services` -> `Repositories`) with role-based access control, cryptographic token verification, and automated image lifecycle management.


 Architecture Overview

The application follows a decoupled N-Tier pattern to separate HTTP contracts, business orchestration, and data persistence:


[ HTTP Requests ]
       │
       ▼
[ API Controllers ]  ──► Returns consistent DTOs & ProblemDetails
       │
       ▼
[ Service Layer ]    ──► Business validation, Token generation, File handling
       │
       ▼
[ Repository Layer ] ──► Deferred LINQ queries (IQueryable), EF Core DbContext
       │
       ▼
[ MS SQL Server ]   ──► Relational schema with Foreign Keys & Unique Constraints