
using Microsoft.AspNetCore.Mvc;
using Students_Info_Api.Service;

namespace Students_Info_Api.Controllers
{
    public class BaseController : ControllerBase
    {
        protected IActionResult ErrorHandler(ErrorType errorType, string message) 
        {

            switch (errorType) 
            {
                case ErrorType.ValidationError:return BadRequest(message);
                case ErrorType.NotFound: return NotFound(message);
                case ErrorType.Unauthorized: return Unauthorized(message);
                case ErrorType.Conflict: return Conflict(message);
                case ErrorType.InternalError: return StatusCode(StatusCodes.Status500InternalServerError,message);                
                default: return StatusCode(StatusCodes.Status500InternalServerError); 
            }           
        }
    }
}
