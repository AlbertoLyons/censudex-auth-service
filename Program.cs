
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using censudex_auth_service.src.interfaces;
using censudex_auth_service.src.services;
using DotNetEnv;
using System.Text;

Env.Load();

var clients_api_url = Environment.GetEnvironmentVariable("CLIENTS_API_URL");

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<ITokenService, TokenService>();

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = Env.GetString("JWT_ISSUER") ?? throw new InvalidOperationException("JWT_ISSUER is not set in environment variables."),
            ValidAudience = Env.GetString("JWT_AUDIENCE") ?? throw new InvalidOperationException("JWT_AUDIENCE is not set in environment variables."),
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Env.GetString("JWT_SIGNING_KEY") ?? throw new InvalidOperationException("JWT_SIGNING_KEY is not set in environment variables.")))
        };
    });

ClientsService clientsService = new ClientsService(new HttpClient(), clients_api_url ?? throw new InvalidOperationException("CLIENTS_API_URL is not set in environment variables."));
//var response = await clientsService.VerifyClientAsync<object>("adminCensudex", "Admin1234!");
builder.Services.AddSingleton(clientsService);
builder.Services.AddOpenApi();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddAuthorization();


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseAuthentication();
app.UseAuthorization();
app.UseHttpsRedirection();
app.MapControllers();

app.Run();