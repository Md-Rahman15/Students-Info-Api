using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Student_Info_Api.Domain_Models;
using Students_Info_Api.DTO.StudentDtoFolder;
using Students_Info_Api.Service.Interfaces;

namespace Students_Info_Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class StudentController : BaseController
    {
        private readonly IMapper mapper;
        private readonly IStudentService servicelayer;
        public StudentController(IMapper mapper, IStudentService servicelayer)
        {
            this.mapper = mapper;
            this.servicelayer = servicelayer;
        }


        //GETALL STUDENTS FROM DATABASE TO A LIST
        [HttpGet]
        [Authorize(Roles = "Admin")]

        public async Task<IActionResult> GetAllStudentsEndPoint([FromQuery] string? search, [FromQuery] string? SortBy, [FromQuery] bool? SortAscending, [FromQuery] string? Gender, [FromQuery] int? PageNumber, [FromQuery] int? PageSize, [FromQuery] bool? byFirstName)
        {
            var result = await servicelayer.GetAllService(SortBy, SortAscending ?? true, Gender, PageNumber, PageSize, search, byFirstName ?? false);

            if (!result.Success)
            {
                return ErrorHandler(result.ErrorType, result.Message);
            }

            var studentDto = mapper.Map<List<ResponseStudentDto>>(result.Data);

            return Ok(studentDto);
        }

        //GET SIGNLE BY ID
        [HttpGet("{getid}")]
        [Authorize(Roles = "Admin")]

        public async Task<IActionResult> GetByIdEndPoint([FromRoute] Guid getid)
        {
            var result = await servicelayer.GetByIdService(getid);

            if (!result.Success)
            {
                return ErrorHandler(result.ErrorType, result.Message);
            }

            var dto = mapper.Map<ResponseStudentDto>(result.Data);

            return Ok(dto);

        }



        //CREATES THE INFO BASED ON ID
        [HttpPost]
        [Consumes("multipart/form-data")]
        [Authorize(Roles = "Student,Admin")]
        public async Task<IActionResult> CreateStudentEndPoint([FromForm] RequestStudentDto inputdata)
        {
            var domainmodel = mapper.Map<Student_DM>(inputdata);

            var result = await servicelayer.CreateService(domainmodel, inputdata.Image);

            if (!result.Success)
            {
                return ErrorHandler(result.ErrorType, result.Message);
            }

            var dto = mapper.Map<ResponseStudentDto>(result.Data);

            return CreatedAtAction("GetByIdEndPoint", new { getid = dto.id }, dto);

        }

        //UPDATES INFO BASED ON ID
        [HttpPatch("{id}")]
        [Authorize(Roles = "Admin")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UpdateStudentEndPoint([FromRoute] Guid id, [FromForm] UpdateRequestStudentDto inputdata)
        {
            //dto to domain model
            var dm = mapper.Map<Student_DM>(inputdata);

            //calling the update service method
            var result = await servicelayer.UpdateService(id, dm, inputdata.Image);

            //if update details are invalid return error
            if (!result.Success)
            {
                return ErrorHandler(result.ErrorType, result.Message);
            }

            //dm to dto
            var dto = mapper.Map<ResponseStudentDto>(result.Data);

            return Ok(dto);
        }




        //DELETES THE INFO BASED ON ID
        [HttpDelete("{deleteid}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteStudentEndPoint([FromRoute] Guid deleteid)
        {
            var result = await servicelayer.DeleteService(deleteid);

            if (!result.Success)
            {
                return ErrorHandler(result.ErrorType, result.Message);
            }

            return NoContent();
        }


        [HttpGet("me")]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> GetMe()
        {
            var result = await servicelayer.GetStudentProfile();

            if (!result.Success)
            {
                return ErrorHandler(result.ErrorType, result.Message);
            }

            var dto = mapper.Map<ResponseStudentDto>(result.Data);

            return Ok(dto);

        }

        [HttpPatch("UpdateMyProfile")]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> UpdateMyProfile([FromForm] UpdateRequestStudentDto input)
        {
            var domainmodel = mapper.Map<Student_DM>(input);

            var result = await servicelayer.UpdateStudentProfile(domainmodel, input.Image);

            if (!result.Success)
            {
               return ErrorHandler(result.ErrorType, result.Message);
            }

            var dto = mapper.Map<ResponseStudentDto>(result.Data);

            return Ok(dto);

        }


    }
}
