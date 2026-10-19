using Microsoft.AspNetCore.Mvc;
using Students_Info_Api.Domain_Models;
using Students_Info_Api.Repository.Interfaces;
using Students_Info_Api.Service.Interfaces;

namespace Students_Info_Api.Service.CourseServiceF
{
    public class CourseService : ICourseService
    {
        private readonly ICoursePoints repoCoursePoints;
        private readonly IStudentPoints studentRepo;

        public CourseService(ICoursePoints repoCoursePoints,IStudentPoints studentRepo)
        {
            this.repoCoursePoints = repoCoursePoints;
            this.studentRepo = studentRepo;
        }

        public CourseResultClass<T> Helper<T>(T? Data, bool boolres, string? msg,ErrorType errorType)
        {
            if (boolres == false)
            {
                CourseResultClass<T> resultFailed = new CourseResultClass<T>()
                {

                    Success = false,
                    Message = msg,
                    Data = default,
                    ErrorType = errorType
                    

                };

                return resultFailed;

            }                   

                CourseResultClass<T> resultSuccess = new CourseResultClass<T>()
                {
                    Success = true,
                    Data = Data,
                    Message = msg,
                    ErrorType = errorType

                };
                return resultSuccess;
        }

        public async Task<CourseResultClass<Course_DM>> CourseCreateService(Course_DM course)
        {
            if (course == null) 
            {
                return Helper<Course_DM>(null, false, "Incorrect info",ErrorType.ValidationError);
            }

            var checkDuplication = await repoCoursePoints.CourseGetNameRepo(course.Name);

            if (checkDuplication != null) 
            {
                return Helper<Course_DM>(null, false, "This course name already exits",ErrorType.Conflict);
            }

            //runs the repo method
            var result = await repoCoursePoints.CourseCreateRepo(course);

            //handles the failures
            if (result == null)
            {

                return Helper(result, false, "Incorrect info",ErrorType.ValidationError);

            }

            //returns the success
            return Helper(result, true, null,ErrorType.None);

        }

        public async Task<CourseResultClass<Course_DM>> CourseDeleteService(Guid id)
        {
            //get students who have same course of deletion if exits 
            var hasCourse = await studentRepo.GetStudentWithCourse(id);

            if (hasCourse) 
            {
                return Helper<Course_DM>(null, false,"Please remove course assisgment from students who have it",ErrorType.Conflict);
            }

            //runs the repo method
            var result = await repoCoursePoints.CourseDeleteRepo(id);

            //handles the failures
            if (result == null)
            {
                return Helper(result, false, "Id not found", ErrorType.NotFound);
            }

            //returns the success
            return Helper(result, true, null, ErrorType.None);

        }

        public async Task<CourseResultClass<List<Course_DM>>> CourseGetAllService()
        {
            //run the repo method
            var result = await repoCoursePoints.CourseGetAllRepo();

            //handle the failure
            if (result == null)
            {
                return Helper(result, false, "Result Not Found", ErrorType.NotFound);
            }

            return Helper(result, true, null, ErrorType.None);

        }

        public async Task<CourseResultClass<Course_DM>> CourseGetByIdService(Guid id)
        {
            //runs the repo method
            var result = await repoCoursePoints.CourseGetByIdRepo(id);

            //handles the failures
            if (result == null)
            {


                return Helper(result, false, "Id Not Found", ErrorType.NotFound);

            }

            //returns the success


            return Helper(result, true, null,ErrorType.None);

        }

        public async Task<CourseResultClass<Course_DM>> CourseUpdateService(Guid id, Course_DM course)
        {
            if (id == Guid.Empty) 
            {
                return Helper<Course_DM>(null, false, "Id not found", ErrorType.NotFound);
            }

            if (course == null) 
            {
                return Helper<Course_DM>(null, false, "Course does not exits", ErrorType.NotFound);
            }

            var courseNameValidation = await repoCoursePoints.CourseGetNameRepo(course.Name);

            if (courseNameValidation != null) 
            {
                return Helper<Course_DM>(null, false, "This course name already exits", ErrorType.Conflict);
            }

            var result = await repoCoursePoints.CourseUpdateRepo(id, course);

            //handles the failures
            if (result == null)
            {
                return Helper(result, false, "Id not found", ErrorType.NotFound);
            }

            //returns the success

            return Helper(result, true, null, ErrorType.None);


        }

        public async Task<CourseResultClass<Course_DM>> CourseGetByName(string courseName)
        {

            if (courseName == null)
            {
                return Helper<Course_DM>(null, false, "Course Not Found", ErrorType.NotFound);
            }

            var result = await  repoCoursePoints.CourseGetNameRepo(courseName);

            if (result == null) 
            {
                return Helper<Course_DM>(null, false, "Course Not Found", ErrorType.NotFound);
            }

            return Helper(result, true,"Course Found", ErrorType.None);



        }


    }

}
