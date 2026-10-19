using AutoMapper;
using Student_Info_Api.Domain_Models;
using Students_Info_Api.AuthData.DM;
using Students_Info_Api.AuthData.DTO.UserDtoFolder;
using Students_Info_Api.AuthData.DTO.UserRequestDtoFolder;
using Students_Info_Api.Domain_Models;
using Students_Info_Api.DTO.CourseDtoFolder;
using Students_Info_Api.DTO.StudentDtoFolder;

namespace Students_Info_Api.AutoMapper
{
    public class MappingProfile:Profile
    {
        public MappingProfile()
        {
            CreateMap<Student_DM,ResponseStudentDto>().ReverseMap();
            CreateMap<Student_DM, RequestStudentDto>().ReverseMap();
            CreateMap<Student_DM, UpdateRequestStudentDto>().ReverseMap();
            CreateMap<Course_DM, Request_Course_Dto>().ReverseMap();
            CreateMap<Course_DM, Response_Course_Dto>().ReverseMap();
            CreateMap<User_DM, User_Register_Request_DTO>().ReverseMap();
            CreateMap<User_DM, User_Register_Response_DTO>().ReverseMap();
            CreateMap<User_DM, User_Login_Request_DTO>().ReverseMap();
            CreateMap<User_DM, User_Login_Response_DTO>().ReverseMap();
            CreateMap<Student_DM, ResponseStudentDto>() .ForMember(dest => dest.CourseName,opt => opt.MapFrom(src => src.Course.Name));
        }

    }
}
