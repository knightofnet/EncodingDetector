using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using AutoIt.Common;
using DetectEncoding.constant;

namespace DetectEncoding.utils
{
    public static class DetectorsUtils
    {

        public static TextEncodingDetect.Encoding DetectEncoding(string filename)
        {
            var textDetect = new TextEncodingDetect();

            byte[] array = File.ReadAllBytes(filename);
            TextEncodingDetect.Encoding encoding = textDetect.DetectEncoding(array, array.Length - 1);
            return encoding;


        }

        public static EnumEol DetectEol(string filename, EnumAppEncoding inEncoding)
        {
            EnumEol enumRet = EnumEol.UNIX;

            using (StreamReader sr = StreamUtils.GetStreamReaderFromEAppEncoding(filename, inEncoding))
            {

                if (sr.BaseStream.Length < 2)
                {
                    return enumRet;
                }

                int n1 = 0;
                int c = 0;
                int p = 0;
                while (sr.Peek() >= 0)
                {
                    p++;

                    if (p > 1)
                    {
                        n1 = c;
                    }

                    c = sr.Read();

                    if (c == 10 && p > 1)
                    {
                        enumRet = n1 == 13 ? EnumEol.DOS : EnumEol.UNIX;
                    }

                }

            }

            return enumRet;
        }

    }
}
