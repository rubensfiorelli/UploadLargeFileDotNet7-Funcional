using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;

namespace UploadLargeFileDotNet7_Funcional.Controllers
{
    [ApiController]
    public class FileUploadController : ControllerBase
    {
        private async Task<string> WriteFile(IFormFile file)
        {
            try
            {
                var extension = "." + file.FileName.Split('.')[file.FileName.Split('.').Length - 1];
                var filename = $"{Guid.NewGuid()}" + extension;
                var filepath = Path.Combine(Directory.GetCurrentDirectory(), "Upload\\Files");

                if (!Directory.Exists(filepath))
                {
                    Directory.CreateDirectory(filepath);
                }

                var exactpath = Path.Combine(Directory.GetCurrentDirectory(), "Upload\\Files", filename);
                using (var stream = new FileStream(exactpath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                return filename;


            }
            catch (Exception)
            {

                throw;
            }
        }

        [HttpPost]
        [Route("uploads")]

        public async Task<IActionResult> UploadFile(IFormFile file, CancellationToken ct)
        {
            try
            {
                var result = await WriteFile(file);

                return Ok(result);

            }
            catch (Exception)
            {

                throw;
            }
        }

        [HttpGet]
        [Route("downloads")]

        public async Task<IActionResult> Download(string filename)
        {
            var filepath = Path.Combine(Directory.GetCurrentDirectory(), "Upload\\Files", filename);

            var provider = new FileExtensionContentTypeProvider();
            if(!provider.TryGetContentType(filepath, out var contentType))
            {
                contentType = "application/octet-stream";
            }

            var bytes = await System.IO.File.ReadAllBytesAsync(filepath);
            return File(bytes, contentType, Path.GetFileName(filepath));
        }

    }
}
