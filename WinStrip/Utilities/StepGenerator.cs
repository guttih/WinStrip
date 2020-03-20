using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WinStrip.Entity;

namespace WinStrip.Utilities
{
    public static class StepGenerator
    {
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
