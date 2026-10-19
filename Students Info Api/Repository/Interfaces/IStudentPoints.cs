using Microsoft.AspNetCore.Mvc;
using Student_Info_Api.Domain_Models;
using Students_Info_Api.DTO;

namespace Students_Info_Api.Repository.Interfaces
{
    public interface IStudentPoints
    {
        public Task<List<Student_DM>> GetAllRepo(string? SortBy, bool? SortAscending, string? Gender , int? PageNumber, int? PageSize , string? search, bool? byFirstName);
        public Task<Student_DM> GetByIdRepo(Guid id);
        public Task<Student_DM> CreateRepo(Student_DM input);
        public Task<Student_DM> UpdateRepo(Guid id,Student_DM input);
        public Task<Student_DM> DeleteRepo(Guid id);
        public Task<Student_DM> GetEmailRepo(string email);
        public Task<Student_DM> GetPhoneNumberRepo(string PhoneNumber);
        public Task<bool> GetStudentWithCourse(Guid courseId);
        public  Task<Student_DM> UserIdCheck(Student_DM student);
        public Task<Student_DM> GetUserId(Guid id);


    }


}
