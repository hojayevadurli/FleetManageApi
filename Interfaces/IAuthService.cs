using static FleetManage.Api.DTOs.AuthDtos;

namespace FleetManage.Api.Interfaces
{
    public interface IAuthService
    {
        Task<(bool ok, object? result, IEnumerable<string>? errors)>
           RegisterCompanyAsync(RegisterCompanyDto dto);
    }
}
