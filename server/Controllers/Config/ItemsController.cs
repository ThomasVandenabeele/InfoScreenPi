using System;
using System.Collections.Generic;
using System.IO;
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
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System.Diagnostics;

namespace InfoScreenPi.Controllers
{
    [Authorize(Policy = "AdminOnly")]
    [Route("Config/[controller]/[action]")]
    public class ItemsController : BaseController
    {
        public ItemsController(IDataService dataService, IHostingEnvironment hostEnvironment)
        : base(dataService, hostEnvironment){}

        [HttpPost]
        public ActionResult ChangeItemState(int id, bool state)
        {
            Item item = _data.GetSingle<Item>(id);
            if (item != null)
            {
                item.Active = state;
                _data.Edit(item);
                _data.Commit();
                return Success(state? $"Status van item '{ item.Title }' veranderd naar actief!" : $"Status van item '{ item.Title }' veranderd naar inactief!");
            }
            return Fail();
        }

        [HttpPost]
        public ActionResult ArchiveItem(int id, bool state)
        {
            Item item = _data.GetSingle<Item>(id);
            if (item != null)
            {
                ((IExpiring)item).Active = !state;
                ((IExpiring)item).Archieved = state;
                _data.Edit(item);
                _data.Commit();
                return Success(state? $"Item {item.Title} gearchiveerd" : $"Item '{item.Title}' terug geactiveerd");
            }
            return Fail();
        }

        [HttpGet]
        public ActionResult ItemsArchive()
        {
            return PartialView("~/Views/Config/Items/Archive.cshtml", _data.GetItemsNoRss(true).ToList());
        }

        [HttpGet]
        public ActionResult Table()
        {
            return PartialView("~/Views/Config/Items/Table.cshtml", _data.GetItemsNoRss(false).ToList());
        }

        [HttpGet]
        public ActionResult CreateItem()
        {
            List<Background> model = _data.GetBackgroundsNoRSS(true).Where(b => !b.Url.Equals("black.jpg")).ToList();
            ViewBag.DefaultDisplayTime = _data.GetSettingByName("DefaultDisplayTime");
            return PartialView("~/Views/Config/Items/CreateItem.cshtml", model);
        }

        [HttpPost]
        public ActionResult RegisterNewItem(string itemTitle, string itemContent, int bgId, string expireDateTime, int displayTime)
        {
            //ItemKind soort = _data.GetSingle<ItemKind>(ik => ik.Description == "CUSTOM");
            Background achtergrond = _data.GetSingle<Background>(bgId);
            _data.Add(
                new CustomItem
                {
                    Title = itemTitle,
                    Content = itemContent,
                    Background = achtergrond,
                    Active = true,
                    Archieved = false,
                    ExpireDateTime = DateTime.Parse(expireDateTime),
                    DisplayTime = displayTime
                }
            );
            _data.Commit();
            return Success();
        }

        [HttpGet]
        public ActionResult EditCustomItem(int id)
        {
            ViewBag.SelectionGrid = (List<Background>) _data.GetBackgroundsNoRSS(true).ToList();
            CustomItem model = _data.GetSingle<CustomItem>(id, i => i.Background);
            //if (model is IStatic) model = _data.GetSingle<Item>(id, i => ((IStatic)i).Background);
            return PartialView("~/Views/Config/Items/EditCustomItem.cshtml", model);
        }

        [HttpPost]
        public IActionResult EditItem(int itemId, string itemTitle, string itemContent, int bgId, string expireDateTime, int displayTime)
        {
            CustomItem item = _data.GetSingle<CustomItem>(itemId, i => i.Background);
            Background bg = _data.GetSingle<Background>(bgId);

            item.Title = itemTitle;
            item.Content = itemContent;
            item.Background = bg;
            item.ExpireDateTime = DateTime.Parse(expireDateTime);
            item.DisplayTime = displayTime;

            _data.Edit(item);
            _data.Commit();
            return Success("Item '" + itemTitle + "' gewijzigd");
        }


        [HttpGet]
        public ActionResult CreateVideoItem()
        {
            return PartialView("~/Views/Config/Items/CreateVideoItem.cshtml");
        }

        [HttpPost]
        [RequestSizeLimit(52428800*2)] // 100MB
        public async Task<IActionResult> UploadVideo(IFormFile video)
        {
            //IFormFile videof = video.First();
            if (video.Length > 0)
            {
                using (var stream = new FileStream(Path.GetTempFileName(), FileMode.Create))
                {
                    await video.CopyToAsync(stream);
                }
            }
            return Success();
        }

        [HttpPost]
        [RequestSizeLimit(52428800*2)] // 100MB
        public async Task<IActionResult> UploadVideoItem(string itemTitle, string expireDateTime, int displayTime, IFormFile video)
        {
            var videoRoot = Path.Combine(_hostEnvironment.WebRootPath, "videos");
            var videoRootTmp = Path.Combine(videoRoot, "tmp");

            string n = string.Format("vid-{0:yyyy-MM-dd_hh-mm-ss-tt}", DateTime.Now);
            var fileName = n + "-" + video.FileName.Replace(" ", "-");
            
            var fullFileName = Path.Combine(videoRoot, fileName);
            var fullFileNameTmp = Path.Combine(videoRootTmp, fileName);
            

            if (video.Length > 0)
            {
                using (var stream = new FileStream(fullFileNameTmp, FileMode.Create))
                {
                    await video.CopyToAsync(stream);
                }
            }

            // Converteer video naar lagere bitrate max 1Mbps
            string ffmpegCommand = "ffmpeg -i " + fullFileNameTmp + " -b:v 1M -maxrate 1.5M -bufsize 0.5M " + fullFileName;
            var outputFfmpeg = ffmpegCommand.Bash();

            if(fullFileNameTmp != null || fullFileNameTmp != string.Empty)
            {
                if(System.IO.File.Exists(fullFileNameTmp))
                {
                    System.IO.File.Delete(fullFileNameTmp);
                }
            }

            _data.Add(
                new VideoItem
                {
                    Title = itemTitle,
                    URL = fileName,
                    Active = true,
                    Archieved = false,
                    ExpireDateTime = DateTime.Parse(expireDateTime),
                    DisplayTime = displayTime
                }
            );
            _data.Commit();

            return Success("Nieuwe video succesvol toegevoegd.");
        }
    }


    // public static class ExtensionMethods
    // {
    //     public static string Bash(this string cmd)
    //     {
    //         var escapedArgs = cmd.Replace("\"", "\\\"");

    //         var process = new Process()
    //         {
    //             StartInfo = new ProcessStartInfo
    //             {
    //                 FileName = "/bin/bash",
    //                 Arguments = $"-c \"{escapedArgs}\"",
    //                 RedirectStandardOutput = true,
    //                 UseShellExecute = false,
    //                 CreateNoWindow = true,
    //             }
    //         };
    //         process.Start();
    //         string result = process.StandardOutput.ReadToEnd();
    //         process.WaitForExit();
    //         return result;
    //     }
    // }

}
