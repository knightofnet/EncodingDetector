using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoIt.Common;
using DetectEncoding.constant;

namespace DetectEncoding.utils
{
    public static class MiscAppUtils
    {
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
    }
}
