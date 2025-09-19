using CarRentalSystem.Application.Common.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRentalSystem.Application.Features.Account.Commands.ExternalLogin
{
    public class ExternalLoginCommandHandler : IRequestHandler<ExternalLoginCommand, bool>
    {
        private readonly IExternalAuthService _externalAuthService;

        public ExternalLoginCommandHandler(IExternalAuthService externalAuthService)
        {
            _externalAuthService = externalAuthService;
        }

        public async Task<bool> Handle(ExternalLoginCommand request, CancellationToken cancellationToken)
        {
            return await _externalAuthService.HandleExternalLoginAsync(request.User);
        }
    }
}
