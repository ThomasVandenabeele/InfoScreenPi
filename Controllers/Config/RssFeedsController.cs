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
                if(state) await ExtractRssItems(id);
                return Json(new {success = true, message = (state? "Abonnement status verandert naar actief!" : "Abonnement status verandert naar inactief!")});    
            }

            return Json(new {success = false, message = "Update is niet gelukt!"});
        }

        [HttpPost]
        public async Task<IActionResult> RegisterRss(string uri, int bgId){
            try
            {
                if(_rssFeedRepository.GetAll().Where(r => r.Url == uri).Count() > 0 ) return Json(new {success = false, message = "RSS abonnement bestaat al"}); 

                XDocument rssString = await RssToXDocument(uri);

                if(rssString != null)
                {
                    string source = Guid.NewGuid().ToString();
                    _itemKindRepository.Add(
                        new ItemKind
                        {
                            Description = "RSS",
                            Source = source
                        }
                    );
                    _itemKindRepository.Commit();

                    _rssFeedRepository.Add(
                        new RssFeed
                        {
                            Active = true,
                            Url = uri,
                            StandardBackground = _backgroundRepository.GetSingle(bgId),
                            Source = source,
                            PublicationDate = rssString.Root.Descendants("channel").Elements("pubDate").First().Value.ParseDate(),
                            Title = rssString.Root.Descendants("channel").Elements("title").First().Value,
                            Description = rssString.Root.Descendants("channel").Elements("description").First().Value
                        });
                    _rssFeedRepository.Commit();

                    await ExtractRssItems(_rssFeedRepository.GetAll().Where(r => r.Url == uri).Last().Id);

                    return Json(new {success = true, message = "RSS abonnement geregistreerd"}); 
                }
                else
                {
                    return Json(new {success = false, message = "RSS abonnement niet geregistreerd"}); 
                }
            }
            catch (Exception e)
            {
                return Json(new {success = false, message = e.Message});
            }
            
        }

        public IActionResult DeleteRssFeed(int id){
            try
            {
                RssFeed rf = _rssFeedRepository.GetSingle(id);

                ItemKind ik = _itemKindRepository.GetAll().Where(i => i.Description == "RSS" && i.Source == rf.Source).First();

                _rssFeedRepository.DeleteRssFeedItems(id);

                _itemKindRepository.Delete(ik);
                _itemKindRepository.Commit();
                
                _rssFeedRepository.Delete(rf);
                _rssFeedRepository.Commit();
                
                return Json(new {success = true, message = "RSS abonnement verwijderd"}); 
            }
            catch(Exception e)
            {
                return Json(new {success = false, message = e.Message});
            }
        }

        [AllowAnonymous]
         public async Task<IActionResult> RenewRssFeeds(){
            List<int> rssfeeds = _rssFeedRepository.GetAll().Where(r => r.Active == true).Select(r => r.Id).ToList();
            //rssfeeds.ForEach(async (rf) => { await ExtractRssItems(rf); });
            //await Task.WhenAll(rssfeeds.Select(rssf => ExtractRssItems(rssf)));
            //Parallel.ForEach(rssfeeds, i => ExtractRssItems(i).Wait());
            foreach(var feed in rssfeeds) await ExtractRssItems(feed); 
            //return Json(new {success = true, message = "Ids verkregen", data = rssfeeds }); 
            return Json(new {success = true, message = "Rss Feeds vernieuwd"}); 
         }

        [AllowAnonymous]
        public async Task ExtractRssItems(int rssFeedId){
            RssFeed rssFeed = _rssFeedRepository.AllIncluding(rss => rss.StandardBackground).Where(rss => rss.Id == rssFeedId).First();
            
            string feedUrl = rssFeed.Url;
            XDocument doc = await RssToXDocument(feedUrl);
        
            if(doc != null)
            {
                //var items = doc.Root.Descendants().First(i => i.Name.LocalName == "channel").Elements().Where(i => i.Name.LocalName == "item").ToList();
                var items = doc.Root.Descendants("channel").Elements("item").ToList();

                DateTime pubDate = doc.Root.Descendants("channel").Elements("pubDate").First().Value.ParseDate();//doc.Root.Descendants().First(i => i.Name.LocalName == "channel").Elements().First(i => i.Name.LocalName == "pubDate").Value);

                ItemKind soort = _itemKindRepository.GetAll().Where(ik => (ik.Description == "RSS" && ik.Source == rssFeed.Source)).First();

                if(pubDate >= rssFeed.PublicationDate){

                    rssFeed.PublicationDate = pubDate;
                    _rssFeedRepository.Edit(rssFeed);
                    _rssFeedRepository.Commit();

                    _rssFeedRepository.DeleteRssFeedItems(rssFeedId);

                    items.ForEach( i => {
                            DateTime itemDate = i.Element("pubDate").Value.ParseDate();
                            if(itemDate.Date != DateTime.Today) return;

                            Background achtergrond = null;
                            if(i.Elements("enclosure").Count() > 0){
                                achtergrond = new Background{
                                    Url = i.Elements("enclosure").Attributes("url").First().Value
                                };
                            }
                            else{
                                achtergrond = rssFeed.StandardBackground;
                            }
                            

                            _itemRepository.Add(
                                new Item
                                {
                                    RssFeed = rssFeed,
                                    Soort = soort,
                                    Title = i.Element("title").Value,
                                    Content = i.Element("description").Value,
                                    Background = achtergrond,
                                    Active = true,
                                    Archieved = false
                                }
                            );
                            _itemRepository.Commit();
                            
                        }
                    );
                    //System.Threading.Thread.Sleep(1000);
                    //return Json(new {success = true, message = "Rss Feed '" + rssFeed.Title + "' vernieuwd"}); 
                }
            }

            //return Json(new {success = false, message = "Er liep iets mis"});    
        }


        [HttpGet]
        public ActionResult Table()
        {
            List<RssFeed> model = _rssFeedRepository.AllIncluding(r => r.StandardBackground).ToList();
            return PartialView("~/Views/Config/RSS/Table.cshtml", model);
        }


        private async Task<XDocument> RssToXDocument(string uri)
        {
            XDocument doc = null;
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(uri);
                var responseMessage = await client.GetAsync(uri);
                var responseString = await responseMessage.Content.ReadAsStringAsync();
                doc = XDocument.Parse(responseString);
            }
            return doc;

        }



    }
}
