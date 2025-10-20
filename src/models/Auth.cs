
namespace censudex_auth_service.src.models
{
    public class Auth
    {
        public Guid Id { get; set; }
        public List<string>? Roles { get; set; }
    }
}