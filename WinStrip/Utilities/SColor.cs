using System.Drawing;

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
    }
}
