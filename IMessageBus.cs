
namespace Becerra.UnityMB
{
    public interface IMessageBus
    {
        void Subscribe(object subscriber);
        void Unsubscribe(object subscriber);
        void Publish<T>(T message) where T : IMessage;
    }
}
