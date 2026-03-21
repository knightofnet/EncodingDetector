using System.IO;

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
            if (array.Length == 0)
            {
                return TextEncodingDetect.Encoding.None;
            }

            return textDetect.DetectEncoding(array, array.Length);
        }

        public static EnumEol DetectEol(string filename, EnumAppEncoding inEncoding)
        {
            EnumEol enumRet = EnumEol.NONE;

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

                while (sr.Peek() >= 0)
                {
                    positionN++;

                    if (positionN > 1)
                    {
                        charAtN1 = charAtN;
                    }
                    charAtN = sr.Read();

                    // Il faut au minimum 2 caractères pour déterminer le EOL
                    if (positionN <= 1) continue;

                    if (charAtN == 10 && charAtN1 == 13)
                    {
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

            return enumRet;
        }

    }
}
