using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace WinStrip.Enums
{
    

    public enum ProgramArgument
    {
        [Description("INVALID_ARGUMENT")] INVALID_ARGUMENT, 
        [Description("BUILDFORSETUP")]    BUILDFORSETUP,

        //ADD NEW ITEMS BEFORE THIS LINE
        [Description("COUNT OF PROGRAMARGUMENTS")] COUNT

    }

    public static class ProgramArgumentExtensions
    {
        public static string ToString(this ProgramArgument val)
        {
            DescriptionAttribute[] attributes = (DescriptionAttribute[])val
               .GetType()
               .GetField(val.ToString())
               .GetCustomAttributes(typeof(DescriptionAttribute), false);
            return attributes.Length > 0 ? attributes[0].Description : string.Empty;
        }
    }

    public static class ProgramArgumentHelper { 

        /// <summary>
        /// Converts a string into ProgramArgument enum
        /// The string must be in upper case.
        /// </summary>
        /// <param name="testIfProgramArgument">String to convert.  </param>
        /// <returns>
        /// Fail: INVALID_ARGUMENT
        /// Success: ProgramArgument which matches the string 
        /// </returns>
        public static ProgramArgument GetEnum(string testIfProgramArgument)
        {
            var values = Enum.GetValues(typeof(ProgramArgument)).OfType<ProgramArgument>().ToList();
            var descriptions = new List<string>();
            for (int i = 0; i < (int)ProgramArgument.COUNT; i++)
                descriptions.Add(values[i].ToString());

            var index = descriptions.IndexOf(testIfProgramArgument);
            if (  index < (int)ProgramArgument.INVALID_ARGUMENT   ||   index >= (int)ProgramArgument.COUNT  )
                return ProgramArgument.INVALID_ARGUMENT;

            return (ProgramArgument)index;
        }
    }
}
