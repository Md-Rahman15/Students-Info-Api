using Microsoft.AspNetCore.Mvc;
using Student_Info_Api.Domain_Models;
using Students_Info_Api.DTO;
using Students_Info_Api.Service.StudentService;

namespace Students_Info_Api.Service.Interfaces
{
    public interface IStudentService
    {
        public Task<StudentsResultClass<List<Student_DM>>> GetAllService(string? SortBy,  bool? SortAscending,
            string? Gender, int? PageNumber, int? PageSize, string? search, bool? byFirstName);
        public Task<StudentsResultClass<Student_DM>> GetByIdService(Guid id);
        public Task<StudentsResultClass<Student_DM>> UpdateService(Guid id, Student_DM input, IFormFile? image);
        public Task<StudentsResultClass<Student_DM>> CreateService(Student_DM input,IFormFile? image);
        public Task<StudentsResultClass<Student_DM>> DeleteService(Guid id);
        public Task<StudentsResultClass<Student_DM>> GetStudentProfile();
        public Task<StudentsResultClass<Student_DM>> UpdateStudentProfile(Student_DM input, IFormFile? image);

    }
}
