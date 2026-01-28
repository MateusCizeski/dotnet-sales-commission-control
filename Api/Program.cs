using Api.Extension;
using Application.DependencyInjection;
using Infra.Data.Context;
using Infra.DependencyInjection;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddApplication();
builder.Services.AddInfra();
builder.Services.AddApiDoc();

builder.Services.AddDbContext<PortalDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
);

builder.Services.AddInfra();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseApiDoc();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
