using CarRentalSystem.Application.Features.Location.Queries.GetSuggestions;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CarRentalSystem.Web.Controllers
{
    [Route("api/location")]
    public class LocationController : ControllerBase
    {
        private readonly IMediator _mediator;

        public LocationController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("suggestions")]
        public async Task<IActionResult> GetSuggestions([FromQuery] string input, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(input))
                return BadRequest("Input cannot be empty.");

            var suggestions = await _mediator.Send(new GetLocationSuggestionsQuery(input), cancellationToken);
            return Ok(suggestions);
        }
    }
}
