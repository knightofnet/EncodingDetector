using System;
using System.IO;
using System.Linq;

using DetectEncoding.constant;
using TextEncodingDetect = DetectEncoding.business.TextEncodingDetect;

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
            EnumEol enumRet = EnumEol.NONE;

            int[] tabInt = new int[500];

            using (StreamReader sr = StreamUtils.GetStreamReaderFromEAppEncoding(filename, inEncoding))
            {
                // Il faut au minimum 2 caractères pour déterminer le EOL
                if (sr.BaseStream.Length < 2)
                {
                    return enumRet;
                }

                int charAtN1 = 0;
                int charAtN = 0;
                int positionN = 0;


                int i = 0;
                while (sr.Peek() >= 0)
                {
                    positionN++;

                    if (positionN > 1)
                    {
                        charAtN1 = charAtN;
                    }
                    charAtN = sr.Read();


                    tabInt[i++] = charAtN;

                    // Il faut au minimum 2 caractères pour déterminer le EOL
                    if (positionN <= 1) continue;

                    if (charAtN == 10 && charAtN1 == 13)
                    {
                        //Console.WriteLine(string.Join(" ", tabInt));
                        enumRet = EnumEol.DOS;
                    }
                    else if (charAtN1 == 10)
                    {
                        enumRet = EnumEol.UNIX;
                    }
                    else if (charAtN1 == 13)
                    {
                        enumRet = EnumEol.MACOS;
                    }

                    if (enumRet != EnumEol.NONE)
                    {
                        return enumRet;
                    }
                }

            }

            //Console.WriteLine(string.Join(" ", tabInt));


            return enumRet;
        }

    }
}
