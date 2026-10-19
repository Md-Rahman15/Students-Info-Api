using Student_Info_Api.Domain_Models;
using Students_Info_Api.AuthData.DM;

namespace Students_Info_Api.Service.UserServiceFolder.UserResultClass
{
    public class UserResultClass<User_DM>
    {
        public bool Success { get; set; }
        public string? Message { get; set; }     
        public User_DM? Data { get; set; }

        public ErrorType errorType { get; set; }
        public string? Token { get; set; }
    }
}
