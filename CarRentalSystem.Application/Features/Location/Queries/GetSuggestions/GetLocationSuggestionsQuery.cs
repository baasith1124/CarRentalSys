using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRentalSystem.Application.Features.Location.Queries.GetSuggestions
{
    public record GetLocationSuggestionsQuery(string Input) : IRequest<string[]>;
    
}
