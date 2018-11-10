using InfoScreenPi.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InfoScreenPi.Infrastructure.Repositories
{
    public class RssFeedRepository : EntityBaseRepository<RssFeed>, IRssFeedRepository
    {
        IItemRepository _itemRepository;
        IBackgroundRepository _backgroundRepository;

        public RssFeedRepository(InfoScreenContext context, IItemRepository itemRepository, IBackgroundRepository backgroundRepository)
            : base(context)
        {
            _itemRepository = itemRepository;
            _backgroundRepository = backgroundRepository;
        }

        public void DeleteRssFeedItems(int rssFeedId){
            RssFeed rssFeed = AllIncluding(rss => rss.StandardBackground).Where(rss => rss.Id == rssFeedId).First();

            _itemRepository.AllIncluding(a => a.Background, a => a.Soort)
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
    }
}