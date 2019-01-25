using System.Collections.Generic;
using InfoScreenPi.Entities;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace InfoScreenPi.Infrastructure.Services
{
    public interface IVolatileDataService : IDataService
    {
        IEnumerable<T> GetAllActive<T>() where T : Item;
        bool AnyRssFeedActive();
    }
}
