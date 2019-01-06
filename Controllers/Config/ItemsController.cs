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
using InfoScreenPi.Infrastructure.Repositories;
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
        private readonly IItemRepository _itemRepository;
        private readonly IHostingEnvironment _hostEnvironment;
        private readonly IBackgroundRepository _backgroundRepository;
        private readonly IItemKindRepository _itemKindRepository;

        public ItemsController(IItemRepository itemRepository,
                               IHostingEnvironment hostEnvironment,
                               IBackgroundRepository backgroundRepository,
                               IItemKindRepository itemKindRepository)
        {
            _itemRepository = itemRepository;
            _hostEnvironment = hostEnvironment;
            _backgroundRepository = backgroundRepository;
            _itemKindRepository = itemKindRepository;
        }

        [HttpPost]
        public ActionResult ChangeItemState(int id, bool state)
        {
            Item item = _itemRepository.GetSingle(id);
            if (item != null)
            {
                item.Active = state;
                _itemRepository.Edit(item);
                _itemRepository.Commit();
                return Success(state? "Item status verandert naar actief!" : "Item status verandert naar inactief!");
            }
            return Fail();
        }

        [HttpPost]
        public ActionResult ArchiveItem(int id, bool state)
        {
            Item item = _itemRepository.GetSingle(id);
            if (item != null)
            {
                item.Archieved = state;
                _itemRepository.Edit(item);
                _itemRepository.Commit();
                return Success(state? "Item verwijderd" : "Item terug geactiveerd");
            }
            return Fail();
        }

        [HttpGet]
        public ActionResult ItemsArchive()
        {
            List<Item> model = _itemRepository.GetAll(a => a.Background, a => a.Soort).Where(i => i.Soort.Description != "RSS" && i.Archieved == true).ToList();
            return PartialView("~/Views/Config/Items/Archive.cshtml", model);
        }

        [HttpGet]
        public ActionResult Table()
        {
            List<Item> model = _itemRepository.GetAll(a => a.Background, a => a.Soort).Where(i => i.Soort.Description != "RSS" && i.Archieved == false).ToList();
            return PartialView("~/Views/Config/Items/Table.cshtml", model);
        }

        [HttpGet]
        public ActionResult CreateItem()
        {
            List<Background> model = _backgroundRepository.GetAllWithoutRSS(true).Where(b => !b.Url.Equals("black.jpg")).ToList();
            return PartialView("~/Views/Config/Items/CreateItem.cshtml", model);
        }

        [HttpPost]
        public ActionResult RegisterNewItem(string itemTitle, string itemContent, int bgId, string expireDateTime)
        {
            ItemKind soort = _itemKindRepository.GetAll().Where(ik => ik.Description == "CUSTOM").First();
            Background achtergrond = _backgroundRepository.GetSingle(bgId);
            _itemRepository.Add(
                new Item
                {
                    Soort = soort,
                    Title = itemTitle,
                    Content = itemContent,
                    Background = achtergrond,
                    Active = true,
                    Archieved = false,
                    ExpireDateTime = DateTime.Parse(expireDateTime)
                }
            );
            _itemRepository.Commit();
            return Success();
        }

        [HttpGet]
        public ActionResult Edit(int id)
        {
            ViewBag.SelectionGrid = (List<Background>) _backgroundRepository.GetAllWithoutRSS(true).ToList();
            Item model = _itemRepository.GetAll(i => i.Background).Where(i => i.Id == id).First();
            return PartialView("~/Views/Config/Items/EditItem.cshtml", model);
        }

        [HttpPost]
        public IActionResult EditItem(int itemId, string itemTitle, string itemContent, int bgId)
        {
            Item item = _itemRepository.GetAll(i => i.Background).Where(i => i.Id == itemId).First();
            Background bg = _backgroundRepository.GetAll().First(b => b.Id == bgId);

            item.Title = itemTitle;
            item.Content = itemContent;
            item.Background = bg;

            _itemRepository.Edit(item);
            _itemRepository.Commit();
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
        public async Task<IActionResult> UploadVideoItem(string itemTitle, string expireDateTime, IFormFile video)
        {

            ItemKind soort = _itemKindRepository.GetAll().Where(ik => ik.Description == "VIDEO").First();
            Background achtergrond = _backgroundRepository.GetAll().First(b => b.Url.Equals("black.jpg"));

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

            _itemRepository.Add(
                new Item
                {
                    Soort = soort,
                    Title = itemTitle,
                    Content = fileName,
                    Background = achtergrond,
                    Active = true,
                    Archieved = false,
                    ExpireDateTime = DateTime.Parse(expireDateTime)
                }
            );
            _itemRepository.Commit();

            return Success();
        }
    }
}
