using Students_Info_Api.Domain_Models;
using Students_Info_Api.Service.CourseServiceF;

namespace Students_Info_Api.Service.Interfaces
{
    public interface ICourseService
    {
        public Task<CourseResultClass<List<Course_DM>>> CourseGetAllService();
        public Task<CourseResultClass<Course_DM>> CourseGetByIdService(Guid id);
        public Task<CourseResultClass<Course_DM>> CourseUpdateService(Guid id,Course_DM course);
        public Task<CourseResultClass<Course_DM>> CourseCreateService(Course_DM course);
        public Task<CourseResultClass<Course_DM>> CourseDeleteService(Guid id);
        public Task<CourseResultClass<Course_DM>> CourseGetByName(string courseName);
    }
}
