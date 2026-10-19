using Students_Info_Api.AuthData.DM;

namespace Students_Info_Api.Service.Interfaces
{
    public interface IEmailService
    {
        public Task<bool> SendTokenLink(User_DM user,string resetlink, string emailaddress);   
    }
}
