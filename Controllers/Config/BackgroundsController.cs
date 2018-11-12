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
using InfoScreenPi.Infrastructure.Repositories;
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
    public class BackgroundsController : Controller
    {
        private InfoScreenContext _context;
        private readonly IMembershipService _membershipService;
        private readonly IUserRepository _userRepository;
        private readonly ILoggingRepository _loggingRepository;
        private readonly IItemRepository _itemRepository;
        private readonly IHostingEnvironment _hostEnvironment;
        private readonly IBackgroundRepository _backgroundRepository;

        const int size = 150;
        const int quality = 75;

        public BackgroundsController(InfoScreenContext context, 
                                IMembershipService membershipService,
                                IUserRepository userRepository,
                                ILoggingRepository _errorRepository,
                                IItemRepository itemRepository,
                                IHostingEnvironment hostEnvironment,
                                IBackgroundRepository backgroundRepository)
        {
            _context = context;
            _membershipService = membershipService;
            _userRepository = userRepository;
            _loggingRepository = _errorRepository;
            _itemRepository = itemRepository;
            _hostEnvironment = hostEnvironment;
            _backgroundRepository = backgroundRepository;
        }

        [HttpGet]
        public IActionResult Grid(){
            return PartialView("~/Views/Config/Backgrounds/Grid.cshtml", _backgroundRepository.GetAllWithoutRSS(false).ToList());
        }

        [HttpGet]
        public IActionResult SelectionGrid(){
            return PartialView("~/Views/Config/Backgrounds/SelectionGrid.cshtml", _backgroundRepository.GetAllWithoutRSS(true).ToList());
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
                    if (filename.ToString().Contains("\\"))
                    {
                        filename = Guid.NewGuid().ToString() + "-" + filename.Split('\\').Last();
                    }
                    filename = Guid.NewGuid().ToString() + "-" + filename;

                    await file.CopyToAsync(new FileStream(Path.Combine(imageRoot, filename), FileMode.Create));

                    Background b = new Background() { Url = filename };
                    _backgroundRepository.Add(b);
                    _backgroundRepository.Commit();

                    /* thumbs */
                    

                    MemoryStream ms = new MemoryStream(); 
                    file.OpenReadStream().CopyTo(ms); 
                    
 
                    System.Drawing.Image inputImage = System.Drawing.Image.FromStream(ms); 
                    Image thumb = GetReducedImage(inputImage);
                    thumb.Save(Path.Combine(imageRootThumb, filename));
                    

                    return Json(new { success = true, message = ("Nieuwe achtergrond geupload") });
                }

                return Json(new { success = false, message = ("Er liep iets mis") });
        }

        public byte[] ImageToByteArray(System.Drawing.Image imageIn)
        {
            using (var ms = new MemoryStream())
            {
                imageIn.Save(ms,System.Drawing.Imaging.ImageFormat.Gif);
                return  ms.ToArray();
            }
        }
        public static readonly List<string> ImageExtensions = new List<string> { ".JPG", ".JPE", ".BMP", ".GIF", ".PNG" };

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

            return Json(new { success = true, message = ("Thumbnails geupdated!") });
            
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
                //Image resized = new Bitmap(width, height);

                Image ReducedImage;

                Image.GetThumbnailImageAbort callb = new Image.GetThumbnailImageAbort(ThumbnailCallback);

                ReducedImage = image.GetThumbnailImage(width, height, callb, IntPtr.Zero);

                return ReducedImage;
            
            }
            
        }
        
        public bool ThumbnailCallback()
        {
            return false;
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            Background b = _backgroundRepository.GetSingle(id);
           List<Background> aantal =  _backgroundRepository.GetAll().ToList();
            
            
            if(!_itemRepository.GetAllCustomItems().Select(i => i.Background).ToList().Contains(b))
            {
                var imageRoot = Path.Combine(_hostEnvironment.WebRootPath, "images/backgrounds");
                FileInfo file = new FileInfo(imageRoot + "/" + b.Url);
                if (file.Exists) file.Delete();
                FileInfo thumb = new FileInfo(imageRoot + "/thumbnails/" + b.Url);
                if (thumb.Exists) thumb.Delete();
                
                _backgroundRepository.Delete(b);
                _backgroundRepository.Commit();
                return Json(new { success = true, message = ("Achtergrond verwijderd") });
            }
            return Json(new { success = false, message = ("Kan achtergrond niet verwijderen, deze is nog in gebruik.") });
            
        }


            
        

    }
}
