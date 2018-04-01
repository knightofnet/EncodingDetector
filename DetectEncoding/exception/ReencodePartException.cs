using System;

namespace DetectEncoding.exception
{
    class ReencodePartException : Exception
    {
        public ReencodePartException(string message, Exception innerException)
            : base(message, innerException)
        {

        }
    }
}
