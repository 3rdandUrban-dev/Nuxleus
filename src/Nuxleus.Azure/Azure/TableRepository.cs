using System;
using System.Collections.Generic;
using System.Data.Services.Client;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.WindowsAzure.StorageClient;
using Microsoft.WindowsAzure;

namespace Azure.Toolkit.Azure
{
    public class TableRepository<T> : IRepository<T>
        where T : TableServiceEntity, new()
    {
        const string Entity = "Entity";
        readonly string _tablename;
        readonly TableServiceContext _tableServiceContext;

        public TableRepository()
        {
            // Create the service context we'll query against
            // Get the settings from the Service Configuration file
            CloudStorageAccount account = CloudStorageAccount.FromConfigurationSetting("DataConnectionString");

            // Create the service context we'll query against
            _tableServiceContext = account.CreateCloudTableClient().GetDataServiceContext();
            //_tableServiceContext = tableServiceContext;
             _tableServiceContext.RetryPolicy = RetryPolicies.Retry(3, TimeSpan.FromSeconds(1));
            _tablename = typeof(T).Name;

            account.CreateCloudTableClient().CreateTableIfNotExist(_tablename);
        }

        public void Commit()
        {
            _tableServiceContext.SaveChanges();
        }

        public void Delete(T itemToDelete)
        {
            Delete(itemToDelete, false);
        }

        public void Delete(T itemToDelete, bool attach)
        {
            if (attach)
                _tableServiceContext.AttachTo(_tablename, itemToDelete, "*");
            _tableServiceContext.DeleteObject(itemToDelete);
            //_tableServiceContext.SaveChanges();
        }

        public T Find(Expression<Func<T, bool>> criteria)
        {
            var table = _tableServiceContext.CreateQuery<T>(_tablename);
            var query = new CloudTableQuery<T>(table, _tableServiceContext.RetryPolicy);
            return query.Where(criteria).FirstOrDefault();
        }

        public IQueryable<T> FindAll()
        {
            var table = _tableServiceContext.CreateQuery<T>(_tablename);
            return new CloudTableQuery<T>(table, _tableServiceContext.RetryPolicy);
        }

        public IQueryable<T> FindAll(Expression<Func<T, bool>> criteria)
        {
            var table = _tableServiceContext.CreateQuery<T>(_tablename);
            var results = table.Where(criteria);
            return new CloudTableQuery<T>(results as DataServiceQuery<T>, _tableServiceContext.RetryPolicy);
        }

        public void Insert(T newItem)
        {
            _tableServiceContext.AddObject(_tablename, newItem);
        }

        public void Update(T itemToUpdate, bool attach)
        {
            if (attach)
                _tableServiceContext.AttachTo(_tablename, itemToUpdate, "*");
            _tableServiceContext.UpdateObject(itemToUpdate);
            //_tableServiceContext.SaveChanges();
        }

        public void Update(T itemToUpdate)
        {
            Update(itemToUpdate, false);
        }

        public int Count()
        {
            var table = _tableServiceContext.CreateQuery<T>(_tablename);
            var results = from c in table
                          select c;

            var query = new CloudTableQuery<T>(results as DataServiceQuery<T>, _tableServiceContext.RetryPolicy);
            IEnumerable<T> queryResults = query.Execute();
            return queryResults.Count();
        }
    }
}
