using Microsoft.EntityFrameworkCore;
using Student_Info_Api.Data;
using Students_Info_Api.Domain_Models;
using Students_Info_Api.Repository.Interfaces;

namespace Students_Info_Api.Repository.CourseRepo
{
    public class RepoCoursePoints : ICoursePoints
    {
        private readonly ApplicationDbContext applicationDbContext;

        public RepoCoursePoints(ApplicationDbContext applicationDbContext)
        {
            this.applicationDbContext = applicationDbContext;
        }

        public async Task<Course_DM> CourseCreateRepo(Course_DM inputdata)
        {
            inputdata.id = Guid.NewGuid();

            await applicationDbContext.Courses.AddAsync(inputdata);

            await applicationDbContext.SaveChangesAsync();

            return inputdata;

        }

        public async Task<Course_DM?> CourseDeleteRepo(Guid id)
        {
           

            var search = await applicationDbContext.Courses.FirstOrDefaultAsync(x => x.id == id);

            if (search == null) 
            {
                return null;
            }

            applicationDbContext.Courses.Remove(search);

            await applicationDbContext.SaveChangesAsync();

            return search;

        }

        public async Task<List<Course_DM>> CourseGetAllRepo()
        {

            var search = await applicationDbContext.Courses.ToListAsync();

            return search;

        }

        public async Task<Course_DM?> CourseGetByIdRepo(Guid id)
        {
           var search = await applicationDbContext.Courses.FirstOrDefaultAsync(x => x.id == id);

            if (search == null) 
            {
                return null;
            }

            return search;         
        }

        public async Task<Course_DM?> CourseGetNameRepo(string courseName)
        {
            if (courseName == null) 
            {
                return null;
            }

            var cleanName = courseName.Trim().ToUpper();
            var result = await applicationDbContext.Courses.FirstOrDefaultAsync(x => x.Name.Trim().ToUpper() == cleanName);

            if (result == null)
            {
                return null;
            }

            return result;

        }

        public async Task<Course_DM?> CourseUpdateRepo(Guid id, Course_DM inputdata)
        {
            var search = await applicationDbContext.Courses.FirstOrDefaultAsync(x => x.id == id);

            if (search == null)
            {

                return null;

            }

            search.Name = inputdata.Name;
            search.Description = inputdata.Description;
            search.Duration = inputdata.Duration;

            await applicationDbContext.SaveChangesAsync();

            return search;


        }
    }
}
