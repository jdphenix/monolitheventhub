using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;

namespace EventHandlerA
{
    public class EventHubBuilder
    {
        private readonly IDictionary<Type, IList<object>> _events;
        private object _exceptionFactory;
        private Type _exceptionType;
        private bool _throwsOnNoSubscriber;
        private bool _throwsOnResultFailure;

        public EventHubBuilder()
        {
            _events = new Dictionary<Type, IList<object>>();
        }

        public EventHub CreateHub()
        {
            var events =
                _events.ToImmutableDictionary(kvp => kvp.Key, kvp => kvp.Value.ToImmutableArray());

            if (_throwsOnResultFailure) return CreateThrowingHub(events);

            return new EventHub(events, _throwsOnNoSubscriber);
        }

        private EventHub CreateThrowingHub(ImmutableDictionary<Type, ImmutableArray<object>> events)
        {
            var hubType = typeof(ThrowingEventHub<>).MakeGenericType(_exceptionType);
            var ctor = hubType.GetConstructors(BindingFlags.NonPublic | BindingFlags.Instance).Single();
            return (EventHub) ctor.Invoke(new[] {events, _throwsOnNoSubscriber, _exceptionFactory});
        }

        public void AddSubscriber<T>(IEventHandler<T> handler)
        {
            var type = typeof(T);
            var handlers = GetHandlers(type);
            handlers.Add(handler);
            _events[type] = handlers;
        }

        /// <summary>
        ///     Throw an exception if no subscriber can handle an event.
        /// </summary>
        public void ThrowsOnNoSubscriber()
        {
            _throwsOnNoSubscriber = true;
        }

        /// <summary>
        ///     Throw an exception of the given type using the given factory
        ///     upon a failed handle result.
        /// </summary>
        public void ThrowsOnResultFailure<TException>(Func<object, TException> factory)
        {
            _throwsOnResultFailure = true;
            _exceptionType = typeof(TException);
            _exceptionFactory = factory;
        }

        private IList<object> GetHandlers(Type type)
        {
            _ = _events.TryGetValue(type, out var handlers);
            return handlers ?? new List<object>();
        }
    }
}