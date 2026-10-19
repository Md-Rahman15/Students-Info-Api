using Students_Info_Api.DTO;
using Students_Info_Api.Service.ImageServiceFolder;

namespace Students_Info_Api.Service.Interfaces
{
    public interface IImageService
    {
        public Task<ImageServiceResultClass> SaveImage(IFormFile? studentimage);
        public ImageServiceResultClass DeleteImage(string studentimage);
    }
}
