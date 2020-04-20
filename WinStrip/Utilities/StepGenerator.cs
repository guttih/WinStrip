using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WinStrip.Entity;
using WinStrip.EntityTransfer;

namespace WinStrip.Utilities
{
    public static class StepGenerator
    {
        /// <summary>
        /// Generates steps between two steps.
        /// The function finds difference between the steps and calculates
        /// steps to create between the two steps.
        ///     Note the list will be ordered by Step From property, low to high.
        ///     You can use the List Reverse function if you want the order high to low
        /// </summary>
        /// <param name="fromStep">The first step in the list</param>
        /// <param name="toStep">The last step in the list</param>
        /// <param name="interval">How much must From step change to create a new Step</param>
        /// <returns>
        ///     A list of steps including the steps firstStep and the lastStep.  
        ///</returns>
        public static List<Step> StripSteps(Step fromStep, Step toStep, StepDifferenceParameters diffInterval=null)
        {
            if (diffInterval == null)
                diffInterval = new StepDifferenceParameters();

            Step firstStep, lastStep;
            if (fromStep.From <= toStep.From)
            {
                firstStep = new Step(fromStep);
                lastStep = new Step(toStep);

            } else
            {
                firstStep = new Step(toStep);
                lastStep = new Step(fromStep);
            }
            
            var diff = new DiffStep(firstStep, lastStep, diffInterval);
            var list = new List<Step> { new Step(firstStep) };
            
            int turn = 1;
            var commonValuesCount = Math.Min(firstStep.ValuesAndColors.values.Count, lastStep.ValuesAndColors.values.Count);
            var commonColorsCount = Math.Min(firstStep.ValuesAndColors.colors.Count, lastStep.ValuesAndColors.colors.Count);
            for (int i = (  firstStep.From + (int)diffInterval.From  ); 
                     i < (  lastStep.From  ); 
                     i += (int)diffInterval.From
                )
            {
                var step = new Step(    i, 
                                        new StripValuesAndColors {      com         = firstStep.ValuesAndColors.com,
                                                                         brightness = (int)Math.Round(firstStep.ValuesAndColors.brightness + (diff.Brightness * i)),
                                                                         delay      = (int)Math.Round(firstStep.ValuesAndColors.delay + (diff.Delay * i)),
                                                                 }
                                    );
                
                for(int x = 0; x < commonValuesCount; x++) 
                    step.ValuesAndColors.values.Add(  (int)Math.Round( firstStep.ValuesAndColors.values[x] + (diff.Values[x] * i) )  );
                
                for (int x = 0; x < commonColorsCount; x++)
                    step.ValuesAndColors.colors.Add(  diff.Colors[x].AddToValues(  firstStep.ValuesAndColors.colors[x], (uint)i )  );


                list.Add(step);
                turn++;
            }
            list.Add(new Step(lastStep));

            return list;
        }

        public static List<Step> StripDimToBright(Step baseStep, ulong startColor, ulong endColor, UInt16 startIndex, UInt16 endIndex, UInt16 interval = 1 )
        {
            var colorFrom    = new SColor(startColor);
            var colorTo      = new SColor(endColor);
            
            int stepCount = endIndex - startIndex;
            stepCount /= interval;
            
            var list = new List<Step>();
            if (stepCount < 1 || baseStep.ValuesAndColors.colors.Count < 1)
                return list;

            int redDiff   = colorTo.Red   - colorFrom.Red;
            int greenDiff = colorTo.Green - colorFrom.Green;
            int blueDiff  = colorTo.Blue  - colorFrom.Blue;
            float redInc    = redDiff   / (float)stepCount;
            float greenInc  = greenDiff / (float)stepCount;
            float blueInc   = blueDiff  / (float)stepCount;
            float red =   colorFrom.Red;
            float green = colorFrom.Green;
            float blue =  colorFrom.Blue;

            for (UInt16 ui = startIndex; ui<endIndex+1; ui+=interval)
            {
                var step = new Step(baseStep);
                step.From = ui;
                var currentColor = new SColor((byte)Math.Round(red), (byte)Math.Round(green), (byte)Math.Round(blue));
                if (ui == endIndex)
                    step.ValuesAndColors.colors[0] = endColor;
                else
                    step.ValuesAndColors.colors[0] = currentColor.ToUlong();
                list.Add(step);
                red+= redInc; green+= greenInc; blue+= blueInc; 
            }

            return list;

        }
    }
}
