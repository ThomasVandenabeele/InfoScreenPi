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
    public abstract class GenericDataService : IGenericDataService
    {

        protected InfoScreenContext _context;

        public GenericDataService(InfoScreenContext context)
        {
            _context = context;
        }
        protected virtual IQueryable<T> ConstructQuery<T>(params Expression<Func<T, object>>[] includeProperties) where T : Entity
        {
          return includeProperties.Aggregate((IQueryable<T>)_context.Set<T>(),(q,p)=>q.Include(p));//.AsNoTracking();
        }
        public IEnumerable<T> GetAll<T>(Expression<Func<T,bool>> predicate, params Expression<Func<T, object>>[] includeProperties) where T : Entity
        {
            return ConstructQuery<T>(includeProperties).Where(predicate);
        }
        public IEnumerable<T> GetAll<T>(params Expression<Func<T, object>>[] includeProperties) where T : Entity
        {
            return ConstructQuery<T>(includeProperties);
        }
        public T GetSingle<T>(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includeProperties) where T : Entity
        {
            return ConstructQuery<T>(includeProperties).FirstOrDefault(predicate);
        }
        public T GetSingle<T>(int id, params Expression<Func<T, object>>[] includeProperties) where T : Entity
        {
            return GetSingle<T>(x => x.Id == id, includeProperties);
        }
        public async Task<T> GetSingleAsync<T>(int id) where T : Entity
        {
            return await _context.Set<T>().FirstOrDefaultAsync(e => e.Id == id);
        }
        public void Add<T>(T entity) where T : Entity
        {
            EntityEntry dbEntityEntry = _context.Entry<T>(entity);
            _context.Set<T>().Add(entity);
            //_context.SaveChanges();
        }

        public void Edit<T>(T entity) where T : Entity
        {
            EntityEntry dbEntityEntry = _context.Entry<T>(entity);
            dbEntityEntry.State = EntityState.Modified;
            //_context.SaveChanges();
        }
        public void Delete<T>(T entity) where T : Entity
        {
            EntityEntry dbEntityEntry = _context.Entry<T>(entity);
            dbEntityEntry.State = EntityState.Deleted;
            //_context.SaveChanges();
        }

        public void Commit()
        {
            Setting s = _context.Settings.ToList().First(setting => setting.SettingName == "DBChanged");
            s.SettingValue = true.ToString();
            EntityEntry dbEnt = _context.Entry<Setting>(s);
            dbEnt.State = EntityState.Modified;

            _context.SaveChanges();
        }

        public void Commit(Task task){
          task.Start();
          Commit();
        }





    }
}
