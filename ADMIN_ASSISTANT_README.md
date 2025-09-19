# Intelligent Admin Assistant for Car Rental System

## Overview

The Intelligent Admin Assistant is a powerful tool that allows administrators to query the Car Rental System database using natural language. It provides intelligent responses and can generate various reports and charts based on database queries.

## Features

### 🧠 Natural Language Processing
- Ask questions in plain English about the database
- Intelligent query interpretation using OpenRouter API (GPT-4o/GPT-5)
- Context-aware responses based on actual database content

### 📊 Database Query Capabilities
The assistant can only answer questions related to these database tables:
- **Customers**: CustomerId, FullName, Email, Address, KYC Status
- **Bookings**: BookingId, CustomerId, CarId, PickupDate, ReturnDate, TotalCost, Status
- **Cars**: CarId, Name, Model, RatePerDay, Year, Transmission, FuelType, ApprovalStatus
- **Invoices**: InvoiceId, PaymentId, GeneratedAt, FilePath

### 📈 Report Generation
- **Excel Reports**: Export data to .xlsx format
- **PDF Reports**: Generate PDF documents with data
- **Word Reports**: Create .docx documents
- **Charts**: Generate line, bar, pie, and area charts

### 🔒 Security
- Admin-only access (requires Admin role)
- Database-only responses (no general knowledge)
- Secure API key management

## Setup Instructions

### 1. OpenRouter API Configuration

1. Sign up for an OpenRouter account at [https://openrouter.ai](https://openrouter.ai)
2. Get your API key from the dashboard
3. Update `appsettings.json`:

```json
{
  "OpenRouter": {
    "ApiKey": "YOUR_OPENROUTER_API_KEY_HERE",
    "Model": "openai/gpt-4o",
    "BaseUrl": "https://openrouter.ai/api/v1"
  }
}
```

### 2. Install Required Packages

The following NuGet packages are required:
- `EPPlus` (for Excel generation)
- `System.Drawing.Common` (for chart generation)

These are already added to the project file.

### 3. Service Registration

The Admin Assistant services are automatically registered in `Program.cs`:

```csharp
// Admin Assistant Services
builder.Services.AddScoped<IAdminAssistantService, AdminAssistantService>();
builder.Services.AddScoped<IOpenRouterService, OpenRouterService>();
builder.Services.AddScoped<IReportGenerationService, ReportGenerationService>();
builder.Services.AddScoped<IChartGenerationService, ChartGenerationService>();
builder.Services.AddHttpClient<IOpenRouterService, OpenRouterService>();
```

### 4. Navigation

The Admin Assistant is accessible through the user dropdown menu for Admin users:
- Navigate to the user dropdown in the top-right corner
- Click "Admin Assistant" (robot icon)

## Usage Examples

### Text Queries
- "How many customers are KYC verified?"
- "What's the total revenue from bookings?"
- "How many cars are available?"
- "Show me all pending bookings"

### Export Requests
- "Export all customers to Excel"
- "Generate a PDF report of all bookings"
- "Create a Word document with car details"

### Chart Generation
- "Show me a line chart of bookings over time"
- "Generate a pie chart of customer KYC status"
- "Create a bar chart of revenue by month"

## API Endpoints

### Process Query
```
POST /AdminAssistant/ProcessQuery
Content-Type: application/json
Body: "Your natural language query"
```

### Generate Report
```
POST /AdminAssistant/GenerateReport
Content-Type: application/json
Body: {
  "reportType": "customers|bookings|cars|invoices",
  "format": "excel|pdf|word",
  "dateFrom": "optional",
  "dateTo": "optional",
  "status": "optional"
}
```

### Generate Chart
```
POST /AdminAssistant/GenerateChart
Content-Type: application/json
Body: {
  "dataType": "customers|bookings|revenue|cars",
  "chartType": "line|bar|pie|area",
  "dateFrom": "optional",
  "dateTo": "optional"
}
```

### Download Reports/Charts
```
GET /AdminAssistant/DownloadReport?reportType=customers&format=excel
GET /AdminAssistant/DownloadChart?dataType=bookings&chartType=line
```

## Architecture

### Core Components

1. **AdminAssistantService**: Main service that processes queries and coordinates responses
2. **OpenRouterService**: Handles communication with OpenRouter API
3. **ReportGenerationService**: Generates Excel, PDF, and Word reports
4. **ChartGenerationService**: Creates various chart types
5. **AdminAssistantController**: Web API controller for handling requests

### Query Processing Flow

1. User submits natural language query
2. System gathers current database context
3. Query is sent to OpenRouter API for intent analysis
4. Based on intent, appropriate action is taken:
   - Count queries → Database aggregation
   - List queries → Data retrieval with export suggestion
   - Export queries → Report generation
   - Chart queries → Chart generation
   - Summary queries → Statistical summaries

### Security Features

- **Role-based access**: Only Admin users can access
- **Database-only responses**: No general knowledge or external information
- **Input validation**: All queries are validated and sanitized
- **Error handling**: Graceful error handling with user-friendly messages

## Customization

### Adding New Query Types

1. Update the `QueryIntent` class in `AdminAssistantService.cs`
2. Add new case in the `ProcessQueryAsync` method
3. Implement the corresponding handler method

### Adding New Report Formats

1. Extend the `IReportGenerationService` interface
2. Implement the new format in `ReportGenerationService.cs`
3. Update the controller to handle the new format

### Adding New Chart Types

1. Extend the `IChartGenerationService` interface
2. Implement the new chart type in `ChartGenerationService.cs`
3. Update the frontend to support the new chart type

## Troubleshooting

### Common Issues

1. **OpenRouter API Errors**
   - Verify API key is correct
   - Check API quota and billing
   - Ensure model name is valid

2. **Report Generation Failures**
   - Check file permissions
   - Verify EPPlus license context
   - Ensure sufficient memory for large datasets

3. **Chart Generation Issues**
   - Verify System.Drawing.Common is installed
   - Check for null data scenarios
   - Ensure proper image format support

### Logging

All errors are logged using the application's logging system. Check the logs for detailed error information.

## Future Enhancements

- Support for more complex queries with joins
- Real-time data updates
- Custom dashboard creation
- Advanced filtering options
- Scheduled report generation
- Email report delivery
- Multi-language support

## Support

For issues or questions regarding the Admin Assistant, please check the application logs and ensure all configuration is correct. The system is designed to be robust and provide helpful error messages when issues occur.
