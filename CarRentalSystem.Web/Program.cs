using CarRentalSystem.Application;
using CarRentalSystem.Application.Common.Interfaces;
using CarRentalSystem.Application.Common.Settings;
using CarRentalSystem.Application.Settings;
using CarRentalSystem.Infrastructure.DependencyInjection;
using CarRentalSystem.Infrastructure.Identity;
using CarRentalSystem.Infrastructure.Persistence;
using CarRentalSystem.Infrastructure.Persistence.Seeders;
using CarRentalSystem.Web.Middleware;
using CarRentalSystem.Web.Filters;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Serilog;
using Serilog.Events;
using System.Globalization;


namespace CarRentalSystem.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // Configure Serilog
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Information)
                .WriteTo.Console()
                .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day)
                .CreateLogger();

            try
            {
                var builder = WebApplication.CreateBuilder(args);

                // Add Serilog
                builder.Host.UseSerilog();

                // Add services to the container.
                builder.Services.AddControllersWithViews(options =>
                {
                    options.Filters.Add<ValidationExceptionFilter>();
                });
                builder.Services.AddRazorPages();

                // Add localization services
                builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");
                builder.Services.Configure<RequestLocalizationOptions>(options =>
                {
                    var supportedCultures = new[]
                    {
                        new CultureInfo("en-US"),
                        new CultureInfo("si-LK"), // Sinhala
                        new CultureInfo("ta-LK")  // Tamil
                    };

                    options.DefaultRequestCulture = new Microsoft.AspNetCore.Localization.RequestCulture("en-US");
                    options.SupportedCultures = supportedCultures;
                    options.SupportedUICultures = supportedCultures;

                    options.RequestCultureProviders.Clear();
                    options.RequestCultureProviders.Add(new Microsoft.AspNetCore.Localization.QueryStringRequestCultureProvider());
                    options.RequestCultureProviders.Add(new Microsoft.AspNetCore.Localization.CookieRequestCultureProvider());
                    options.RequestCultureProviders.Add(new Microsoft.AspNetCore.Localization.AcceptLanguageHeaderRequestCultureProvider());
                });
                builder.Services.AddMediatR(typeof(AssemblyReference).Assembly);//from application layer
                builder.Services.AddValidatorsFromAssembly(typeof(AssemblyReference).Assembly);//from application layer
                
                // Add Validation Behavior for automatic validation
                builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(CarRentalSystem.Application.Behaviors.ValidationBehavior<,>));
                
                // Register localization service
                builder.Services.AddScoped<CarRentalSystem.Application.Common.Interfaces.ILocalizationService, CarRentalSystem.Web.Services.LocalizationService>();
                builder.Services.AddAutoMapper(typeof(Program).Assembly, typeof(AssemblyReference).Assembly);//from application&Web layer


            //  google 
            builder.Services.AddInfrastructureServices();
            builder.Services.Configure<GoogleSettings>(
                builder.Configuration.GetSection("Google"));
            //Email Contact message 
            builder.Services.Configure<EmailSettings>(
                builder.Configuration.GetSection("EmailSettings"));
           
            //Stripe Configuration
            builder.Services.Configure<StripeSettings>(
                builder.Configuration.GetSection("Stripe"));

            // Admin Assistant Services
            builder.Services.AddScoped<CarRentalSystem.Application.Common.Interfaces.IAdminAssistantService, CarRentalSystem.Application.Features.AdminAssistant.AdminAssistantService>();
            builder.Services.AddScoped<CarRentalSystem.Application.Common.Interfaces.IOpenRouterService, CarRentalSystem.Application.Features.AdminAssistant.OpenRouterService>();
            builder.Services.AddScoped<CarRentalSystem.Application.Common.Interfaces.IReportGenerationService, CarRentalSystem.Application.Features.AdminAssistant.ReportGenerationService>();
            builder.Services.AddScoped<CarRentalSystem.Application.Common.Interfaces.IChartGenerationService, CarRentalSystem.Application.Features.AdminAssistant.ChartGenerationService>();
            builder.Services.AddHttpClient<CarRentalSystem.Application.Common.Interfaces.IOpenRouterService, CarRentalSystem.Application.Features.AdminAssistant.OpenRouterService>();

            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddIdentity<ApplicationUser, IdentityRole<Guid>>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredLength = 6;
                // Configure to use email as username
                options.User.RequireUniqueEmail = true;
            })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();
            //google auth 2.0
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultScheme = IdentityConstants.ApplicationScheme;
                options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
            })
            .AddGoogle("Google", options =>
            {
                options.ClientId = builder.Configuration["Google:ClientId"];
                options.ClientSecret = builder.Configuration["Google:ClientSecret"];
            });
            // Security Headers
            builder.Services.AddAntiforgery(options =>
            {
                options.HeaderName = "__RequestVerificationToken";
                options.SuppressXFrameOptionsHeader = false;
            });
            
            builder.Services.AddHsts(options =>
            {
                options.MaxAge = TimeSpan.FromDays(365);
                options.IncludeSubDomains = true;
                options.Preload = true;
            });
            
            //Enable session state
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                options.Cookie.SameSite = SameSiteMode.Strict;
            });

            



            var app = builder.Build();

            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    SeederRunner.RunAllSeedersAsync(services).GetAwaiter().GetResult();
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "An error occurred while seeding the database");
                }
            }

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseGlobalExceptionHandling();
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }
            else
            {
                app.UseDeveloperExceptionPage();
            }

            // Security middleware
            app.Use(async (context, next) =>
            {
                context.Response.Headers["X-Content-Type-Options"] = "nosniff";
                context.Response.Headers["X-Frame-Options"] = "DENY";
                context.Response.Headers["X-XSS-Protection"] = "1; mode=block";
                context.Response.Headers["Referrer-Policy"] = "strict-origin-when-cross-origin";
                await next();
            });

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseRequestLocalization();
            app.UseSession();
            app.UseAuthentication();
            app.UseAuthorization();
            //app.UseEndpoints();
            app.MapStaticAssets();
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}")
                .WithStaticAssets();

            app.Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Application terminated unexpectedly");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }
    }
}
