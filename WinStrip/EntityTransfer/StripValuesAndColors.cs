using System.Collections.Generic;

namespace WinStrip.EntityTransfer
{
    public class StripValuesAndColors
    {
        public int delay { get; set; }
        public int com { get; set; }
        public int brightness { get; set; }
        public List<int> values { get; set; }
        public List<ulong> colors { get; set; }

        public StripValuesAndColors() {
            values = new List<int>();
            colors = new List<ulong>();
        }

        public StripValuesAndColors(StripValuesAndColors item) {
            delay      = item.delay;
            com        = item.com;
            brightness = item.brightness;
            values     = new List<int>(item.values);
            colors     = new  List<ulong>(item.colors);
        }

    }
}
