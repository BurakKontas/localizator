using Localizator.Auth.Application;
using Localizator.Auth.Application.Interfaces.Validators;
using Localizator.Auth.Domain.Interfaces.Strategy;
using Localizator.Auth.Infrastructure;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddAuthInfrastructure();
builder.Services.AddAuthApplication();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var optionsFactory = scope.ServiceProvider.GetRequiredService<IAuthOptionsFactory>();
    var validator = scope.ServiceProvider.GetRequiredService<IAuthOptionsValidatorResolver>();

    var options = optionsFactory.Create();
    validator.Validate(options);
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
