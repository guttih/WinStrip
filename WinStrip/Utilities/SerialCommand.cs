using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace WinStrip.Utilities
{
    public enum SerialCommand
    {
        [Description("STATUS")      ] STATUS,
        [Description("BUFFERSIZE")  ] BUFFERSIZE,
        [Description("SEPARATOR")   ] SEPARATOR,
        [Description("PROGRAMCOUNT")] PROGRAMCOUNT,
        [Description("PROGRAMINFO") ] PROGRAMINFO,
        [Description("ALLSTATUS")   ] ALLSTATUS,
        [Description("COLORS")      ] COLORS,
        [Description("VALUES")      ] VALUES,
        [Description("PIXELCOUNT")  ] PIXELCOUNT,


        //ADD NEW ITEMS BEFORE THIS LINE
        [Description("COUNT OF SERIAL COMMANDS")] COUNT

    }

    public static class SerialCommandExtensions
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

    public static class ValidateSerialCommandResponse
    {
        public static bool Validate(SerialCommand serialCommand, string commandResponce)
        {
            if (string.IsNullOrEmpty(commandResponce))
                return false;

            switch (serialCommand)
            {
                case SerialCommand.STATUS     : return "OK".Equals(commandResponce);
                case SerialCommand.SEPARATOR  : return commandResponce.Length == 1;
                case SerialCommand.PIXELCOUNT:
                case SerialCommand.PROGRAMCOUNT:
                case SerialCommand.BUFFERSIZE : try {

                                                        int value = 0;
                                                        int.TryParse(commandResponce, out value);
                                                        return true;
                                                    }
                                                    catch
                                                    {
                                                        return false;
                                                    }
                

                /* these are json responces, we could check by try serialize but it takes time */

                case SerialCommand.COLORS:      return commandResponce[0] == '[' && commandResponce.EndsWith("]");

                case SerialCommand.PROGRAMINFO:
                case SerialCommand.ALLSTATUS:
                case SerialCommand.VALUES:
                                                return commandResponce[0] == '{' && commandResponce.EndsWith("}");

            }
            return false;
        }
    }
}
