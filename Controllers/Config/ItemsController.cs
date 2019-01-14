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
                return Success(state? "Item status verandert naar actief!" : "Item status verandert naar inactief!");
            }
            return Fail();
        }

        [HttpPost]
        public ActionResult ArchiveItem(int id, bool state)
        {
            Item item = _data.GetSingle<Item>(id);
            if (item != null)
            {
                item.Archieved = state;
                _data.Edit(item);
                _data.Commit();
                return Success(state? "Item verwijderd" : "Item terug geactiveerd");
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
            ItemKind soort = _data.GetSingle<ItemKind>(ik => ik.Description == "CUSTOM");
            Background achtergrond = _data.GetSingle<Background>(bgId);
            _data.Add(
                new Item
                {
                    Soort = soort,
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
        public ActionResult Edit(int id)
        {
            ViewBag.SelectionGrid = (List<Background>) _data.GetBackgroundsNoRSS(true).ToList();
            Item model = _data.GetSingle<Item>(id, i => i.Background);
            return PartialView("~/Views/Config/Items/EditItem.cshtml", model);
        }

        [HttpPost]
        public IActionResult EditItem(int itemId, string itemTitle, string itemContent, int bgId, string expireDateTime, int displayTime)
        {
            Item item = _data.GetSingle<Item>(itemId, i => i.Background);
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

            ItemKind soort = _data.GetSingle<ItemKind>(ik => ik.Description == "VIDEO");
            Background achtergrond = _data.GetSingle<Background>(b => b.Url.Equals("black.jpg"));

            var videoRoot = Path.Combine(_hostEnvironment.WebRootPath, "videos");
            string n = string.Format("vid-{0:yyyy-MM-dd_hh-mm-ss-tt}", DateTime.Now);
            var fileName = n + "-" + video.FileName.Replace(" ", "-");
            var fullFileName = Path.Combine(videoRoot, fileName);
            if (video.Length > 0)
            {
                using (var stream = new FileStream(fullFileName, FileMode.Create))
                {
                    await video.CopyToAsync(stream);
                }
            }

            _data.Add(
                new Item
                {
                    Soort = soort,
                    Title = itemTitle,
                    Content = fileName,
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
    }
}
