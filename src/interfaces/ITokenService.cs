using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using censudex_auth_service.src.models;

namespace censudex_auth_service.src.interfaces
{
    /// <summary>
    /// Interfaz para el servicio de tokens.
    /// </summary>
    public interface ITokenService
    {
        /// <summary>
        /// Crea un token para el usuario dado.
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        string CreateToken(Auth user);
        /// <summary>
        /// Valida el token dado.
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        ClaimsPrincipal? ValidateToken(string token);
        /// <summary>
        /// Invalida el token dado.
        /// </summary>
        /// <param name="token"></param>
        void InvalidateToken(string token);
    }
}