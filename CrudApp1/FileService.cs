
namespace CrudApp1
{
    public class FileService
    {
        private readonly IWebHostEnvironment environment;

        public FileService(IWebHostEnvironment environment)
        {
            this.environment = environment;
        }
        public string UploadImage(IFormFile file)
        {

            var filePath = Path.Combine(environment.ContentRootPath, "Images",file.FileName,"_",DateTime.Now.ToShortDateString());
            using var fileStream = new FileStream(filePath, FileMode.Create);
            file.CopyTo(fileStream);
            return filePath;

        }
    }
}
