using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using AryxDevLibrary.utils;
using AryxDevLibrary.utils.cliParser;
using AutoIt.Common;
using DetectEncoding.business;
using DetectEncoding.business.parsing;
using DetectEncoding.constant;
using DetectEncoding.dto;
using DetectEncoding.utils;

namespace DetectEncoding
{
    class Program
    {
        static void Main(string[] args)
        {
            ProgramParser argParser = new ProgramParser();
            var l = LangMgr.Instance;
            try
            {
                // Lecture des options d'entrée.
                ProgramArgs objArgs = argParser.ParseDirect(args);

                if (objArgs.SilenceLevel < 1)
                {
                    ShowHeaderApp();
                }

                if (objArgs.SilenceLevel < 2)
                {
                    Console.WriteLine(" InputFile: {0}", objArgs.InputFileName);
                }

                var resultDEncodingInFile = DetectorsUtils.DetectEncoding(objArgs.InputFileName);
                if (resultDEncodingInFile == TextEncodingDetect.Encoding.None)
                {
                    if (objArgs.SilenceLevel < 2)
                    {
                        Console.WriteLine(LangMgr.Instance["programNoEncodingFound"]);
                    }

                    Environment.Exit(1);
                    return;
                }

                EnumAppEncoding inEncTransType = MiscAppUtils.FromTextEncoding(resultDEncodingInFile);
                EnumEol resultEol = DetectorsUtils.DetectEol(objArgs.InputFileName, inEncTransType);
                if (objArgs.SilenceLevel < 2)
                {
                    Console.WriteLine(" Input : {0}; {1}", resultDEncodingInFile, resultEol.Libelle);
                }


                OutputConf outConf = new OutputConf
                {
                    InputEncoding = inEncTransType
                };

                if (!objArgs.IsConvertMode || inEncTransType == null)
                {
                    return;
                }


                outConf.OutputEncoding = objArgs.OutputEncoding;
                outConf.OutputFileName = objArgs.OutputFileName;
                outConf.OutputEol = objArgs.OutputEol;


                if (outConf.InputEncoding != null)
                {
                    outConf.InputEol = resultEol;
                    SetDefaultValueForConverter(objArgs.InputFileName, outConf);

                    if (objArgs.SilenceLevel < 2)
                    {
                        Console.WriteLine(" Output: {0}; {1}", outConf.OutputEncoding.Libelle,
                            outConf.OutputEol.Libelle);
                    }



                    OutputFileWriter ofwWriter = new OutputFileWriter(objArgs.InputFileName, outConf);
                    ofwWriter.ToFile();

                }

# if DEBUG
                //Console.ReadLine();
# endif
                Environment.Exit(0);
            }
            catch (CliParsingException e)
            {

                ProgramArgs objArgs = argParser.EarlyParse(args);
                if (objArgs.SilenceLevel < 1)
                {
                    ShowHeaderApp();
                }
# if DEBUG
                Console.Write(e);
# else
                if (objArgs.SilenceLevel < 2) {
                    Console.Write(e.Message);
                }
# endif
                if (objArgs.SilenceLevel < 1)
                {
                    argParser.ShowSyntax();
                    Console.WriteLine();
                    Console.WriteLine(LangMgr.Instance["programMoreHelpOn"]);
                }

                Environment.Exit(3);

            }
            catch (Exception e)
            {
# if DEBUG
                Console.Write(e);
# else

                Console.Write(e.Message);
                
# endif
                Environment.Exit(4);
            }

        }

        private static void ShowHeaderApp()
        {
            Console.ForegroundColor = ConsoleColor.White;

            Console.WriteLine(" Encoding Detector - v{0}", System.Reflection.Assembly.GetExecutingAssembly().GetName().Version);
            Console.WriteLine(" ===================================");
            Console.WriteLine(LangMgr.Instance["programShowHeaderAuthor"] + " 2018");
            Console.WriteLine("");

            Console.WriteLine(LangMgr.Instance["programShowHeaderDisc"]);
            Console.WriteLine(" https://github.com/AutoItConsulting/text-encoding-detect");
            Console.WriteLine("");


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
    }
}
