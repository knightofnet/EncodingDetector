using System;
using AutoIt.Common;
using DetectEncoding.constant;

namespace DetectEncoding.utils
{
    public static class MiscAppUtils
    {
        public delegate void Cdtl();

        public static EnumAppEncoding FromTextEncoding(TextEncodingDetect.Encoding result)
        {
            if (TextEncodingDetect.Encoding.Ansi.Equals(result))
            {
                return EnumAppEncoding.ANSI;
            }

            if (TextEncodingDetect.Encoding.Ascii.Equals(result))
            {
                return EnumAppEncoding.ASCII;
            }

            if (TextEncodingDetect.Encoding.Utf8Bom.Equals(result))
            {
                return EnumAppEncoding.UTF8_BOM;
            }
            if (TextEncodingDetect.Encoding.Utf8Nobom.Equals(result))
            {
                return EnumAppEncoding.UTF8_NOBOM;
            }

            if (TextEncodingDetect.Encoding.Utf16BeBom.Equals(result))
            {
                return EnumAppEncoding.UTF16BE_BOM;
            }
            if (TextEncodingDetect.Encoding.Utf16BeNoBom.Equals(result))
            {
                return EnumAppEncoding.UTF16BE_NOBOM;
            }

            if (TextEncodingDetect.Encoding.Utf16LeBom.Equals(result))
            {
                return EnumAppEncoding.UTF16LE_BOM;
            }
            if (TextEncodingDetect.Encoding.Utf16LeNoBom.Equals(result))
            {
                return EnumAppEncoding.UTF16LE_NOBOM;
            }
            return null;
        }

        public static void ConditionnalExecCode(int batchSilenceLvl, int targetSilenceLevel, Cdtl delegat)
        {
            if (batchSilenceLvl < targetSilenceLevel)
            {
                delegat.Invoke();
            }
        }

        public static void ConditionnalWrtLine(int batchSilenceLvl, int targetSilenceLevel)
        {
            if (batchSilenceLvl < targetSilenceLevel)
            {
                Console.WriteLine();
            }
        }

        public static void ConditionnalWrtLine(int batchSilenceLvl, int targetSilenceLevel, string format)
        {
            if (batchSilenceLvl < targetSilenceLevel)
            {
                Console.WriteLine(format);
            }
        }

        public static void ConditionnalWrtLine(int batchSilenceLvl, int targetSilenceLevel, string format, object arg0)
        {
            if (batchSilenceLvl < targetSilenceLevel)
            {
                Console.WriteLine(format, arg0);
            }
        }

        public static void ConditionnalWrtLine(int batchSilenceLvl, int targetSilenceLevel, string format, object arg0, object arg1)
        {
            if (batchSilenceLvl < targetSilenceLevel)
            {
                Console.WriteLine(format, arg0, arg1);
            }
        }

    }
}
