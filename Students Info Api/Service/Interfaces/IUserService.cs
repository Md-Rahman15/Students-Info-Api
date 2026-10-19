using Students_Info_Api.AuthData.DM;
using Students_Info_Api.AuthData.DTO.Others;
using Students_Info_Api.AuthData.DTO.UserRequestDtoFolder;
using Students_Info_Api.Service.UserServiceFolder.UserResultClass;

namespace Students_Info_Api.Service.Interfaces
{
    public interface IUserService
    {
        public Task<UserResultClass<User_DM>> UserRegisterService(User_DM user , User_Register_Request_DTO userdto);
        public Task<UserResultClass<User_DM>> LoginUserService(User_DM user,User_Login_Request_DTO logindto);
        public Task<UserResultClass<User_DM>> PasswordChangeService(Change_Password_Dto change_Password);
        public Task<UserResultClass<string>> ForgotPasswordService(string email);
        public Task<UserResultClass<User_DM>> EmailBasedPasswordResetService(EmailBasedResetPasswordDto input);
        public Task<UserResultClass<string>> UserAccountEmailChangeService(NewEmailInfo input);
        public Task<UserResultClass<string>> UpdateUserNewEmailAddressService(string token);
        public Task<UserResultClass<string>> UserEmailVerifactionService(string token);

    }
}
