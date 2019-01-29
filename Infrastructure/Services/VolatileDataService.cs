using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using InfoScreenPi.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Query.Internal;

namespace InfoScreenPi.Infrastructure.Services
{
  public class VolatileDataService : DataService, IVolatileDataService
  {

    public VolatileDataService(InfoScreenContext context) : base(context)
    {}

    protected override IQueryable<T> ConstructQuery<T>(params Expression<Func<T, object>>[] includeProperties)
    {
      return includeProperties.Aggregate((IQueryable<T>)_context.Set<T>(),(q,p)=>q.Include(p)).AsNoTracking();
    }

//    public IEnumerable<T> GetAllActive<T>() where T : Item
//    {
//      //return GetAll<T>(i => i.Active, (typeof(T) is IStatic)? i => ((IStatic) i).Background);
//      //return GetAll<T>(i => i.Active);
//      return GetAll<T>(i =>  i is RSSItem, i => (RssFeed) i).Background);
//        
//                         .Concat(GetAll<T>(i => i is IStatic && !(i is RSSItem), (IStatic i) => i.Background))
//                         .Concat(GetAll<T>(i => !(i is IStatic)));
//    }

    public IEnumerable<Item> GetAllActive()
    {
      return GetAll<RSSItem>(i => i.Background, i => i.RssFeed)
                   .Concat(GetAll<Item>(i => i is IStatic && !(i is RSSItem), i => ((IStatic) i).Background))
                   .Concat(GetAll<Item>(i => !(i is IStatic))).Where(i=>i.Active);
      
    }
    
    
    public bool AnyRssFeedActive()
    {
      return GetAll<RssFeed>(f=>f.Active).FirstOrDefault() != null;
    }

  }
}
