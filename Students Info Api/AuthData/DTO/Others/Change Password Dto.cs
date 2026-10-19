using System.ComponentModel.DataAnnotations;

namespace Students_Info_Api.AuthData.DTO.Others
{
    public class Change_Password_Dto
    {
        [Required]
        [MaxLength(320,ErrorMessage = "Too Long Password")]
        public string CurrentPassword { get; set; }

        [Required]
        [MinLength(8, ErrorMessage = "password must be At least 8 characters.")]
        [RegularExpression(@"^(?=.*[A-Z])(?=.*[a-z])(?=.*[^a-zA-Z0-9]).+$", ErrorMessage = "Password must contain at least one uppercase letter, one lowercase letter, and one special character.")]
        public string NewPassword { get; set; }

        [Required]
        
        public string ConfirmNewPassword { get; set; }
    }
}
