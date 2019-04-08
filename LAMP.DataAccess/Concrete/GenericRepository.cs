using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace LAMP.DataAccess
{    
    /// <summary>
    /// GenericRepository is responsible for handling database acitivities
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private DbContext Context;

        public GenericRepository(DbContext context)
        {
            Context = context;
        }

        public virtual IQueryable<T> AsQueryable()
        {
            return Context.Set<T>().AsQueryable();
        }

        public virtual IEnumerable<T> GetWithRawSql(string query, params object[] parameters)
        {
            return Context.Set<T>().SqlQuery(query, parameters);
        }
        public IEnumerable<T> GetAll()
        {
            return Context.Set<T>().ToList();
        }

        public IEnumerable<T> Find(System.Linq.Expressions.Expression<Func<T, bool>> predicate)
        {
            return Context.Set<T>().Where(predicate);
        }

        public T Single(System.Linq.Expressions.Expression<Func<T, bool>> predicate)
        {
            return Context.Set<T>().Single(predicate);
        }

        public T SingleOrDefault(System.Linq.Expressions.Expression<Func<T, bool>> predicate)
        {
            return Context.Set<T>().SingleOrDefault(predicate);
        }

        public T First(System.Linq.Expressions.Expression<Func<T, bool>> predicate)
        {
            return Context.Set<T>().First(predicate);
        }

        public T GetById(long id)
        {
            return Context.Set<T>().Find(id);
        }

        public void Add(IEnumerable<T> entity)
        {
            Context.Set<T>().AddRange(entity);
        }

        public void Delete(IEnumerable<T> entity)
        {
            Context.Set<T>().RemoveRange(entity);
        }

        public void Add(T entity)
        {
            Context.Set<T>().Add(entity);
        }

        public void Delete(T entity)
        {
            Context.Set<T>().Remove(entity);
        }

        public void Update(T entity)
        {
            Context.Entry(entity).State = EntityState.Modified;
        }

        
        //For performance improvement
        public IQueryable<T> RetrieveAll()
        {
            return Context.Set<T>();
        }

        public IQueryable<T> FindBy(System.Linq.Expressions.Expression<Func<T, bool>> predicate)
        {
            return Context.Set<T>().Where(predicate);
        }

        public void Add(IQueryable<T> entity)
        {
            Context.Set<T>().AddRange(entity);
        }

        public void Delete(IQueryable<T> entity)
        {
            Context.Set<T>().RemoveRange(entity);
        }

        public IEnumerable<T> ExecWithStoreProcedure(string query, params object[] parameters)
        {
            return Context.Database.SqlQuery<T>(query, parameters);
        }

        // For Direct SQL
        public int ExecuteSqlCommand(string sql)
        {
            return Context.Database.ExecuteSqlCommand(sql);
        }

        public int ExecuteSqlCommand(string sql, object[] parameters)
        {
            return Context.Database.ExecuteSqlCommand(sql, parameters);
        }
    }
}
