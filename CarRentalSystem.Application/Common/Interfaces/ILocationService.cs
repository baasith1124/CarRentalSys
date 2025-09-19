using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRentalSystem.Application.Common.Interfaces
{
    public interface ILocationService
    {
        Task<string[]> GetPlaceSuggestionsAsync(string input, CancellationToken cancellationToken);
    }
}
