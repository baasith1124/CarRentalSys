using CarRentalSystem.Application.Common.Interfaces;
using CarRentalSystem.Application.Helpers.EmailTemplates;
using CarRentalSystem.Application.Settings;
using CarRentalSystem.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRentalSystem.Application.Features.ContactMessages.Commands.CreateContactMessage
{
    public class CreateContactMessageHandler : IRequestHandler<CreateContactMessageCommand, bool>
    {
        private readonly IContactMessageRepository _repository;
        private readonly IEmailService _emailService;
        private readonly EmailSettings _settings;

        public CreateContactMessageHandler(
            IContactMessageRepository repository,
            IEmailService emailService,
            IOptions<EmailSettings> options)
        {
            _repository = repository;
            _emailService = emailService;
            _settings = options.Value;
        }

        public async Task<bool> Handle(CreateContactMessageCommand request, CancellationToken cancellationToken)
        {
            var entity = new ContactMessage
            {
                Name = request.Name,
                Email = request.Email,
                Message = request.Message
            };

            await _repository.SaveMessageAsync(entity);

            var htmlBody = ContactEmailTemplateHelper.GenerateHtml(request.Name, request.Email, request.Message);

            await _emailService.SendEmailAsync(
                _settings.SupportEmail,
                $"New Contact Message from {request.Name}",
                htmlBody
            );

            return true;
        }
    }

}
