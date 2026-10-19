namespace Students_Info_Api.AuthData.DTO.UserDtoFolder
{
    public class User_Login_Response_DTO
    {
        public Guid UserId { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Roles { get; set; }
        public string Token { get; set; }
        public string Message { get; set; }

    }
}
