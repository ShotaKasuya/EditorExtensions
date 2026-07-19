using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Xml.Linq;
using UnityEngine;

namespace Module.SvgUtility
{
    public static class Svg
    {
        public static readonly XNamespace Ns = "http://www.w3.org/2000/svg";
        public static readonly XNamespace XlinkNs = "http:/www.w3.org3/1999/xlink";
    }

    /// <summary>
    /// 数値をSVG向け(カルチャ非依存・小数点はピリオド固定)にフォーマットするヘルパー
    /// </summary>
    internal static class SvgFormat
    {
        public static string Number(double value)
        {
            return value.ToString("F2", CultureInfo.InvariantCulture);
        }

        public static string Points(IEnumerable<Vector2> pointEnumerable)
        {
            var builder = new StringBuilder();
            var first = true;
            foreach (var point in pointEnumerable)
            {
                if (!first)
                {
                    builder.Append(' ');
                }

                builder.Append(Number(point.x)).Append(' ').Append(Number(point.y));
                first = false;
            }

            return builder.ToString();
        }
    }
}