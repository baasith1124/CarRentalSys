using CarRentalSystem.Application.Common.Interfaces;
using CarRentalSystem.Infrastructure.External.Services;
using CarRentalSystem.Infrastructure.Identity;
using CarRentalSystem.Infrastructure.Persistence.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Stripe;
using System.Net.Http;

namespace CarRentalSystem.Infrastructure.DependencyInjection
{
    public static class InfrastructureServiceRegistration
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
        {
            // Register all repositories
            services.AddScoped<IBookingRepository, EfBookingRepository>();
            services.AddScoped<IPaymentRepository, EfPaymentRepository>();
            services.AddScoped<IInvoiceRepository, EfInvoiceRepository>();
            services.AddScoped<IKYCRepository, EfKYCRepository>();
            services.AddScoped<INotificationRepository, EfNotificationRepository>();
            services.AddScoped<ICustomerRepository, EfCustomerRepository>();
            services.AddScoped<ICarRepository, EfCarRepository>();
            services.AddScoped<ICarApprovalStatusRepository, EfCarApprovalStatusRepository>();
            services.AddScoped<ICarDocumentRepository, EfCarDocumentRepository>();

            services.AddHttpClient<ILocationService, GoogleLocationService>();

            services.AddScoped<IContactMessageRepository, EfContactMessageRepository>();
            services.AddScoped<IEmailService, MailKitEmailService>();
            services.AddScoped<IIdentityService, EfIdentityService>();
            services.AddScoped<IExternalAuthService, ExternalAuthService>();




            //// Register DbContext
            //services.AddDbContext<ApplicationDbContext>();

            return services;
        }
    }
}
