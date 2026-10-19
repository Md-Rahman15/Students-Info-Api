using System.ComponentModel.DataAnnotations;

namespace Students_Info_Api.DTO.StudentDtoFolder
{
    public class RequestStudentDto : IValidatableObject
    {

        [Required]
        public IFormFile Image { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(100)]
        public string Email { get; set; }

        [Required]
        [RegularExpression(@"^[6-9]\d{9}$")]
        public string PhoneNumber { get; set; }

        [Required]

        public DateOnly DateofBirth { get; set; }

        [Required]
        public string Gender { get; set; }

        [Required]
        public DateOnly EnrollmentDate { get; set; }

        [Required]
        public string CourseName { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Gender.Equals("Male", StringComparison.OrdinalIgnoreCase) || Gender.Equals("Female", StringComparison.OrdinalIgnoreCase) || Gender.Equals("Others", StringComparison.OrdinalIgnoreCase))
            {

            }
            else
            {


                yield return new ValidationResult("Gender must be Male, Female, or Others.", new[] { nameof(Gender) });

            }

            if (EnrollmentDate < DateofBirth)
            {

                yield return new ValidationResult("Enrollment date must before 2025", new[] { nameof(EnrollmentDate) });

            }

            int age;

            age = DateTime.Today.Year - DateofBirth.Year;

            if (age <= 16)
            {
                //birthday not happend this year
                if (DateTime.Today.Month < DateofBirth.Month || DateTime.Today.Month == DateofBirth.Month && DateTime.Today.Day < DateofBirth.Day)
                {

                    age--;

                }

                if (age < 16)
                {
                    yield return new ValidationResult("Student must be at least 16 years old.",
                    new[] { nameof(DateofBirth) });

                }


            }
        }
    }
}
