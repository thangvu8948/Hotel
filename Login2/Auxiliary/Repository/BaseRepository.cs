using Login2.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Login2.Auxiliary.Repository
{
    class BaseRepository<TEntity> : IRepository<TEntity> where TEntity : class

    {

        //internal hotelEntities context;
        private static int maxConnection = 10;
        internal DbSet<TEntity> dbSet;

        //public BaseRepository()
        //{
        //    this.context = Pooling.Instance.getFreeContext();
        //    dbSet = context.Set<TEntity>();
        //}
        //public BaseRepository(hotelEntities context)
        //{
        //    this.context = context;
        //    this.dbSet = context.Set<TEntity>();
        //}



        public IEnumerable<TEntity> GetAll()
        {
            var context = Pooling.Instance.getFreeContext();
            dbSet = context.Set<TEntity>();
            var res = dbSet.ToList();
            context.IsUsing = false;
            return res;
        }
        public virtual IEnumerable<TEntity> GetWithRawSql(string query,

            params object[] parameters)
        {
            var context = Pooling.Instance.getFreeContext();
            dbSet = context.Set<TEntity>();
            var res = dbSet.SqlQuery(query, parameters).ToList();
            context.IsUsing = false;
            return res;
        }

        public virtual IEnumerable<TEntity> Get(

            Expression<Func<TEntity, bool>> filter = null,

            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,

            string includeProperties = "")

        {
            var context = Pooling.Instance.getFreeContext();

            dbSet = context.Set<TEntity>();
            IQueryable<TEntity> query = dbSet;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            if (includeProperties != null)
            {
                foreach (var includeProperty in includeProperties.Split

                (new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProperty);
                }
            }

            if (orderBy != null)
            {
                var res = orderBy(query).ToList();
                context.IsUsing = false;
                return res;
            }
            else
            {
                var res = query.ToList();
                context.IsUsing = false;
                return res;
            }


        }

        public virtual TEntity GetByID(object id)
        {
            var context = Pooling.Instance.getFreeContext();
            dbSet = context.Set<TEntity>();
            context.IsUsing = false;
            return dbSet.Find(id);
        }

        public virtual void Insert(TEntity entity)
        {
            var context = Pooling.Instance.getFreeContext();
            dbSet = context.Set<TEntity>();
            dbSet.Add(entity);
            context.SaveChanges();
            context.IsUsing = false;
        }

        public virtual void Delete(object id)
        {
            var context = Pooling.Instance.getFreeContext();
            dbSet = context.Set<TEntity>();
            TEntity entityToDelete = dbSet.Find(id);
            Delete(entityToDelete);
            context.SaveChanges();
            context.IsUsing = false;
        }

        public virtual void Delete(TEntity entityToDelete)
        {
            var context = Pooling.Instance.getFreeContext();
            dbSet = context.Set<TEntity>();
            if (context.Entry(entityToDelete).State == EntityState.Detached)

            {
                dbSet.Attach(entityToDelete);

            }
            dbSet.Remove(entityToDelete);
            context.SaveChanges();
            context.IsUsing = false;
        }


        public virtual void Update(TEntity entityToUpdate)
        {
            var context = Pooling.Instance.getFreeContext();

            dbSet = context.Set<TEntity>();
            dbSet.Attach(entityToUpdate);

            context.Entry(entityToUpdate).State = EntityState.Modified;
            context.SaveChanges();
            context.IsUsing = false;

        }
        public void Save()
        {
            var context = Pooling.Instance.getFreeContext();

            dbSet = context.Set<TEntity>();
            context.SaveChanges();
            context.IsUsing = false;
        }
        ~BaseRepository()
        {

        }
    }
}
