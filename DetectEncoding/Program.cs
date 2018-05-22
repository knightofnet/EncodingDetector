using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using AryxDevLibrary.extensions;
using AryxDevLibrary.utils.cliParser;
using DetectEncoding.business;
using DetectEncoding.business.parsing;
using DetectEncoding.constant;
using DetectEncoding.constant.texts;
using DetectEncoding.dto;
using DetectEncoding.exception;
using DetectEncoding.utils;

namespace DetectEncoding
{
    class Program
    {
        static void Main(string[] args)
        {
            LangMgr.AddResourceManager("fr", Resource_fr_Fr.ResourceManager);
            LangMgr.AddResourceManager("en", Resource_en_US.ResourceManager);
            LangMgr.AddResourceManager("es", Resource_es_ES.ResourceManager);
            //LangMgr.ForceCulture("es");
            TranslateCliParser();

            ProgramParser argParser = new ProgramParser();
            EnumExitCode batchExitCode;

            try
            {

                // Lecture des options d'entrée.
                ProgramArgs objArgs = argParser.ParseDirect(args);

                MiscAppUtils.ConditionnalExecCode(objArgs.SilenceLevel, 1, ShowHeaderApp);
                // N'affiche que le header
                if (objArgs.IsShowAbout)
                {
                    Console.WriteLine(LangMgr.Instance["programMoreHelpOn"]);
                    Console.WriteLine();
                    batchExitCode = EnumExitCode.ABOUT_SHOWN;
                    Environment.Exit(batchExitCode.ExitCode);
                }
                MiscAppUtils.ConditionnalWrtLine(objArgs.SilenceLevel, 2, " InputFile: {0}", objArgs.InputFileName);

                // Détection encodage
                EnumAppEncoding inEncTransType;
                EnumEol resultEol;
                EnumExitCode exitCode = DetecteFileEncAndEol(out inEncTransType, out resultEol, objArgs);
                if (exitCode != EnumExitCode.DETECT_OK)
                {
                    batchExitCode = exitCode;
                }

                OutputConf outConf = new OutputConf();
                if (!objArgs.IsConvertMode)
                {
                    batchExitCode = EnumExitCode.DETECT_OK;
                }
                else
                {

                    // Réencodage
                    outConf = new OutputConf
                    {
                        InputEncoding = inEncTransType
                    };
                    Reencode(objArgs, resultEol, outConf);
                    batchExitCode = EnumExitCode.DETECT_AND_REENC_OK;

                }

                if (objArgs.OutputPattern != null)
                {
                    ShowPatternedOutput(objArgs, inEncTransType, resultEol, outConf, batchExitCode);
                }

# if DEBUG
                Console.ReadLine();
# else
                MiscAppUtils.ConditionnalWrtLine(objArgs.SilenceLevel, 1, "");
# endif
                Environment.Exit(batchExitCode.ExitCode);
            }
            catch (CliParsingException e)
            {
                // --------------
                // >> une erreur est survenue lors du parsing des options d'entrée du batch.
                // --------------

                ProgramArgs objArgs = argParser.EarlyParse(args);
                MiscAppUtils.ConditionnalExecCode(objArgs.SilenceLevel, 1, ShowHeaderApp);

# if DEBUG
                Console.Write(e);
# else
                MiscAppUtils.ConditionnalWrtLine(objArgs.SilenceLevel, 2, e.Message);

# endif
                MiscAppUtils.ConditionnalExecCode(objArgs.SilenceLevel, 1, delegate
                {
                    argParser.ShowSyntax();
                    Console.WriteLine();
                    Console.WriteLine(LangMgr.Instance["programMoreHelpOn"]);
                    Console.WriteLine();
                });

                Environment.Exit(EnumExitCode.ERROR_PARAM_IN.ExitCode);

            }
            catch (DetectEncodeException e)
            {
                // --------------
                // >> une erreur est survenue lors de la datection de l'encodage.
                // --------------
# if DEBUG
                Console.Write(e);
                Console.ReadKey();
# else

                Console.Write(e.Message);

# endif
                Environment.Exit(EnumExitCode.ERROR_DETECT_ENC.ExitCode);

            }
            catch (ReencodePartException e)
            {
                // --------------
                // >> une erreur est survenue lors du réencodage.
                // --------------
# if DEBUG
                Console.Write(e);
                Console.ReadKey();
# else

                Console.Write(e.Message);

# endif
                Environment.Exit(EnumExitCode.ERROR_REENC.ExitCode);

            }
            catch (Exception e)
            {
                // --------------
                // >> une erreur non prévue est survenue.
                // --------------

# if DEBUG
                Console.Write(e);
                Console.ReadKey();
# else

                Console.Write(e.Message);

# endif
                Environment.Exit(EnumExitCode.ERROR_UNEXPECTED.ExitCode);
            }

        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="eAppEncoding"></param>
        /// <param name="eEol"></param>
        /// <param name="objArgs"></param>
        /// <returns></returns>
        private static EnumExitCode DetecteFileEncAndEol(out EnumAppEncoding eAppEncoding, out EnumEol eEol, ProgramArgs objArgs)
        {
            try
            {

                var resultDEncodingInFile = DetectorsUtils.DetectEncoding(objArgs.InputFileName);

                eAppEncoding = MiscAppUtils.FromTextEncoding(resultDEncodingInFile);
                eEol = DetectorsUtils.DetectEol(objArgs.InputFileName, eAppEncoding);
                MiscAppUtils.ConditionnalWrtLine(objArgs.SilenceLevel, 2, " Input : {0}; {1}", eAppEncoding == null ? "NONE" : eAppEncoding.Libelle,
                    eEol.Libelle);


                if (eAppEncoding != null) return EnumExitCode.DETECT_OK;

                MiscAppUtils.ConditionnalWrtLine(objArgs.SilenceLevel, 2, LangMgr.Instance["programNoEncodingFound"]);
                return EnumExitCode.NO_ENC_FOUND;
            }
            catch (Exception e)
            {
                throw new DetectEncodeException("Erreur lors de la détection de l'encodage", e);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="objArgs"></param>
        /// <param name="resultEol"></param>
        /// <param name="outConf"></param>
        private static void Reencode(ProgramArgs objArgs, EnumEol resultEol, OutputConf outConf)
        {
            try
            {

                outConf.OutputEncoding = objArgs.OutputEncoding;
                outConf.OutputFileName = objArgs.OutputFileName;
                outConf.OutputEol = objArgs.OutputEol;


                if (outConf.InputEncoding != null)
                {
                    outConf.InputEol = resultEol;
                    SetDefaultValueForConverter(objArgs.InputFileName, outConf);
                    MiscAppUtils.ConditionnalWrtLine(objArgs.SilenceLevel, 2, " Output: {0}; {1}",
                        outConf.OutputEncoding.Libelle,
                        outConf.OutputEol.Libelle);


                    OutputFileWriter ofwWriter = new OutputFileWriter(objArgs.InputFileName, outConf);
                    ofwWriter.ToFile();
                }
            }
            catch (Exception ex)
            {
                throw new ReencodePartException("Erreur lors du ré-encodage", ex);
            }
        }

        private static void ShowHeaderApp()
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("");
            Console.WriteLine(" Encoding Detector - v{0}", Assembly.GetExecutingAssembly().GetName().Version);
            Console.WriteLine(" ===================================");
            Console.WriteLine(LangMgr.Instance["programShowHeaderAuthor"] + " 2018");
            Console.WriteLine("");

            Console.WriteLine(LangMgr.Instance["programShowHeaderDisc"]);
            Console.WriteLine(" https://github.com/AutoItConsulting/text-encoding-detect");
            Console.WriteLine("");

            Console.ResetColor();

        }



        private static void SetDefaultValueForConverter(string inputFileName, OutputConf outConf)
        {
            if (outConf.OutputEncoding == null)
            {
                outConf.OutputEncoding = outConf.InputEncoding;
            }

            if (outConf.OutputEol.Equals(EnumEol.NONE))
            {
                outConf.OutputEol = outConf.InputEol;
            }

            if (outConf.OutputFileName == null)
            {
                FileInfo fI = new FileInfo(inputFileName);
                outConf.OutputFileName = fI.Name.Replace(fI.Extension, "") + "-Out" + fI.Extension;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="batchArgs"></param>
        /// <param name="resultInputEncoding"></param>
        /// <param name="resultInputEol"></param>
        /// <param name="outputConfObj"></param>
        /// <param name="exitCode"></param>
        private static void ShowPatternedOutput(ProgramArgs batchArgs, EnumAppEncoding resultInputEncoding, EnumEol resultInputEol, OutputConf outputConfObj, EnumExitCode exitCode)
        {
            StringBuilder str = new StringBuilder(batchArgs.OutputPattern);
            str.Replace("[IN_FILE]", batchArgs.InputFileName);
            str.Replace("[IN_ENC]", resultInputEncoding.Libelle);
            str.Replace("[IN_EOL]", resultInputEol.Libelle);

            str.Replace("[OUT_ENC]", outputConfObj.OutputEncoding != null ? outputConfObj.OutputEncoding.Libelle : "");
            str.Replace("[OUT_EOL]", outputConfObj.OutputEol != null ? outputConfObj.OutputEol.Libelle : "");
            str.Replace("[OUT_FILE]", outputConfObj.OutputFileName ?? "");

            str.Replace("[WITH_REENC]", batchArgs.IsConvertMode ? "true" : "false");
            str.Replace("[EXIT_CODE]", exitCode.ExitCode.ToString());
            str.Replace("[EXIT_CODE_LBL]", exitCode.Libelle);

            str.Replace("[APP_VERSION]", Assembly.GetExecutingAssembly().GetName().Version.ToString());

            Console.WriteLine(str.ToString());


        }

        private static void TranslateCliParser()
        {

            String langKey = "cliparserAlsoStr";
            if (!LangMgr.Instance[langKey].IsEmpty())
                CliParserLangRef.CwWriteLines_Also = LangMgr.Instance[langKey];

            langKey = "cliparserRequiredStr";
            if (!LangMgr.Instance[langKey].IsEmpty())
                CliParserLangRef.CwWriteLines_Required = LangMgr.Instance[langKey];

            langKey = "cliparserSyntaxTplStr";
            if (!LangMgr.Instance[langKey].IsEmpty())
                CliParserLangRef.ShowSyntax_SyntaxTpl = LangMgr.Instance[langKey];

            langKey = "cliparserOptMissingStr";
            if (!LangMgr.Instance[langKey].IsEmpty())
                CliParserLangRef.CheckOption_OptionNotPresent = LangMgr.Instance[langKey];


            langKey = "cliparserOptArgMissingStr";
            if (!LangMgr.Instance[langKey].IsEmpty())
                CliParserLangRef.CheckOption_OptionMustHaveArg = LangMgr.Instance[langKey];

        }
    }
}
