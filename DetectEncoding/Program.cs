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
using DetectEncoding.business.parsing;
using DetectEncoding.constant;
using DetectEncoding.dto;

namespace DetectEncoding
{
    class Program
    {
        static void Main(string[] args)
        {
            ProgramParser argParser = new ProgramParser();
            try
            {

                ShowHeaderApp();

                ProgramArgs objArgs = argParser.ParseDirect(args);


                Console.WriteLine(" InputFile: {0}", objArgs.InputFileName);
                var result = DetectEncodingBis(objArgs.InputFileName);



                if (result != TextEncodingDetect.Encoding.None)
                {

                    EnumAppEncoding inEncTransType = FromTextEncoding(result);


                    var resultEol = DetectEol(objArgs.InputFileName, inEncTransType);


                    OutputConf outConf = new OutputConf
                    {
                        InputEncoding = inEncTransType
                    };

                    Console.Write(" Encoding: {0}", result);


                    if (resultEol == EnumEOL.DOS)
                    {
                        Console.WriteLine("; Dos");
                    }
                    else if (resultEol == EnumEOL.UNIX)
                    {
                        Console.WriteLine("; Unix");
                    }

                    if (!objArgs.IsConvertMode || inEncTransType == null)
                    {
                        return;
                    }


                    outConf.OutputEncoding = objArgs.OutputEncoding;
                    outConf.OutputFileName = objArgs.OutputFileName;
                    outConf.OutputEol = objArgs.OutputEol;

                    if (outConf.InputEncoding != null)
                    {
                        SetDefaultValueForConverter(objArgs.InputFileName, outConf);

                        Console.WriteLine(" Output : Encoding: {0}; {1}", outConf.OutputEncoding.Libelle, outConf.OutputEol);

                        outConf.InputEol = resultEol;
                        ConvertFile(objArgs.InputFileName, outConf);

                    }

                }

# if DEBUG
                //Console.ReadLine();
# endif

            }
            catch (CliParsingException e)
            {

# if DEBUG
                Console.Write(e);
# else
                Console.Write(e.Message);
# endif
                argParser.ShowSyntax();

            }

        }

        private static void ShowHeaderApp()
        {
            var dft = Console.ForegroundColor;

            Console.ForegroundColor = ConsoleColor.White;

            Console.WriteLine(" Encoding Detector with reencoding !");
            Console.WriteLine(" ===================================");
            Console.WriteLine(" by Aryx - Wolfaryx informatique - 2018");
            Console.WriteLine("");
            Console.ResetColor();

            Console.WriteLine("");
            Console.WriteLine(" Encoding detection based on work from AutoIt Consulting :");
            Console.WriteLine(" https://github.com/AutoItConsulting/text-encoding-detect");



            Console.WriteLine("");
            Console.ResetColor();
        }

        private static void ConvertFile(string inputFileName, OutputConf outConf)
        {



            ToFile(inputFileName, outConf.InputEncoding, outConf.OutputEncoding, outConf.OutputEol, outConf.OutputFileName);
        }

        private static void SetDefaultValueForConverter(string inputFileName, OutputConf outConf)
        {
            if (outConf.OutputEncoding == null)
            {
                outConf.OutputEncoding = outConf.InputEncoding;
            }

            if (outConf.OutputEol.Equals(EnumEOL.NONE))
            {
                outConf.OutputEol = outConf.InputEol;
            }

            if (outConf.OutputFileName == null)
            {
                FileInfo fI = new FileInfo(inputFileName);
                outConf.OutputFileName = fI.Name.Replace(fI.Extension, "") + "-Out" + fI.Extension;
            }
        }


        private static EnumAppEncoding FromTextEncoding(TextEncodingDetect.Encoding result)
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

        private static EnumEOL DetectEol(string filename, EnumAppEncoding inEncoding)
        {
            EnumEOL enumRet = EnumEOL.UNIX;

            String s;
            using (StreamReader sr = GetStreamReaderFromEAppEncoding(filename, inEncoding))
            {


                if (sr.BaseStream.Length < 2)
                {
                    return enumRet;
                }
                //ensure file is not so small

                int n1 = 0;
                int c = 0;
                int p = 0;
                while (sr.Peek() >= 0)
                {
                    p++;

                    if (p > 1)
                    {
                        n1 = c;
                    }


                    c = sr.Read();



                    if (c == 10 && p > 1) //if (s1 == 13 && s2 == 10) //if sure its windows format
                    {
                        //file is end with CR-LF or LF ...
                        if (n1 == 13)
                        {
                            enumRet = EnumEOL.DOS;
                        } //file is end with CR-LF (Windows EOL format)
                        else
                        {
                            enumRet = EnumEOL.UNIX;
                        } //file is end with just LF, (UNIX/OSX format)
                    }

                }




            }

            return enumRet;
        }





        public static TextEncodingDetect.Encoding DetectEncodingBis(string filename)
        {
            var textDetect = new TextEncodingDetect();

            byte[] array = File.ReadAllBytes(filename);
            TextEncodingDetect.Encoding encoding = textDetect.DetectEncoding(array, array.Length - 1);
            return encoding;


        }


        private static void ToFile(string inputFileName, EnumAppEncoding inputEncoding, EnumAppEncoding outputEncoding, EnumEOL outputEol, string outputFileName)
        {


            String eolChar = "\r\n";
            if (outputEol == EnumEOL.UNIX)
            {
                eolChar = "\n";
            }

            using (StreamReader sr = GetStreamReaderFromEAppEncoding(inputFileName, inputEncoding))
            {
                using (StreamWriter sw = GetStreamWriterFromEAppEncoding(outputFileName, outputEncoding))
                {
                    String line = null;
                    while ((line = sr.ReadLine()) != null)
                    {

                        sw.Write(line + eolChar);

                    }
                }
            }

        }

        private static StreamReader GetStreamReaderFromEAppEncoding(string inputFileName, EnumAppEncoding inputEncoding)
        {
            return new StreamReader(inputFileName, inputEncoding.CsEncoding);
        }

        private static StreamWriter GetStreamWriterFromEAppEncoding(string outFileName, EnumAppEncoding encoding)
        {
            if (encoding.Equals(EnumAppEncoding.UTF8_NOBOM) || encoding.Equals(EnumAppEncoding.UTF8_BOM))
            {
                return new StreamWriter(File.Open(outFileName, FileMode.Create), new UTF8Encoding(encoding.OptionWithUtf8Bom));
            }

            if (encoding.Equals(EnumAppEncoding.UTF16BE_NOBOM) || encoding.Equals(EnumAppEncoding.UTF16BE_BOM))
            {
                return new StreamWriter(File.Open(outFileName, FileMode.Create), new UnicodeEncoding(true, encoding.OptionWithUtf8Bom));
            }

            if (encoding.Equals(EnumAppEncoding.UTF16LE_NOBOM) || encoding.Equals(EnumAppEncoding.UTF16LE_BOM))
            {
                return new StreamWriter(File.Open(outFileName, FileMode.Create), new UnicodeEncoding(false, encoding.OptionWithUtf8Bom));
            }

            return new StreamWriter(File.Open(outFileName, FileMode.Create), encoding.CsEncoding);
        }
    }
}
