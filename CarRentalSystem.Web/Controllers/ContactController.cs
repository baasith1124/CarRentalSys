using AutoMapper;
using CarRentalSystem.Application.Features.ContactMessages.Commands.CreateContactMessage;
using CarRentalSystem.Web.ViewModels.ContactMessage;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CarRentalSystem.Web.Controllers
{
    public class ContactController :ControllerBase
    
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;

        public ContactController(IMediator mediator, IMapper mapper)
        {
            _mediator = mediator;
            _mapper = mapper;
        }

        [HttpPost]
        [Route("Contact/Send")]
        public async Task<IActionResult> Send([FromBody] ContactMessageViewModel viewModel)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { message = "Invalid input." });

            var command = _mapper.Map<CreateContactMessageCommand>(viewModel);

            var result = await _mediator.Send(command);

            if (result)
                return Ok(new { message = "Your message has been sent successfully!" });

            return BadRequest(new { message = "Something went wrong. Please try again." });
        }
    }

}

