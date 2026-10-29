using Api.OpenApi;
using Api.Security;
using ApiBase.Controller.Extensions;
using Application;
using Application.Abstractions;
using Application.Exceptions;
using Domain.Exceptions;
using Infra;
using Infra.Persistence;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

// ---------- Services ----------

builder.Services.AddControllers()
    .AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

builder.Services.AddApplication();
builder.Services.AddInfra(builder.Configuration);

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUser, HttpContextCurrentUser>();

// Errors → ProblemDetails: domain and business rules 400, NotFoundException 404, anything else 500 without details.
builder.Services.AddApiBaseExceptionHandler(options =>
{
    options.Map<DomainException>(StatusCodes.Status400BadRequest);
    options.Map<BusinessRuleException>(StatusCodes.Status400BadRequest);
});

// Authentication (JWT) — fail at startup instead of answering 500 on every request.
if (string.IsNullOrWhiteSpace(builder.Configuration["Jwt:Secret"]))
{
    throw new InvalidOperationException("Jwt:Secret is not configured. Use 'dotnet user-secrets set Jwt:Secret <value>' or the Jwt__Secret environment variable.");
}

builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("Jwt"));
builder.Services.AddSingleton<ITokenService, JwtTokenService>();
builder.Services.AddApiBaseJwt(builder.Configuration);
builder.Services.PostConfigure<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme, options =>
{
    // Keep the short claim names of the token ("role", "sub") instead of the long WS-* URIs.
    options.MapInboundClaims = false;
    options.TokenValidationParameters.RoleClaimType = TokenClaims.Role;
    options.TokenValidationParameters.NameClaimType = TokenClaims.Subject;
});

// Every endpoint requires a valid token unless it says [AllowAnonymous] (deny by default).
builder.Services.AddAuthorizationBuilder()
    .SetFallbackPolicy(new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build());

// Brute force protection on the login: N attempts per minute per client IP.
int loginPermitLimit = builder.Configuration.GetValue("RateLimiting:LoginPermitLimit", 5);
builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    options.AddPolicy(RateLimitPolicies.Login, httpContext => RateLimitPartition.GetFixedWindowLimiter(
        httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
        _ => new FixedWindowRateLimiterOptions { PermitLimit = loginPermitLimit, Window = TimeSpan.FromMinutes(1) }));
});

builder.Services.AddOpenApi(options => options.AddDocumentTransformer<BearerSecurityTransformer>());

// ---------- Pipeline ----------

var app = builder.Build();

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi().AllowAnonymous();
    app.UseSwaggerUI(options => options.SwaggerEndpoint("/openapi/v1.json", "Sales Commission API v1"));
}

app.UseAuthentication();
app.UseAuthorization();
app.UseRateLimiter();

app.MapControllers();

if (app.Environment.IsDevelopment())
{
    // Development only. In production the schema is changed by the pipeline (migration bundle / script),
    // not by each API instance at startup — several instances would race, and the app user would need DDL rights.
    using var scope = app.Services.CreateScope();
    await scope.ServiceProvider.GetRequiredService<PortalDbContext>().Database.MigrateAsync();
    await scope.ServiceProvider.GetRequiredService<DevelopmentDataSeeder>().SeedAsync(app.Configuration);
}

app.Run();

/// <summary>Visible to WebApplicationFactory in the integration tests.</summary>
public partial class Program { }
