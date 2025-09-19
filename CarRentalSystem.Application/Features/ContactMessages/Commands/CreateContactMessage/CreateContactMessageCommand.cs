using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRentalSystem.Application.Features.ContactMessages.Commands.CreateContactMessage
{
    public class CreateContactMessageCommand : IRequest<bool>
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Message { get; set; }
    }

}
