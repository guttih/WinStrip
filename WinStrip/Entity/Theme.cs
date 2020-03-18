using System;
using System.Collections.Generic;
using System.Web.Script.Serialization;
using WinStrip.EntityTransfer;

namespace WinStrip.Entity
{
    class Theme:IComparer<Theme>
    {
        public Theme()
        {
            Steps = new List<Step>();
        }
        public Theme(string name)
        {
            this.Name = name;
            Steps = new List<Step>();
        }

        public string Name { get; set; }
        public List<Step> Steps { get; set; }

        /// <summary>
        /// Sorts the steps in descending order and makes sure that the last step will be From 0
        /// </summary>
        public void SortStepsAndFix()
        {
            Steps.Sort(new Step());
            if (Steps.Count > 0 && Steps[Steps.Count-1].From != 0)
            {
                Steps[Steps.Count - 1].From = 0;
            }
        }

        public int Compare(Theme x, Theme y)
        {
            return x.Name.CompareTo(y.Name);
        }

        internal Step GetAppropriateStep(int cpuValue)
        {
            
            var step = Steps.Find(s => s.From <= cpuValue);
            return step;
        }

        public bool AddStep(string from, string values, string colors)
        {
            try
            {
                int fromInt = Convert.ToInt32(from);
                var serializer = new JavaScriptSerializer();

                var valuesObj = serializer.Deserialize<StripValues>(values);
                var colorsObj = serializer.Deserialize<StripColors>(colors);
                Steps.Add(new Step { From = fromInt, Colors = colorsObj, Values = valuesObj });

            } catch (Exception ex)
            {
                return false;
            }
            return true;
        }
    }
}
