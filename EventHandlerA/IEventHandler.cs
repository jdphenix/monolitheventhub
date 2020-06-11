using System.Threading.Tasks;

namespace EventHandlerA
{
    public interface IEventHandler<in T>
    {
        Task<IHandleResult> Handle(T evt);
    }
}