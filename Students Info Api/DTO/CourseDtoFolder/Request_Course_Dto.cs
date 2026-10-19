using System.ComponentModel.DataAnnotations;

namespace Students_Info_Api.DTO.CourseDtoFolder
{
    public class Request_Course_Dto
    {
        [Required]
        [StringLength(20, MinimumLength = 3)]
        public string Name { get; set; }

        [Required]
        [StringLength(250, MinimumLength = 10)]
        public string Description { get; set; }

        [Required]
        [StringLength(25, MinimumLength = 3)]
        public string Duration { get; set; }


    }
}
