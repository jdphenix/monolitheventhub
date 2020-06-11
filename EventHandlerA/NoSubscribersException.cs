using System;

namespace EventHandlerA
{
    public class NoSubscribersException : Exception
    {
        private readonly object _evt;

        public NoSubscribersException(Type type, object evt)
        {
            _evt = evt;
            Type = type;
        }

        public Type Type { get; }

        public override string ToString()
        {
            return $"Event type: {Type.FullName}: {_evt}\n{base.ToString()}";
        }
    }
}