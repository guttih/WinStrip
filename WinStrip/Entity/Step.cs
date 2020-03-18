using System;
using System.Collections.Generic;
using System.Web.Script.Serialization;
using WinStrip.EntityTransfer;

namespace WinStrip.Entity
{
    class Step : IComparer<Step>

    {
        public int From { get; set; }
        public StripValues Values { get; set; }
        public StripColors Colors { get; set; }

        public int Compare(Step x, Step y)
        {
            return y.From.CompareTo(x.From);
        }


        public string ToJson()
        {
            var serializer = new JavaScriptSerializer();
            var ret = new {
                delay = Values.delay,
                com = Values.com,
                brightness = Values.brightness,
                values = Values.values,
                colors = Colors.colors
            };
            return serializer.Serialize(ret);
        }

       

        public string ColorsToJson()
        {
            var serializer = new JavaScriptSerializer();
            return serializer.Serialize(Colors);
        }

        public string ValuesToJson()
        {
            var serializer = new JavaScriptSerializer();
            return serializer.Serialize(Values);
        }

    }
}
