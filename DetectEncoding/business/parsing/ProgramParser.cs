using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using AryxDevLibrary.utils;
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
            Description = String.Format("Convertie l'encodage du fichier analysé dans un encodage cible : {0}. " +
                                        "La conversion n'est possible que si l'encodage source a été détecté.",
                                        EnumAppEncoding.LibelleJoined()),
            HasArgs = true,
            IsMandatory = false,
            Name = "TargetEnc"
        };

        private readonly Option _optionTargetEol = new Option()
        {
            ShortOpt = "e",
            LongOpt = "end-of-line-to",
            Description = String.Format("Convertie le caractère de fin de ligne : {0}. La conversion n'est possible " +
                                        "que si l'encodage source a été détecté.",
                                        EnumEol.LibelleJoined()),
            HasArgs = true,
            IsMandatory = false,
            Name = "TargetEol"
        };

        private readonly Option _optionOutputFile = new Option()
        {
            ShortOpt = "o",
            LongOpt = "output-file",
            Description = "Fichier cible pour la conversion. Si omis, le fichier se présentera sous la forme " +
                          "[Nom fichier input]-Out[Extension fichier input]. Si égale \"SAME_AS_INPUT\" alors " +
                          "même fichier que celui de l'option -f",
            HasArgs = true,
            IsMandatory = false,
            Name = "OutputFile"
        };

        private readonly Option _optionSilenceLevel = new Option()
        {
            ShortOpt = "s",
            LongOpt = "silence-level",
            Description = "Permet de régler le nombre d'éléments affichés. 0 : tout est affiché (comme si -s absent)," +
                          " 1: juste les lignes de traitement, 2: rien n'est affiché",
            HasArgs = true,
            IsMandatory = false,
            Name = "SilenceLevel"
        };



        public ProgramParser()
        {
            AddOption(_optionFile);
            AddOption(_optionTargetEnc);
            AddOption(_optionTargetEol);
            AddOption(_optionOutputFile);
            AddOption(_optionSilenceLevel);
        }


        public override ProgramArgs ParseDirect(string[] args)
        {
            return Parse(args, ParseTrt);
        }

        public ProgramArgs EarlyParse(string[] args)
        {
            ClearOptions();
            AddOption(_optionSilenceLevel);

            ProgramArgs pargs = Parse(args, delegate(Dictionary<string, Option> arg)
            {

                ProgramArgs p = new ProgramArgs();

                string silenceLevelRaw = GetSingleOptionValue(_optionSilenceLevel.Name, arg);
                if (!StringUtils.IsEmpty(silenceLevelRaw))
                {
                    short result = 0;
                    if (Int16.TryParse(silenceLevelRaw, out result) && result >= 0 && result <= 2)
                    {
                        p.SilenceLevel = result;
                    }
                }
                else
                {
                    p.SilenceLevel = 0;
                }

                return p;

            });

            ClearOptions();
            AddOption(_optionFile);
            AddOption(_optionTargetEnc);
            AddOption(_optionTargetEol);
            AddOption(_optionOutputFile);
            AddOption(_optionSilenceLevel);

            return pargs;


        }

        private ProgramArgs ParseTrt(Dictionary<string, Option> arg)
        {
            ProgramArgs p = new ProgramArgs();
            p.OutputEol = EnumEol.NONE;

            string fileUrl = GetSingleOptionValue(_optionFile.Name, arg);
            string fullPath = Path.GetFullPath(fileUrl);
            if (!File.Exists(fullPath))
            {
                throw new CliParsingException(String.Format("Le fichier {0} n'existe pas", fullPath));
            }
            p.InputFileName = fullPath;

            string silenceLevelRaw = GetSingleOptionValue(_optionSilenceLevel.Name, arg);
            if (!StringUtils.IsEmpty(silenceLevelRaw))
            {
                short result = 0;
                if (Int16.TryParse(silenceLevelRaw, out result) && result >= 0 && result <= 2)
                {
                    p.SilenceLevel = result;
                }
                else
                {
                    throw new CliParsingException(String.Format("L'option -s doit être un entier de 0 à 2." +
                                                                " '{0}' fournit.",
                        result));
                }
            }
            else
            {
                p.SilenceLevel = 0;
            }

            if (HasOption(_optionTargetEol.Name, arg) || HasOption(_optionTargetEnc.Name, arg) ||
                HasOption(_optionOutputFile.Name, arg))
            {
                p.IsConvertMode = true;
            }

            if (HasOption(_optionTargetEol.Name, arg))
            {
                string eolInput = GetSingleOptionValue(_optionTargetEol.Name, arg).ToUpper();
                EnumEol enEolIn = EnumEol.GetFromLibelle(eolInput.ToUpper());
                if (enEolIn == null)
                {
                    throw new CliParsingException(
                        String.Format("Le type de caractère de fin de ligne {0} n'existe pas. " +
                                      "Type de caractères de fin de ligne possibles : {1}",
                                      eolInput, EnumEol.LibelleJoined()));
                }

                p.OutputEol = enEolIn;
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

                String outputFile = GetSingleOptionValue(_optionOutputFile.Name, arg);
                if ("SAME_AS_INPUT".Equals(outputFile))
                {
                    outputFile = p.InputFileName;
                }
                p.OutputFileName = outputFile;
            }


            return p;


        }


    }
}
