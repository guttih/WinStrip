using System;
using System.Collections.Generic;
using System.Web.Script.Serialization;
using WinStrip.EntityTransfer;

namespace WinStrip.Entity
{
    class Step : IComparer<Step>

    {
        public Step(int from, string valuesAndColors)
        {
            From = from;
            var serializer = new JavaScriptSerializer();
            ValuesAndColors =  serializer.Deserialize<StripValuesAndColors>(valuesAndColors);
        }
        public Step(Step step) {
            From = step.From;
            ValuesAndColors = new StripValuesAndColors(step.ValuesAndColors);
        }

        public Step() {}
        public int From { get; set; }
        public StripValuesAndColors ValuesAndColors{ get; set; }

        public int Compare(Step x, Step y)
        {
            return y.From.CompareTo(x.From);
        }


        public string ValuesAndColorsToJson()
        {
            var serializer = new JavaScriptSerializer();
            return serializer.Serialize(ValuesAndColors);
        }

    }
}
