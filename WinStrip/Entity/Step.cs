using System;
using System.Collections.Generic;
using System.Web.Script.Serialization;
using WinStrip.EntityTransfer;

namespace WinStrip.Entity
{
    public class Step : IComparer<Step>

    {
        public int From { get; set; }
        public StripValuesAndColors ValuesAndColors { get; set; }
        public Step() {
            ValuesAndColors = new StripValuesAndColors();
        }

        public Step(int from, StripValuesAndColors valuesAndColors)
        {
            From = from;
            ValuesAndColors = valuesAndColors;

        }

        /// <summary>
        /// Constructor for a step created from one int and a json string
        /// </summary>
        /// <param name="from">From value in the step</param>
        /// <param name="valuesAndColors">The Json string to create the step from</param>
        /// <param name="fixSpacesAndTabs">If spaces and tabs are found in the string valuesAndColors, remove them.</param>
        public Step(int from, string valuesAndColors, bool fixSpacesAndTabs = false)
        {
            if (fixSpacesAndTabs)
            {
                valuesAndColors = valuesAndColors.Replace(" ", "");
                valuesAndColors = valuesAndColors.Replace("\t", "");
                valuesAndColors = valuesAndColors.Replace("\r", "");
                valuesAndColors = valuesAndColors.Replace("\n", "");
            }
            
            From = from;
            var serializer = new JavaScriptSerializer();
            ValuesAndColors =  serializer.Deserialize<StripValuesAndColors>(valuesAndColors);
        }
        public Step(Step step) {
            From = step.From;
            ValuesAndColors = new StripValuesAndColors(step.ValuesAndColors);
        }

        
        

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
