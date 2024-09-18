using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AryxDevLibrary.utils;

namespace DetectEncoding.constant
{
    public class EnumAppEncoding
    {

        public static readonly EnumAppEncoding UTF8_BOM = new EnumAppEncoding("UTF8_BOM", Encoding.UTF8, true);
        public static readonly EnumAppEncoding UTF8_NOBOM = new EnumAppEncoding("UTF8_NOBOM", Encoding.UTF8, false);
        public static readonly EnumAppEncoding ANSI = new EnumAppEncoding("ANSI", Encoding.Default, false);

        public static readonly EnumAppEncoding UTF16LE_BOM = new EnumAppEncoding("UTF16LE", Encoding.Unicode, true);
        public static readonly EnumAppEncoding UTF16BE_BOM = new EnumAppEncoding("UTF16BE", Encoding.Unicode, true);

        public static readonly EnumAppEncoding UTF16LE_NOBOM = new EnumAppEncoding("UTF16LE_NOBOM", Encoding.Unicode, false);
        public static readonly EnumAppEncoding UTF16BE_NOBOM = new EnumAppEncoding("UTF16BE_NOBOM", Encoding.Unicode, false);

        public static readonly EnumAppEncoding ASCII = new EnumAppEncoding("ASCII", Encoding.ASCII, false);



        public static IEnumerable<EnumAppEncoding> Values
        {
            get
            {
                yield return UTF8_BOM;
                yield return UTF8_NOBOM;
                yield return ANSI;
                yield return UTF16LE_BOM;
                yield return UTF16BE_BOM;
                yield return UTF16LE_NOBOM;
                yield return UTF16BE_NOBOM;
                yield return ASCII;

            }
        }

        public string Libelle { get; private set; }
        public Encoding CsEncoding { get; private set; }
        public bool OptionWithUtf8Bom { get; private set; }



        private EnumAppEncoding(string libelle, Encoding csEncoding, bool optionWithBom)
        {
            Libelle = libelle;
            CsEncoding = csEncoding;
            OptionWithUtf8Bom = optionWithBom;

        }

        public static string LibelleJoined()
        {
            List<string> strRet = Values.Select(value => value.Libelle).ToList();

            return string.Join(", ", strRet);
        }

        public static EnumAppEncoding GetFromLibelle(string encodingInput)
        {
            if (StringUtils.IsNullOrWhiteSpace(encodingInput))
            {
                return null;
            }

            return Values.FirstOrDefault(value => encodingInput.Equals(value.Libelle));
        }
    }
}
