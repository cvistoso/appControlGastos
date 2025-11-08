# Quick Start Guide - Expense Control App

## Prerequisites
- .NET 9.0 SDK
- PostgreSQL 15 (or Docker)

## Option 1: Local Development

1. **Install PostgreSQL 15**
   ```bash
   # macOS with Homebrew
   brew install postgresql@15
   brew services start postgresql@15
   
   # Create database
   createdb ExpenseControlDB
   ```

2. **Clone and run the application**
   ```bash
   git clone <repository-url>
   cd appControlGastos
   dotnet restore
   dotnet run
   ```

3. **Access the application**
   - Web UI: `https://localhost:5001`
   - API Docs: `https://localhost:5001/swagger`

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

