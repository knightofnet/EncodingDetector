using DetectEncoding.constant;

namespace DetectEncoding.dto
{
    public class ProgramArgs
    {

        public string InputFileName { get; set; }

        public bool IsConvertMode { get; set; }

        public EnumAppEncoding OutputEncoding { get; set; }

        public EnumEol OutputEol { get; set; }

        public string OutputFileName { get; set; }
        public int SilenceLevel { get; set; }
        public string OutputPattern { get; set; }

        public bool IsShowAbout { get; set; }
    }
}
