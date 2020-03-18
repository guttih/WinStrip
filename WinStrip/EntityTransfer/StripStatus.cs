using System.Collections.Generic;

namespace WinStrip.EntityTransfer
{
    public class StripStatus
    {
        public List <StripProgram> programs { get; set; }
        public int delay { get; set; }
        public int com { get; set; }
        public int brightness { get; set; }
        public List<int> values { get; set; }
        public List<ulong> colors { get; set; }
    }
}
