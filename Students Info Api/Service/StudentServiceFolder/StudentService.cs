
using System.Security.Claims;
using Student_Info_Api.Domain_Models;
using Students_Info_Api.Repository.Interfaces;
using Students_Info_Api.Service.Interfaces;

namespace Students_Info_Api.Service.StudentService
{
    public class StudentService : IStudentService
    {
        private readonly IStudentPoints repoStudentsPoints;
        private readonly ICoursePoints coursePoints;
        private readonly IImageService imageService;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly ILogger<StudentService> logger;

        public StudentService(IStudentPoints repoStudentsPoints, ICoursePoints coursePoints, IImageService imageService, IHttpContextAccessor httpContextAccessor, ILogger<StudentService> logger)
        {
            this.repoStudentsPoints = repoStudentsPoints;
            this.coursePoints = coursePoints;
            this.imageService = imageService;
            this.httpContextAccessor = httpContextAccessor;
            this.logger = logger;
        }

        public StudentsResultClass<T> Helper<T>(T? data, bool resBool, string? msg ,ErrorType errorType)
        {
            if (resBool == false)
            {
                StudentsResultClass<T> resultFailure = new StudentsResultClass<T>
                {
                    Success = false,
                    Message = msg,
                    Data = default,
                    ErrorType = errorType
                };
                return resultFailure;
            }

              StudentsResultClass<T> resultSuccess = new StudentsResultClass<T>
              {
                Success = true,
                Data = data,
                Message = msg,
                ErrorType = ErrorType.None
              };
               return resultSuccess;
        }

        public async Task<StudentsResultClass<Student_DM>> CreateService(Student_DM input, IFormFile image)
        {
            if (image == null) 
            {
                return Helper<Student_DM>(null, false, "Please upload valid image", ErrorType.ValidationError);
            }

            //get email info
            var emailinfo = await repoStudentsPoints.GetEmailRepo(input.Email);

            //get the mobile info
            var mobileinfo = await repoStudentsPoints.GetPhoneNumberRepo(input.PhoneNumber);

            //incorrect email
            if (emailinfo != null)
            {
                return Helper<Student_DM>(null, false, "Email Already Exists", ErrorType.Conflict);
            }

            //incorrect mobile number
            if (mobileinfo != null)
            {
                return Helper<Student_DM>(null, false, "Phone Number Already Exists", ErrorType.Conflict);
            }


            //course validaiton and assiging course id based on name
            var coursevalidation = await coursePoints.CourseGetNameRepo(input.CourseName);

            //incorrect course id
            if (coursevalidation == null)
            {
                //returns the error
                return Helper<Student_DM>(null, false, "Invalid Course Id", ErrorType.ValidationError);
            }

            //assing the course id to dm data
            input.CourseId = coursevalidation.id;           

            //check if user does not exits then
            if (input.UserId == null)
            {
                //get guid of user
                var getUserGuid = GetUserGuidFromJwtClaim();

                if (!getUserGuid.Success)
                {
                    return Helper<Student_DM>(null, false, getUserGuid.Message, getUserGuid.ErrorType);
                }

                //assing guid of user to dm data
                input.UserId = getUserGuid.Data;

                var useridcheck = await repoStudentsPoints.UserIdCheck(input);

                if (useridcheck != null)
                {
                    return Helper<Student_DM>(null, false, "Student Profile Already Exists", ErrorType.Conflict);
                }

            }

            //call image save method which saves image 

            var imagesave = await imageService.SaveImage(image);

            if (!imagesave.Success)
            {
                return Helper<Student_DM>(null, false, "Image could not be saved", imagesave.errorType);
            }

            //assign img url in domain model data

            input.ImageUrl = imagesave.Data;

            //assign couse name
            input.CourseName = coursevalidation.Name;

            //it call repo method to save the data
            var result = await repoStudentsPoints.CreateRepo(input);

            if (result == null && image != null)
            {
                if (input.ImageUrl != null)
                {
                    var imgdelete =  imageService.DeleteImage(input.ImageUrl);
                    if (!imgdelete.Success) 
                    {
                        logger.LogError(imgdelete.Message,imgdelete);               
                    }
                }            
            }

            //handles if info is invalid
            if (result == null)
            {
                return Helper(result, false, "Incorrect information", ErrorType.ValidationError);
            }

            //successfully data handled
            return Helper(result, true, null, ErrorType.None);

        }

        public async Task<StudentsResultClass<Student_DM>> DeleteService(Guid id)
        {
            var student = await repoStudentsPoints.GetByIdRepo(id);

            if (student == null)
            {
                return Helper(student, false, "Invalid Id", ErrorType.NotFound);
            }

            var existingimageurl = student.ImageUrl;

            //runs delete id repo code
            var result = await repoStudentsPoints.DeleteRepo(id);

            //invalid id check
            if (result == null)
            {
                return Helper(result, false, "Invalid Id", ErrorType.NotFound);
            }

            var imagedelte = imageService.DeleteImage(existingimageurl);

            if (!imagedelte.Success)
            {
                logger.LogError(imagedelte.Message);
            }

            //operation completed successfully
            return Helper(result, true, null, ErrorType.None);

        }

        public async Task<StudentsResultClass<List<Student_DM>>> GetAllService(string? SortBy, bool? SortAscending,
            string? Gender, int? PageNumber, int? PageSize, string? search , bool? byFirstName)
        {
            if (PageNumber <= 0) 
            {
                return Helper<List<Student_DM>>(null, false, "Invalid page number",ErrorType.ValidationError);
            }
            if (PageSize <= 0 || PageSize > 100) 
            {
                return Helper<List<Student_DM>>(null, false, "Invalid page size", ErrorType.ValidationError);
            }

            //run the getall repo code
            var result = await repoStudentsPoints.GetAllRepo(SortBy, SortAscending, Gender, PageNumber, PageSize, search,byFirstName);

            //handle if not result found
            if (result == null)
            {
                return Helper<List<Student_DM>>(null, false, "Result not found", ErrorType.NotFound);
            }

            //opeartion completed successfully
            return Helper(result, true, null, ErrorType.None);
        }

        public async Task<StudentsResultClass<Student_DM>> GetByIdService(Guid id)
        {
            //runs thes getbyid repo code
            var result = await repoStudentsPoints.GetByIdRepo(id);

            //hanlde if id is invalid
            if (result == null)
            {
                return Helper(result, false, "Invalid Id", ErrorType.NotFound);
            }

            //opeartion completed successfully
            return Helper(result, true, null,ErrorType.None);
        }

        public async Task<StudentsResultClass<Student_DM>> UpdateService(Guid id, Student_DM input, IFormFile? image)
        {
            //gets email from the db
            var emailinfo = await repoStudentsPoints.GetEmailRepo(input.Email);

            //gets the mobile number from db
            var mobileinfo = await repoStudentsPoints.GetPhoneNumberRepo(input.PhoneNumber);

            var student = await repoStudentsPoints.GetByIdRepo(id);

            //incorrect email
            if (emailinfo != null && emailinfo.id != id)
            {
                return Helper<Student_DM>(null, false, "Email Already Exists", ErrorType.Conflict);
            }

            //incorrect mobile number
            if (mobileinfo != null && mobileinfo.id != id)
            {
                return Helper<Student_DM>(null, false, "Phone Number Already Exists", ErrorType.Conflict);
            }

            if (student == null)
            {
                return Helper<Student_DM>(null, false, "Student id does not exists", ErrorType.NotFound);
            }

            //gets the courseid from db
            var coursevalidation = await coursePoints.CourseGetNameRepo(input.CourseName);

            //incorrect courseid
            if (coursevalidation == null)
            {
                //return resultError;
                return Helper<Student_DM>(null, false, "Invalid Course Id", ErrorType.ValidationError);
            }

            input.CourseId = coursevalidation.id;

            //url of an existingimage
            var existingimageurldelete = student.ImageUrl;
            //url of an updateimage
            string? updateimageurl = null;
            if (image == null)
            {
                input.ImageUrl = existingimageurldelete;
            }
            else
            {
                //saving the new image
                var updateimage = await imageService.SaveImage(image);
                //returns the error messae
                if (!updateimage.Success)
                {
                    return Helper<Student_DM>(null, false, updateimage.Message, ErrorType.InternalError);
                }
                //updates the new url in imageurl
                input.ImageUrl = updateimage.Data;
                updateimageurl = updateimage.Data;
            }

       

            //runs the update repo code 
            var result = await repoStudentsPoints.UpdateRepo(id, input);

            if (result == null && image != null)
            {
                var newImgDelte = imageService.DeleteImage(updateimageurl);
                if (!newImgDelte.Success)
                {
                    logger.LogError(newImgDelte.Message);
                }
            }

            //handles if info is invalid
            if (result == null)
            {
                return Helper(result, false, "Incorrect information", ErrorType.ValidationError);
            }

            if (result != null && image != null)
            {
                //assign course name 
                input.CourseName = coursevalidation.Name;

                var oldImgDelte = imageService.DeleteImage(existingimageurldelete);

                if (!oldImgDelte.Success)
                {
                    logger.LogError(oldImgDelte.Message,oldImgDelte);
                }

            }

            //opeartion completed successfully
            return Helper(result, true, null, ErrorType.None);
        }

        public async Task<StudentsResultClass<Student_DM>> GetStudentProfile()
        {
            var getUserGuid = GetUserGuidFromJwtClaim();

            if (!getUserGuid.Success)
            {
                return Helper<Student_DM>(null, false, getUserGuid.Message,getUserGuid.ErrorType);
            }
            var getuser = await repoStudentsPoints.GetUserId(getUserGuid.Data);

            if (getuser == null)
            {
                return Helper<Student_DM>(null, false, "Student id not found", ErrorType.NotFound);
            }

            return Helper<Student_DM>(getuser, true, null, ErrorType.None);
        }


        public async Task<StudentsResultClass<Student_DM>> UpdateStudentProfile(Student_DM input, IFormFile? image)
        {
            var getUserGuid = GetUserGuidFromJwtClaim();

            if (!getUserGuid.Success)
            {
                return Helper<Student_DM>(null, false, getUserGuid.Message,getUserGuid.ErrorType);
            }

            var getguid = await repoStudentsPoints.GetUserId(getUserGuid.Data);

            if (getguid == null)
            {
                return Helper<Student_DM>(null, false, "Id not found", ErrorType.NotFound);
            }

            var final = await UpdateService(getguid.id, input, image);

            if (!final.Success)
            {
                return Helper<Student_DM>(null, false, "could not update info try again some time later", final.ErrorType);
            }

            return Helper<Student_DM>(final.Data, true, null, ErrorType.None);
        }

        public StudentsResultClass<Guid> GetUserGuidFromJwtClaim()
        {
            if (httpContextAccessor.HttpContext == null)
            {
                return Helper<Guid>(default, false, "HttpContext Missing", ErrorType.Unauthorized);
            }

            //getting user claim info
            var userIdentification = httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier);

            if (userIdentification == null)
            {
                return Helper<Guid>(default, false, "User Not Found. Please login to continue", ErrorType.Unauthorized);
            }

            //using the claiminfo and converting it info to user guid userid

            var guidConversion = Guid.TryParse(userIdentification.Value, out Guid userId);

            //if guid conversion fails send error
            if (!guidConversion)
            {
                return Helper<Guid>(default, false, "Guid conversion fails", ErrorType.InternalError);
            }

            return Helper<Guid>(userId, true, "Guid conversion success",ErrorType.None);

        }




    }
}
