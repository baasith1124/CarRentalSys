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
        services.AddHttpClient<IGoogleMapsService, GoogleMapsService>();
        services.AddScoped<IPdfService, PdfService>();
        services.AddScoped<IBookingTimeoutService, BookingTimeoutService>();
        services.AddHostedService<BookingTimeoutService>();

            services.AddScoped<IContactMessageRepository, EfContactMessageRepository>();
            services.AddScoped<IEmailService, MailKitEmailService>();
            services.AddScoped<IIdentityService, EfIdentityService>();
            services.AddScoped<IExternalAuthService, ExternalAuthService>();
            services.AddScoped<IOTPRepository, OTPRepository>();
            services.AddScoped<IOTPService, CarRentalSystem.Application.Features.OTP.Services.OTPService>();




            //// Register DbContext
            //services.AddDbContext<ApplicationDbContext>();

            return services;
        }
    }
}
