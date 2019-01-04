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
    public class RssFeedsController : BaseController
    {
        private readonly IBackgroundRepository _backgroundRepository;
        private readonly IRssFeedRepository _rssFeedRepository;

        public RssFeedsController(IBackgroundRepository backgroundRepository,
                                  IRssFeedRepository rssFeedRepository)
        {
            _backgroundRepository = backgroundRepository;
            _rssFeedRepository = rssFeedRepository;
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
                return Success(state? "Abonnement status verandert naar actief!" : "Abonnement status verandert naar inactief!");
            }
            return Fail();
        }

        [HttpPost]
        public async Task<IActionResult> RegisterRss(string uri, int bgId){
            try
            {
                if(_rssFeedRepository.GetAll().Where(r => r.Url == uri).Count() > 0 ) return Json(new {success = false, message = "RSS abonnement bestaat al"});

                await _rssFeedRepository.RegisterRss(uri, bgId);
                await _rssFeedRepository.ExtractRssItems(_rssFeedRepository.GetAll().Where(r => r.Url == uri).Last().Id);

                return Success("RSS abonnement geregistreerd");

            }
            catch (Exception e)
            {
                return Fail(e.Message);
            }
        }

        public IActionResult DeleteRssFeed(int id){
            try
            {
                _rssFeedRepository.DeleteRssFeed(id);
                return Success("RSS abonnement verwijderd");
            }
            catch(Exception e)
            {
                return Fail(e.Message);
            }
        }

         public async Task<IActionResult> RenewRssFeeds(){
             if (_rssFeedRepository.RenewActiveRssFeeds().Result) return Success("Rss Feeds vernieuwd");
             else return Fail("Geen Rss Feeds actief");
         }


        [HttpGet]
        public ActionResult Table()
        {
            return PartialView("~/Views/Config/RSS/Table.cshtml", _rssFeedRepository.GetAll(r => r.StandardBackground).ToList());
        }



    }
}
