using System.Globalization;
using UnityEngine;

namespace Module.SvgUtility
{
    public static class SvgColorExtension
    {
        public static SvgElement Fill(this SvgElement element, Color color)
            => element.Fill(color.ToSvgColorString());

        public static SvgElement Fill(this SvgElement element, Color32 color32)
            => element.Fill(color32.ToSvgColorString());

        public static SvgElement Stroke(this SvgElement element, Color color)
            => element.Stroke(color.ToSvgColorString());

        public static SvgElement Stroke(this SvgElement element, Color32 color32)
            => element.Stroke(color32.ToSvgColorString());

        /// <summary>
        /// `Unity`の`Color`を`SVG`で解釈可能な色文字列に変換する
        /// </summary>
        public static string ToSvgColorString(this Color color)
        {
            byte r = (byte)Mathf.RoundToInt(Mathf.Clamp01(color.r) * 255f);
            byte g = (byte)Mathf.RoundToInt(Mathf.Clamp01(color.g) * 255f);
            byte b = (byte)Mathf.RoundToInt(Mathf.Clamp01(color.b) * 255f);
            float a = Mathf.Clamp01(color.a);

            return ToSvgColorString(r, g, b, a);
        }

        /// <summary>
        /// `Unity`の`Color32`を`SVG`で解釈可能な色文字列に変換する
        /// </summary>
        public static string ToSvgColorString(this Color32 color)
        {
            return ToSvgColorString(color.r, color.g, color.b, color.a / 255f);
        }

        private static string ToSvgColorString(byte red, byte green, byte blue, float alpha)
        {
            if (alpha >= 1f)
            {
                return $"#{red:X2}{green:X2}{blue:X2}";
            }

            string alphaText = alpha.ToString("F2", CultureInfo.InvariantCulture);
            return $"rgba({red}, {green}, {blue}, {alphaText})";
        }
    }
}