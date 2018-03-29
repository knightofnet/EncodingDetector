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
            Description = LangMgr.Instance["parserOptFileDesc"],
            HasArgs = true,
            IsMandatory = true,
            Name = "OptionFile"
        };

        private readonly Option _optionTargetEnc = new Option()
        {
            ShortOpt = "c",
            LongOpt = "convert-to",
            Description = String.Format(LangMgr.Instance["parserOptTargetEncDesc"],
                                        EnumAppEncoding.LibelleJoined()),
            HasArgs = true,
            IsMandatory = false,
            Name = "TargetEnc"
        };

        private readonly Option _optionTargetEol = new Option()
        {
            ShortOpt = "e",
            LongOpt = "end-of-line-to",
            Description = String.Format(LangMgr.Instance["parserOptTargetEolDesc"],
                                        EnumEol.LibelleJoined()),
            HasArgs = true,
            IsMandatory = false,
            Name = "TargetEol"
        };

        private readonly Option _optionOutputFile = new Option()
        {
            ShortOpt = "o",
            LongOpt = "output-file",
            Description = LangMgr.Instance["parserOptOutputFileDesc"],
            HasArgs = true,
            IsMandatory = false,
            Name = "OutputFile"
        };

        private readonly Option _optionSilenceLevel = new Option()
        {
            ShortOpt = "s",
            LongOpt = "silence-level",
            Description = LangMgr.Instance["parserOptSilenceLevelDesc"],
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
                throw new CliParsingException(String.Format(LangMgr.Instance["parserParseTrtFileNotFoundTxt"], fullPath));
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
                    throw new CliParsingException(String.Format(LangMgr.Instance["parserParseTrtSilenceLvlWrontTxt"],
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
                        String.Format(LangMgr.Instance["parserParseTrtEolNotExistTxt"],
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
