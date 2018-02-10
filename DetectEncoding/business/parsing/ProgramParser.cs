using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using AryxDevLibrary.utils.cliParser;
using DetectEncoding.constant;
using DetectEncoding.dto;

namespace DetectEncoding.business.parsing
{
    public class ProgramParser : CliParser<ProgramArgs>
    {
        private readonly Option _optionFile = new Option()
        {
            ShortOpt = "f",
            LongOpt = "file",
            Description = "chemin du fichier à analyser",
            HasArgs = true,
            IsMandatory = true,
            Name = "OptionFile"
        };

        private readonly Option _optionTargetEnc = new Option()
        {
            ShortOpt = "c",
            LongOpt = "convert-to",
            Description = String.Format("Convertie l'encodage du fichier analysé dans un encodage cible : {0}. La conversion n'est possible que si l'encodage source a été détecté.", EnumAppEncoding.LibelleJoined()),
            HasArgs = true,
            IsMandatory = false,
            Name = "TargetEnc"
        };

        private readonly Option _optionTargetEOL = new Option()
        {
            ShortOpt = "e",
            LongOpt = "end-of-line-to",
            Description = "Convertie le caractère de fin de ligne : DOS, UNIX. La conversion n'est possible que si l'encodage source a été détecté.",
            HasArgs = true,
            IsMandatory = false,
            Name = "TargetEol"
        };

        private readonly Option _optionOutputFile = new Option()
        {
            ShortOpt = "o",
            LongOpt = "output-file",
            Description = "Fichier cible pour la conversion. Si omis, le fichier se présentera sous la forme [Nom fichier input]-Conv[Extension fichier input].",
            HasArgs = true,
            IsMandatory = false,
            Name = "OutputFile"
        };



        public ProgramParser()
        {
            AddOption(_optionFile);
            AddOption(_optionTargetEnc);
            AddOption(_optionTargetEOL);
            AddOption(_optionOutputFile);
        }


        public override ProgramArgs ParseDirect(string[] args)
        {
            return Parse(args, ParseTrt);
        }

        private ProgramArgs ParseTrt(Dictionary<string, Option> arg)
        {
            ProgramArgs p = new ProgramArgs();
            p.OutputEol = EnumEOL.NONE;

            string fileUrl = GetSingleOptionValue(_optionFile.Name, arg);
            string fullPath = Path.GetFullPath(fileUrl);
            if (!File.Exists(fullPath))
            {
                throw new CliParsingException(String.Format("Le fichier {0} n'existe pas", fullPath));
            }
            p.InputFileName = fullPath;

            if (HasOption(_optionTargetEOL.Name, arg) || HasOption(_optionTargetEnc.Name, arg) ||
                HasOption(_optionOutputFile.Name, arg))
            {
                p.IsConvertMode = true;
            }

            if (HasOption(_optionTargetEOL.Name, arg))
            {
                string eolInput = GetSingleOptionValue(_optionTargetEOL.Name, arg).ToUpper();
                if ("DOS".Equals(eolInput))
                {
                    p.OutputEol = EnumEOL.DOS;
                }
                else if ("UNIX".Equals(eolInput))
                {
                    p.OutputEol = EnumEOL.UNIX;
                }
            }

            if (HasOption(_optionTargetEnc.Name, arg))
            {
                string encodingInput = GetSingleOptionValue(_optionTargetEnc.Name, arg).ToUpper();

                EnumAppEncoding enEncIn = EnumAppEncoding.GetFromLibelle(encodingInput);
                if (enEncIn != null)
                {
                    p.OutputEncoding = enEncIn;
                }
                else
                {

                    if ("UTF8".Equals(encodingInput) || "UTF-8".Equals(encodingInput) || "UTF8BOM".Equals(encodingInput))
                    {
                        p.OutputEncoding = EnumAppEncoding.UTF8_BOM;
                    }
                    if ("UTF8NOBOM".Equals(encodingInput) | "UTF-8-NOBOM".Equals(encodingInput))
                    {
                        p.OutputEncoding = EnumAppEncoding.UTF8_NOBOM;
                    }
                    else if ("ANSI".Equals(encodingInput))
                    {
                        p.OutputEncoding = EnumAppEncoding.ANSI;
                    }

                }
            }

            if (HasOption(_optionOutputFile.Name, arg))
            {
                p.OutputFileName = GetSingleOptionValue(_optionOutputFile.Name, arg);
            }


            return p;


        }
    }
}
