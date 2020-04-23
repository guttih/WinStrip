using System;
using System.Runtime.Serialization;

namespace WinStrip
{
    [Serializable]
    internal class ClipboardToRowsException : Exception
    {
        public ClipboardToRowsException()
        {
        }

        public ClipboardToRowsException(string message) : base(message)
        {
        }

        public ClipboardToRowsException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected ClipboardToRowsException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}