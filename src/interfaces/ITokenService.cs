using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using censudex_auth_service.src.models;

namespace censudex_auth_service.src.interfaces
{
    public interface ITokenService
    {
        string CreateToken(Auth user);

    }
}