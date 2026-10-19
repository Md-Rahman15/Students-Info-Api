using System.ComponentModel.DataAnnotations.Schema;
using Students_Info_Api.AuthData.DM;
using Students_Info_Api.Domain_Models;

namespace Student_Info_Api.Domain_Models
{
    public class Student_DM
    {
        public Guid id { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public DateOnly DateofBirth { get; set; }
        public string Gender { get; set; }
        public DateOnly EnrollmentDate { get; set; }
        public string ImageUrl { get; set; }

        //navigation properties
        public Guid CourseId { get; set; }
        public Guid? UserId { get; set; }                                     

        [NotMapped]
        public string CourseName { get; set; }
        public Course_DM Course { get; set; }
        public User_DM User { get; set; }


    }
}
