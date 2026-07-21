using System.IO;
using System.Xml.Linq;
using Unity.VectorGraphics;
using UnityEngine.UIElements;

namespace Module.SvgUtility
{
    /// <summary>
    /// SvgElementに対してSVGの見た目・属性をメソッドチェーンで設定する拡張メソッド群
    /// </summary>
    public static class SvgStyleExtension
    {
        public static SvgElement Fill(this SvgElement element, string color)
        {
            var attr = new XAttribute("fill", color);
            element.AddAttr(attr);
            return element;
        }

        public static SvgElement Stroke(this SvgElement element, string color)
        {
            var attr = new XAttribute("stroke", color);
            element.AddAttr(attr);
            return element;
        }

        public static SvgElement StrokeWidth(this SvgElement element, float width)
        {
            var attr = new XAttribute("stroke-width", SvgFormat.Number(width));
            element.AddAttr(attr);
            return element;
        }

        public static SvgElement Opacity(this SvgElement element, float opacity)
        {
            var attr = new XAttribute("opacity", SvgFormat.Number(opacity));
            element.AddAttr(attr);
            return element;
        }

        public static SvgElement FillOpacity(this SvgElement element, float opacity)
        {
            var attr = new XAttribute("fill-opacity", SvgFormat.Number(opacity));
            element.AddAttr(attr);
            return element;
        }

        public static SvgElement StrokeOpacity(this SvgElement element, float opacity)
        {
            var attr = new XAttribute("stroke-opacity", SvgFormat.Number(opacity));
            element.AddAttr(attr);
            return element;
        }

        public static SvgElement StrokeDasharray(this SvgElement element, string dasharray)
        {
            var attr = new XAttribute("stroke-dasharray", dasharray);
            element.AddAttr(attr);
            return element;
        }

        public static SvgElement Transform(this SvgElement element, string transform)
        {
            var attr = new XAttribute("transform", transform);
            element.AddAttr(attr);
            return element;
        }

        /// <summary>
        /// 任意の属性のための汎用メソッド
        /// </summary>
        public static SvgElement Attr(this SvgElement element, string name, object value)
        {
            var attr = new XAttribute(name, value is float f ? SvgFormat.Number(f) : value.ToString());
            element.AddAttr(attr);
            return element;
        }

        public static void ApplySvg(this Image image, SvgDocument document)
        {
            image.vectorImage = CreateVectorImage(document.ToString());
        }

        public static VectorImage CreateVectorImage(string svgText)
        {
            using var reader = new StringReader(svgText);
            var sceneInfo = SVGParser.ImportSVG(reader);
            var tessOptions = new VectorUtils.TessellationOptions()
            {
                StepDistance = 1f,
                SamplingStepSize = 1f,
                MaxCordDeviation = 0.5f,
                MaxTanAngleDeviation = 0.1f,
            };
            var geometry = VectorUtils.TessellateScene(sceneInfo.Scene, tessOptions);

            var vectorImage = VectorUtils.BuildVectorImage(geometry, 100u);

            return vectorImage;
        }
    }
}