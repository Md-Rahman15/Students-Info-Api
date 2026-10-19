using Students_Info_Api.DTO;
using Students_Info_Api.Service.Interfaces;

namespace Students_Info_Api.Service.ImageServiceFolder
{
    public class ImageService : IImageService
    {
        private readonly IWebHostEnvironment webHostEnvironment;
        private readonly IHttpContextAccessor httpClient;
        private readonly ILogger<ImageService> logger;

        public ImageServiceResultClass Helper(string? data, bool resBool, string? msg,ErrorType errorType)
        {
            if (resBool == false)
            {
                ImageServiceResultClass resultFailure = new ImageServiceResultClass
                {

                    Success = false,
                    Message = msg,
                    Data = default,
                    errorType = errorType

                };
                return resultFailure;
            }
                ImageServiceResultClass resultSuccess = new ImageServiceResultClass
                {
                    Success = true,
                    Data = data,
                    Message = msg,
                    errorType = errorType
                };
                return resultSuccess; 
        }

        public ImageService(IWebHostEnvironment webHostEnvironment, IHttpContextAccessor httpClient,ILogger<ImageService> logger)
        {
            this.webHostEnvironment = webHostEnvironment;
            this.httpClient = httpClient;
            this.logger = logger;
        }
        public async Task<ImageServiceResultClass> SaveImage(IFormFile? studentimage)
        { 
           //how to know if update controller run and how return back result without messing with old photo

            if (studentimage == null || studentimage.Length == 0)
            {
                return Helper(default, false, "Invalid image size",ErrorType.ValidationError);
            }

            if (studentimage.Length > 5 * 1024 * 1024)// 5MB CHECK
            {
                return Helper(default, false, "Image Size should less 5 megabytes", ErrorType.ValidationError);
            }

            if (!Path.GetExtension(studentimage.FileName).Equals(".jpg", StringComparison.OrdinalIgnoreCase) && !Path.GetExtension(studentimage.FileName).Equals(".png", StringComparison.OrdinalIgnoreCase) && !Path.GetExtension(studentimage.FileName).Equals(".jpeg", StringComparison.OrdinalIgnoreCase))
            {
                return Helper(default, false, "Invalid image extension please upload only jpg , png ,jpeg only ", ErrorType.ValidationError);
            }

            var guid = Guid.NewGuid();

            var file = guid + Path.GetExtension(studentimage.FileName);

            var filepath = Path.Combine(webHostEnvironment.WebRootPath, "Uploads", "Students", file);

            var folderpath = Path.Combine(webHostEnvironment.WebRootPath, "Uploads", "Students");

            if (Directory.Exists(folderpath) == false)
            {
                Directory.CreateDirectory(folderpath);
            }

            using FileStream stream = File.Create(filepath);

            await studentimage.CopyToAsync(stream);

            var request = httpClient.HttpContext?.Request;

            string url = request == null
            ? $"/Uploads/Students/{file}"
            : $"{request.Scheme}://{request.Host}/Uploads/Students/{file}";
           
            return Helper(url,true,null, ErrorType.None);
        }

        public ImageServiceResultClass DeleteImage(string studenturl)
        {
     
            //get file name
            var filename = Path.GetFileName(studenturl);

            //directory path of file
            var Directorypath = Path.Combine(webHostEnvironment.WebRootPath, "Uploads", "Students");

            //get the folder path
            var filePath = Path.Combine(webHostEnvironment.WebRootPath, "Uploads", "Students",filename);

            //if directory doest not exits return error
            if (!Directory.Exists(Directorypath)) 
            {
                return Helper(null,false,"Directory does not exists", ErrorType.NotFound);
            }
            //if file doest not exits return error
            if (!File.Exists(filePath)) 
            {
                return Helper(null, false, "file does not exists", ErrorType.NotFound);
            }

            try
            {
                //else delete the file
                File.Delete(filePath);

                //return the filepath and success
                return Helper(filePath, true, "File has been successfully deleted", ErrorType.None);
            }
            catch (Exception ex)
            {
                //file not deleted log execption
                logger.LogError(ex,ex.Message);
                //return the failure
                return Helper(null, false, "file can't be deleted", ErrorType.InternalError);
            }
   
           

        }
    }
}