using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using censudex_auth_service.src.dtos;
using censudex_auth_service.src.interfaces;
using censudex_auth_service.src.models;
using censudex_auth_service.src.services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace censudex_auth_service.src.controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ITokenService _tokenService;
        private readonly ClientsService _clientsService;
        public AuthController(ITokenService tokenService, ClientsService clientsService)
        {
            _tokenService = tokenService;
            _clientsService = clientsService;
        }
        [HttpPost]
        public async Task<IActionResult> Login([FromBody] LoginDTO loginDTO)
        {
            var client = await _clientsService.VerifyClientAsync<object>(loginDTO.Username, loginDTO.Password);
            if (client == null)
            {
                return Unauthorized("Usuario o contraseña incorrectos.");
            }
            var token = _tokenService.CreateToken(client);
            var result = new
            {
                Token = token
            };
            return Ok(result);
        }
        [HttpGet]
        [Authorize]
        public IActionResult ValidateToken()
        {
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var principal = _tokenService.ValidateToken(token);
            if (principal == null)
            {
                return Unauthorized();
            }
            var result = new
            {
                Id = principal.FindFirstValue(ClaimTypes.NameIdentifier),
                Roles = principal.FindAll(ClaimTypes.Role).Select(c => c.Value)
            };
            return Ok(result);
        }
        [HttpPost("logout")]
        [Authorize]
        public IActionResult Logout()
        {
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            _tokenService.InvalidateToken(token);
            return Ok("Sesión cerrada correctamente.");
        }
    }
}
    
