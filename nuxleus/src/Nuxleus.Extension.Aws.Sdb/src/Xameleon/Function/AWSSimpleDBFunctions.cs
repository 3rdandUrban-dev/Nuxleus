using System.IO;
using System.Collections;
using com.amazon.sdb;

namespace Xameleon.Function 
{
    public class AWSSimpleDB {
    
        Sdb _sdb;
        
        public AWSSimpleDB()
        {
            createSdb(System.Environment.GetEnvironmentVariable("SDB_ACCESS_KEY"), System.Environment.GetEnvironmentVariable("SDB_SECRET_KEY"));
        }
        
        public AWSSimpleDB(string awsAccessKey, string awsSecretKey)
        {
            createSdb(awsAccessKey, awsSecretKey);
        }
        
        private void createSdb(string awsAccessKey, string awsSecretKey)
        {
            SdbConnection sdbConnection = new SdbConnection(awsAccessKey, awsSecretKey);
            _sdb = new Sdb(sdbConnection);
        }
    
        public void CreateDomain(string domainName) 
        {
            //System.Console.WriteLine(domainName);
            try {
              _sdb.CreateDomain(domainName);
            } catch (SdbException ex) {
              handleException(ex);
            }
        }
        public void PutAttributes(string domain, string item, ArrayList attributes) 
        {
            Item myItem = GetItem(GetDomain(domain), item);
            
            try {
              myItem.PutAttributes(attributes);
            } catch (SdbException ex) {
              handleException(ex);
            }
        }

        public void DeleteAttribute(string domain, string item, ArrayList attributes) 
        {
            Item myItem = GetItem(GetDomain(domain), item);
            try {
              myItem.DeleteAttributes(attributes);
            } catch (SdbException ex) {
              handleException(ex);
            }
        }
        public void DeleteAttribute(string domain, string item, string attName) 
        {
            Item myItem = GetItem(GetDomain(domain), item);
            ArrayList attribute = new ArrayList();
            attribute.Add(new Attribute(attName));
            try {
              myItem.DeleteAttributes(attribute);
            } catch (SdbException ex) {
              handleException(ex);
            }
        }
        public void DeleteAttribute(string domain, string item, string attName, string attValue) 
        {
            Item myItem = GetItem(GetDomain(domain), item);
            ArrayList attribute = new ArrayList();
            attribute.Add(new Attribute(attName, attValue));
            try {
              myItem.DeleteAttributes(attribute);
            } catch (SdbException ex) {
              handleException(ex);
            }
        }
        public void DeleteItem(string domain, string item) 
        {
            Item myItem = GetItem(GetDomain(domain), item);
            try {
              DeleteItem(myItem);
            } catch (SdbException ex) {
              handleException(ex);
            }
        }
        public void DeleteItem(Item item) 
        {
            try {
              item.DeleteAttributes();
            } catch (SdbException ex) {
              handleException(ex);
            }
        }
        public Domain GetDomain(string domainName) 
        {
            try {
              return _sdb.GetDomain(domainName);
            } catch (SdbException ex) {
              handleException(ex);
              return null;
            }
        }
        public GetAttributesResponse GetAttribute(string domain, string item) 
        {
            Item myItem = GetItem(GetDomain(domain), item);
            try {
              return GetAttribute(myItem);
            } catch (SdbException ex) {
              handleException(ex);
              return null;
            }
        }
        public GetAttributesResponse GetAttribute(Item item) 
        {
            try {
              return item.GetAttributes();
            } catch (SdbException ex) {
              handleException(ex);
              return null;
            }
        }
        public Item GetItem (string domain, string item) 
        {
            Domain myDomain = GetDomain(domain);
            try {
              return GetItem(myDomain, item);
            } catch (SdbException ex) {
              handleException(ex);
              return null;
            }
        }
        public Item GetItem (Domain domain, string item) 
        {
            try {
              return domain.GetItem(item);
            } catch (SdbException ex) {
              handleException(ex);
              return null;
            }
        }
        public ArrayList GetArrayList() 
        {
            return new ArrayList();
        }
        public void AddAttribute(ArrayList arrayList, string attName, string attValue) 
        {
            arrayList.Add(new Attribute(attName, attValue));
        }
        public QueryResponse QueryDomain (string domain, string attName, string attValue) 
        {
            Domain myDomain = GetDomain(domain);
            return myDomain.Query("[\"attName\" = \"attValue\"]");
        }
        public static void handleException(SdbException ex)
        {
            System.Console.WriteLine("Failure: {0}: {1} ({2})", ex.ErrorCode, ex.Message, ex.RequestId);
        }

    }
}