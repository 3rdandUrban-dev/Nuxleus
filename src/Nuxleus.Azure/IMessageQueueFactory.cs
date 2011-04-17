
namespace Azure.Toolkit
{
    public interface IMessageQueueFactory
    {
        IMessageQueue<T> Create<T>(string queueName);
    }
}
