using Microsoft.EntityFrameworkCore;
using SurfsUp.Data;
using SurfsUp.Models;

namespace SurfsUp.Services
{
    public class ImageService
    {
        private readonly SurfsUpContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ImageService(SurfsUpContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task SaveImages(int boardId, IList<IFormFile> attachments)
        {
            string rootPath = _webHostEnvironment.WebRootPath.Replace('\\', '/');

            foreach (var formFile in attachments)
            {
                if (formFile.Length > 0)
                {
                    var filePath = Path.Combine(rootPath + "/images/");

                    if (!Directory.Exists(filePath))
                    {
                        Directory.CreateDirectory(filePath);
                    }

                    filePath = Path.Combine(rootPath + "/images/", formFile.FileName);

                    using (var stream = System.IO.File.Create(filePath))
                    {
                        await formFile.CopyToAsync(stream);
                    }

                    filePath = Path.Combine("/images/", formFile.FileName);

                    _context.Images.Add(new Image { BoardId = boardId, Path = filePath });
                }
            }

            await _context.SaveChangesAsync();
        }

        public async Task DeleteImagesAsync(int boardId)
        {
            string rootPath = _webHostEnvironment.WebRootPath.Replace('\\', '/');

            var images = await _context.Images.Where(x => x.BoardId == boardId).ToListAsync();

            foreach (var image in images)
            {
                _context.Images.Remove(image);
                var filePath = Path.Combine(rootPath + image.Path);

                if (File.Exists(filePath))
                {
                    if (!filePath.Contains("SeedImages"))
                    {
                        File.Delete(filePath);
                    }
                }
            }

            await _context.SaveChangesAsync();
        }

        public async Task DeleteImageAsync(int id)
        {
            string rootPath = _webHostEnvironment.WebRootPath.Replace('\\', '/');

            var image = await _context.Images.FindAsync(id);

            _context.Images.Remove(image);
            var filePath = Path.Combine(rootPath + image.Path);

            if (File.Exists(filePath))
            {
                if (!filePath.Contains("SeedImages"))
                {
                    File.Delete(filePath);
                }
            }

            await _context.SaveChangesAsync();
        }
    }

}
