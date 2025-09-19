using CarRentalSystem.Application.Contracts.AdminAssistant;

namespace CarRentalSystem.Application.Common.Interfaces
{
    public interface IAdminAssistantService
    {
        Task<AssistantResponse> ProcessQueryAsync(string query, CancellationToken cancellationToken = default);
        Task<byte[]> GenerateExcelReportAsync(ReportRequest request, CancellationToken cancellationToken = default);
        Task<byte[]> GeneratePdfReportAsync(ReportRequest request, CancellationToken cancellationToken = default);
        Task<byte[]> GenerateWordReportAsync(ReportRequest request, CancellationToken cancellationToken = default);
        Task<byte[]> GenerateChartAsync(ChartRequest request, CancellationToken cancellationToken = default);
    }
}
