using System;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Threading.Tasks;

namespace EventHandlerA
{
    public class ThrowingEventHub<T> : EventHub where T : Exception
    {
        private readonly Func<object, T> _exceptionFactory;

        internal ThrowingEventHub(
            ImmutableDictionary<Type, ImmutableArray<object>> events,
            bool throwOnNoSubscriber,
            Func<object, T> exceptionFactory) :
            base(events, throwOnNoSubscriber)
        {
            Debug.Assert(exceptionFactory != null, nameof(exceptionFactory) + " != null");
            _exceptionFactory = exceptionFactory;
        }

        protected override async Task<IHandleResult> Handle<TEvent>(IEventHandler<TEvent> handler, TEvent evt)
        {
            var result = await handler.Handle(evt).ConfigureAwait(false);

            if (result.IsFailure) throw _exceptionFactory(evt);

            return result;
        }
    }
}