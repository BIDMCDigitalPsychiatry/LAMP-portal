using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace LAMP.DataAccess
{    
    /// <summary>
    /// Interface IGenericRepository for capable of class GenericRepository
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IGenericRepository<T> where T : class
    {
        IQueryable<T> AsQueryable();
        IEnumerable<T> GetAll();
        IEnumerable<T> GetWithRawSql(string query, params object[] parameters);
        IEnumerable<T> Find(Expression<Func<T, bool>> predicate);
        T Single(Expression<Func<T, bool>> predicate);
        T SingleOrDefault(Expression<Func<T, bool>> predicate);
        T First(Expression<Func<T, bool>> predicate);
        T GetById(long id);
        void Add(IEnumerable<T> entity);
        void Delete(IEnumerable<T> entity);
        void Add(T entity);
        void Delete(T entity);
        void Update(T entity);

       
        //For Performance improvement
        IQueryable<T> RetrieveAll();
        IQueryable<T> FindBy(Expression<Func<T, bool>> predicate);
        void Add(IQueryable<T> entity);
        void Delete(IQueryable<T> entity);
        IEnumerable<T> ExecWithStoreProcedure(string query, params object[] parameters);

        // For Direct SQL
        int ExecuteSqlCommand(string sql);
        int ExecuteSqlCommand(string sql, object[] parameters);
    }
}
