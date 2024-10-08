﻿using System;
using System.IO;
using DetectEncoding.constant;
using DetectEncoding.dto;
using DetectEncoding.utils;

namespace DetectEncoding.business
{
    class OutputFileWriter
    {

        public string InputFileName { get; private set; }

        public string OutputFileName { get; private set; }

        public EnumAppEncoding InputEncoding { get; private set; }

        public EnumAppEncoding OutputEncoding { get; private set; }

        public EnumEol OutputEol { get; private set; }

        private readonly bool _useTempOutputFile;

        public OutputFileWriter(string inputFileName, OutputConf outConf)
        {
            InputFileName = inputFileName;
            InputEncoding = outConf.InputEncoding;
            OutputEncoding = outConf.OutputEncoding;
            OutputEol = outConf.OutputEol;
            OutputFileName = outConf.OutputFileName;

            if (OutputFileName == InputFileName)
            {
                _useTempOutputFile = true;
            }


        }

        public void ToFile()
        {

            string outFileName = OutputFileName;
            if (_useTempOutputFile)
            {
                outFileName = Path.GetTempFileName();
            }

            // Ecriture du fichier de sortie.
            using (StreamReader sr = StreamUtils.GetStreamReaderFromEAppEncoding(InputFileName, InputEncoding))
            {
                using (StreamWriter sw = StreamUtils.GetStreamWriterFromEAppEncoding(outFileName, OutputEncoding))
                {
                    string line = null;

                    while ((line = sr.ReadLine()) != null)
                    {
                        sw.Write(line);
                        sw.Write(OutputEol.EolChar);
                    }
                }
            }

            if (_useTempOutputFile)
            {
                if (File.Exists(OutputFileName))
                {
                    File.Delete(OutputFileName);
                }
                File.Move(outFileName, OutputFileName);
            }

        }


    }
}
