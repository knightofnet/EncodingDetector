﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AryxDevLibrary.utils;

namespace DetectEncoding.constant
{
    public class EnumEol
    {

        public static readonly EnumEol NONE = new EnumEol("NONE", "");
        public static readonly EnumEol DOS = new EnumEol("DOS", "\r\n");
        public static readonly EnumEol UNIX = new EnumEol("UNIX", "\n");




        public static IEnumerable<EnumEol> Values
        {
            get
            {
                yield return NONE;
                yield return DOS;
                yield return UNIX;

            }
        }

        public string Libelle { get; private set; }
        public string EolChar { get; private set; }



        private EnumEol(string libelle, string eolChar)
        {
            Libelle = libelle;
            EolChar = eolChar;

        }

        public static string LibelleJoined()
        {
            List<String> strRet = Values.Select(value => value.Libelle).ToList();

            return String.Join(", ", strRet);
        }

        public static EnumEol GetFromLibelle(string encodingInput)
        {
            if (StringUtils.IsNullOrWhiteSpace(encodingInput))
            {
                return null;
            }

            foreach (var value in Values)
            {
                if (encodingInput.Equals(value.Libelle)) return value;
            }
            return null;
        }
    }
}
