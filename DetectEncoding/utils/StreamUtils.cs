using System.IO;
using System.Text;
using DetectEncoding.constant;

namespace DetectEncoding.utils
{
    class StreamUtils
    {
        public static StreamReader GetStreamReaderFromEAppEncoding(string inputFileName, EnumAppEncoding encoding)
        {
            if (encoding.Equals(EnumAppEncoding.UTF8_NOBOM) || encoding.Equals(EnumAppEncoding.UTF8_BOM))
            {
                return new StreamReader(File.Open(inputFileName, FileMode.Open), new UTF8Encoding(encoding.OptionWithUtf8Bom));
            }

            if (encoding.Equals(EnumAppEncoding.UTF16BE_NOBOM) || encoding.Equals(EnumAppEncoding.UTF16BE_BOM))
            {
                return new StreamReader(File.Open(inputFileName, FileMode.Open), new UnicodeEncoding(true, encoding.OptionWithUtf8Bom));
            }

            if (encoding.Equals(EnumAppEncoding.UTF16LE_NOBOM) || encoding.Equals(EnumAppEncoding.UTF16LE_BOM))
            {
                return new StreamReader(File.Open(inputFileName, FileMode.Open), new UnicodeEncoding(false, encoding.OptionWithUtf8Bom));
            }


            return new StreamReader(File.Open(inputFileName, FileMode.Open), encoding.CsEncoding);
        }

        public static StreamWriter GetStreamWriterFromEAppEncoding(string outFileName, EnumAppEncoding encoding)
        {
            if (encoding.Equals(EnumAppEncoding.UTF8_NOBOM) || encoding.Equals(EnumAppEncoding.UTF8_BOM))
            {
                return new StreamWriter(File.Open(outFileName, FileMode.Create), new UTF8Encoding(encoding.OptionWithUtf8Bom));
            }

            if (encoding.Equals(EnumAppEncoding.UTF16BE_NOBOM) || encoding.Equals(EnumAppEncoding.UTF16BE_BOM))
            {
                return new StreamWriter(File.Open(outFileName, FileMode.Create), new UnicodeEncoding(true, encoding.OptionWithUtf8Bom));
            }

            if (encoding.Equals(EnumAppEncoding.UTF16LE_NOBOM) || encoding.Equals(EnumAppEncoding.UTF16LE_BOM))
            {
                return new StreamWriter(File.Open(outFileName, FileMode.Create), new UnicodeEncoding(false, encoding.OptionWithUtf8Bom));
            }

            return new StreamWriter(File.Open(outFileName, FileMode.Create), encoding.CsEncoding);
        }
    }
}
