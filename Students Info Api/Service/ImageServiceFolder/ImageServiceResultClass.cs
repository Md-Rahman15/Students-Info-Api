using Students_Info_Api.AuthData.DM;

namespace Students_Info_Api.Service.ImageServiceFolder
{
    public class ImageServiceResultClass
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public string? Data { get; set; }
        public ErrorType errorType { get; set; }

        

    }
}
