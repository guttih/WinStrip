using System;
using System.Collections.Generic;
using System.Web.Script.Serialization;
using WinStrip.EntityTransfer;
using System.Linq;

namespace WinStrip.Entity
{
    public class Theme : IComparer<Theme>
    {
        public Theme()
        {
            Steps = new List<Step>();
        }
        public Theme(string name, bool isDefault = false)
        {
            Name = name;
            Default = isDefault;
            Steps = new List<Step>();
        }

        public string Name { get; set; }
        
        /// <summary>
        /// Is this theme the them which will start when the application starts
        /// </summary>
        public bool Default {get; set; }
        public List<Step> Steps { get; set; }

        /// <summary>
        /// Sorts the steps in descending order and makes sure that the last step will be From 0
        /// </summary>
        public void SortStepsAndFix(bool ReverseOrder = false)
        {
            Steps = SortStepsAndFix(Steps, ReverseOrder);
        }

        public List<Step> SortStepsAndFix(List<Step> list, bool ReverseOrder)
        {
            list.Sort(new Step());
            list = RemoveDublicatesFromASortedList(list);
            if (list.Count > 0 && list[0].From !=0)
                list[0].From = 0;//there must be a zero from step

            if (ReverseOrder)
                list.Reverse();

            return list;
        }

        /// <summary>
        /// Removes dublicated steps.
        /// Note, the List must be sorted before calling this method;
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public List<Step> RemoveDublicatesFromASortedList(List<Step> list)
        {
            //Here list must be sorted in order for this to work
            return list.GroupBy(a => a.From).Select(b => b.First()).ToList();

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

        public bool AddStep(string from, string valuesAndColors)
        {
            try
            {
                int fromInt = Convert.ToInt32(from);
                var serializer = new JavaScriptSerializer();

                var valuesAndColorsObj = serializer.Deserialize<StripValuesAndColors>(valuesAndColors);
                Steps.Add(new Step { From = fromInt, ValuesAndColors = valuesAndColorsObj });

            } catch (Exception)
            {
                return false;
            }
            return true;
        }
    }
}
