using System.Security.Claims;
using censudex_auth_service.src.models;
using Microsoft.AspNetCore.Identity;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using censudex_auth_service.src.interfaces;

namespace censudex_auth_service.src.services
{
    public class TokenService : ITokenService
    {
        private readonly SymmetricSecurityKey _key;
        private static List<string> _validTokens = new List<string>();

        public TokenService()
        {
            var signingKey = Environment.GetEnvironmentVariable("JWT_SIGNING_KEY") ?? throw new ArgumentNullException("JWT_SIGNING_KEY environment variable is not set.");
            _key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(signingKey));
        }
        public string CreateToken(Auth user)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            };

            if (user.Roles != null)
            {
                foreach (var role in user.Roles)
                {
                    claims.Add(new Claim(ClaimTypes.Role, role));
                }
            }

            var creds = new SigningCredentials(_key, SecurityAlgorithms.HmacSha512Signature);
            var issuer = Environment.GetEnvironmentVariable("JWT_ISSUER") ?? throw new ArgumentNullException("JWT Issuer cannot be null or empty."); ;
            var audience = Environment.GetEnvironmentVariable("JWT_AUDIENCE") ?? throw new ArgumentNullException("JWT Audience cannot be null or empty.");

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = creds,
                Issuer = issuer,
                Audience = audience
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            _validTokens.Add(tokenHandler.WriteToken(token));
            return tokenHandler.WriteToken(token);
        }
        public ClaimsPrincipal? ValidateToken(string token)
        {
            if (!_validTokens.Contains(token))
            {
                return null;
            }
            var tokenHandler = new JwtSecurityTokenHandler();
            var issuer = Environment.GetEnvironmentVariable("JWT_ISSUER") ?? throw new ArgumentNullException("JWT Issuer cannot be null or empty.");
            var audience = Environment.GetEnvironmentVariable("JWT_AUDIENCE") ?? throw new ArgumentNullException("JWT Audience cannot be null or empty.");
            try
            {
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
            catch
            {
                return null;
            }
        }
        public void InvalidateToken(string token)
        {
            _validTokens.Remove(token);
        }
    }
}