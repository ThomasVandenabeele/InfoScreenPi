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
using System.Net.Http;
using System.Xml.Linq;
using InfoScreenPi.Extensions;
using Microsoft.SyndicationFeed;
using Microsoft.SyndicationFeed.Rss;
using Microsoft.SyndicationFeed.Atom;
using System.Xml;

namespace InfoScreenPi.Controllers
{
    [Authorize(Policy = "AdminOnly")]
    [Route("Config/[controller]/[action]")]
    public class RssFeedsController : Controller
    {
        private InfoScreenContext _context;
        private readonly IMembershipService _membershipService;
        private readonly IUserRepository _userRepository;
        private readonly ILoggingRepository _loggingRepository;
        private readonly IItemRepository _itemRepository;
        private readonly IHostingEnvironment _hostEnvironment;
        private readonly IBackgroundRepository _backgroundRepository;
        private readonly IRssFeedRepository _rssFeedRepository;
        private readonly IItemKindRepository _itemKindRepository;

        public RssFeedsController(InfoScreenContext context, 
                                IMembershipService membershipService,
                                IUserRepository userRepository,
                                ILoggingRepository _errorRepository,
                                IItemRepository itemRepository,
                                IHostingEnvironment hostEnvironment,
                                IBackgroundRepository backgroundRepository,
                                IRssFeedRepository rssFeedRepository,
                                IItemKindRepository itemKindRepository)
        {
            _context = context;
            _membershipService = membershipService;
            _userRepository = userRepository;
            _loggingRepository = _errorRepository;
            _itemRepository = itemRepository;
            _hostEnvironment = hostEnvironment;
            _backgroundRepository = backgroundRepository;
            _rssFeedRepository = rssFeedRepository;
            _itemKindRepository = itemKindRepository;
        }

        [HttpGet]
        public IActionResult CreateRssFeed(){
            return PartialView("~/Views/Config/RSS/CreateRssFeed.cshtml", _backgroundRepository.GetAllWithoutRSS(true).ToList());
        }

        [HttpPost]
        public async Task<ActionResult> ChangeRssFeedState(int id, bool state)
        {
            RssFeed rf = _rssFeedRepository.GetSingle(id);
            if (rf != null)
            {
                rf.Active = state;
                _rssFeedRepository.Edit(rf);
                _rssFeedRepository.Commit();

                _rssFeedRepository.DeleteRssFeedItems(id);
                if(state) await _rssFeedRepository.ExtractRssItems(id);
                return Json(new {success = true, message = (state? "Abonnement status verandert naar actief!" : "Abonnement status verandert naar inactief!")});    
            }

            return Json(new {success = false, message = "Update is niet gelukt!"});
        }

        [HttpPost]
        public async Task<IActionResult> RegisterRss(string uri, int bgId){
            try
            {
                
                if(_rssFeedRepository.GetAll().Where(r => r.Url == uri).Count() > 0 ) return Json(new {success = false, message = "RSS abonnement bestaat al"});

                await _rssFeedRepository.RegisterRss(uri, bgId);
                await _rssFeedRepository.ExtractRssItems(_rssFeedRepository.GetAll().Where(r => r.Url == uri).Last().Id);
                
                return Json(new {success = true, message = "RSS abonnement geregistreerd"});
               
            }
            catch (Exception e)
            {
                return Json(new {success = false, message = e.Message});
            }
            
        }

        public IActionResult DeleteRssFeed(int id){
            try
            {
                _rssFeedRepository.DeleteRssFeed(id);
                return Json(new {success = true, message = "RSS abonnement verwijderd"}); 
            }
            catch(Exception e)
            {
                return Json(new {success = false, message = e.Message});
            }
        }

         public async Task<IActionResult> RenewRssFeeds(){
             var renewed = _rssFeedRepository.RenewActiveRssFeeds().Result;
             if (renewed)
             {
                 return Json(new {success = true, message = "Rss Feeds vernieuwd"});     
             }
             else
             {
                 return Json(new {success = false, message = "Geen Rss Feeds actief"}); 
             }
         }


        [HttpGet]
        public ActionResult Table()
        {
            List<RssFeed> model = _rssFeedRepository.AllIncluding(r => r.StandardBackground).ToList();
            return PartialView("~/Views/Config/RSS/Table.cshtml", model);
        }



    }
}
