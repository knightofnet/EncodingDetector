using System.Collections.Generic;

namespace DetectEncoding.constant
{
    public class EnumExitCode
    {
        public static readonly EnumExitCode DETECT_OK = new EnumExitCode("DETECT_OK", 0);
        public static readonly EnumExitCode DETECT_AND_REENC_OK = new EnumExitCode("DETECT_AND_REENC_OK", 1);
        public static readonly EnumExitCode ABOUT_SHOWN = new EnumExitCode("ABOUT_SHOWN", 2);


        public static readonly EnumExitCode NO_ENC_FOUND = new EnumExitCode("NO_ENC_FOUND", 20);


        public static readonly EnumExitCode ERROR_PARAM_IN = new EnumExitCode("ERROR_PARAM_IN", 50);
        public static readonly EnumExitCode ERROR_DETECT_ENC = new EnumExitCode("ERROR_DETECT_ENC", 51);
        public static readonly EnumExitCode ERROR_REENC = new EnumExitCode("ERROR_REENC", 52);

        public static readonly EnumExitCode ERROR_UNEXPECTED = new EnumExitCode("ERROR_UNEXPECTED", 99);




        public static IEnumerable<EnumExitCode> Values
        {
            get
            {
                yield return DETECT_OK;
                yield return DETECT_AND_REENC_OK;
                yield return ABOUT_SHOWN;
                yield return NO_ENC_FOUND;
                yield return ERROR_PARAM_IN;
                yield return ERROR_DETECT_ENC;
                yield return ERROR_REENC;
                yield return ERROR_UNEXPECTED;

            }
        }

        public string Libelle { get; private set; }
        public int ExitCode { get; private set; }



        private EnumExitCode(string libelle, int exitCode)
        {
            Libelle = libelle;
            ExitCode = exitCode;

        }


    }
}
