using System.ComponentModel.DataAnnotations;

namespace Students_Info_Api.AuthData.DM
{
    public class NewEmailInfo
    {
        [Required]
        public string Password { get; set; }

        [Required]
        [EmailAddress]
        public string EmailAddress { get; set; }

        [Required]
        [EmailAddress]
        public string ConfirmEmailAddress { get; set; }


    }
}
