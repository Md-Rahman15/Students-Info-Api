using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Students_Info_Api.AuthData.DM;
using Students_Info_Api.AuthData.DTO.Others;
using Students_Info_Api.AuthData.DTO.UserDtoFolder;
using Students_Info_Api.AuthData.DTO.UserRequestDtoFolder;
using Students_Info_Api.Service.Interfaces;

namespace Students_Info_Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : BaseController
    {
        private readonly IUserService userService;
        private readonly IMapper mapper;

        public AuthController(IUserService userService, IMapper mapper)
        {
            this.userService = userService;
            this.mapper = mapper;
        }

        [HttpPost]
        [HttpPost("Register")]

        public async Task<IActionResult> RegisterUser([FromBody] User_Register_Request_DTO register)
        {
            var user_dm = mapper.Map<User_DM>(register);

            var result = await userService.UserRegisterService(user_dm, register);

            if (!result.Success)
            {
                return ErrorHandler(result.errorType,result.Message);
            }

            var dto = mapper.Map<User_Register_Response_DTO>(result.Data);

            dto.Message = result.Message;

            return Ok(dto.Message);
        }


        [HttpPost]
        [HttpPost("Login")]
        public async Task<IActionResult> LoginUser([FromBody] User_Login_Request_DTO login)
        {
            var user_dm = mapper.Map<User_DM>(login);

            var result = await userService.LoginUserService(user_dm, login);

            if (!result.Success)
            {
                return ErrorHandler(result.errorType,result.Message);
            }

            var dto = mapper.Map<User_Login_Response_DTO>(result.Data);

            dto.Token = result.Token;

            return Ok(dto);

        }

        [HttpPatch]
        [HttpPatch("Password Change")]
        public async Task<IActionResult> PasswordChangeLoggedInUser([FromBody] Change_Password_Dto input)
        {
            var result = await userService.PasswordChangeService(input);

            if (!result.Success)
            {
                return ErrorHandler(result.errorType, result.Message);
            }

            return Ok(result.Message);

        }

        [HttpPost]
        [HttpPost("Forgot Password")]
        public async Task<IActionResult> ForgetPassword([FromBody] RequestPasswordResetDto email)
        {
            var result = await userService.ForgotPasswordService(email.Email);

            if (!result.Success)
            {
                return ErrorHandler(result.errorType,result.Message);
            }

            return Ok(result.Message);
        }

        [HttpPost]
        [HttpPost("Email Based Password Reset")]

        public async Task<IActionResult> EmailBasedPasswordReset([FromBody] EmailBasedResetPasswordDto input) 
        {

            var result = await userService.EmailBasedPasswordResetService(input);

            if (!result.Success)
            {
                return ErrorHandler(result.errorType, result.Message);
            }

            return Ok(result.Message);


        }

        [HttpPost]
        [HttpPost("UserAccountEmailChange")]
        public async Task<IActionResult> UserAccountEmailChange([FromBody] NewEmailInfo input) 
        {
            var result = await userService.UserAccountEmailChangeService(input);
            
            if (!result.Success)
            {
                return ErrorHandler(result.errorType, result.Message);
            }
            return Ok(result.Message);
        }

        [HttpPost]
        [HttpPost("EmailBasedEmailReset")]
        public async Task<IActionResult> EmailBasedEmailReset([FromBody]string token)
        {
            var result = await userService.UpdateUserNewEmailAddressService(token);

            if (!result.Success)
            {
                return ErrorHandler(result.errorType, result.Message);
            }

            return Ok(result.Message);

        }

        [HttpPost]
        [HttpPost("EmailTokenVerify")]
        public async Task<IActionResult> EmailTokenVerify([FromBody] string token)
        {
            var result = await userService.UserEmailVerifactionService(token);

            if (!result.Success)
            {
                return ErrorHandler(result.errorType, result.Message);
            }

            return Ok(result.Message);

        }
    }
}
