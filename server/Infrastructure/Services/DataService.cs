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
      List<int> exclAchtergronden = GetAll<Item>(i => i is IStatic && (i is RSSItem  || (archieved? false : ((IExpiring)i).Archieved)))
                                           .Select(i => ((IStatic) i).BackgroundId).ToList();
      return GetAll<Background>(a => !exclAchtergronden.Contains(a.Id));
    }
    public IEnumerable<Item> GetItemsNoRss(bool archived)
    {
      IQueryable<Item> items = ConstructQuery<Item>().Where(i => (i is IExpiring) && (((IExpiring)i).Archieved == archived));
      IEnumerable<Item> items2 = items.Where(i => i is IStatic).Include(i => ((IStatic) i).Background).Concat(items.Where(i=>!(i is IStatic)));
      return items2.OrderBy(i => i.Id);
    }
    public bool CheckItemState()
    {
      bool any = false;
      GetAll<Item>(i => !(i is RSSItem) && !(i is ClockItem) && !(i is WeatherItem) && i.Active && DateTime.Compare(((IExpiring)i).ExpireDateTime, DateTime.Now) <= 0)
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
    /*
    public IEnumerable<IStatic> GetAllStatic(params Expression<Func<Item, object>>[] includeProperties)
    {
      return (IEnumerable<IStatic>)GetAll<Item>(includeProperties).Where(i=>i is IStatic);
    }
    public IEnumerable<IExpiring> GetAllExpiring(params Expression<Func<Item, object>>[] includeProperties)
    {
      return (IEnumerable<IExpiring>)GetAll<Item>(includeProperties).Where(i=>i is IExpiring);
    }
    public IEnumerable<IStatic> GetAllStatic(Expression<Func<IStatic,bool>> predicate, params Expression<Func<Item, object>>[] includeProperties)
    {
      return ((IQueryable<IStatic>)GetAllStatic(includeProperties)).Where(predicate).AsEnumerable();
    }
    public IEnumerable<IExpiring> GetAllExpiring(Expression<Func<IExpiring,bool>> predicate, params Expression<Func<Item, object>>[] includeProperties)
    {
      return ((IQueryable<IExpiring>)GetAllExpiring(includeProperties)).Where(predicate).AsEnumerable();
    }
    */
  }
}
