# CarRental

A full-stack car rental management system built with .NET 10 and Blazor. The solution consists of a REST API backend, a Blazor frontend, and a PostgreSQL database — all containerized and running in a single Docker network.

---

## Architecture

```
CarRental/
├── CarRental.API              # ASP.NET Core 10 REST API
├── CarRental.Application      # CQRS handlers, commands, queries (MediatR)
├── CarRental.Domain           # Entities, interfaces, domain results
├── CarRental.Infrastructure   # EF Core, repositories, JWT, mail, 2FA
└── CarRental.Web              # Blazor frontend (TODO)
```

The backend follows a **Clean Architecture** pattern with CQRS via MediatR. The API is the only entry point — the Blazor app communicates with it over HTTP.

---

## Tech Stack

### Backend
- **.NET 10** — ASP.NET Core Web API
- **Entity Framework Core 10** — ORM with Npgsql provider
- **PostgreSQL** — primary database
- **MediatR** — CQRS pattern
- **ASP.NET Core Identity** — user management and role-based authentication
- **JWT Bearer** — stateless authentication with access and refresh tokens
- **TOTP 2FA** — two-factor authentication via authenticator apps (Otp.NET)
- **Serilog** — structured logging
- **Scalar** — API documentation UI

### Frontend
- **Blazor** — .NET-based web frontend (TODO)

### Infrastructure
- **Docker & Docker Compose** — containerized deployment
- **SMTP** — transactional email (registration confirmation, password reset)

---

## Features

### Identity
- User registration with email confirmation (6-digit code)
- JWT authentication with separate web and mobile token lifetimes
- Refresh token rotation with per-client revocation
- Forgot password / reset password flow
- Change password (authenticated)
- Two-factor authentication via TOTP authenticator app
- 2FA recovery codes
- Role-based access control (Admin, Manager, Customer)

### User Management
- List and retrieve users (Admin, Manager)
- Update user profile (own profile or Admin)
- Deactivate user accounts (Admin)
- Admin 2FA revocation (Admin)

### Manufacturers
- Full CRUD for vehicle manufacturers
- Public read access, write access restricted by role

### Pickup Locations
- Full CRUD for pickup locations
- Per-location working hours management (by day of week)
- Per-location holiday schedule with optional modified hours
- Public read access, write access restricted by role

---

## Running with Docker

All three services — API, Blazor frontend, and PostgreSQL — run in the same Docker network.
