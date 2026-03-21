using System;

namespace DetectEncoding.exception
{
    internal class DetectEncodeException : Exception
    {
        public DetectEncodeException(string message, Exception innerException)
            : base(message, innerException)
        {

        }
    }
}
