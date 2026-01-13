# FleetManage API

A .NET 8 Web API for fleet management, enabling multi-tenant SaaS capabilities with Stripe integration.

## 🚀 Getting Started

Follow these instructions to set up the project on your local machine.

### Prerequisites

1.  **PostgreSQL** (Database)
    *   Install PostgreSQL (v14 or newer recommended)
    *   Ensure it is running on port `5432` (default).
2.  **.NET 8 SDK**
    *   [Download .NET 8](https://dotnet.microsoft.com/download/dotnet/8.0)

### 🛠️ Setup Guide

#### 1. Clone the Repository
```bash
git clone https://github.com/hojayevadurli/FleetManageApi.git
cd FleetManageApi/FleetManage.Api
```

#### 2. Configure Secrets (Important!)
We use **User Secrets** to keep sensitive data out of GitHub. Run these commands in your terminal (inside the `FleetManage.Api` folder) to set up your local environment:

**Database:**
```powershell
dotnet user-secrets set "ConnectionStrings:Default" "Host=localhost;Port=5432;Database=FleetManage;Username=postgres;Password=YOUR_POSTGRES_PASSWORD"
```

**Stripe (Required for content/registration):**
Get these keys from your Stripe Dashboard (Test Mode).
```powershell
dotnet user-secrets set "Stripe:PublishableKey" "pk_test_..."
dotnet user-secrets set "Stripe:SecretKey" "sk_test_..."
dotnet user-secrets set "Stripe:WebhookSecret" "whsec_..."
```

**Security & Email:**
```powershell
dotnet user-secrets set "Jwt:Key" "YOUR_SUPER_SECRET_KEY_MUST_BE_32_CHARS_LONG"
dotnet user-secrets set "Email:Password" "your-app-password"
```

#### 3. Create the Database
This command will create the `FleetManage` database and apply all tables/migrations.
```bash
dotnet ef database update
```

#### 4. Run the API
```bash
dotnet run
```
The API will be available at:
*   **Http**: `http://localhost:5139` (or your local IP)
*   **Swagger UI**: `http://localhost:5139/swagger`

## 🔑 Key Features
*   **Multi-Tenancy**: Data isolation per company.
*   **Stripe Integration**: Automated billing and subscription handling.
*   **Role-Based Auth**: Admin vs User permissions.
