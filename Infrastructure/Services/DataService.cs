using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using InfoScreenPi.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace InfoScreenPi.Infrastructure.Services
{
  public class DataService : GenericDataService, IDataService
  {

    public DataService(InfoScreenContext context) : base(context)
    {}

    public IEnumerable<Background> GetBackgroundsNoRSS(bool archieved)
    {
      List<Background> exclAchtergronden = GetAll<Item>(i => i is RSSItem  || (archieved? false : ((IExpiring)i).Archieved),
                                                        i => ((IStatic)i).Background)
                                           .Select(i => ((IStatic)i).Background).ToList();
      return GetAll<Background>(a => !exclAchtergronden.Contains(a) && !a.Url.Equals("black.jpg"));
    }
    public IEnumerable<Item> GetItemsNoRss(bool archived)
    {
      return GetAll<Item>(i => !(i is RSSItem) && ((IExpiring)i).Archieved == archived, a => ((IStatic)a).Background);
    }
    public IEnumerable<Item> GetAllCustomItems()
    {
        return GetAll<Item>(i => !(i is RSSItem), i => ((IStatic)i).Background);
    }
    public IEnumerable<Item> GetAllActiveCustomItems()
    {
        return GetAllCustomItems().Where(i => i.Active && !((IExpiring)i).Archieved);
    }
    public IEnumerable<Item> GetAllActiveRSSItems()
    {
        return GetAll<RSSItem>(i=>i.Active, i=>i.Background, i=>i.RssFeed);
    }
    public bool CheckItemState()
    {
      bool any = false;
      GetAll<Item>(i => i.Active && DateTime.Compare(((IExpiring)i).ExpireDateTime, DateTime.Now) <= 0)
               .ToList().ForEach(i=>
                {
                    any = true;
                    i.Active = false;
                    Edit(i);
                    Commit();
                });
      return any;
    }
    public string GetSettingByName(string setting)
    {
      return GetAll<Setting>(i => i.SettingName == setting)
              .Select(i => i.SettingValue).First();
    }
    public void SetSettingByName(string key, string value)
    {
      var s = GetSingle<Setting>(setting => setting.SettingName == key);
      s.SettingValue = value;
      Edit(s);
      Commit();
    }
    public User GetSingleByUsername(string username)
    {
      return GetSingle<User>(x => x.Username == username);
    }
    public IEnumerable<Role> GetUserRoles(string username)
    {
      User user = GetSingle<User>(u => u.Username == username, u => u.UserRoles);
      if(user != null) return user.UserRoles.Select(ur => _context.Roles.FirstOrDefault(r => r.Id == ur.RoleId)).AsEnumerable();
      else return null;
    }

    public IEnumerable<IStatic> GetAllStatic(params Expression<Func<Item, object>>[] includeProperties)
    {
      //return (IEnumerable<IStatic>)GetAll<RSSItem>(includeProperties) + (IEnumerable<IStatic>)GetAll<CustomItem>(includeProperties);
      return (IEnumerable<IStatic>)GetAll<Item>(includeProperties).Where(i=>i is IStatic);
    }
    public IEnumerable<IExpiring> GetAllExpiring(params Expression<Func<Item, object>>[] includeProperties)
    {
      //return (IEnumerable<IExpiring>)GetAll<VideoItem>(includeProperties) + (IEnumerable<IExpiring>)GetAll<CustomItem>(includeProperties);
      return (IEnumerable<IExpiring>)GetAll<Item>(includeProperties).Where(i=>i is IExpiring);
    }
    /*public IStatic GetStatic*/
    public IEnumerable<IStatic> GetAllStatic(Expression<Func<Item,bool>> predicate, params Expression<Func<Item, object>>[] includeProperties)
    {
      //return (IEnumerable<IStatic>)GetAll<RSSItem>((Expression<Func<RSSItem,bool>>)predicate, (Expression<Func<RSSItem, object>>[])includeProperties) + (IEnumerable<IStatic>)GetAll<CustomItem>((Expression<Func<CustomItem,bool>>)predicate, (Expression<Func<CustomItem, object>>[])includeProperties);
      //return (IEnumerable<IStatic>)GetAll<Item>(predicate,includeProperties).Where(i=>i is RSSItem || i is CustomItem);
      return (IEnumerable<IStatic>)((IQueryable<Item>)GetAllStatic(includeProperties)).Where(predicate);
    }
    public IEnumerable<IExpiring> GetAllExpiring(Expression<Func<Item,bool>> predicate, params Expression<Func<Item, object>>[] includeProperties)
    {
      //return (IEnumerable<IExpiring>)GetAll<VideoItem>(predicate, includeProperties) + (IEnumerable<IExpiring>)GetAll<CustomItem>(predicate, includeProperties);
      //return (IEnumerable<IExpiring>)GetAll<Item>(predicate,includeProperties).Where(i=>i is VideoItem || i is CustomItem);
      return (IEnumerable<IExpiring>)((IQueryable<Item>)GetAllExpiring(includeProperties)).Where(predicate);
    }

  }
}
