# Car Rental System

A comprehensive web application built with ASP.NET Core MVC that facilitates car rental services between car owners and customers. The system provides a platform where car owners can list their vehicles for rent, and customers can search, book, and pay for rental services.

## 🚀 Features

### Core Functionality
- **Car Management**: Register, manage, and approve car listings with detailed specifications
- **Booking System**: Advanced search, real-time availability checking, and booking management
- **Payment Processing**: Secure Stripe integration for credit/debit card payments
- **KYC Verification**: Document upload and admin approval workflow for identity verification
- **Notification System**: Real-time notifications and email updates
- **Multi-language Support**: English, Sinhala, and Tamil language support
- **Intelligent Admin Assistant**: AI-powered database queries and report generation

### Advanced Features
- **3D Interactive Dashboard**: Modern UI with animated charts and visualizations
- **Location Services**: Google Maps integration for pickup/drop-off locations
- **PDF Generation**: Automatic invoice and receipt generation
- **Excel Reports**: Data export capabilities
- **Email Notifications**: Automated email system for important events
- **Security**: Role-based access control, secure authentication, and data protection

## 👥 User Roles and Permissions

### 🔧 Admin Role
**Full system access and management capabilities**
- Approve/reject car listings and KYC documents
- Manage all bookings and payments
- Access comprehensive analytics and reports
- User management and system configuration
- Access to Intelligent Admin Assistant for database queries
- Generate system documentation and reports
- Monitor system performance and security

### 👤 Customer Role
**Browse, book, and manage rental services**
- Browse and search available cars with advanced filters
- Create and manage bookings
- Upload KYC documents for verification
- Make secure payments through Stripe integration
- View booking history and download invoices
- Receive real-time notifications and email updates
- Access customer dashboard with spending analytics



## 🛠️ Technology Stack

### Backend Technologies
- **.NET 9.0** - Core framework
- **ASP.NET Core MVC** - Web framework
- **Entity Framework Core 9.0.7** - ORM
- **SQL Server** - Database
- **MediatR** - CQRS pattern implementation
- **FluentValidation** - Input validation
- **AutoMapper** - Object mapping

### Frontend Technologies
- **Bootstrap 5** - CSS framework
- **Chart.js** - Interactive charts and visualizations
- **jQuery** - JavaScript library
- **HTML5/CSS3** - Markup and styling
- **Responsive design** - Mobile compatibility

### External Services
- **Stripe** - Payment processing
- **Google Maps API** - Location services
- **Google OAuth 2.0** - Authentication
- **Gmail SMTP** - Email services
- **OpenRouter API** - AI-powered admin assistant

### Development Tools
- **Visual Studio 2022** - IDE
- **Git** - Version control
- **Serilog** - Logging framework
- **iTextSharp** - PDF generation
- **EPPlus** - Excel file generation

## 📋 Prerequisites

Before running the application, ensure you have the following installed:

- **.NET 9.0 SDK** - [Download here](https://dotnet.microsoft.com/download/dotnet/9.0)
- **SQL Server** - LocalDB (included with Visual Studio) or full SQL Server instance
- **Visual Studio 2022** or **VS Code** - For development
- **Git** - For version control

## 🚀 Quick Start Guide

### 1. Clone the Repository
```bash
git clone [repository-url]
cd CarRentalSystem_02
```

### 2. Restore Dependencies
```bash
dotnet restore
```

### 3. Create appsettings.json
**⚠️ IMPORTANT**: The `appsettings.json` file is not included in the repository for security reasons. You must create this file manually.

Create a new file named `appsettings.json` in the `CarRentalSystem.Web` project root with the following content:

**API KEYS AVAILABLE ON DOCUMENT  CarRental.Pdf**

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=CarRentalSystemDb;Trusted_Connection=True;MultipleActiveResultSets=true"
  },
  "Google": {
    "PlacesApiKey": "YOUR_GOOGLE_PLACES_API_KEY",
    "ClientId": "YOUR_GOOGLE_CLIENT_ID",
    "ClientSecret": "YOUR_GOOGLE_CLIENT_SECRET"
  },
  "GoogleMaps": {
    "ApiKey": "YOUR_GOOGLE_MAPS_API_KEY"
  },
  "EmailSettings": {
    "FromName": "Car Rental",
    "FromEmail": "your-email@gmail.com",
    "Password": "your-app-password",
    "SmtpServer": "smtp.gmail.com",
    "SmtpPort": 587,
    "SupportEmail": "support@yourcompany.com"
  },
  "Stripe": {
    "PublishableKey": "pk_test_YOUR_STRIPE_PUBLISHABLE_KEY",
    "SecretKey": "sk_test_YOUR_STRIPE_SECRET_KEY"
  },
  "OpenRouter": {
    "ApiKey": "YOUR_OPENROUTER_API_KEY",
    "Model": "openai/gpt-4o",
    "BaseUrl": "https://openrouter.ai/api/v1"
  }
}
```


### 4. Configure External Services

#### Database Connection
- **For LocalDB**: Use the provided connection string (default)
- **For SQL Server**: Replace with your server details
- **For Azure SQL**: Use Azure connection string format

#### Google Services Setup
1. Create a [Google Cloud Console](https://console.cloud.google.com/) project
2. Enable the following APIs:
   - Places API
   - Maps JavaScript API
   - Google+ API (for OAuth)
3. Generate API keys and OAuth credentials
4. Replace placeholder values in appsettings.json

#### Email Configuration
1. Use Gmail with App Password (recommended)
2. Enable 2-factor authentication on your Gmail account
3. Generate an App Password in Google Account settings
4. Update `FromEmail` and `Password` values in appsettings.json

#### Stripe Setup
1. Create a [Stripe](https://stripe.com/) account
2. Get API keys from Stripe Dashboard
3. Use test keys for development (`pk_test_` and `sk_test_`)
4. Use live keys for production (`pk_live_` and `sk_live_`)

#### OpenRouter AI (Optional)
1. Sign up at [OpenRouter](https://openrouter.ai/)
2. Get API key from dashboard
3. Choose AI model (gpt-4o recommended)
4. This is only needed for Admin Assistant feature

### 5. Database Setup
```bash
dotnet ef database update
```

### 6. Run the Application
```bash
dotnet run --project CarRentalSystem.Web
```

### 7. Access the Application
- Open browser to `https://localhost:5001`
- **Default admin credentials**:
  - Email: `admin@carrental.com`
  - Password: `Admin@123`

## 🔧 Configuration Details

### appsettings.json Structure

| Section | Description | Required |
|---------|-------------|----------|
| `Logging` | Logging configuration | Yes |
| `ConnectionStrings` | Database connection | Yes |
| `Google` | Google services (Places, OAuth) | Yes |
| `GoogleMaps` | Maps API configuration | Yes |
| `EmailSettings` | SMTP email configuration | Yes |
| `Stripe` | Payment processing | Yes |
| `OpenRouter` | AI assistant (optional) | yes |

### Environment-Specific Configuration

- **Development**: `appsettings.Development.json`
- **Production**: `appsettings.Production.json`
- **Environment Variables**: Highest priority (recommended for production)

### Security Best Practices

- ✅ Never commit `appsettings.json` with real API keys
- ✅ Use environment variables in production
- ✅ Implement Azure Key Vault or similar for secrets
- ✅ Regularly rotate API keys and passwords
- ✅ Use different keys for development and production

## 🏗️ Project Structure

```
CarRentalSystem_02/
├── CarRentalSystem.Domain/          # Core business entities
├── CarRentalSystem.Application/     # Business logic and use cases
├── CarRentalSystem.Infrastructure/  # Data access and external services
├── CarRentalSystem.Web/            # Web application and UI
└── CarRentalSystem.sln             # Solution file
```

### Architecture Layers

- **Domain Layer**: Core business entities and enums
- **Application Layer**: Use cases, CQRS with MediatR, validation
- **Infrastructure Layer**: Data access, external services, identity
- **Web Layer**: Controllers, views, static assets

## 🚀 Deployment

### Production Deployment Steps

1. **Build for Production**
   ```bash
   dotnet publish -c Release -o ./publish
   ```

2. **Database Migration**
   ```bash
   dotnet ef database update --environment Production
   ```

3. **Environment Variables** (Recommended)
   - Set sensitive configuration as environment variables
   - Use Azure Key Vault or similar for production secrets

4. **Web Server Configuration**
   - IIS: Configure application pool and site
   - Nginx: Set up reverse proxy
   - Apache: Configure mod_proxy

5. **SSL Certificate**
   - Install SSL certificate for HTTPS
   - Configure automatic HTTPS redirection

## 🔒 Security Features

- **Authentication & Authorization**: ASP.NET Core Identity with role-based access
- **Data Protection**: HTTPS enforcement, SQL injection prevention, XSS protection
- **Security Headers**: Comprehensive security headers and CSRF protection
- **Session Management**: Secure session cookies with timeout configuration
- **Input Validation**: FluentValidation for server-side validation
- **Payment Security**: Stripe PCI-compliant payment processing
- **Logging & Monitoring**: Comprehensive logging with Serilog

## 🆘 Troubleshooting

### Common Issues

#### Database Connection Issues
- Verify connection string format
- Check SQL Server service status
- Ensure database exists and is accessible
- Run migrations: `dotnet ef database update`

#### Authentication Problems
- Check user roles and permissions
- Verify password requirements
- Clear browser cookies and cache
- Check identity configuration

#### Payment Processing Errors
- Verify Stripe API keys
- Check webhook endpoints
- Validate payment intent creation
- Review Stripe dashboard for errors

#### Configuration Issues
- Verify `appsettings.json` exists and is valid JSON
- Check all required configuration sections
- Ensure API keys are correct and active
- Validate connection strings and credentials

### Support Resources
- Application logs in `logs/` directory
- Database logs and performance metrics
- External service dashboards
- .NET documentation and community forums

## 📚 Additional Documentation

For comprehensive documentation including:
- Detailed API endpoints
- Database schema
- Advanced configuration options
- Security implementation details
- Performance optimization guides

Generate the complete PDF documentation from the Admin Panel:
1. Login as admin
2. Go to Admin Dashboard
3. Click "Generate Docs" button
4. Download the complete PDF documentation

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Add tests if applicable
5. Submit a pull request

## 📄 License

This project is licensed under the MIT License - see the LICENSE file for details.

## 📞 Support

For technical support or questions:
- Check the application logs in the `logs/` directory
- Review the troubleshooting section above
- Generate comprehensive documentation from the Admin Panel
- Create an issue in the repository

---

**Note**: This README provides a quick start guide. For complete setup instructions, configuration details, and advanced features, generate the comprehensive PDF documentation from the Admin Panel after running the application.
