using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DetectEncoding.constant;

namespace DetectEncoding.dto
{
    public class ProgramArgs
    {

        public string InputFileName { get; set; }

        public bool IsConvertMode { get; set; }

        public EnumAppEncoding OutputEncoding { get; set; }

        public EnumEOL OutputEol { get; set; }

        public string OutputFileName { get; set; }
    }
}
