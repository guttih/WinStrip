using System;
using WinStrip.Entity;

namespace WinStrip.Utilities
{
    class DiffStep 
    {
        public Decimal     From { get; set; }

        public Decimal[]       Values = new Decimal[3];
        public DiffColor[] Colors = new DiffColor[] {   new DiffColor(), new DiffColor(), new DiffColor(),
                                                        new DiffColor(), new DiffColor(), new DiffColor()   };
        
        public Decimal Delay { get; set; }
        public Decimal Com { get; set; }
        public Decimal Brightness { get; set; }
        public uint Count { get; private set; }

        private static Decimal getStepIncrement(int fromValue, int toValue, uint count, bool doCalculation)
        {
            if (!doCalculation)
                return 0;

            return (Decimal)(toValue - fromValue) / ((Decimal)count - 1) ;
        }

        public DiffStep(Step step1, Step step2, StepDifferenceParameters diffInterval)
        {
            if (diffInterval == null)
                diffInterval = new StepDifferenceParameters();

            uint  diffFrom = (uint)Math.Abs(step1.From - step2.From);

            if (diffFrom < 2)
            {
                diffFrom = 2;
                From = 1;
            } else
            {
                diffFrom = diffFrom + 1 / diffInterval.From;
            }

            Count = diffFrom;

                /// count = 3  From 0, to 2 interval 1
                /// count = 3  From 0  to interval 2
            Delay      = getStepIncrement(step1.ValuesAndColors.delay,      step2.ValuesAndColors.delay,      Count, diffInterval.Delay);
            Brightness = getStepIncrement(step1.ValuesAndColors.brightness, step2.ValuesAndColors.brightness, Count, diffInterval.Brightness);
            
            int count = Math.Min(   step1.ValuesAndColors.values.Count, 
                                    step2.ValuesAndColors.values.Count);

            for(int i = 0; i < count; i++)
                Values[i] = getStepIncrement(step1.ValuesAndColors.values[i], step2.ValuesAndColors.values[i], Count, diffInterval.Values[i]);

            count = Math.Min(   step1.ValuesAndColors.colors.Count, 
                                step2.ValuesAndColors.colors.Count);
            for (int i = 0; i < count; i++)
            {
                SColor color1 = new SColor(step1.ValuesAndColors.colors[i]);
                SColor color2 = new SColor(step2.ValuesAndColors.colors[i]);
                Colors[i].Red   = getStepIncrement(color1.Red  , color2.Red  , Count, diffInterval.Colors[i].Red);
                Colors[i].Green = getStepIncrement(color1.Green, color2.Green, Count, diffInterval.Colors[i].Green);
                Colors[i].Blue  = getStepIncrement(color1.Blue , color2.Blue , Count, diffInterval.Colors[i].Blue );
            }
        }
    }

    public class StepDifferenceParameters
    {
        public uint From { get; set; }

        public bool[] Values = new bool[3];
        public StepDifferenceParameterColor[] Colors = new StepDifferenceParameterColor[6]  { new StepDifferenceParameterColor(),new StepDifferenceParameterColor(),new StepDifferenceParameterColor(),
                                                                        new StepDifferenceParameterColor(),new StepDifferenceParameterColor(),new StepDifferenceParameterColor()   };

        public bool Delay { get; set; }
        public bool Brightness { get; set; }

        
        public StepDifferenceParameters(uint fromInterval, StepDifferenceParametersTypes parameterType = StepDifferenceParametersTypes.ALL)
        {
            From = fromInterval;
        }


        public StepDifferenceParameters(StepDifferenceParametersTypes parameterType = StepDifferenceParametersTypes.ALL)
        {
            From = 1;
            Init(parameterType);
        }

        public void SetAllValues(bool newValue)
        {
            Delay = newValue;
            Brightness = newValue;
            Values[0] = Values[1] = Values[2] = newValue;
            for(int i = 0; i<Colors.Length; i++)
                Colors[i].Red = Colors[i].Green = Colors[i].Blue = newValue;
        }
        private void Init(StepDifferenceParametersTypes parameterType)
        {
            switch(parameterType)
            {
                case StepDifferenceParametersTypes.ALL:
                    SetAllValues(true);
                    break;

            }
            
        }


    }

    public enum StepDifferenceParametersTypes
    {
        ALL,
        NONE
    }

    public class StepDifferenceParameterColor
    {
        public bool Red { get; set; }
        public bool Green { get; set; }
        public bool Blue { get; set; }
    }

    public class DiffColor
    {
        public Decimal Red { get; set; }
        public Decimal Green { get; set; }
        public Decimal Blue { get; set; }

        internal ulong AddToValues(ulong ulColor, uint multiplier)
        {
            SColor color = new SColor(ulColor);
            color.AddToValues((int)Math.Round(Red * multiplier), (int)Math.Round(Green * multiplier), (int)Math.Round(Blue * multiplier));
            return color.ToUlong();
        }
    }

}
