using Microsoft.EntityFrameworkCore;
using SurfsUpAPI.Data;
using SurfsUpClassLibrary.Models;

namespace SurfsUpAPI.Services
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

        // Gemmer nye billeder i wwwroot/images og tilføjer et nyt Image objekt til databasen
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
                        // tilføjer billeder til wwwroot
                        await formFile.CopyToAsync(stream);
                    }

                    filePath = Path.Combine("/images/", formFile.FileName);

                    // tilføjer image objekt til databasen
                    _context.Images.Add(new Image { BoardId = boardId, Path = filePath });
                }
            }

            await _context.SaveChangesAsync();
        }

        // hvis vi fjerner et board, bliver den her metode kaldt fra BoardsControllerens DeleteConfirmed metode.
        // den fjerner alle billeder som tilhørte det board.
        public async Task DeleteImagesAsync(int boardId)
        {
            string rootPath = _webHostEnvironment.WebRootPath.Replace('\\', '/');

            // henter alle billeder med specifikt boardId fra databasen
            // og laver IQueryable til List.
            var images = await _context.Images.Where(x => x.BoardId == boardId).ToListAsync();

            // fjerner alle billeder fra listen
            foreach (var image in images)
            {
                // fjerner fra databasen
                _context.Images.Remove(image);

                var filePath = Path.Combine(rootPath + image.Path);

                if (File.Exists(filePath))
                {
                    // hvis billederne er i SeedImages, så fjerner den dem ik.
                    if (!filePath.Contains("SeedImages"))
                    {
                        // fjerner fra wwwroot/images
                        File.Delete(filePath);
                    }
                }
            }

            await _context.SaveChangesAsync();
        }

        // hvis vi fjerner et enkelt billede i Edit Boards, bliver den her kaldt ImagesControlleren
        public async Task DeleteImageAsync(int id)
        {
            string rootPath = _webHostEnvironment.WebRootPath.Replace('\\', '/');

            var image = await _context.Images.FindAsync(id);

            // fjerner fra databasen
            _context.Images.Remove(image);
            var filePath = Path.Combine(rootPath + image.Path);

            if (File.Exists(filePath))
            {
                if (!filePath.Contains("SeedImages"))
                {
                    // fjerner fra wwwroot/images
                    File.Delete(filePath);
                }
            }

            await _context.SaveChangesAsync();
        }
    }

}
