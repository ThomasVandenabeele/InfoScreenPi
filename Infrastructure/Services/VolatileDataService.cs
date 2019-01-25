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
  public class VolatileDataService : DataService, IVolatileDataService
  {

    public VolatileDataService(InfoScreenContext context) : base(context)
    {}

    protected override IQueryable<T> ConstructQuery<T>(params Expression<Func<T, object>>[] includeProperties)
    {
      return includeProperties.Aggregate((IQueryable<T>)_context.Set<T>(),(q,p)=>q.Include(p)).AsNoTracking();//.AsNoTracking();
    }

    public IEnumerable<T> GetAllActive<T>() where T : Item
    {
      return GetAll<T>(i=>i.Active);
    }
    public bool AnyRssFeedActive()
    {
      return GetAll<RssFeed>(f=>f.Active).FirstOrDefault() != null;
    }

  }
}
