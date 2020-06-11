namespace EventHandlerA
{
    public static class ResultFactory
    {
        public static IHandleResult Success => new SuccessResult();

        public static IHandleResult CreateFailure<T>(T tag)
        {
            return new FailureResult<T>(tag);
        }

        private class SuccessResult : IHandleResult
        {
            public bool IsFailure => false;
        }

        private class FailureResult<T> : IHandleResult
        {
            public FailureResult(T tag)
            {
                Tag = tag;
            }

            public T Tag { get; }

            public bool IsFailure => true;
        }
    }
}