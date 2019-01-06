using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using InfoScreenPi.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace InfoScreenPi.Infrastructure.Repositories
{
    public class EntityBaseRepository<T> : IEntityBaseRepository<T>
            where T : class, IEntityBase, new()
    {

        private InfoScreenContext _context;

        public EntityBaseRepository(InfoScreenContext context)
        {
            _context = context;
        }
        private IQueryable<T> ConstructQuery(params Expression<Func<T, object>>[] includeProperties)
        {
          return includeProperties.Aggregate((IQueryable<T>)_context.Set<T>(),(q,p)=>q.Include(p));
        }
        public IEnumerable<T> GetAll(Expression<Func<T,bool>> predicate, params Expression<Func<T, object>>[] includeProperties)
        {
            return ConstructQuery(includeProperties).Where(predicate).AsEnumerable();
        }
        public IEnumerable<T> GetAll(params Expression<Func<T, object>>[] includeProperties)
        {
            return ConstructQuery(includeProperties).AsEnumerable();
        }
        public T GetSingle(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includeProperties)
        {
            return ConstructQuery(includeProperties).FirstOrDefault(predicate);
        }
        public T GetSingle(int id, params Expression<Func<T, object>>[] includeProperties)
        {
            return GetSingle(x => x.Id == id, includeProperties);
        }
        public async Task<T> GetSingleAsync(int id)
        {
            return await _context.Set<T>().FirstOrDefaultAsync(e => e.Id == id);
        }
        public void Add(T entity)
        {
            EntityEntry dbEntityEntry = _context.Entry<T>(entity);
            _context.Set<T>().Add(entity);
        }

        public void Edit(T entity)
        {
            EntityEntry dbEntityEntry = _context.Entry<T>(entity);
            dbEntityEntry.State = EntityState.Modified;
        }
        public void Delete(T entity)
        {
            EntityEntry dbEntityEntry = _context.Entry<T>(entity);
            dbEntityEntry.State = EntityState.Deleted;
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
