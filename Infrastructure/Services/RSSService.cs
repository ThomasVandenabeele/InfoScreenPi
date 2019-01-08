using InfoScreenPi.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml.Linq;
using InfoScreenPi.Extensions;

namespace InfoScreenPi.Infrastructure.Services
{
    public class RSSService : IRSSService
    {
        private readonly IGenericDataService _data;

        public RSSService(IDataService dataService)
        {
            _data = dataService;
        }

        public async Task RegisterRss(string uri, int bgId)
        {
            XDocument rssString = await RssToXDocument(uri);

            if (rssString != null)
            {
                string source = Guid.NewGuid().ToString();
                _data.Add(
                    new ItemKind
                    {
                        Description = "RSS",
                        Source = source
                    }
                );
                _data.Add(
                    new RssFeed
                    {
                        Active = true,
                        Url = uri,
                        StandardBackground = _data.GetSingle<Background>(bgId),
                        Source = source,
                        PublicationDate = rssString.Root.Descendants("channel").Elements("pubDate").First().Value
                            .ParseDate(),
                        Title = rssString.Root.Descendants("channel").Elements("title").First().Value,
                        Description = rssString.Root.Descendants("channel").Elements("description").First().Value
                    });
                _data.Commit();
            }
        }

        public void DeleteRssFeed(int rssFeedId)
        {
            RssFeed rf = _data.GetSingle<RssFeed>(rssFeedId);

            ItemKind ik = _data.GetSingle<ItemKind>(i => i.Description == "RSS" && i.Source == rf.Source);

            DeleteRssFeedItems(rssFeedId);

            _data.Delete(ik);
            _data.Delete(rf);
            _data.Commit();
        }

        public void DeleteRssFeedItems(int rssFeedId){
            RssFeed rssFeed = _data.GetSingle<RssFeed>(rss => rss.Id == rssFeedId, rss => rss.StandardBackground);

            _data.GetAll<Item>(a => a.Background, a => a.Soort)
                    .Where(i => (i.Soort.Source == rssFeed.Source && i.Soort.Description == "RSS"))
                    .ToList()
                    .ForEach(i =>
                    {
                        if(i.Background != rssFeed.StandardBackground) _data.Delete(i.Background);
                        _data.Delete(i);
                        _data.Commit();
                    });
        }

        public async Task<bool> RenewActiveRssFeeds()
        {
            List<int> activeRssfeeds = _data.GetAll<RssFeed>(r => r.Active).Select(r => r.Id).ToList();
            foreach(var feed in activeRssfeeds) await ExtractRssItems(feed);
            return activeRssfeeds.Count > 0;
        }

        public async Task ExtractRssItems(int rssFeedId){
            RssFeed rssFeed = _data.GetSingle<RssFeed>(rss => rss.Id == rssFeedId, rss => rss.StandardBackground);

            string feedUrl = rssFeed.Url;
            XDocument doc = await RssToXDocument(feedUrl);

            if(doc != null)
            {
                //var items = doc.Root.Descendants().First(i => i.Name.LocalName == "channel").Elements().Where(i => i.Name.LocalName == "item").ToList();
                var items = doc.Root.Descendants("channel").Elements("item").ToList();

                DateTime pubDate = doc.Root.Descendants("channel").Elements("pubDate").First().Value.ParseDate();//doc.Root.Descendants().First(i => i.Name.LocalName == "channel").Elements().First(i => i.Name.LocalName == "pubDate").Value);

                ItemKind soort = _data.GetSingle<ItemKind>(ik => (ik.Description == "RSS" && ik.Source == rssFeed.Source));

                if(pubDate >= rssFeed.PublicationDate){

                    rssFeed.PublicationDate = pubDate;
                    _data.Edit(rssFeed);
                    _data.Commit();

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


                            _data.Add(
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
                            _data.Commit();

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
