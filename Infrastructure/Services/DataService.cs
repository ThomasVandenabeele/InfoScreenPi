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
      List<Background> exclAchtergronden = GetAll<Item>(i => i.Soort.Description == "RSS"  || (archieved? false : i.Archieved),
                                                        i => i.Background, i => i.Soort)
                                           .Select(i => i.Background).ToList();
      return GetAll<Background>(a => !exclAchtergronden.Contains(a) && !a.Url.Equals("black.jpg"));
    }
    public IEnumerable<Item> GetItemsNoRss(bool archived)
    {
      return GetAll<Item>(i => i.Soort.Description != "RSS" && i.Archieved == archived, a => a.Background, a => a.Soort);
    }
    public IEnumerable<Item> GetAllCustomItems()
    {
        return GetAll<Item>(i => (i.Soort.Description == "CUSTOM" || i.Soort.Description == "VIDEO" || i.Soort.Description == "CLOCK" || i.Soort.Description == "WEATHER"),
                      i => i.Soort, i => i.Background);
    }
    public IEnumerable<Item> GetAllActiveCustomItems()
    {
        return GetAllCustomItems().Where(i => i.Active && !i.Archieved);
    }
    public IEnumerable<Item> GetAllActiveRSSItems()
    {
        return GetAll<Item>(i => i.Soort.Description == "RSS" && i.Active && !i.Archieved,
                      i=>i.Soort, i=>i.Background, i=>i.RssFeed);
    }
    public bool CheckItemState()
    {
      bool any = false;
      GetAll<Item>(i => i.Active && DateTime.Compare(i.ExpireDateTime, DateTime.Now) <= 0)
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
  }
}
