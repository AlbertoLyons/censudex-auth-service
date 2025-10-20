
namespace censudex_auth_service.src.models
{
    /// <summary>
    /// Modelo de autenticación.
    /// </summary>
    public class Auth
    {
        /// <summary>
        /// Identificador único del usuario.
        /// </summary>
        public Guid Id { get; set; }
        /// <summary>
        /// Roles del usuario.
        /// </summary>
        public List<string>? Roles { get; set; }
    }
}