# Quick Start Guide - Expense Control App

## Prerequisites
- .NET 9.0 SDK
- PostgreSQL 15 (or Docker)

## Option 1: Local Development (sin Docker)

1. **Requisitos**
   - .NET 10 SDK (o .NET 9; el proyecto está configurado para .NET 10)
   - PostgreSQL 15+ en ejecución (puerto 5432)

2. **PostgreSQL**
   ```bash
   # macOS con Homebrew
   brew install postgresql@15
   brew services start postgresql@15
   ```
   La aplicación crea la base de datos automáticamente en el primer arranque (`ExpenseControlDB_Dev` en Development).  
   Ajusta usuario/contraseña en `appsettings.Development.json` si es necesario (por defecto: `postgres` / `password`).

3. **Ejecutar la aplicación**
   ```bash
   cd appControlGastos
   dotnet restore
   dotnet run --launch-profile http
   ```
   Se usa el puerto **5050** (evita conflicto con AirPlay en macOS que usa 5000).

4. **Acceder a la aplicación**
   - Web UI: **http://localhost:5050**
   - API Docs: **http://localhost:5050/swagger**

## Option 2: Docker (Recommended)

1. **Run with Docker Compose**
   ```bash
   git clone <repository-url>
   cd appControlGastos
   docker-compose up --build
   ```

2. **Access the application**
   - Web UI: `http://localhost:5000`
   - API Docs: `http://localhost:5000/swagger`

## Default Login
- **Email**: admin@expensecontrol.com
- **Password**: admin123

## Features Available
- ✅ User registration and authentication
- ✅ Add, edit, delete transactions
- ✅ Categorize income and expenses
- ✅ Dashboard with KPIs and charts
- ✅ Generate PDF and Excel reports
- ✅ Responsive web interface
- ✅ RESTful API with Swagger documentation

## Next Steps
1. Register a new user account
2. Add some sample transactions
3. Explore the dashboard and reports
4. Check the API documentation at `/swagger`

## Troubleshooting
- If PostgreSQL connection fails, check your connection string in `appsettings.json`
- For Docker issues, ensure Docker Desktop is running
- Check logs in the console for detailed error messages

