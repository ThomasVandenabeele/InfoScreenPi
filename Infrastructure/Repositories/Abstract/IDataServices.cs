using InfoScreenPi.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InfoScreenPi.Infrastructure.Repositories
{
    public interface IItemRepository : IEntityBaseRepository<Item> 
    { 
        IEnumerable<Item> GetAllCustomItems();
        IEnumerable<Item> GetAllActiveCustomItems();
        IEnumerable<Item> GetAllActiveRSSItems();
        bool CheckItemState();
    }

    public interface IBackgroundRepository : IEntityBaseRepository<Background> 
    { 
        IEnumerable<Background> GetAllWithoutRSS(Boolean archieved);
    }

    public interface ILoggingRepository : IEntityBaseRepository<Error> { }

    public interface IItemKindRepository : IEntityBaseRepository<ItemKind> { }

    public interface IRoleRepository : IEntityBaseRepository<Role> { }

    public interface IRssFeedRepository : IEntityBaseRepository<RssFeed> 
    {
        void DeleteRssFeedItems(int rssFeedId);
        Task ExtractRssItems(int rssFeedId);
        Task RegisterRss(string uri, int bgId);
        Task DeleteRssFeed(int rssFeedId);
        Task<bool> RenewActiveRssFeeds();
    }

    public interface IUserRepository : IEntityBaseRepository<User>
    {
        User GetSingleByUsername(string username);
        IEnumerable<Role> GetUserRoles(string username);
    }

    public interface IUserRoleRepository : IEntityBaseRepository<UserRole> { }

    public interface ISettingRepository : IEntityBaseRepository<Setting> { 
        string GetSettingByName(string setting);
        void SetSettingByName(string key, string value);
    }
}