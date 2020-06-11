using System;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Threading.Tasks;

namespace EventHandlerA
{
    public class EventHub
    {
        private readonly ImmutableDictionary<Type, ImmutableArray<object>> _events;
        private readonly bool _throwOnNoSubscriber;

        internal EventHub(
            ImmutableDictionary<Type, ImmutableArray<object>> events,
            bool throwOnNoSubscriber)
        {
            Debug.Assert(events != null, nameof(events) + " != null");

            _events = events;
            _throwOnNoSubscriber = throwOnNoSubscriber;
        }

        public async Task Dispatch<T>(T evt)
        {
            var tasks = GetTasks(evt);
            await Task.WhenAll(tasks).ConfigureAwait(false);
        }

        private ImmutableArray<object> GetHandlers(Type type)
        {
            _ = _events.TryGetValue(type, out var handlers);
            return handlers;
        }

        private Task<IHandleResult>[] GetTasks<T>(T evt)
        {
            var handlers = GetHandlers(typeof(T));
            if (_throwOnNoSubscriber && handlers.IsDefault) throw new NoSubscribersException(typeof(T), evt);
            if (handlers.IsDefault) return new Task<IHandleResult>[] { };

            var tasks = new Task<IHandleResult>[handlers.Length];

            for (var i = 0; i < tasks.Length; i++)
            {
                var handler = handlers[i] as IEventHandler<T>;
                Debug.Assert(handler != null, nameof(handler) + " != null");

                tasks[i] = Handle(handler, evt);
            }

            return tasks;
        }

        protected virtual Task<IHandleResult> Handle<T>(IEventHandler<T> handler, T evt)
        {
            return handler.Handle(evt);
        }
    }
}