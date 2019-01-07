using Microsoft.EntityFrameworkCore;
using InfoScreenPi.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace InfoScreenPi.Infrastructure.Repositories
{
    public interface IEntityBaseRepository<T> where T : class, IEntityBase, new()
    {
        //IEnumerable<T> AllIncluding(params Expression<Func<T, object>>[] includeProperties);
        //Task<IEnumerable<T>> AllIncludingAsync(params Expression<Func<T, object>>[] includeProperties);
        //IEnumerable<T> GetAll();
        IEnumerable<T> GetAll(params Expression<Func<T, object>>[] includeProperties);
        IEnumerable<T> GetAll(Expression<Func<T,bool>> predicate, params Expression<Func<T, object>>[] includeProperties);
        //Task<IEnumerable<T>> GetAllAsync();
        T GetSingle(int id, params Expression<Func<T, object>>[] includeProperties);
        T GetSingle(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includeProperties);
        Task<T> GetSingleAsync(int id);
        //IEnumerable<T> FindBy(Expression<Func<T, bool>> predicate);
        //Task<IEnumerable<T>> FindByAsync(Expression<Func<T, bool>> predicate);
        void Add(T entity);
        void Delete(T entity);
        void Edit(T entity);
        void Commit();
        void Commit(Task task);
    }
}
