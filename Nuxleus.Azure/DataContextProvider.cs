using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ninject.Activation;
using System.Data.Linq;
using System.Configuration;

namespace Azure.Toolkit
{
    public class DataContextProvider : Provider<DataContext>
    {
        const string ConnectionStringName = "SQLAzure";

        protected override DataContext CreateInstance(IContext context)
        {
            string connectionString = ConfigurationManager.ConnectionStrings[ConnectionStringName].ConnectionString;
            return new DataContext(connectionString);
        }
    }
}
