using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using DetectEncoding.constant;
using DetectEncoding.dto;
using DetectEncoding.utils;

namespace DetectEncoding.business
{
    class OutputFileWriter
    {

        private const string DOS_EOLCHAR = "\r\n";
        private const string UNIX_EOLCHAR = "\n";

        public String InputFileName { get; private set; }

        public String OutputFileName { get; private set; }

        public EnumAppEncoding InputEncoding { get; private set; }

        public EnumAppEncoding OutputEncoding { get; private set; }

        public EnumEol OutputEol { get; private set; }

        public OutputFileWriter(string inputFileName, OutputConf outConf)
        {
            InputFileName = inputFileName;
            InputEncoding = outConf.InputEncoding;
            OutputEncoding = outConf.OutputEncoding;
            OutputEol = outConf.OutputEol;
            OutputFileName = outConf.OutputFileName;


        }

        public void ToFile()
        {

            // Ecriture du fichier de sortie.
            using (StreamReader sr = StreamUtils.GetStreamReaderFromEAppEncoding(InputFileName, InputEncoding))
            {
                using (StreamWriter sw = StreamUtils.GetStreamWriterFromEAppEncoding(OutputFileName, OutputEncoding))
                {
                    String line = null;
                    while ((line = sr.ReadLine()) != null)
                    {
                        sw.Write(line + OutputEol.EolChar);

                    }
                }
            }

        }


    }
}
