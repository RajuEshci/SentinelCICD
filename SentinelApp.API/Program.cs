using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using Serilog.Events;
using System.Text;
using SentinelApp.API.Filters;
using SentinelApp.API.Middleware;
using SentinelApp.Application.Core;
using SentinelApp.Infrastructure;
using SentinelApp.Infrastructure.Database;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .Enrich.WithMachineName()
    .Enrich.WithThreadId()
    .Enrich.WithProperty("Application", "SentinelApp.API")
    .Enrich.WithProperty("Environment", builder.Environment.EnvironmentName)
    .CreateLogger();

builder.Host.UseSerilog();

// Remove server headers
builder.WebHost.ConfigureKestrel(options => options.AddServerHeader = false);

// Security services
builder.Services.AddAntiforgery();
builder.Services.AddHsts(options =>
{
    options.Preload = true;
    options.IncludeSubDomains = true;
    options.MaxAge = TimeSpan.FromDays(365);
});

builder.Services.AddHttpsRedirection(options =>
{
    options.HttpsPort = 443;
});

builder.Services.AddControllers(options =>
{
    options.Filters.Add<AutoValidationFilter>();
    options.Filters.Add<EncryptionFilter>();
});

builder.Services.AddScoped<Email>();
builder.Services.AddScoped<AutoValidationFilter>();
builder.Services.AddEndpointsApiExplorer();

// Swagger configuration
builder.Services.AddSwaggerGen(s =>
{
    s.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    s.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });

    s.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "SentinelApp API",
        Version = "v1",
        Description = "API for Sentinel App"
    });
});

builder.Services.AddInfrastructure(builder.Configuration);

var corsConfig = builder.Configuration.GetSection("Cors:SecurePolicy");

// CORS Configuration
builder.Services.AddCors(options =>
{
    options.AddPolicy("SecurePolicy", policy =>
    {
        policy.WithOrigins(corsConfig["Origins"]?.Split(',', StringSplitOptions.RemoveEmptyEntries)
                          ?? corsConfig.GetSection("Origins").Get<string[]>())
              .WithMethods(corsConfig.GetSection("Methods").Get<string[]>()
                          ?? new[] { "GET", "POST", "PUT", "DELETE" })
              .WithHeaders(corsConfig.GetSection("Headers").Get<string[]>()
                          ?? new[] { "Authorization", "Content-Type", "Accept" })
              .AllowCredentials();
    });
});

//builder.Services.AddCors(options =>
//{
//    options.AddPolicy("SecurePolicy", policy =>
//    {
//        policy.WithOrigins("https://SentinelAppweb.eshci.com",
//                          "https://SentinelAppwebib.eshci.com",
//                          "https://localhost:4200",
//                          "https://localhost:4300",
//                          "https://localhost:4400",
//                          "http://localhost:4200",
//                          "http://localhost:4300",
//                          "http://localhost:4400")
//              .WithMethods("GET", "POST", "PUT", "DELETE")
//              .WithHeaders("Authorization", "Content-Type", "Accept")
//              .AllowCredentials();
//    });
//});

// JWT Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)),
            ClockSkew = TimeSpan.Zero
        };

        options.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = context =>
            {
                Log.Warning("Authentication failed: {Error}", context.Exception.Message);
                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.DefaultPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
});

builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.SuppressModelStateInvalidFilter = true;
});
builder.Services.AddHttpContextAccessor();

var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
    var config =
        scope.ServiceProvider.GetRequiredService<IConfiguration>();

    var connectionString =
        config.GetConnectionString("DefaultConnection");

    var dryRun =
        config.GetValue<bool>("DatabaseMigration:DryRun");

    if (dryRun)
    {
        DatabaseMigration.Run(connectionString, true);
        return;
    }

    if (config.GetValue<bool>("DatabaseBackup:Enabled"))
    {
        DatabaseBackupService.BackupDatabase(
            connectionString,
            config.GetValue<string>("DatabaseBackup:Path"));
    }

    DatabaseMigration.Run(connectionString, false);
}
// SECURITY HEADERS MIDDLEWARE - MUST BE FIRST
app.Use(async (context, next) =>
{
    // Remove server identification headers
    context.Response.Headers.Remove("Server");
    context.Response.Headers.Remove("X-Powered-By");
    context.Response.Headers.Remove("X-AspNet-Version");
    context.Response.Headers.Remove("X-AspNetMvc-Version");

    // A+ Grade Security Headers
    context.Response.Headers["Strict-Transport-Security"] = "max-age=31536000; includeSubDomains; preload";
    context.Response.Headers["X-Frame-Options"] = "DENY";
    context.Response.Headers["X-Content-Type-Options"] = "nosniff";
    context.Response.Headers["X-XSS-Protection"] = "1; mode=block";
    context.Response.Headers["Referrer-Policy"] = "strict-origin-when-cross-origin";
    context.Response.Headers["Permissions-Policy"] = "geolocation=(), microphone=(), camera=(), payment=()";

    // Content Security Policy - Adjust based on your actual dependencies
    context.Response.Headers["Content-Security-Policy"] =
        "default-src 'self'; " +
        "script-src 'self' 'unsafe-inline' 'unsafe-eval'; " +
        "style-src 'self' 'unsafe-inline'; " +
        "img-src 'self' data: https:; " +
        "connect-src 'self'; " +
        "font-src 'self'; " +
        "object-src 'none'; " +
        "frame-ancestors 'none'; " +
        "base-uri 'self'; " +
        "form-action 'self';";

    // Modern security headers
    context.Response.Headers["Cross-Origin-Embedder-Policy"] = "require-corp";
    context.Response.Headers["Cross-Origin-Opener-Policy"] = "same-origin";
    context.Response.Headers["Cross-Origin-Resource-Policy"] = "same-origin";

    // API cache control
    if (context.Request.Path.StartsWithSegments("/api"))
    {
        context.Response.Headers["Cache-Control"] = "no-store, no-cache, must-revalidate";
        context.Response.Headers["Pragma"] = "no-cache";
        context.Response.Headers["Expires"] = "0";
    }

    await next();
});

// Pipeline configuration
if (!app.Environment.IsDevelopment())
{
}
    app.UseHsts();

//app.UseHttpsRedirection();
app.UseCors("SecurePolicy");

// Custom middlewares
app.UseMiddleware<ModelBindingMiddleware>();
app.UseMiddleware<ValidationMiddleware>();
app.UseMiddleware<AuthenticationResponseMiddleware>();

app.UseSerilogRequestLogging(options =>
{
    options.GetLevel = (httpContext, elapsed, ex) =>
    {
        if (ex != null || httpContext.Response.StatusCode > 499)
            return LogEventLevel.Error;
        if (elapsed > 1000)
            return LogEventLevel.Warning;
        return LogEventLevel.Information;
    };

    options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
    {
        diagnosticContext.Set("RequestHost", httpContext.Request.Host.Value);
        diagnosticContext.Set("RequestScheme", httpContext.Request.Scheme);
        diagnosticContext.Set("UserAgent", httpContext.Request.Headers.UserAgent);
        diagnosticContext.Set("RemoteIpAddress", httpContext.Connection.RemoteIpAddress);
    };
});

app.UseRouting();
// Swagger configuration
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "SentinelApp API V1");
    c.RoutePrefix = string.Empty;
    // Additional Swagger security
    c.ConfigObject.AdditionalItems["persistAuthorization"] = true;
});

app.UseMiddleware<RequestDecryptMiddleware>();
app.UseMiddleware<RequestResponseLoggingMiddleware>();
app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<ResponseEncryptMiddleware>();


app.MapControllers().RequireAuthorization();

try
{
    Log.Information("Starting SentinelApp API");
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