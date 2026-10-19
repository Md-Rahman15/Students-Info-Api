using System.ComponentModel.DataAnnotations;

namespace Students_Info_Api.AuthData.DTO.Others
{
    public class RequestPasswordResetDto
    {
        [Required]
        [EmailAddress]
        [MaxLength(320,ErrorMessage ="Invalid email")]
        public string Email { get; set; }
    }
}
