using System;
using System.Collections;
using System.Collections.Generic;
using System.Web.Script.Serialization;
using WinStrip.EntityTransfer;

namespace WinStrip.Entity
{
    public class Step : IComparable, IComparable<Step>, IComparer, IComparer<Step>,  IEquatable<Step>, IEqualityComparer<Step>

    {
        public int From { get; set; }
        public StripValuesAndColors ValuesAndColors { get; set; }

        public Step() => Init(0, null);
        public Step(int from) => Init(from, null);
        public Step(int from, StripValuesAndColors valuesAndColors) => Init(from, valuesAndColors);

        public Step(Step step) => Init(step.From, new StripValuesAndColors(step.ValuesAndColors));

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

            var serializer = new JavaScriptSerializer();
            Init(from, serializer.Deserialize<StripValuesAndColors>(valuesAndColors));
        }

        private void Init(int from, StripValuesAndColors valuesAndColors)
        {
            From = from;
            if (valuesAndColors == null)
                ValuesAndColors = new StripValuesAndColors();
            else 
                ValuesAndColors = valuesAndColors;
        }

        public string ValuesAndColorsToJson()
        {
            var serializer = new JavaScriptSerializer();
            return serializer.Serialize(ValuesAndColors);
        }



        /// <summary>
        /// Compares a Step left to a Step Right and returns an indication of their relative values.

        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns>
        ///     Less than zero    if left is less than right.
        ///     Zero              if left is equal to right.
        ///     Greater than zero if left is greater than right.
        /// ></returns>
        public int Compare(Step left, Step right)
        {
            if ((object)left == null) return -1;
            if ((object)right == null) return  1;

            return left.From.CompareTo(right.From);
        }

        public bool Equals(Step x, Step y)
        {
            return Compare(x, y) == 0;
        }

        public int GetHashCode(Step obj)
        {
            return obj.GetHashCode();
        }

        public override int GetHashCode()
        {
            return From.GetHashCode();
        }


        public bool Equals(Step other)
        {
            return Equals(this, other);
        }

        public int CompareTo(object other)
        {
            if (other == null) return 1;

            Step otherStep = other as Step;

            if (otherStep != null)
                return CompareTo(otherStep);
            
            throw new ArgumentException("Object is not a Step");
        }

        public int CompareTo(Step other)
        {
            if ((object)other == null) return 1;

            return this.Compare(this, other);
        }

        public int Compare(object left, object right)
        {
            if (right == null) return 1;
            if (left == null) return -1;

            return Compare((Step)left, (Step)right);
        }

        public override bool Equals(object right)
        {
            return this.CompareTo(right) == 0;
        }

        public static bool operator ==(Step left, Step right)
        {
            if ((object)left == null)
            {
                return (object)right == null;
            }
            return left.Equals(right);
        }

        public static bool operator !=(Step left, Step right)
        {
            if ((object)left == null)
            {
                return (object)right != null;
            }

            return !left.Equals(right);
        }

        public static bool operator < (Step left, Step right)
        {
            return left.Compare(left, right) == -1;
        }

        public static bool operator > (Step left, Step right)
        {
            return left.Compare(left, right) == 1;
        }

        public override string ToString() 
        {
            return $"{From} : {ValuesAndColorsToJson()}";
        }

    }

   
}
