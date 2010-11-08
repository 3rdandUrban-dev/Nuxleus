using System.IO;
using System.Text;
using System.Xml.Serialization;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.StorageClient;

namespace Azure.Toolkit.Azure
{
    public class MessageQueue<T> : IMessageQueue<T>
    {
        readonly CloudQueue _queue;
        CloudQueueMessage _currentMessage;

        public MessageQueue(string queueName)
        {
            var storageAccount = CloudStorageAccount.FromConfigurationSetting("DataConnectionString");
            CloudQueueClient client = storageAccount.CreateCloudQueueClient();
            _queue = client.GetQueueReference(queueName);

            _queue.CreateIfNotExist();
        }

        public void AddMessage(T message)
        {
            using (var m = new MemoryStream())
            {
                var xs = new XmlSerializer(typeof(T));
                xs.Serialize(m, message);
                string userXml = Encoding.ASCII.GetString(m.ToArray());
                var cloudQueueMessage = new CloudQueueMessage(userXml);

                _queue.AddMessage(cloudQueueMessage);
            }
        }

        public void DeleteMessage()
        {
            _queue.DeleteMessage(_currentMessage);
        }

        public T GetMessage()
        {
            _currentMessage = _queue.GetMessage();
            if (_currentMessage == null)
                return default(T);

            using (var m = new MemoryStream(Encoding.ASCII.GetBytes(_currentMessage.AsString)))
            {
                var xs = new XmlSerializer(typeof(T));
                return (T)xs.Deserialize(m);
            }
        }
    }
}
