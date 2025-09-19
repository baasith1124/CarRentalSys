using CarRentalSystem.Application.Contracts.AdminAssistant;

namespace CarRentalSystem.Application.Common.Interfaces
{
    public interface IChartGenerationService
    {
        Task<byte[]> GenerateChartAsync(ChartRequest request, CancellationToken cancellationToken = default);
    }
}
