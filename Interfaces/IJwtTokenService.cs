using FleetManage.Api.Data;

namespace FleetManage.Api.Interfaces
{
    public interface IJwtTokenService
    {
        string CreateToken(AppUser user);
    }
}
