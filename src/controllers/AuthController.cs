using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using censudex_auth_service.src.dtos;
using censudex_auth_service.src.interfaces;
using censudex_auth_service.src.models;
using censudex_auth_service.src.services;
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
                return Unauthorized("Invalid username or password.");
            }
            var token = _tokenService.CreateToken(client);
            var result = new
            {
                Token = token
            };
            return Ok(result);
        }
    }
}