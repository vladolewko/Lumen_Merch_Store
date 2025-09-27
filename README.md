# Lumen Merch Store

ASP.NET Core web-store with MySQL DB.

## Installation Guide

### Requirements
- .NET 8.0 SDK
- MySQL Server 8.0+

### Steps to Run

1. **Clone**
   ```bash
   git clone https://github.com/your-username/lumen-merch-store.git
   cd lumen-merch-store
   ```

2. **Create dependencies**
   ```bash
   dotnet restore
   ```

3**Replace connection string to your data**

   File `appsettings.json`:
   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Server=localhost;Database=LumenStoreDb;User=root;Password=YOUR_PASSWORD;Port=3306;"
   }
   ```

5. **Run migration**
   ```bash
   dotnet ef database update
   ```

6. **Run project from terminal/or UI**
   ```bash
   dotnet run
   ```

Application URL: https://localhost:5001

## Project Structure

- `Controllers/` - MVC controllers
- `Models/` - Entity Framework models
- `Views/` - Razor views
- `Data/` - DbContext
- `ViewModels/` - models for views

## Authentication & Authorization

- Registration: `/Account/Register`
- Login: `/Account/Login`
- Roles: Admin, User (default)