# Inventory Management Dashboard

An enterprise-grade, full-stack Inventory Management Dashboard built with **ASP.NET Core 9 Web API**, **Entity Framework Core**, **SQL Server**, and **Angular 18+**.

---

## 🌟 Key Features

### Backend (.NET 9 Web API)
- **SQL Server & EF Core**: Code-first migrations with automatic schema initialization and data seeding.
- **Robust Inventory Rules**:
  - Atomic stock movements with strict concurrency handling.
  - Prevention of negative stock balances (returns 409 Conflict with informative error details).
  - Cascade delete restrictions protecting products with active transaction history and categories containing active products.
- **RESTful Endpoints**:
  - Products catalog CRUD, multi-attribute filtering (category, stock status), text search, and pagination.
  - Categories management with associated product counts.
  - Stock transaction audit trails (Stock-In, Stock-Out, and Stock Reconciliation Adjustments).
  - High-performance Dashboard KPIs and exception reports.

### Frontend (Angular 18+ Standalone Architecture)
- **Modular Component Design**: Every component features dedicated template (`.html`), styles (`.scss`), and TypeScript logic (`.ts`).
- **Interactive Dashboard**: Real-time KPI summary cards (Total Products, Units in Stock, Low Stock Alerts, Out of Stock Items) and live transaction feeds.
- **Product Management**: Responsive catalog table with instant search, category filtering, stock status pills, and detail views.
- **Inventory Operations**: Dedicated forms for Stock In, Stock Out, and physical stock count adjustments with live client-side validation.
- **Audit Logs & Reports**: Comprehensive transaction history with before/after balances and dedicated low-stock exception reporting.

---

## 🏗️ Architecture & Project Structure

```text
├── backend/
│   ├── Controllers/        # Web API endpoints (Products, Categories, Stock, Dashboard)
│   ├── Data/               # ApplicationDbContext & DbSeeder
│   ├── DTOs/               # Strongly-typed request/response models
│   ├── Entities/           # EF Core entity models (Product, Category, StockTransaction)
│   ├── Migrations/         # EF Core migration history
│   ├── Services/           # Business logic layer with validation rules
│   └── Validators/         # Fluent validation rules
│
├── frontend/
│   └── src/app/
│       ├── core/           # Services, models, and HTTP interceptors
│       ├── features/
│       │   ├── categories/ # Category list and creation/edit forms
│       │   ├── dashboard/  # Overview metrics, charts, and preview tables
│       │   ├── products/   # Product catalog, detail, and forms
│       │   ├── reports/    # Low stock & out-of-stock reporting
│       │   └── stock-transactions/ # Stock in/out/adjustment forms and audit trail
│       └── layout/         # Application header and sidebar navigation
└── InventoryDashboard.sln
```

---

## 🚀 Getting Started

### Prerequisites
- [.NET 9.0 SDK](https://dotnet.microsoft.com/)
- [Node.js (v18+)](https://nodejs.org/) & npm
- [SQL Server](https://www.microsoft.com/sql-server/) (or LocalDB)

### 1. Backend Setup
```bash
cd backend
dotnet restore
dotnet run
```
The API server starts on `http://localhost:5109` (or `https://localhost:7109`).

### 2. Frontend Setup
```bash
cd frontend
npm install
npm start
```
Navigate to `http://localhost:4200/` in your browser.

---

## 🔒 Business Logic Highlights
- **Stock-Out Protection**: Rejects deductions exceeding current stock to prevent negative inventory balances.
- **Stock Reconciliation**: Adjustments compute positive or negative adjustments dynamically based on current on-hand quantities.
- **Referential Integrity**: Categories with linked products cannot be deleted. Products with historical transactions cannot be deleted.
