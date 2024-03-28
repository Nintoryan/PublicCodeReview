using System;
using UnityEngine;

namespace Util
{
    public static class DodoColors
    {
        public static Color TextWhite => HexToColor("FDF5E7");
        public static Color TextBlack => HexToColor("362F2F");
        public static Color TextPumpkin => HexToColor("C65908");
        public static Color TextOrange => HexToColor("ED8233");
        public static Color LightBrown => HexToColor("A89B85");
        public static Color TextGray => HexToColor("505A60");
        public static Color TextOtherGray => HexToColor("BE9773");
        public static Color ButtonGray => HexToColor("848484");
        public static Color ButtonGrayOutline => HexToColor("555555");
        public static Color TextDisabledButton => HexToColor("D0AE8D");

        public static Color ButtonOrange => TextOrange;
        public static Color ButtonOrangeOutline => TextPumpkin;
        
        
        public static Color HexToColor(string hex,float alpha = 1f)
        {
            if (hex.Length != 6)
                throw new ArgumentException("Hex color should be 6 characters long.");

            var r = byte.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
            var g = byte.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
            var b = byte.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);

            return new Color(r / 255f, g / 255f, b / 255f, alpha);
        }
    }
}