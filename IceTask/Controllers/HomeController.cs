using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Linq;

namespace IceTask.Controllers
{
    public class HomeController : Controller
    {
        private readonly string uploadDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult UploadImage()
        {
            try
            {
                var file = Request.Form.Files[0];
                if (file.Length > 0)
                {
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    var filePath = Path.Combine(uploadDirectory, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        file.CopyTo(stream);
                    }

                    TempData["Message"] = "File uploaded successfully.";
                    TempData["ImagePath"] = "/uploads/" + fileName;
                }
                else
                {
                    TempData["Error"] = "No file selected.";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error uploading file: {ex.Message}";
            }

            return RedirectToAction("Index");
        }

        public IActionResult Gallery()
        {
            var files = Directory.GetFiles(uploadDirectory)
                                 .Select(filePath => Path.GetFileName(filePath))
                                 .ToList();
            return View(files);
        }

        [HttpPost]
        public IActionResult DeleteFile(string fileName)
        {
            var filePath = Path.Combine(uploadDirectory, fileName);
            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
                TempData["Message"] = "File deleted successfully.";
            }
            else
            {
                TempData["Error"] = "File not found.";
            }

            return RedirectToAction("Gallery");
        }
    }
}
