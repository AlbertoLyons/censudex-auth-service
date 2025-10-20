using System.Security.Claims;
using censudex_auth_service.src.models;
using Microsoft.AspNetCore.Identity;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using censudex_auth_service.src.interfaces;

namespace censudex_auth_service.src.services
{
    /// <summary>
    /// Servicio para la gestión de tokens JWT.
    /// </summary>
    public class TokenService : ITokenService
    {
        // Clave simétrica utilizada para firmar los tokens JWT.
        private readonly SymmetricSecurityKey _key;
        // Lista estática para almacenar tokens válidos.
        private static List<string> _validTokens = new List<string>();
        /// <summary>
        /// Constructor que inicializa la clave de firma a partir de una variable de entorno.
        /// </summary>
        public TokenService()
        {
            // Obtiene la clave de firma desde las variables de entorno
            var signingKey = Environment.GetEnvironmentVariable("JWT_SIGNING_KEY") ?? throw new ArgumentNullException("JWT_SIGNING_KEY environment variable is not set.");
            _key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(signingKey));
        }
        /// <summary>
        /// Crea un token JWT para el usuario autenticado.
        /// </summary>
        /// <param name="user"></param>
        /// <returns>Token creado</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public string CreateToken(Auth user)
        {
            // Define los claims del token
            var claims = new List<Claim>
            {
                // Identificador único del token
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                // Agrega el ID del usuario como claim
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            };
            // Agrega los roles del usuario como claims
            if (user.Roles != null)
            {
                foreach (var role in user.Roles)
                {
                    claims.Add(new Claim(ClaimTypes.Role, role));
                }
            }
            // Crea las credenciales de firma
            var creds = new SigningCredentials(_key, SecurityAlgorithms.HmacSha512Signature);
            // Obtiene el issuer y audience desde las variables de entorno
            var issuer = Environment.GetEnvironmentVariable("JWT_ISSUER") ?? throw new ArgumentNullException("JWT Issuer cannot be null or empty.");
            var audience = Environment.GetEnvironmentVariable("JWT_AUDIENCE") ?? throw new ArgumentNullException("JWT Audience cannot be null or empty.");
            // Define el descriptor del token
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                // Establece la expiración del token a 1 día
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = creds,
                Issuer = issuer,
                Audience = audience
            };
            // Crea el token
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            // Almacena el token en la lista de tokens válidos
            _validTokens.Add(tokenHandler.WriteToken(token));
            // Devuelve el token como una cadena
            return tokenHandler.WriteToken(token);
        }
        /// <summary>
        /// Valida un token JWT y devuelve los claims asociados si es válido.
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public ClaimsPrincipal? ValidateToken(string token)
        {
            // Verifica si el token está en la lista de tokens válidos
            if (!_validTokens.Contains(token))
            {
                return null;
            }
            // Crea el manejador de tokens JWT
            var tokenHandler = new JwtSecurityTokenHandler();
            var issuer = Environment.GetEnvironmentVariable("JWT_ISSUER") ?? throw new ArgumentNullException("JWT Issuer cannot be null or empty.");
            var audience = Environment.GetEnvironmentVariable("JWT_AUDIENCE") ?? throw new ArgumentNullException("JWT Audience cannot be null or empty.");
            try
            {
                // Valida el token y obtiene los claims
                var principal = tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = _key,
                    ValidateIssuer = true,
                    ValidIssuer = issuer,
                    ValidateAudience = true,
                    ValidAudience = audience,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                return principal;
            }
            // Si la validación falla, devuelve null
            catch
            {
                return null;
            }
        }
        /// <summary>
        /// Invalida un token JWT para el cierre de sesión, removiéndolo de la lista de tokens válidos.
        /// </summary>
        /// <param name="token"></param>
        public void InvalidateToken(string token)
        {
            _validTokens.Remove(token);
        }
    }
}