using CarRentalSystem.Application.Common.Interfaces;
using CarRentalSystem.Application.Contracts.AdminAssistant;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarRentalSystem.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminAssistantController : Controller
    {
        private readonly IAdminAssistantService _adminAssistantService;
        private readonly ILogger<AdminAssistantController> _logger;

        public AdminAssistantController(
            IAdminAssistantService adminAssistantService,
            ILogger<AdminAssistantController> logger)
        {
            _adminAssistantService = adminAssistantService;
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ProcessQuery([FromBody] string query)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(query))
                {
                    return Json(new AssistantResponse
                    {
                        Message = "Please provide a valid query.",
                        IsSuccess = false
                    });
                }

                var response = await _adminAssistantService.ProcessQueryAsync(query);
                return Json(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing admin assistant query: {Query}", query);
                return Json(new AssistantResponse
                {
                    Message = "I can only answer questions related to the database records.",
                    IsSuccess = false,
                    ErrorMessage = ex.Message
                });
            }
        }

        [HttpPost]
        public async Task<IActionResult> GenerateReport([FromBody] ReportRequest request)
        {
            try
            {
                byte[] fileData;
                string fileName;
                string contentType;

                switch (request.ReportType.ToLower())
                {
                    case "excel":
                        fileData = await _adminAssistantService.GenerateExcelReportAsync(request);
                        fileName = $"{request.ReportType}_report_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
                        contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                        break;
                    case "pdf":
                        fileData = await _adminAssistantService.GeneratePdfReportAsync(request);
                        fileName = $"{request.ReportType}_report_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";
                        contentType = "application/pdf";
                        break;
                    case "word":
                        fileData = await _adminAssistantService.GenerateWordReportAsync(request);
                        fileName = $"{request.ReportType}_report_{DateTime.Now:yyyyMMdd_HHmmss}.docx";
                        contentType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
                        break;
                    default:
                        return Json(new AssistantResponse
                        {
                            Message = "Unsupported report format.",
                            IsSuccess = false
                        });
                }

                _logger.LogInformation("File generated successfully: {FileName}, Size: {Size} bytes", fileName, fileData.Length);
                return File(fileData, contentType, fileName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating report: {ReportType}", request.ReportType);
                return Json(new AssistantResponse
                {
                    Message = "Error generating report.",
                    IsSuccess = false,
                    ErrorMessage = ex.Message
                });
            }
        }

        [HttpPost]
        public async Task<IActionResult> GenerateChart([FromBody] ChartRequest request)
        {
            try
            {
                var chartData = await _adminAssistantService.GenerateChartAsync(request);
                var fileName = $"{request.DataType}_chart_{DateTime.Now:yyyyMMdd_HHmmss}.png";
                
                return File(chartData, "image/png", fileName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating chart: {DataType}", request.DataType);
                return Json(new AssistantResponse
                {
                    Message = "Error generating chart.",
                    IsSuccess = false,
                    ErrorMessage = ex.Message
                });
            }
        }

        [HttpGet]
        public async Task<IActionResult> DownloadReport(string reportType, string format)
        {
            try
            {
                _logger.LogInformation("DownloadReport requested: ReportType={ReportType}, Format={Format}", reportType, format);
                
                var request = new ReportRequest
                {
                    ReportType = reportType,
                    DateFrom = Request.Query["dateFrom"].FirstOrDefault(),
                    DateTo = Request.Query["dateTo"].FirstOrDefault(),
                    Status = Request.Query["status"].FirstOrDefault()
                };

                byte[] fileData;
                string fileName;
                string contentType;

                switch (format.ToLower())
                {
                    case "excel":
                        fileData = await _adminAssistantService.GenerateExcelReportAsync(request);
                        fileName = $"{reportType}_report_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
                        contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                        break;
                    case "pdf":
                        fileData = await _adminAssistantService.GeneratePdfReportAsync(request);
                        fileName = $"{reportType}_report_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";
                        contentType = "application/pdf";
                        break;
                    case "word":
                        fileData = await _adminAssistantService.GenerateWordReportAsync(request);
                        fileName = $"{reportType}_report_{DateTime.Now:yyyyMMdd_HHmmss}.docx";
                        contentType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
                        break;
                    default:
                        return BadRequest("Unsupported format");
                }

                _logger.LogInformation("Download file generated successfully: {FileName}, Size: {Size} bytes", fileName, fileData.Length);
                return File(fileData, contentType, fileName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error downloading report: {ReportType} - {Format}", reportType, format);
                return BadRequest("Error generating report");
            }
        }

        [HttpGet]
        public async Task<IActionResult> DownloadChart(string dataType, string chartType)
        {
            try
            {
                var request = new ChartRequest
                {
                    DataType = dataType,
                    ChartType = chartType,
                    DateFrom = Request.Query["dateFrom"].FirstOrDefault(),
                    DateTo = Request.Query["dateTo"].FirstOrDefault(),
                    GroupBy = Request.Query["groupBy"].FirstOrDefault()
                };

                var chartData = await _adminAssistantService.GenerateChartAsync(request);
                var fileName = $"{dataType}_chart_{DateTime.Now:yyyyMMdd_HHmmss}.png";
                
                return File(chartData, "image/png", fileName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error downloading chart: {DataType} - {ChartType}", dataType, chartType);
                return BadRequest("Error generating chart");
            }
        }
    }
}
