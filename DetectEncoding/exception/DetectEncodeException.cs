using System;

namespace DetectEncoding.exception
{
    class DetectEncodeException : Exception
    {
        public DetectEncodeException(string message, Exception innerException)
            : base(message, innerException)
        {

        }
    }
}
