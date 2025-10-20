
namespace censudex_auth_service.src.dtos
{
    /// <summary>
    /// DTO para el login de usuario.
    /// </summary>
    public class LoginDTO
    {
        /// <summary>
        /// Nombre de usuario. Puede ser correo electrónico o nombre de usuario.
        /// </summary>
        public required string Username { get; set; }
        /// <summary>
        /// Contraseña de usuario.
        /// </summary>
        public required string Password { get; set; }
    }
}