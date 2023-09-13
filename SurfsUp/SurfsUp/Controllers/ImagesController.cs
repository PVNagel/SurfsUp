using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Differencing;
using Microsoft.EntityFrameworkCore;
using SurfsUp.Data;
using SurfsUp.Services;

namespace SurfsUp.Controllers
{
    public class ImagesController : Controller
    {
        private readonly ImageService _imageService;
        public ImagesController(ImageService imageService) 
        {
            _imageService = imageService;
        }

        //POST: Images/Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        // bliver kaldt fra Boards Edit View, hvis man sletter et enkelt billede.
        public async Task<IActionResult> Delete(int id, int boardId)
        {
            try
            {
                await _imageService.DeleteImageAsync(id);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            return RedirectToAction(nameof(BoardsController.Edit), "Boards", new { id = boardId });
        }
    }
}
