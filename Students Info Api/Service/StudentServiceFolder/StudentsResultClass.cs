using System.Net;
using Microsoft.IdentityModel.Tokens.Experimental;
using Student_Info_Api.Domain_Models;

namespace Students_Info_Api.Service.StudentService
{
    public class StudentsResultClass<Student_DM>
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public Student_DM? Data { get; set; }
        public ErrorType ErrorType { get; set; }

    }
}
