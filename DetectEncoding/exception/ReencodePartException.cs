using System;

namespace DetectEncoding.exception
{
    internal class ReencodePartException : Exception
    {
        public ReencodePartException(string message, Exception innerException)
            : base(message, innerException)
        {

        }
    }
}
