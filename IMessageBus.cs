
namespace Becerra.UnityMB
{
    public interface IMessageBus
    {
        void Subscribe<T>(IHandler<T> subscriber) where T : IMessage;
        void Unsubscribe<T>(IHandler<T> subscriber) where T : IMessage;
        void Publish<T>(T message) where T : IMessage;
    }
}
