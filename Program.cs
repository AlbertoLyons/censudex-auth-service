using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using censudex_auth_service.src.interfaces;
using censudex_auth_service.src.services;
using DotNetEnv;
using System.Text;
using Grpc.AspNetCore.Web;
// Carga las variables de entorno
Env.Load();
// Configura la aplicación web
var builder = WebApplication.CreateBuilder(args);
// Registra el servicio de tokens en el contenedor de dependencias
builder.Services.AddScoped<ITokenService, TokenService>();
// Configura la autenticación JWT
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

// Agrega servicios al contenedor de dependencias
builder.Services.AddOpenApi();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
// Configura la autorización
builder.Services.AddAuthorization();
// Configura gRPC
builder.Services.AddGrpc();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}
// Configura el middleware de gRPC-Web
app.UseGrpcWeb();
app.MapGrpcService<UserProto.UserService.UserServiceClient>().EnableGrpcWeb();
// Utiliza la autenticación y autorización en la aplicación
app.UseAuthentication();
app.UseAuthorization();
app.UseHttpsRedirection();
app.MapControllers();

app.Run();