using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ninject.Activation;
using Microsoft.WindowsAzure.StorageClient;
using Microsoft.WindowsAzure;

namespace Azure.Toolkit.Azure
{
    public class TableServiceContextProvider : Provider<TableServiceContext>
    {
        const string ConnectionStringName = "DataConnectionString";

        protected override TableServiceContext CreateInstance(IContext context)
        {
            CloudStorageAccount account = CloudStorageAccount.FromConfigurationSetting(ConnectionStringName);
            return account.CreateCloudTableClient().GetDataServiceContext();
        }
    }
}
