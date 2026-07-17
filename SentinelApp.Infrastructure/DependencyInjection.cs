using DinkToPdf;
using DinkToPdf.Contracts;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SentinelApp.Application.Interfaces;
using SentinelApp.Application.Services;
using SentinelApp.Application.Validators;
using SentinelApp.Domain.Interfaces;
using SentinelApp.Persistence.Data;
using SentinelApp.Persistence.Repositories;



namespace SentinelApp.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Database Context
        services.AddScoped<IDbContext, DbContext>();       
        services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        services.AddHttpContextAccessor();
        services.AddSingleton(typeof(IConverter), new SynchronizedConverter(new PdfTools()));
        
        services.AddSingleton<IEncryptionService, EncryptionService>();
        
        
        // Repositories
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<ICheckDuplicateRepository, CheckDuplicateRepository>();
        services.AddScoped<IConsent, ConsentRepository>();
        services.AddScoped<IInformativePages, InformativePagesRepository>();
        services.AddScoped<IMedicalHistory, MedicalHistoryRepository>();
        services.AddScoped<IVersionRepository, VersionRepository>();

        // Services
        services.AddScoped<ICurrentUserService, CurrentUserService>();
        services.AddScoped<IVersionService, VersionService>();
		services.AddScoped<IConsentService, ConsentService>();
		services.AddScoped<IAuthService, AuthService>();
		services.AddScoped<IInformativeService, InformativePagesService>();
		services.AddScoped<IMedicalHistoryService, MedicalHistoryService>();
		services.AddScoped<IActionService, ActionService>();
		services.AddScoped<IPdfService, PdfService>();

        services.AddValidatorsFromAssemblyContaining<RegisterUserValidator>();
        services.AddValidatorsFromAssemblyContaining<UpdateUserValidator>();
        return services;
    }
}