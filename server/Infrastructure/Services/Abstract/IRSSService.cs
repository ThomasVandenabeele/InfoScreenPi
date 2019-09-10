using System.Threading.Tasks;
using System.Xml.Linq;

namespace InfoScreenPi.Infrastructure.Services
{
    public interface IRSSService
    {
        Task RegisterRss(string uri, int bgId);
        void DeleteRssFeed(int rssFeedId);
        void DeleteRssFeedItems(int rssFeedId);
        Task<bool> RenewActiveRssFeeds();
        Task ExtractRssItems(int rssFeedId);
    }
}
