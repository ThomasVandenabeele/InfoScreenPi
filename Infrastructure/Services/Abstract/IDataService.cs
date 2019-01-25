using System.Collections.Generic;
using InfoScreenPi.Entities;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace InfoScreenPi.Infrastructure.Services
{
    public interface IDataService : IGenericDataService
    {
        IEnumerable<Background> GetBackgroundsNoRSS(bool archieved);
        IEnumerable<Item> GetItemsNoRss(bool archived);
        IEnumerable<Item> GetAllCustomItems();
        IEnumerable<Item> GetAllActiveCustomItems();
        IEnumerable<Item> GetAllActiveRSSItems();
        IEnumerable<T> GetAllActive<T>() where T : Item;
        bool CheckItemState();
        string GetSettingByName(string setting);
        void SetSettingByName(string key, string value);
        User GetSingleByUsername(string username);
        IEnumerable<Role> GetUserRoles(string username);
        IEnumerable<IStatic> GetAllStatic(Expression<Func<Item,bool>> predicate, params Expression<Func<Item, object>>[] includeProperties);
        IEnumerable<IStatic> GetAllStatic(params Expression<Func<Item, object>>[] includeProperties);
        IEnumerable<IExpiring> GetAllExpiring(Expression<Func<Item,bool>> predicate, params Expression<Func<Item, object>>[] includeProperties);
        IEnumerable<IExpiring> GetAllExpiring(params Expression<Func<Item, object>>[] includeProperties);
    }
}
