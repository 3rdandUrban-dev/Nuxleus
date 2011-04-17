using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Azure.Toolkit.Azure;

namespace Azure.Toolkit
{

    public class MessageQueueFacory : IMessageQueueFactory
    {
        public IMessageQueue<T> Create<T>(string queueName)
        {
            return new MessageQueue<T>(queueName);
        }
    }
    
}
