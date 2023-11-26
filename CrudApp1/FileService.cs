
namespace CrudApp1
{
    public class FileService
    {
        private readonly IWebHostEnvironment _webHostEnvironment;

        public FileService(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        public string UploadImage(IFormFile file)
        {
            //var rootPath = _webHostEnvironment.ContentRootPath;

            var relativePath = "/Images/";
            var folderpath = Path.Combine(_webHostEnvironment.ContentRootPath,"wwwroot", "Images");
            //var folderpath = Path.Combine(@"wwwroot", "Images");

            if (!Directory.Exists(folderpath))
            {
                Directory.CreateDirectory(folderpath);
            }
            //DateTime.Now.ToString("dd-MM-yyyy") + "_" +

            var uniqueFileName =  Path.GetFileName(file.FileName) ;
            var filePath = Path.Combine(folderpath, uniqueFileName);

            using var fileStream = new FileStream(filePath, FileMode.Create);
            file.CopyTo(fileStream);

            relativePath += uniqueFileName;

            return relativePath;

        }
    }
}
