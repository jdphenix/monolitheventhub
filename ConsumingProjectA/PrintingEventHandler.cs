using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using EventHandlerA;

namespace ConsumingProjectA
{
    internal class PrintingEventHandler<T> : IEventHandler<T>
    {
        private readonly string _message;
        private readonly TextWriter _writer;

        public PrintingEventHandler(TextWriter writer, string message = null)
        {
            Debug.Assert(writer != null, nameof(writer) + " != null");
            _writer = writer;
            _message = message;
        }

        public Task<IHandleResult> Handle(T evt)
        {
            _writer.WriteLine($"{typeof(T).Name}: {_message}: {evt}");
            return Task.FromResult(ResultFactory.Success);
        }
    }
}