using System;
using System.Linq;
using System.Linq.Expressions;

namespace Azure.Toolkit
{
    public interface IRepository<T>
    {
        void Commit();
        void Delete(T itemToDelete);
        void Delete(T itemToDelete, bool attach);
        T Find(Expression<Func<T, bool>> criteria);
        IQueryable<T> FindAll();
        IQueryable<T> FindAll(Expression<Func<T, bool>> criteria);
        void Insert(T itemToInsert);
        void Update(T itemToUpdate, bool attach);
        void Update(T itemToUpdate);
    }
}
