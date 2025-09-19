using CarRentalSystem.Application.Common.Interfaces;
using CarRentalSystem.Application.Contracts.AdminAssistant;
using CarRentalSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace CarRentalSystem.Application.Features.AdminAssistant
{
    public class AdminAssistantService : IAdminAssistantService
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly IBookingRepository _bookingRepository;
        private readonly ICarRepository _carRepository;
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly IOpenRouterService _openRouterService;
        private readonly IReportGenerationService _reportGenerationService;
        private readonly IChartGenerationService _chartGenerationService;
        private readonly IConfiguration _configuration;

        public AdminAssistantService(
            ICustomerRepository customerRepository,
            IBookingRepository bookingRepository,
            ICarRepository carRepository,
            IInvoiceRepository invoiceRepository,
            IOpenRouterService openRouterService,
            IReportGenerationService reportGenerationService,
            IChartGenerationService chartGenerationService,
            IConfiguration configuration)
        {
            _customerRepository = customerRepository;
            _bookingRepository = bookingRepository;
            _carRepository = carRepository;
            _invoiceRepository = invoiceRepository;
            _openRouterService = openRouterService;
            _reportGenerationService = reportGenerationService;
            _chartGenerationService = chartGenerationService;
            _configuration = configuration;
        }

        public async Task<AssistantResponse> ProcessQueryAsync(string query, CancellationToken cancellationToken = default)
        {
            try
            {
                // First, try to extract database context and intent from the query
                var databaseContext = await GetDatabaseContextAsync(cancellationToken);
                var intent = await AnalyzeQueryIntentAsync(query, databaseContext, cancellationToken);

                switch (intent.Type)
                {
                    case "count":
                        return await HandleCountQueryAsync(intent, cancellationToken);
                    case "list":
                        return await HandleListQueryAsync(intent, cancellationToken);
                    case "export":
                        return await HandleExportQueryAsync(intent, cancellationToken);
                    case "chart":
                        return await HandleChartQueryAsync(intent, cancellationToken);
                    case "summary":
                        return await HandleSummaryQueryAsync(intent, cancellationToken);
                    default:
                        return new AssistantResponse
                        {
                            Message = "I can only answer questions related to the database records.",
                            IsSuccess = false
                        };
                }
            }
            catch (Exception ex)
            {
                return new AssistantResponse
                {
                    Message = "I can only answer questions related to the database records.",
                    IsSuccess = false,
                    ErrorMessage = ex.Message
                };
            }
        }

        private async Task<DatabaseContext> GetDatabaseContextAsync(CancellationToken cancellationToken)
        {
            var customers = await _customerRepository.GetAllCustomersAsync(cancellationToken);
            var bookings = await _bookingRepository.GetAllBookingsAsync(cancellationToken);
            var cars = await _carRepository.GetAllCarsAsync(cancellationToken);
            var invoices = await _invoiceRepository.GetAllInvoicesAsync(cancellationToken);

            return new DatabaseContext
            {
                Customers = customers,
                Bookings = bookings,
                Cars = cars,
                Invoices = invoices
            };
        }

        private async Task<QueryIntent> AnalyzeQueryIntentAsync(string query, DatabaseContext context, CancellationToken cancellationToken)
        {
            var systemPrompt = $@"
You are an AI assistant that analyzes queries for a car rental system database.
You can ONLY work with these database tables:
- Customers (Id, FullName, Email, Address, IsKycVerified)
- Bookings (BookingId, CustomerId, CarId, PickupDate, ReturnDate, TotalCost, BookingStatus, PaymentStatus)
- Cars (CarId, Name, Model, RatePerDay, CarApprovalStatus)
- Invoices (InvoiceId, PaymentId, GeneratedAt)

Current database context:
- Total Customers: {context.Customers.Count}
- Total Bookings: {context.Bookings.Count}
- Total Cars: {context.Cars.Count}
- Total Invoices: {context.Invoices.Count}

Analyze the user query and return a JSON response with:
- type: 'count', 'list', 'export', 'chart', 'summary', or 'unrelated'
- entity: 'customers', 'bookings', 'cars', 'invoices'
- filters: any specific filters mentioned
- format: 'excel', 'pdf', 'word', 'chart' (if export requested)
- chartType: 'line', 'bar', 'pie', 'area' (if chart requested)

If the query is unrelated to the database, return type: 'unrelated'.

Query: {query}";

            var configuredModel = _configuration["OpenRouter:Model"] ?? "openai/gpt-4o";
            
            var openRouterRequest = new OpenRouterRequest
            {
                Query = systemPrompt,
                Model = configuredModel,
                Temperature = 0.1,
                MaxTokens = 500
            };

            var response = await _openRouterService.ProcessQueryAsync(openRouterRequest, cancellationToken);
            
            if (!response.IsSuccess)
            {
                // If OpenRouter fails, try to analyze the query locally
                return AnalyzeQueryLocally(query);
            }

            try
            {
                // Try to extract JSON from the response
                var jsonStart = response.Content.IndexOf('{');
                var jsonEnd = response.Content.LastIndexOf('}');
                
                if (jsonStart >= 0 && jsonEnd > jsonStart)
                {
                    var jsonContent = response.Content.Substring(jsonStart, jsonEnd - jsonStart + 1);
                    var intent = JsonSerializer.Deserialize<QueryIntent>(jsonContent);
                    return intent ?? AnalyzeQueryLocally(query);
                }
                else
                {
                    // If no JSON found, analyze locally
                    return AnalyzeQueryLocally(query);
                }
            }
            catch (Exception ex)
            {
                // If JSON parsing fails, analyze locally
                return AnalyzeQueryLocally(query);
            }
        }

        private async Task<AssistantResponse> HandleCountQueryAsync(QueryIntent intent, CancellationToken cancellationToken)
        {
            int count = 0;
            string entityName = "";

            switch (intent.Entity)
            {
                case "customers":
                    var customers = await _customerRepository.GetAllCustomersAsync(cancellationToken);
                    count = ApplyFilters(customers, intent.Filters).Count();
                    entityName = "customers";
                    break;
                case "bookings":
                    var bookings = await _bookingRepository.GetAllBookingsAsync(cancellationToken);
                    count = ApplyFilters(bookings, intent.Filters).Count();
                    entityName = "bookings";
                    break;
                case "cars":
                    var cars = await _carRepository.GetAllCarsAsync(cancellationToken);
                    count = ApplyFilters(cars, intent.Filters).Count();
                    entityName = "cars";
                    break;
                case "invoices":
                    var invoices = await _invoiceRepository.GetAllInvoicesAsync(cancellationToken);
                    count = ApplyFilters(invoices, intent.Filters).Count();
                    entityName = "invoices";
                    break;
            }

            return new AssistantResponse
            {
                Message = $"Total {entityName}: {count}",
                ResponseType = "text"
            };
        }

        private async Task<AssistantResponse> HandleListQueryAsync(QueryIntent intent, CancellationToken cancellationToken)
        {
            // For list queries, we'll provide a summary and suggest export
            var countResponse = await HandleCountQueryAsync(intent, cancellationToken);
            
            return new AssistantResponse
            {
                Message = $"{countResponse.Message}. Would you like to export this data to Excel, PDF, or Word?",
                ResponseType = "text"
            };
        }

        private async Task<AssistantResponse> HandleExportQueryAsync(QueryIntent intent, CancellationToken cancellationToken)
        {
            var reportRequest = new ReportRequest
            {
                ReportType = intent.Entity,
                AdditionalFilters = intent.Filters
            };

            byte[] fileData;
            string fileName;
            string fileType;

            switch (intent.Format)
            {
                case "excel":
                    fileData = await _reportGenerationService.GenerateExcelReportAsync(reportRequest, cancellationToken);
                    fileName = $"{intent.Entity}_report_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
                    fileType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    break;
                case "pdf":
                    fileData = await _reportGenerationService.GeneratePdfReportAsync(reportRequest, cancellationToken);
                    fileName = $"{intent.Entity}_report_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";
                    fileType = "application/pdf";
                    break;
                case "word":
                    fileData = await _reportGenerationService.GenerateWordReportAsync(reportRequest, cancellationToken);
                    fileName = $"{intent.Entity}_report_{DateTime.Now:yyyyMMdd_HHmmss}.docx";
                    fileType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
                    break;
                default:
                    return new AssistantResponse
                    {
                        Message = "I can only answer questions related to the database records.",
                        IsSuccess = false
                    };
            }

            return new AssistantResponse
            {
                Message = $"{intent.Format.ToUpper()} file generated: [Download Link]",
                ResponseType = "download",
                FileName = fileName,
                FileType = fileType,
                DownloadUrl = $"/AdminAssistant/DownloadReport?reportType={intent.Entity}&format={intent.Format}"
            };
        }

        private async Task<AssistantResponse> HandleChartQueryAsync(QueryIntent intent, CancellationToken cancellationToken)
        {
            var chartRequest = new ChartRequest
            {
                ChartType = intent.ChartType,
                DataType = intent.Entity,
                AdditionalFilters = intent.Filters
            };

            var chartData = await _chartGenerationService.GenerateChartAsync(chartRequest, cancellationToken);

            return new AssistantResponse
            {
                Message = $"{intent.ChartType.ToUpper()} chart generated: [Download Link]",
                ResponseType = "chart",
                FileName = $"{intent.Entity}_chart_{DateTime.Now:yyyyMMdd_HHmmss}.png",
                FileType = "image/png",
                DownloadUrl = $"/AdminAssistant/DownloadChart?dataType={intent.Entity}&chartType={intent.ChartType}"
            };
        }

        private async Task<AssistantResponse> HandleSummaryQueryAsync(QueryIntent intent, CancellationToken cancellationToken)
        {
            var context = await GetDatabaseContextAsync(CancellationToken.None);
            
            string summary = intent.Entity switch
            {
                "customers" => $"Total customers: {context.Customers.Count}, KYC verified: {context.Customers.Count(c => c.IsKycVerified)}",
                "bookings" => $"Total bookings: {context.Bookings.Count}, Total revenue: ${context.Bookings.Sum(b => b.TotalCost):F2}",
                "cars" => $"Total cars: {context.Cars.Count}, Average rate: ${context.Cars.Average(c => c.RatePerDay):F2}/day",
                "invoices" => $"Total invoices: {context.Invoices.Count}",
                _ => "I can only answer questions related to the database records."
            };

            return new AssistantResponse
            {
                Message = summary,
                ResponseType = "text"
            };
        }

        private IEnumerable<T> ApplyFilters<T>(IEnumerable<T> data, Dictionary<string, object>? filters)
        {
            if (filters == null || !filters.Any())
                return data;

            // Apply basic filters based on entity type
            // This is a simplified implementation - in a real scenario, you'd have more sophisticated filtering
            return data;
        }

        public async Task<byte[]> GenerateExcelReportAsync(ReportRequest request, CancellationToken cancellationToken = default)
        {
            return await _reportGenerationService.GenerateExcelReportAsync(request, cancellationToken);
        }

        public async Task<byte[]> GeneratePdfReportAsync(ReportRequest request, CancellationToken cancellationToken = default)
        {
            return await _reportGenerationService.GeneratePdfReportAsync(request, cancellationToken);
        }

        public async Task<byte[]> GenerateWordReportAsync(ReportRequest request, CancellationToken cancellationToken = default)
        {
            return await _reportGenerationService.GenerateWordReportAsync(request, cancellationToken);
        }

        public async Task<byte[]> GenerateChartAsync(ChartRequest request, CancellationToken cancellationToken = default)
        {
            return await _chartGenerationService.GenerateChartAsync(request, cancellationToken);
        }

        private QueryIntent AnalyzeQueryLocally(string query)
        {
            var lowerQuery = query.ToLower();
            
            // Check for customer-related queries
            if (lowerQuery.Contains("customer") || lowerQuery.Contains("customers"))
            {
                if (lowerQuery.Contains("how many") || lowerQuery.Contains("count") || lowerQuery.Contains("total"))
                {
                    return new QueryIntent { Type = "count", Entity = "customers" };
                }
                if (lowerQuery.Contains("export") || lowerQuery.Contains("excel") || lowerQuery.Contains("pdf"))
                {
                    var format = lowerQuery.Contains("excel") ? "excel" : 
                                lowerQuery.Contains("pdf") ? "pdf" : 
                                lowerQuery.Contains("word") ? "word" : "excel";
                    return new QueryIntent { Type = "export", Entity = "customers", Format = format };
                }
                return new QueryIntent { Type = "summary", Entity = "customers" };
            }
            
            // Check for booking-related queries
            if (lowerQuery.Contains("booking") || lowerQuery.Contains("bookings"))
            {
                if (lowerQuery.Contains("how many") || lowerQuery.Contains("count") || lowerQuery.Contains("total"))
                {
                    return new QueryIntent { Type = "count", Entity = "bookings" };
                }
                if (lowerQuery.Contains("export") || lowerQuery.Contains("excel") || lowerQuery.Contains("pdf"))
                {
                    var format = lowerQuery.Contains("excel") ? "excel" : 
                                lowerQuery.Contains("pdf") ? "pdf" : 
                                lowerQuery.Contains("word") ? "word" : "excel";
                    return new QueryIntent { Type = "export", Entity = "bookings", Format = format };
                }
                return new QueryIntent { Type = "summary", Entity = "bookings" };
            }
            
            // Check for car-related queries
            if (lowerQuery.Contains("car") || lowerQuery.Contains("cars"))
            {
                if (lowerQuery.Contains("how many") || lowerQuery.Contains("count") || lowerQuery.Contains("total"))
                {
                    return new QueryIntent { Type = "count", Entity = "cars" };
                }
                if (lowerQuery.Contains("export") || lowerQuery.Contains("excel") || lowerQuery.Contains("pdf"))
                {
                    var format = lowerQuery.Contains("excel") ? "excel" : 
                                lowerQuery.Contains("pdf") ? "pdf" : 
                                lowerQuery.Contains("word") ? "word" : "excel";
                    return new QueryIntent { Type = "export", Entity = "cars", Format = format };
                }
                return new QueryIntent { Type = "summary", Entity = "cars" };
            }
            
            // Check for invoice-related queries
            if (lowerQuery.Contains("invoice") || lowerQuery.Contains("invoices"))
            {
                if (lowerQuery.Contains("how many") || lowerQuery.Contains("count") || lowerQuery.Contains("total"))
                {
                    return new QueryIntent { Type = "count", Entity = "invoices" };
                }
                if (lowerQuery.Contains("export") || lowerQuery.Contains("excel") || lowerQuery.Contains("pdf"))
                {
                    var format = lowerQuery.Contains("excel") ? "excel" : 
                                lowerQuery.Contains("pdf") ? "pdf" : 
                                lowerQuery.Contains("word") ? "word" : "excel";
                    return new QueryIntent { Type = "export", Entity = "invoices", Format = format };
                }
                return new QueryIntent { Type = "summary", Entity = "invoices" };
            }
            
            // Check for revenue-related queries
            if (lowerQuery.Contains("revenue") || lowerQuery.Contains("money") || lowerQuery.Contains("income"))
            {
                return new QueryIntent { Type = "summary", Entity = "bookings" };
            }
            
            // Default to unrelated if no database entities are mentioned
            return new QueryIntent { Type = "unrelated" };
        }
    }

    public class DatabaseContext
    {
        public List<Customer> Customers { get; set; } = new();
        public List<Booking> Bookings { get; set; } = new();
        public List<Car> Cars { get; set; } = new();
        public List<Invoice> Invoices { get; set; } = new();
    }

    public class QueryIntent
    {
        public string Type { get; set; } = string.Empty;
        public string Entity { get; set; } = string.Empty;
        public Dictionary<string, object>? Filters { get; set; }
        public string? Format { get; set; }
        public string? ChartType { get; set; }
    }
}
