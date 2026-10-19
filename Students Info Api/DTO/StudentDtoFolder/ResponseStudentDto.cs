using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Students_Info_Api.Domain_Models;

namespace Students_Info_Api.DTO.StudentDtoFolder
{
    public class ResponseStudentDto
    {

        public Guid id { get; set; }
        public string ImageUrl { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public DateOnly DateofBirth { get; set; }
        public string Gender { get; set; }
        public DateOnly EnrollmentDate { get; set; }

        public string CourseName { get; set; }


    }
}
