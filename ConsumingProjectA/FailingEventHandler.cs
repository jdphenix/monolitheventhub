using System.Threading.Tasks;
using EventHandlerA;

namespace ConsumingProjectA
{
    internal class FailingEventHandler : IEventHandler<string>
    {
        public Task<IHandleResult> Handle(string evt)
        {
            return Task.FromResult(ResultFactory.CreateFailure("Damnit"));
        }
    }
}