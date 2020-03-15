using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WinStrip.Entity
{
    class StripValues
    {
        public int delay { get; set; }
        public int com { get; set; }
        public List<int> values { get; set; }
        public List<ulong> colors { get; set; }
    }
}
