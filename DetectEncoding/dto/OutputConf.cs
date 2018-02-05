using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using DetectEncoding.constant;

namespace DetectEncoding.dto
{
    public class OutputConf
    {

        public EnumAppEncoding InputEncoding { get; set; }

        public EnumEOL InputEol { get; set; }

        public EnumAppEncoding OutputEncoding { get; set; }

        public EnumEOL OutputEol { get; set; }

        public string OutputFileName { get; set; }

        public OutputConf()
        {
            InputEol = EnumEOL.NONE;
            OutputEol = EnumEOL.NONE;
        }

    }
}
