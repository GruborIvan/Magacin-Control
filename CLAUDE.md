# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

**Magacin Control** is a WPF desktop application for warehouse management (PanSport). Warehouse employees use it to verify that scanned products match invoices loaded from CSV files.

## Build & Run

```bash
# Build
dotnet build CSS_PA_Otprema.csproj

# Run (Debug — uses LocalConnectionString)
dotnet run --project CSS_PA_Otprema

# Run (Release — uses DeployConnectionString pointing to 192.168.1.10)
dotnet run --configuration Release --project CSS_PA_Otprema

# EF Core migrations
dotnet ef migrations add MigrationName
dotnet ef database update
```

Open `CSS_PA_Otprema.sln` in Visual Studio 2022 for IDE development.

## Tech Stack

- .NET 6.0 WPF, C#, target `net6.0-windows`
- **ORM**: Entity Framework Core 7 (`AppDbContext`)
- **DI**: Autofac + `Microsoft.Extensions.Hosting`
- **Mapping**: AutoMapper 12 (profiles in `Modules/AutoMapperModule.cs`)
- **CSV parsing**: CsvHelper 30
- **Excel export**: NPOI 2.6
- **Logging**: Serilog (writes to desktop log file)
- **Database**: SQL Server (`MagacinPan05` database)

## Architecture

### Folder Structure

```
Windows/        — WPF Views (code-behind only, no .xaml ViewModels)
ViewModels/     — MVVM ViewModels for data binding
Models/
  DboModels/    — EF Core entities + AppDbContext
  CsvModels/    — CsvHelper import models
Repository/     — Data access (EF Core, async)
Services/       — Business logic
Interfaces/     — Abstractions for all repos and services
Parsers/        — CSV file parsing (FileParser.cs)
Modules/        — Autofac DI modules (AutoMapper, DbConnection)
Migrations/     — EF Core migrations
Dialog/         — DialogHandler for error/confirm dialogs
Ini_Files/      — INI configuration files
```

### Startup Flow

`App.xaml.cs` → builds `IHost` with Serilog → runs EF migrations on startup → shows `AuthenticationWindow` → on login, opens `MainWindow`.

### DI Registration

All services/repos registered in `App.xaml.cs` using `Microsoft.Extensions.DependencyInjection`. Autofac modules (`AutoMapperModule`, `DbConnectionModule`) are loaded via `ConfigureContainer`. `AppDbContext` uses a factory pattern (transient) to support concurrent async operations.

### Database Access Pattern

Repository → Service → ViewModel/Window. All DB calls are async (`Task`-based). `RobaRepository` handles most operations:

- **Faktura** (invoices) → `RobaZaPakovanje` table → `FakturaDbo`
- **Ident** (line items) → `RobaZaPakovanjeItem` table → `IdentDbo`
- **Barcode mappings** → `IdentBarkod` table → `IdentBarkodDbo`

### Connection String Selection

`#if DEBUG` uses `LocalConnectionString` (dev SQL Server); Release build uses `DeployConnectionString` (production at `192.168.1.10`). Both defined in `appsettings.json` / `appsettings.development.json`.

### Key Business Logic

- **CSV Import**: Three-file import per invoice batch — base file, `_code.txt` (product codes), `_item.txt` (quantities). `FileParser.cs` handles parsing and barcode-duplicate detection.
- **Service codes**: Items with codes starting with `7xx` are exempt from barcode scanning.
- **Invoice status**: `U radu` (in progress) → `Završeno` (done). Can only mark done when all non-service items are scanned correctly (`KolicinaSaFakture == PripremljenaKolicina`).
- **Barcode validation**: Duplicate barcode-to-product assignments are detected and flagged at import time.
