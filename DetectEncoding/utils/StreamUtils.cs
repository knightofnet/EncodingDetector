using System.IO;
using System.Text;
using DetectEncoding.constant;

namespace DetectEncoding.utils
{
    internal class StreamUtils
    {
        public static StreamReader GetStreamReaderFromEAppEncoding(string inputFileName, EnumAppEncoding encoding)
        {
            Encoding csEncoding;
            if (encoding.Equals(EnumAppEncoding.UTF8_NOBOM) || encoding.Equals(EnumAppEncoding.UTF8_BOM))
            {
                csEncoding = new UTF8Encoding(encoding.OptionWithUtf8Bom);
            }
            else if (encoding.Equals(EnumAppEncoding.UTF16BE_NOBOM) || encoding.Equals(EnumAppEncoding.UTF16BE_BOM))
            {
                csEncoding = new UnicodeEncoding(true, encoding.OptionWithUtf8Bom);
            }
            else if (encoding.Equals(EnumAppEncoding.UTF16LE_NOBOM) || encoding.Equals(EnumAppEncoding.UTF16LE_BOM))
            {
                csEncoding = new UnicodeEncoding(false, encoding.OptionWithUtf8Bom);
            }
            else
            {
                csEncoding = encoding.CsEncoding;
            }

            return new StreamReader(File.OpenRead(inputFileName), csEncoding);
        }

        public static StreamWriter GetStreamWriterFromEAppEncoding(string outFileName, EnumAppEncoding encoding)
        {
            Encoding csEncoding;
            if (encoding.Equals(EnumAppEncoding.UTF8_NOBOM) || encoding.Equals(EnumAppEncoding.UTF8_BOM))
            {
                csEncoding = new UTF8Encoding(encoding.OptionWithUtf8Bom);
            }
            else if (encoding.Equals(EnumAppEncoding.UTF16BE_NOBOM) || encoding.Equals(EnumAppEncoding.UTF16BE_BOM))
            {
                csEncoding = new UnicodeEncoding(true, encoding.OptionWithUtf8Bom);
            }
            else if (encoding.Equals(EnumAppEncoding.UTF16LE_NOBOM) || encoding.Equals(EnumAppEncoding.UTF16LE_BOM))
            {
                csEncoding = new UnicodeEncoding(false, encoding.OptionWithUtf8Bom);
            }
            else
            {
                csEncoding = encoding.CsEncoding;
            }

            return new StreamWriter(File.Open(outFileName, FileMode.Create, FileAccess.Write), csEncoding);
        }
    }
}
