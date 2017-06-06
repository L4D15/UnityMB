
namespace Becerra.UnityMB
{
    public interface IHandler<T> where T : IMessage
    {
        void Handle(T message);
    }
}
