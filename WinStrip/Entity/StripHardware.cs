using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WinStrip.Entity
{
    class StripHardware
    {
        public string Type { get; set; }
        public string ColorScheme { get; set; }
        public int DataPin { get; set; }
        public int ClockPin { get; set; }
        public int PixelCount { get; set; }
    }
}
