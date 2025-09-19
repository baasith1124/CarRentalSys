namespace CarRentalSystem.Application.Common.Interfaces
{
    public interface ILocalizationService
    {
        string GetString(string key, params object[] args);
        string GetString(string key);
    }
}
