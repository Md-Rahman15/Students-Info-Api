using Students_Info_Api.Domain_Models;

namespace Students_Info_Api.Repository.Interfaces
{
    public interface ICoursePoints
    {

        public Task<Course_DM> CourseCreateRepo(Course_DM inputdata);

        public Task<List<Course_DM>> CourseGetAllRepo();

        public Task<Course_DM?> CourseUpdateRepo(Guid id,Course_DM inputdata);

        public Task<Course_DM?> CourseGetByIdRepo(Guid id);

        public Task<Course_DM?> CourseDeleteRepo(Guid id);

        public Task<Course_DM?> CourseGetNameRepo(string courseName);





    }
}
