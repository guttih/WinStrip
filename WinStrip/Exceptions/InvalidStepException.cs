using System;
using System.Runtime.Serialization;

namespace WinStrip
{
    [Serializable]
    internal class InvalidStepException : Exception
    {
        public string From { get; set; }
        public InvalidStepException()
        {
        }

        public InvalidStepException(string message) : base(message)
        {
        }

        public InvalidStepException(string from, string message) : base(message)
        {
            From = from;
        }

        public InvalidStepException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected InvalidStepException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}