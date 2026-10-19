using System.ComponentModel.DataAnnotations;

namespace Students_Info_Api.AuthData.DTO.UserRequestDtoFolder
{
    public class User_Login_Request_DTO
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [MinLength(8,ErrorMessage = "password must be At least 8 characters.")]
        [RegularExpression(@"^(?=.*[A-Z])(?=.*[a-z])(?=.*[^a-zA-Z0-9]).+$", ErrorMessage = "Password must contain at least one uppercase letter, one lowercase letter, and one special character.")]
        public string Password { get; set; }
    }
}
