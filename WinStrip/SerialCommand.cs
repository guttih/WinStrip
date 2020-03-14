using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace WinStrip
{
    public enum SerialCommand
    {

        [Description("STATUS")    ] STATUS,
        [Description("BUFFERSIZE")] BUFFERSIZE,
        [Description("SEPARATOR") ] SEPARATOR
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
                case SerialCommand.STATUS    : return "OK".Equals(commandResponce);
                case SerialCommand.SEPARATOR : return commandResponce.Length == 1;                    
                case SerialCommand.BUFFERSIZE:  try {

                                                        int value = 0;
                                                        int.TryParse(commandResponce, out value);
                                                        return true;
                                                    }
                                                    catch
                                                    {
                                                        return false;
                                                    }

            }
            return false;
        }
    }
}
