namespace Students_Info_Api.AuthData.DM
{
    public class User_DM
    {
        public Guid UserId { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string Roles { get; set; }
        public bool IsEmailVerified { get; set; }
    }
}
