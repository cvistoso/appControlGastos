# Expense Control App

A comprehensive personal expense management system built with ASP.NET Core 9.0 and modern web technologies.

## Features

### Core Functionality
- **User Management**: Registration, authentication, and role-based access control
- **Transaction Management**: Add, edit, delete, and categorize income and expenses
- **Category Management**: Create and manage custom categories for transactions
- **Dashboard**: Real-time KPIs and financial overview
- **Reports**: Generate PDF and Excel reports with filtering options
- **Data Visualization**: Interactive charts and graphs for financial trends

### Technical Features
- **JWT Authentication**: Secure token-based authentication
- **RESTful API**: Clean, well-documented API endpoints
- **Responsive Design**: Mobile-friendly Bootstrap 5.3.0 interface
- **Real-time Updates**: Dynamic dashboard with live data
- **Export Capabilities**: PDF and Excel report generation
- **Docker Support**: Containerized deployment ready

## Technology Stack

### Backend
- **ASP.NET Core 9.0** with C# 12
- **PostgreSQL 15** database
- **Entity Framework Core 9.0.8** ORM
- **JWT Bearer Token** authentication
- **Swagger/OpenAPI** documentation
- **BCrypt** password hashing

### Frontend
- **Bootstrap 5.3.0** UI framework
- **Font Awesome 6.4.0** icons
- **Chart.js** for data visualization
- **Vanilla JavaScript ES6+**
- **Single Page Application (SPA)** architecture

### Infrastructure
- **Docker** containerization
- **Docker Compose** for multi-container setup
- **Entity Framework Migrations** for database management

## Getting Started

### Prerequisites
- .NET 9.0 SDK
- PostgreSQL 15
- Docker (optional)

### Local Development

1. **Clone the repository**
   ```bash
   git clone <repository-url>
   cd appControlGastos
   ```

2. **Configure the database**
   - Install PostgreSQL 15
   - Create a database named `ExpenseControlDB`
   - Update the connection string in `appsettings.json`

3. **Restore dependencies**
   ```bash
   dotnet restore
   ```

4. **Run the application**
   ```bash
   dotnet run
   ```

5. **Access the application**
   - Web UI: `https://localhost:5001`
   - API Documentation: `https://localhost:5001/swagger`

### Docker Deployment

1. **Build and run with Docker Compose**
   ```bash
   docker-compose up --build
   ```

2. **Access the application**
   - Web UI: `http://localhost:5000`
   - API Documentation: `http://localhost:5000/swagger`

## API Documentation

### Authentication Endpoints
- `POST /api/auth/login` - User login
- `POST /api/auth/register` - User registration
- `GET /api/auth/me` - Get current user profile

### Transaction Endpoints
- `GET /api/transactions` - Get transactions with filtering
- `GET /api/transactions/{id}` - Get specific transaction
- `POST /api/transactions` - Create new transaction
- `PUT /api/transactions/{id}` - Update transaction
- `DELETE /api/transactions/{id}` - Delete transaction
- `GET /api/transactions/category-summary` - Get category summary

### Dashboard Endpoints
- `GET /api/dashboard` - Get dashboard data
- `GET /api/dashboard/monthly-trends` - Get monthly trends
- `GET /api/dashboard/projections` - Get cash flow projections

### Category Endpoints
- `GET /api/categories` - Get categories
- `GET /api/categories/{id}` - Get specific category
- `POST /api/categories` - Create category (Admin only)
- `PUT /api/categories/{id}` - Update category (Admin only)
- `DELETE /api/categories/{id}` - Delete category (Admin only)

### Report Endpoints
- `GET /api/reports/pdf` - Generate PDF report
- `GET /api/reports/excel` - Generate Excel report

## Default Credentials

The application comes with a default admin user:
- **Email**: admin@expensecontrol.com
- **Password**: admin123

## Configuration

### Environment Variables
- `ConnectionStrings__DefaultConnection` - PostgreSQL connection string
- `JwtSettings__SecretKey` - JWT secret key
- `JwtSettings__Issuer` - JWT issuer
- `JwtSettings__Audience` - JWT audience
- `JwtSettings__ExpiryInMinutes` - Token expiry time

### Database Configuration
The application uses Entity Framework Core with PostgreSQL. The database will be automatically created on first run with seed data including default categories and admin user.

## Security Features

- **JWT Authentication**: Secure token-based authentication
- **Password Hashing**: BCrypt for secure password storage
- **CORS Configuration**: Properly configured for development and production
- **Input Validation**: Data annotations and model validation
- **SQL Injection Prevention**: Entity Framework Core parameterized queries
- **XSS Protection**: Proper output encoding and validation

## Development

### Project Structure
```
appControlGastos/
├── Controllers/          # API Controllers
├── Data/                # Entity Framework DbContext
├── Models/              # Data models and DTOs
├── Services/            # Business logic services
├── wwwroot/             # Static web files
│   ├── css/            # Custom styles
│   ├── js/             # JavaScript files
│   └── index.html      # Main HTML file
├── Program.cs           # Application entry point
├── appsettings.json     # Configuration
└── Dockerfile          # Docker configuration
```

### Adding New Features
1. Create models in the `Models` folder
2. Add services in the `Services` folder
3. Create controllers in the `Controllers` folder
4. Update the frontend JavaScript as needed

## Contributing

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Add tests if applicable
5. Submit a pull request

## License

This project is licensed under the MIT License - see the LICENSE file for details.

## Support

For support and questions, please open an issue in the repository or contact the development team.

## Changelog

### Version 1.0.0
- Initial release
- User authentication and management
- Transaction management
- Category management
- Dashboard with KPIs and charts
- PDF and Excel report generation
- Docker support
- Responsive web interface

