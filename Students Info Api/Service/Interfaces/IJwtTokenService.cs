using Students_Info_Api.AuthData.DM;

namespace Students_Info_Api.Service.Interfaces
{
    public interface IJwtTokenService
    {
        string GenerateJwtToken(User_DM user);
    }
}
