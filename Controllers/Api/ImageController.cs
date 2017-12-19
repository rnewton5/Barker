using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Barker.Models;
using System.IO;
using System.Drawing;
using Barker.Data;
using Microsoft.AspNetCore.Identity;
using System.Net;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Barker.Controllers.Api
{
    public class ImageController : Controller
    {
        private readonly BarkerDbContext _context;
        private readonly UserManager<User> _userManager;

        public ImageController(BarkerDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpPost]
        public JsonResult UploadImage(IList<IFormFile> files)
        {
            try
            {
                IFormFile uploadedImage = files.FirstOrDefault();
                if (uploadedImage == null || uploadedImage.ContentType.ToLower().StartsWith("image/"))
                {
                    MemoryStream ms = new MemoryStream();
                    uploadedImage.OpenReadStream().CopyTo(ms);

                    System.Drawing.Image image = System.Drawing.Image.FromStream(ms);
                    var imageId = Guid.NewGuid();
                    var user = _userManager.GetUserAsync(User).Result;
                    Models.Image imageEntity = new Models.Image()
                    {
                        Id = imageId,
                        Name = uploadedImage.Name,
                        Data = ms.ToArray(),
                        Width = image.Width,
                        Height = image.Height,
                        ContentType = uploadedImage.ContentType,
                        User = user
                    };

                    _context.Images.Add(imageEntity);
                    _context.SaveChanges();
                }

                Response.StatusCode = (int)HttpStatusCode.Created;
                return Json(new { Message = "Image Uploaded!!" });
            }
            catch (Exception e)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json(new { Message = e.Message });
            }
        }

        [HttpGet]
        public FileStreamResult ViewImage(Guid id)
        {
            Models.Image image = _context.Images.FirstOrDefault(m => m.Id == id);

            MemoryStream ms = new MemoryStream(image.Data);

            return new FileStreamResult(ms, image.ContentType);
        }
    }
}
