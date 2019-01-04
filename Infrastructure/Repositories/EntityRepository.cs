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
        /*private IQueryable<T> ConstructQuery(Expression<Func<T,bool>> predicate, params Expression<Func<T, object>>[] includeProperties)
        {
          return ConstructQuery(includeProperties).Where(predicate);
        }*/
        public IEnumerable<T> GetAll(Expression<Func<T,bool>> predicate, params Expression<Func<T, object>>[] includeProperties)
        {
            return ConstructQuery(includeProperties).Where(predicate).AsEnumerable();
        }
        public IEnumerable<T> GetAll(params Expression<Func<T, object>>[] includeProperties)
        {
            //IQueryable<T> query = _context.Set<T>();

            /*foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }*/
            //return query2.AsEnumerable();
            return ConstructQuery(includeProperties).AsEnumerable();
        }
        public T GetSingle(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includeProperties)
        {
            return ConstructQuery(includeProperties).FirstOrDefault(predicate);
            /*IQueryable<T> query = _context.Set<T>();
            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }

            return query.Where(predicate).FirstOrDefault();*/
        }
        /*public T GetSingle(Expression<Func<T, bool>> predicate)
        {
            return _context.Set<T>().FirstOrDefault(predicate);
        }*/
        public T GetSingle(int id, params Expression<Func<T, object>>[] includeProperties)
        {
            return GetSingle(x => x.Id == id, includeProperties);
        }


        /*public virtual IEnumerable<T> GetAll()
        {
            return AllIncluding();
            //_context.Set<T>().AsEnumerable();
        }
        /*public virtual async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _context.Set<T>().ToListAsync();
        }*/






        public async Task<T> GetSingleAsync(int id)
        {
            return await _context.Set<T>().FirstOrDefaultAsync(e => e.Id == id);
        }







        /*public virtual async Task<IEnumerable<T>> AllIncludingAsync(params Expression<Func<T, object>>[] includeProperties)
        {
            IQueryable<T> query = _context.Set<T>();
            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }
            return await query.ToListAsync();
        }*/


        /*public virtual IEnumerable<T> FindBy(Expression<Func<T, bool>> predicate)
        {
            return _context.Set<T>().Where(predicate);
        }*/

        /*public virtual async Task<IEnumerable<T>> FindByAsync(Expression<Func<T, bool>> predicate)
        {
            return await _context.Set<T>().Where(predicate).ToListAsync();
        }*/

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
