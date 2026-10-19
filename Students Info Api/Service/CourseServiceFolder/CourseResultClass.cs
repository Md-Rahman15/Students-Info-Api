using Students_Info_Api.Domain_Models;

namespace Students_Info_Api.Service.CourseServiceF
{
    public class CourseResultClass<Course_DM>
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public Course_DM? Data { get; set; }
        public ErrorType ErrorType { get; set; }

    }
}
