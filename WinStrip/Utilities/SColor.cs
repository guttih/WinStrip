using System;
using System.Drawing;
using System.Linq;

namespace WinStrip.Utilities
{
    public class SColor
    {
        public byte Red { get; set; }
        public byte Green { get; set; }
        public byte Blue { get; set; }

        public SColor(byte red, byte green, byte blue)
        {
            Red   = red;
            Green = green;
            Blue  = blue;
        }

        public SColor(SColor sColor)
        {
            Red   = sColor.Red;
            Green = sColor.Green;
            Blue  = sColor.Blue;
        }

        public SColor(Color color)
        {
            Red   = color.R;
            Green = color.G;
            Blue  = color.B;
        }

        public SColor(ulong encodedColor)
        {
            SetColor(encodedColor);
        }

        public void SetColor(ulong encodedColor)
        {
            SColor temp = DecodeColor(encodedColor);
            Red = temp.Red;
            Green = temp.Green;
            Blue = temp.Blue;
        }

        public ulong EncodeColor(SColor color)
        {
            ulong uiColor = (ulong)color.Red   << 16 |
                            (ulong)color.Green << 8  |
                            (ulong)color.Blue;
            return uiColor;
        }
        public SColor DecodeColor(ulong ulColor)
        {
            return new SColor
            (
                (byte)(ulColor >> 16),
                (byte)(ulColor >> 8),
                (byte)ulColor
            );
        }

        public ulong ToUlong()
        {
            return EncodeColor(this);
        }
        public Color Color => Color.FromArgb(Red, Green, Blue);

        /// <summary>
        /// Adds value(s) to the color.
        /// </summary>
        /// <param name="red">How much do you want to add to the red part of the color</param>
        /// <param name="green">How much do you want to add to the green part of the color</param>
        /// <param name="blue">How much do you want to add to the blue part of the color</param>
        public void AddToValues(int red, int green, int blue)
        {
            //add byte to int is OK, but int to byte is not
            red   += Red;
            green += Green;
            blue  += Blue;
            //after using the ints for adding now we can set the result
            Red   = (byte)red;
            Green = (byte)green;
            Blue  = (byte)blue;

        }

        /// <summary>
        /// Returns the color values as a hexadecimal string
        /// </summary>
        public override string ToString()
        {
            return "0x" + string.Join("", (new byte[] { Red, Green, Blue }).Select(b => b.ToString("X2")).ToArray());
        }
    }
}
