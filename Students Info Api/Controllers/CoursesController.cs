
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Students_Info_Api.Domain_Models;
using Students_Info_Api.DTO.CourseDtoFolder;
using Students_Info_Api.Service.Interfaces;


namespace Students_Info_Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class CoursesController : BaseController
    {

        private readonly IMapper mapper;
        private readonly ICourseService courseService;

        public CoursesController(IMapper mapper, ICourseService courseService)
        {

            this.mapper = mapper;

            this.courseService = courseService;
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateCourseEndpoint(Request_Course_Dto inputdata)
        {
            var domainmodel = mapper.Map<Course_DM>(inputdata);

            var result = await courseService.CourseCreateService(domainmodel);

            if (!result.Success)
            {
                return ErrorHandler(result.ErrorType, result.Message);
            }


            var dto = mapper.Map<Response_Course_Dto>(result.Data);

            return CreatedAtAction("CourseGetByIdEndpoint", new { getid = dto.id }, dto);

        }

        [HttpGet]
        [Authorize(Roles = "Student,Admin")]
        public async Task<IActionResult> CourseGetAllEndpoint()
        {

            var result = await courseService.CourseGetAllService();

            if (!result.Success)
            {
                return ErrorHandler(result.ErrorType, result.Message);
            }


            var dto = mapper.Map<List<Response_Course_Dto>>(result.Data);

            return Ok(dto);

        }


        [HttpPatch("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CourseUpdateEndpoint([FromRoute] Guid id, [FromBody] Request_Course_Dto inputdata)
        {

            var domainmodel = mapper.Map<Course_DM>(inputdata);

            var result = await courseService.CourseUpdateService(id, domainmodel);

            if (!result.Success)
            {
                return ErrorHandler(result.ErrorType, result.Message);
            }

            var dto = mapper.Map<Response_Course_Dto>(result.Data);

            return Ok(dto);

        }

        [HttpGet("{getid}")]
        [Authorize(Roles = "Admin,Student")]
        public async Task<IActionResult> CourseGetByIdEndpoint([FromRoute] Guid getid)
        {


            var result = await courseService.CourseGetByIdService(getid);

            if (!result.Success)
            {
                return ErrorHandler(result.ErrorType, result.Message);
            }

            var dto = mapper.Map<Response_Course_Dto>(result.Data);

            return Ok(dto);


        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CourseDeleteEndpoint(Guid id)
        {

            var result = await courseService.CourseDeleteService(id);

            if (!result.Success)
            {
                return ErrorHandler(result.ErrorType, result.Message);
            }

            var dto = mapper.Map<Response_Course_Dto>(result.Data);

            return NoContent();


        }


    }
}
