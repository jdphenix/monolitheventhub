using System;

namespace ConsumingProjectA
{
    internal class BusinessException : Exception
    {
        public BusinessException(string message) : base(message)
        {
        }
    }
}