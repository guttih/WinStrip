using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace WinStrip.Utilities
{
    public enum StripProgram
    {
        [Description("OFF")         ] OFF,
        [Description("RESET")       ] RESET,
        [Description("SINGLE_COLOR")] SINGLE_COLOR,
        [Description("MULTI_COLOR") ] MULTI_COLOR,
        [Description("UP")          ] UP,
        [Description("DOWN")        ] DOWN,
        [Description("UP_DOWN")     ] UP_DOWN,
        [Description("STARS")       ] STARS,
        [Description("RAINBOW")     ] RAINBOW,
        [Description("CYLON")       ] CYLON,
        [Description("SECTIONS")    ] SECTIONS,

        /*add next type above this line*/
        STRIP_PROGRAMS_COUNT
    }
    public static class StripProgramExtensions
    {
        public static string ToString(this SerialCommand val)
        {
            DescriptionAttribute[] attributes = (DescriptionAttribute[])val
               .GetType()
               .GetField(val.ToString())
               .GetCustomAttributes(typeof(DescriptionAttribute), false);
            return attributes.Length > 0 ? attributes[0].Description : string.Empty;
        }
    }
}
