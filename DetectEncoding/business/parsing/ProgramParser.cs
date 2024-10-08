﻿using System;
using System.Collections.Generic;
using System.IO;
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
            IsMandatory = false,
            Name = "OptionFile"
        };

        private readonly Option _optionTargetEnc = new Option()
        {
            ShortOpt = "c",
            LongOpt = "convert-to",
            Description = string.Format(LangMgr.Instance["parserOptTargetEncDesc"],
                                        EnumAppEncoding.LibelleJoined()),
            HasArgs = true,
            IsMandatory = false,
            Name = "TargetEnc"
        };

        private readonly Option _optionTargetEol = new Option()
        {
            ShortOpt = "e",
            LongOpt = "end-of-line-to",
            Description = string.Format(LangMgr.Instance["parserOptTargetEolDesc"],
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

        private readonly Option _optionPatternedOutput = new Option()
        {
            ShortOpt = "p",
            LongOpt = "output-pattern",
            Description = LangMgr.Instance["parserPatternedOutputDesc"],
            HasArgs = true,
            IsMandatory = false,
            Name = "PatternedOutput"
        };

        private readonly Option _optionAbout = new Option()
        {
            ShortOpt = "a",
            LongOpt = "about",
            Description = LangMgr.Instance["parserOptAboutDesc"],
            HasArgs = false,
            IsMandatory = false,
            Name = "About"
        };


        public ProgramParser()
        {
            AddDefaultOptions();
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
                    if (short.TryParse(silenceLevelRaw, out result) && result >= 0 && result <= 2)
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
            AddDefaultOptions();

            return pargs;


        }

        private void AddDefaultOptions()
        {
            AddOption(_optionFile);
            AddOption(_optionTargetEnc);
            AddOption(_optionTargetEol);
            AddOption(_optionOutputFile);
            AddOption(_optionSilenceLevel);
            AddOption(_optionPatternedOutput);
            AddOption(_optionAbout);
        }

        private ProgramArgs ParseTrt(Dictionary<string, Option> arg)
        {
            ProgramArgs p = new ProgramArgs();
            p.OutputEol = EnumEol.NONE;

            // About
            if (HasOption(_optionAbout, arg))
            {
                p.IsShowAbout = true;
                return p;
            }

            // Silence level and output-pattern sont mutuellement exclusive
            if (HasOption(_optionSilenceLevel, arg) && HasOption(_optionPatternedOutput, arg))
            {
                throw new CliParsingException(string.Format(LangMgr.Instance["parserParseErrorSandPin"], _optionSilenceLevel.ShortOpt, _optionPatternedOutput.ShortOpt));
            }

            // Input File path
            if (!HasOption(_optionFile, arg))
            {
                throw new CliParsingException(string.Format(LangMgr.Instance["cliparserOptMissingStr"], _optionFile.ShortOpt));
            }
            string fileUrl = GetSingleOptionValue(_optionFile.Name, arg);
            string fullPath = Path.GetFullPath(fileUrl);
            if (!File.Exists(fullPath))
            {
                throw new CliParsingException(string.Format(LangMgr.Instance["parserParseTrtFileNotFoundTxt"], fullPath));
            }
            p.InputFileName = fullPath;

            // Silence Level
            string silenceLevelRaw = GetSingleOptionValue(_optionSilenceLevel.Name, arg);
            if (!StringUtils.IsEmpty(silenceLevelRaw))
            {
                short result = 0;
                if (short.TryParse(silenceLevelRaw, out result) && result >= 0 && result <= 2)
                {
                    p.SilenceLevel = result;
                }
                else
                {
                    throw new CliParsingException(string.Format(LangMgr.Instance["parserParseTrtSilenceLvlWrontTxt"],
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

            // Target EOL
            if (HasOption(_optionTargetEol.Name, arg))
            {
                string eolInput = GetSingleOptionValue(_optionTargetEol.Name, arg).ToUpper();
                EnumEol enEolIn = EnumEol.GetFromLibelle(eolInput.ToUpper());
                if (enEolIn == null)
                {
                    throw new CliParsingException(
                        string.Format(LangMgr.Instance["parserParseTrtEolNotExistTxt"],
                                      eolInput, EnumEol.LibelleJoined()));
                }

                p.OutputEol = enEolIn;
            }

            // Target Enc
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

            // OutputFileName
            if (HasOption(_optionOutputFile.Name, arg))
            {

                string outputFile = GetSingleOptionValue(_optionOutputFile.Name, arg);
                if ("SAME_AS_INPUT".Equals(outputFile))
                {
                    outputFile = p.InputFileName;
                }
                p.OutputFileName = outputFile;
            }


            // PatternedOutput
            if (HasOption(_optionPatternedOutput, arg))
            {

                p.OutputPattern = GetSingleOptionValue(_optionPatternedOutput, arg);
                p.SilenceLevel = 99;

            }


            return p;


        }


    }
}
