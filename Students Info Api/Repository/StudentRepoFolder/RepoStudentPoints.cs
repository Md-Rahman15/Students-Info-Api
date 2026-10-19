using Microsoft.EntityFrameworkCore;
using Student_Info_Api.Data;
using Student_Info_Api.Domain_Models;
using Students_Info_Api.Repository.Interfaces;

namespace Students_Info_Api.Repository.StudentRepo
{
    public class RepoStudentPoints : IStudentPoints
    {
        private readonly ApplicationDbContext applicationDbContext;
        public RepoStudentPoints(ApplicationDbContext applicationDbContext)
        {
            this.applicationDbContext = applicationDbContext;
        }

        public async Task<Student_DM> UserIdCheck(Student_DM student)
        {

            var search = await applicationDbContext.Students.FirstOrDefaultAsync(x => x.UserId == student.UserId);

            if (search == null)
            {

                return null;

            }

            return search;
        }

        public async Task<Student_DM> CreateRepo(Student_DM input)
        {

            //creates new guid id 
            input.id = Guid.NewGuid();

            //add the input info to ddb
            await applicationDbContext.AddAsync(input);

            //saves the input info to db
            await applicationDbContext.SaveChangesAsync();

            //retunrs data
            return input;

        }

        public async Task<Student_DM> DeleteRepo(Guid id)
        {
            //finds the id from input
            var search = await applicationDbContext.Students.FirstOrDefaultAsync(x => x.id == id);

            //if id is invalid returns error
            if (search == null)
            {
                return null;
            }

            //delete info based on input id
            applicationDbContext.Remove(search);

            //saves the changes
            await applicationDbContext.SaveChangesAsync();

            //returns the deleted info one last time
            return search;

        }

        public async Task<List<Student_DM>> GetAllRepo(string? sortBy, bool? sortAscending,
        string? gender, int? pageNumber, int? pageSize, string? search,bool? byFirstName)
        {
            //we can perform all query opeartion on students bcoz of asqueryable();
            var result = applicationDbContext.Students.Include(x => x.Course).AsQueryable();

            //search the students based on thier first or last name with bool toggle
            result = SearchByFirstOrLastName(result, search, byFirstName);

            //gender query
            if (gender != null)
            {
                //takes gender input convert it to upper of both db and input and compare give back result based on gender
                result = result.Where(students => students.Gender.ToUpper() == gender.Trim().ToUpper());
            }

            //sorting students based on sortby and bool
            result = SortByGetAll(sortBy, result, sortAscending);     

            //paginates the pages
            result = PaginationGetAll(result, pageNumber, pageSize);

            //Returning the result by converting it from iquerable to tolistasync 
            var students = await result.ToListAsync();

            foreach (var student in students)
            {
                student.CourseName = student.Course?.Name;
            }

            return students;

        }
        public async Task<Student_DM> GetByIdRepo(Guid id)
        {
            //searches the find in database
            var search = await applicationDbContext.Students.Include(x => x.Course).FirstOrDefaultAsync(x => x.id == id);

            //if match not found return null
            if (search == null)
            {
                return null;
            }
            search.CourseName = search.Course.Name;

            //return all the found data
            return search;
        }
        public async Task<Student_DM> GetEmailRepo(string email)
        {
            //searches the email id from db
            var studentemail = await applicationDbContext.Students.FirstOrDefaultAsync(x => x.Email == email);

            //if match not found return null
            if (studentemail == null)
            {
                return null;
            }
            //reutrn the matched data
            return studentemail;

        }
        public async Task<Student_DM> GetPhoneNumberRepo(string PhoneNumber)
        {
            //searches the mobile number from db
            var getNumber = await applicationDbContext.Students.FirstOrDefaultAsync(x => x.PhoneNumber == PhoneNumber);

            //if match not found return null
            if (getNumber == null)
            {
                return null;
            }

            //reutrn the matched data
            return getNumber;

        }
        public async Task<Student_DM> UpdateRepo(Guid id, Student_DM input)
        {
            //search the students exist or not
            var search = await applicationDbContext.Students.Include(x => x.Course).FirstOrDefaultAsync(x => x.id == id);

            //if not student not found return null
            if (search == null)
            {
                return null;
            }

            //if matched then update the database info from input info
            search.FirstName = input.FirstName;
            search.LastName = input.LastName;
            search.Email = input.Email;
            search.PhoneNumber = input.PhoneNumber;
            search.Gender = input.Gender;
            search.DateofBirth = input.DateofBirth;
            search.EnrollmentDate = input.EnrollmentDate;
            search.CourseId = input.CourseId;
            search.ImageUrl = input.ImageUrl;

            //save the changes
            await applicationDbContext.SaveChangesAsync();

            //returns the updated info
            return search;


        }
        public async Task<Student_DM> GetUserId(Guid id)
        {
            var search = await applicationDbContext.Students.Include(x => x.Course).FirstOrDefaultAsync(x => x.UserId == id);

            if (search == null)
            {
                return null;
            }

            search.CourseName = search.Course.Name;

            return search;
        }
        public IQueryable<Student_DM> SearchByFirstOrLastName(IQueryable<Student_DM> result, string? search, bool? byFirstName)
        {
            if (search != null)
            {

                if (byFirstName == true)
                {
                    result = result.Where(x => x.FirstName.Trim().ToUpper().Contains(search.Trim().ToUpper()));
                    return result;

                        
                }
                if (byFirstName == false)  
                {
                    result = result.Where(x => x.LastName.Trim().ToUpper().Contains(search.Trim().ToUpper()));

                    return result;
                }
            }

            return result;

        }
        public IQueryable<Student_DM> SortByGetAll(string? sortBy, IQueryable<Student_DM> result, bool? sortAscending)
        {

            //sort operations
            if (sortBy != null)
            {
                // sort by name with A to Z
                if (sortBy.Contains("FirstName", StringComparison.OrdinalIgnoreCase) && sortAscending == true)
                {
                    result = result.OrderBy(students => students.FirstName);

                }
                //sort by name with Z to A
                else if (sortBy.Contains("FirstName", StringComparison.OrdinalIgnoreCase) && sortAscending == false)
                {
                    result = result.OrderByDescending(students => students.FirstName);
                }

                //sort by enrollment date with 1-100
                if (sortBy.Contains("EnrollmentDate", StringComparison.OrdinalIgnoreCase) && sortAscending == true)
                {

                    result = result.OrderBy(students => students.EnrollmentDate);

                }
                //sort by enrollment date with 100-1
                else if (sortBy.Contains("EnrollmentDate", StringComparison.OrdinalIgnoreCase) && sortAscending == false)
                {
                    result = result.OrderByDescending(students => students.EnrollmentDate);
                }
            }

            return result;

        }
        public IQueryable<Student_DM> PaginationGetAll(IQueryable<Student_DM> result, int? pageNumber, int? pageSize) 
        {   
            //pagination 
            int PageNumberDefault = pageNumber ?? 1;
            int PageSizeDefault = pageSize ?? 50;     

            result = result.Skip((PageNumberDefault - 1) * PageSizeDefault).Take(PageSizeDefault);

            return result;

        }
        public async Task<bool> GetStudentWithCourse(Guid courseId)
        {
            var search = await applicationDbContext.Students.FirstOrDefaultAsync(x => x.CourseId == courseId);

            if (search == null) 
            {
                return false;
            }

            return true;

        }
    }
}
