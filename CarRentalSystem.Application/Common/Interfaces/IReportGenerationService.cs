using CarRentalSystem.Application.Contracts.AdminAssistant;

namespace CarRentalSystem.Application.Common.Interfaces
{
    public interface IReportGenerationService
    {
        Task<byte[]> GenerateExcelReportAsync(ReportRequest request, CancellationToken cancellationToken = default);
        Task<byte[]> GeneratePdfReportAsync(ReportRequest request, CancellationToken cancellationToken = default);
        Task<byte[]> GenerateWordReportAsync(ReportRequest request, CancellationToken cancellationToken = default);
    }
}
