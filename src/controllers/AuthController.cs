using System.Security.Claims;
using censudex_auth_service.src.dtos;
using censudex_auth_service.src.interfaces;
using censudex_auth_service.src.models;
using censudex_auth_service.src.services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace censudex_auth_service.src.controllers
{
    /// <summary>
    /// Controlador de autenticación de usuarios.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        /// <summary>
        /// Servicio de tokens
        /// </summary>
        private readonly ITokenService _tokenService;
        /// <summary>
        /// Constructor del controlador de autenticación.
        /// </summary>
        /// <param name="tokenService"></param>
        public AuthController(ITokenService tokenService)
        {
            _tokenService = tokenService;
        }
        /// <summary>
        /// Método para iniciar sesión del usuario.
        /// </summary>
        /// <param name="loginDTO"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Login([FromBody] Auth auth)
        {
            // Verificar las credenciales del usuario
            // Si las credenciales son incorrectas, devolver Unauthorized
            if (auth == null)
            {
                return Unauthorized("Datos de autenticación no proporcionados.");
            }
            // Crear el token JWT
            var token = _tokenService.CreateToken(auth);
            // Devolver el token al cliente
            var result = new
            {
                Token = token
            };
            return Ok(result);
        }
        /// <summary>
        /// Método para validar el token del usuario.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize]
        public IActionResult ValidateToken()
        {
            // Obtener el token de la cabecera Authorization del Bearer Token
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            // Validar el token
            var principal = _tokenService.ValidateToken(token);
            // Si el token no es válido, devolver Unauthorized
            if (principal == null)
            {
                return Unauthorized();
            }
            // Devolver los datos del usuario en caso de que el token sea válido
            var result = new
            {
                Id = principal.FindFirstValue(ClaimTypes.NameIdentifier),
                Roles = principal.FindAll(ClaimTypes.Role).Select(c => c.Value)
            };
            return Ok(result);
        }
        /// <summary>
        /// Método para cerrar sesión del usuario, invalidando el token.
        /// </summary>
        /// <returns></returns>
        [HttpPost("logout")]
        [Authorize]
        public IActionResult Logout()
        {
            // Obtener el token de la cabecera Authorization del Bearer Token
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            // Invalida el token
            _tokenService.InvalidateToken(token);
            var response = new
            {
                Message = "Sesión cerrada correctamente."
            };
            return Ok(response);
        }
    }
}
    
