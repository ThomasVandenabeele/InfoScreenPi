using System.Collections.Generic;
using InfoScreenPi.Entities;

namespace InfoScreenPi.Infrastructure.Services
{
    public interface IDataService : IGenericDataService
    {
        IEnumerable<Background> GetBackgroundsNoRSS(bool archieved);
        IEnumerable<Item> GetItemsNoRss(bool archived);
        IEnumerable<Item> GetAllCustomItems();
        IEnumerable<Item> GetAllActiveCustomItems();
        IEnumerable<Item> GetAllActiveRSSItems();
        bool CheckItemState();
        string GetSettingByName(string setting);
        void SetSettingByName(string key, string value);
        User GetSingleByUsername(string username);
        IEnumerable<Role> GetUserRoles(string username);
    }
}
