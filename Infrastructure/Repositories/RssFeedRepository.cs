using InfoScreenPi.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml.Linq;
using InfoScreenPi.Extensions;

namespace InfoScreenPi.Infrastructure.Repositories
{
    public class RssFeedRepository : EntityBaseRepository<RssFeed>, IRssFeedRepository
    {
        IItemRepository _itemRepository;
        IBackgroundRepository _backgroundRepository;
        IItemKindRepository _itemKindRepository;

        public RssFeedRepository(InfoScreenContext context, IItemRepository itemRepository, IBackgroundRepository backgroundRepository, IItemKindRepository itemKindRepository)
            : base(context)
        {
            _itemRepository = itemRepository;
            _backgroundRepository = backgroundRepository;
            _itemKindRepository = itemKindRepository;
        }

        public async Task RegisterRss(string uri, int bgId)
        {
            XDocument rssString = await RssToXDocument(uri);

            if (rssString != null)
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

                Add(
                    new RssFeed
                    {
                        Active = true,
                        Url = uri,
                        StandardBackground = _backgroundRepository.GetSingle(bgId),
                        Source = source,
                        PublicationDate = rssString.Root.Descendants("channel").Elements("pubDate").First().Value
                            .ParseDate(),
                        Title = rssString.Root.Descendants("channel").Elements("title").First().Value,
                        Description = rssString.Root.Descendants("channel").Elements("description").First().Value
                    });
                Commit();
            }
        }

        public async Task DeleteRssFeed(int rssFeedId)
        {
            RssFeed rf = GetSingle(rssFeedId);

            ItemKind ik = _itemKindRepository.GetAll().Where(i => i.Description == "RSS" && i.Source == rf.Source).First();

            DeleteRssFeedItems(rssFeedId);

            _itemKindRepository.Delete(ik);
            _itemKindRepository.Commit();

            Delete(rf);
            Commit();
        }

        public void DeleteRssFeedItems(int rssFeedId){
            RssFeed rssFeed = GetAll(rss => rss.StandardBackground).Where(rss => rss.Id == rssFeedId).First();

            _itemRepository.GetAll(a => a.Background, a => a.Soort)
                    .Where(i => (i.Soort.Source == rssFeed.Source && i.Soort.Description == "RSS"))
                    .ToList()
                    .ForEach(i =>
                    {
                        if(i.Background != rssFeed.StandardBackground){
                            _backgroundRepository.Delete(i.Background);
                            _backgroundRepository.Commit();
                        }

                        _itemRepository.Delete(i);
                        _itemRepository.Commit();
                    });

        }

        public async Task<bool> RenewActiveRssFeeds()
        {
            List<int> activeRssfeeds = GetAll().Where(r => r.Active).Select(r => r.Id).ToList();
            foreach(var feed in activeRssfeeds) await ExtractRssItems(feed);
            return activeRssfeeds.Count > 0;
        }

        public async Task ExtractRssItems(int rssFeedId){
            RssFeed rssFeed = GetAll(rss => rss.StandardBackground).Where(rss => rss.Id == rssFeedId).First();

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
                    Edit(rssFeed);
                    Commit();

                    DeleteRssFeedItems(rssFeedId);

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
                                    Archieved = false,
                                    ExpireDateTime = DateTime.Now.AddDays(1)
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
