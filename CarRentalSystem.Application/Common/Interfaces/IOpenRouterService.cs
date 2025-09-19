using CarRentalSystem.Application.Contracts.AdminAssistant;

namespace CarRentalSystem.Application.Common.Interfaces
{
    public interface IOpenRouterService
    {
        Task<OpenRouterResponse> ProcessQueryAsync(OpenRouterRequest request, CancellationToken cancellationToken = default);
    }
}
