using CarRentalSystem.Application.Common.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRentalSystem.Application.Features.Location.Queries.GetSuggestions
{
    public class GetLocationSuggestionsQueryHandler : IRequestHandler<GetLocationSuggestionsQuery, string[]>
    {
        private readonly ILocationService _locationService;

        public GetLocationSuggestionsQueryHandler(ILocationService locationService)
        {
            _locationService = locationService;
        }

        public async Task<string[]> Handle(GetLocationSuggestionsQuery request, CancellationToken cancellationToken)
        {
            return await _locationService.GetPlaceSuggestionsAsync(request.Input, cancellationToken);
        }
    }

}
