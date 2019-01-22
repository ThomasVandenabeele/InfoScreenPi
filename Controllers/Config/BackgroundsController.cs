using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using InfoScreenPi.Entities;
using InfoScreenPi.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using InfoScreenPi.ViewModels;
using InfoScreenPi.Infrastructure.Services;
using InfoScreenPi.Infrastructure.Core;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using Microsoft.Net.Http.Headers;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace InfoScreenPi.Controllers
{
    [Authorize(Policy = "AdminOnly")]
    [Route("Config/[controller]/[action]")]
    public class BackgroundsController : BaseController
    {
        public static readonly List<string> ImageExtensions = new List<string> { ".JPG", ".JPE", ".BMP", ".GIF", ".PNG" };

        const int size = 150;
        const int quality = 75;

        public BackgroundsController(IDataService dataService, IHostingEnvironment hostEnvironment)
        : base(dataService, hostEnvironment) {}

        [HttpGet]
        public IActionResult Grid(){
            return PartialView("~/Views/Config/Backgrounds/Grid.cshtml", _data.GetBackgroundsNoRSS(false).Where(b => !b.Url.Equals("black.jpg")).ToList());
        }

        [HttpGet]
        public IActionResult SelectionGrid(){
            return PartialView("~/Views/Config/Backgrounds/SelectionGrid.cshtml", _data.GetBackgroundsNoRSS(true).ToList());
        }
        [HttpPost]
        public async Task<ActionResult> FileUpload(IFormFile file)
        {
            // libc6-dev and libgdiplus installed ??
            var parsedContentDisposition = ContentDispositionHeaderValue.Parse(file.ContentDisposition);
            string filename = parsedContentDisposition.FileName.Trim().ToString(); //NAKIJKEN

            var imageRoot = Path.Combine(_hostEnvironment.WebRootPath, "images/backgrounds");
            var imageRootThumb = Path.Combine(_hostEnvironment.WebRootPath, "images/backgrounds/thumbnails");
            if (file.Length > 0)
            {
                filename = Guid.NewGuid().ToString() + "-" + filename.Split('\\').Last();

                await file.CopyToAsync(new FileStream(Path.Combine(imageRoot, filename), FileMode.Create));

                Background b = new Background() { Url = filename };
                _data.Add(b);
                _data.Commit();

                /* thumbs */
                MemoryStream ms = new MemoryStream();
                file.OpenReadStream().CopyTo(ms);

                System.Drawing.Image inputImage = System.Drawing.Image.FromStream(ms);
                Image thumb = GetReducedImage(inputImage);
                thumb.Save(Path.Combine(imageRootThumb, filename));

                return Success();
            }
            return Fail();
        }
        public IActionResult CreateThumbs(){
            string path = Path.Combine(_hostEnvironment.WebRootPath, "images");
            string[] dirs = Directory.GetDirectories(path);
            string[] files = Directory.GetFiles(dirs[0]);
            foreach (string dirFile in Directory.GetDirectories(path))
            {
                foreach (string fileName in Directory.GetFiles(dirFile ))
                {
                    string[] t = fileName.Split('/');
                    string name = t[t.Length-1];

                    // fileName  is the file name
                    string ext = Path.GetExtension(fileName).ToUpperInvariant();
                    if (ImageExtensions.Contains(ext))
                    {
                        Console.WriteLine(fileName);
                        Console.WriteLine(name);

                        MemoryStream ms = new MemoryStream();
                        using (FileStream stream = System.IO.File.Open(fileName, FileMode.Open))
                        {
                            stream.CopyTo(ms);
                        }

                        System.Drawing.Image inputImage = System.Drawing.Image.FromStream(ms);
                        Image thumb = GetReducedImage(inputImage);

                        var imageRootThumb = Path.Combine(_hostEnvironment.WebRootPath, "images/backgrounds/thumbnails");
                        thumb.Save(Path.Combine(imageRootThumb, name));
                    }
                }
            }
            return Success("Thumbnails geupdated!");
        }
        public Image GetReducedImage(Image img)
        {
            using (var image = new Bitmap(img))
            {
                int width, height;
                if (image.Width > image.Height)
                {
                    width = size;
                    height = Convert.ToInt32(image.Height * size / (double)image.Width);
                }
                else
                {
                    width = Convert.ToInt32(image.Width * size / (double)image.Height);
                    height = size;
                }

                Image ReducedImage;
                Image.GetThumbnailImageAbort callb = new Image.GetThumbnailImageAbort(()=>false);
                ReducedImage = image.GetThumbnailImage(width, height, callb, IntPtr.Zero);
                return ReducedImage;
            }
        }
        [HttpPost]
        public IActionResult Delete(int id)
        {
            var b = _data.GetSingle<Background>(id);

            if(!_data.GetAll<CustomItem>().Select(i => i.Background).Contains(b))
            {
                var imageRoot = Path.Combine(_hostEnvironment.WebRootPath, "images/backgrounds");
                FileInfo file = new FileInfo(imageRoot + "/" + b.Url);
                if (file.Exists) file.Delete();
                FileInfo thumb = new FileInfo(imageRoot + "/thumbnails/" + b.Url);
                if (thumb.Exists) thumb.Delete();

                _data.Delete(b);
                _data.Commit();
                return Success();
            }
            return Fail("Kan achtergrond niet verwijderen, deze is nog in gebruik.");
        }

        private byte[] ImageToByteArray(System.Drawing.Image imageIn)
        {
            using (var ms = new MemoryStream())
            {
                imageIn.Save(ms,System.Drawing.Imaging.ImageFormat.Gif);
                return  ms.ToArray();
            }
        }
    }
}
