using System;
using System.Data.Linq;
using System.Linq;
using System.Linq.Expressions;

namespace Azure.Toolkit
{
    public class LinqToSqlRepository<T> : IRepository<T>, IDisposable
        where T : class, new()
    {
        readonly DataContext _dataContext;
        bool _disposed;

        public LinqToSqlRepository(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void Commit()
        {
            _dataContext.SubmitChanges();
        }

        public void Delete(T itemToDelete)
        {
            var table = LookupTableFor(typeof(T));
            table.DeleteOnSubmit(itemToDelete);
        }

        public void Delete(T itemToDelete, bool attach)
        {
            var table = LookupTableFor(typeof(T));
            if (attach)
                table.Attach(itemToDelete);
            table.DeleteOnSubmit(itemToDelete);
        }

        public T Find(Expression<Func<T, bool>> criteria)
        {
            return LookupTableFor(typeof(T)).Cast<T>().Where(criteria).FirstOrDefault();
        }

        public IQueryable<T> FindAll()
        {
            return LookupTableFor(typeof(T)).Cast<T>();
        }

        public IQueryable<T> FindAll(Expression<Func<T, bool>> criteria)
        {
            return LookupTableFor(typeof(T)).Cast<T>().Where(criteria);
        }

        public void Insert(T itemToInsert)
        {
            var table = LookupTableFor(typeof(T));
            table.InsertOnSubmit(itemToInsert);
        }

        public void Update(T itemToUpdate, bool attach)
        {
            if (!attach)
                return;
            var table = LookupTableFor(typeof(T));
            table.Attach(itemToUpdate);
        }

        public void Update(T itemToUpdate)
        {
            // no op because we don't have to do this for LinqToSql
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    if (_dataContext != null)
                        _dataContext.Dispose();
                }
            }
            _disposed = true;
        }

        public IQueryable<T> Find()
        {
            return LookupTableFor(typeof(T)).Cast<T>();
        }

        ITable LookupTableFor(Type entityType)
        {
            return _dataContext.GetTable(entityType);
        }
    }
}
