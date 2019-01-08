using Microsoft.EntityFrameworkCore;
using InfoScreenPi.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace InfoScreenPi.Infrastructure.Services
{
    public interface IGenericDataService
    {
        IEnumerable<T> GetAll<T>(params Expression<Func<T, object>>[] includeProperties) where T : Entity;
        IEnumerable<T> GetAll<T>(Expression<Func<T,bool>> predicate, params Expression<Func<T, object>>[] includeProperties) where T : Entity;
        T GetSingle<T>(int id, params Expression<Func<T, object>>[] includeProperties) where T : Entity;
        T GetSingle<T>(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includeProperties) where T : Entity;
        Task<T> GetSingleAsync<T>(int id) where T : Entity;
        void Add<T>(T entity) where T : Entity;
        void Delete<T>(T entity) where T : Entity;
        void Edit<T>(T entity) where T : Entity;
        void Commit();
        void Commit(Task task);
    }
}
