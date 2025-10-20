
using censudex_auth_service.src.services;
using DotNetEnv;

Env.Load();

var clients_api_url = Environment.GetEnvironmentVariable("CLIENTS_API_URL");

var builder = WebApplication.CreateBuilder(args);

ClientsService clientsService = new ClientsService(new HttpClient(), clients_api_url ?? throw new InvalidOperationException("CLIENTS_API_URL is not set in environment variables."));
//var response = await clientsService.VerifyClientAsync<object>("adminCensudex", "Admin1234!");
builder.Services.AddSingleton(clientsService);

builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.Run();