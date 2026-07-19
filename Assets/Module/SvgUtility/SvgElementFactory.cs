using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;

namespace Module.SvgUtility
{
    public static class SvgElementFactory
    {
        public static SvgElement Rect(Vector2 position, Vector2 size)
        {
            var rect = new XElement(Svg.Ns + "rect",
                new XAttribute("x", SvgFormat.Number(position.x)),
                new XAttribute("y", SvgFormat.Number(position.y)),
                new XAttribute("width", SvgFormat.Number(size.x)),
                new XAttribute("height", SvgFormat.Number(size.y))
            );

            return new SvgElement(rect);
        }

        public static SvgElement Rect(Vector2 position, Vector2 size, Vector2 radius)
        {
            var rect = Rect(position, size);
            rect.AddAttr(new XAttribute("rx", SvgFormat.Number(radius.x)));
            rect.AddAttr(new XAttribute("ry", SvgFormat.Number(radius.y)));

            return rect;
        }

        public static SvgElement Circle(Vector2 center, float radius)
        {
            var circle = new XElement(Svg.Ns + "circle",
                new XAttribute("cx", SvgFormat.Number(center.x)),
                new XAttribute("cy", SvgFormat.Number(center.y)),
                new XAttribute("r", SvgFormat.Number(radius))
            );
            return new SvgElement(circle);
        }

        public static SvgElement Ellipse(Vector2 center, Vector2 radius)
        {
            var ellipse = new XElement(Svg.Ns + "ellipse",
                new XAttribute("cx", SvgFormat.Number(center.x)),
                new XAttribute("cy", SvgFormat.Number(center.y)),
                new XAttribute("rx", SvgFormat.Number(radius.x)),
                new XAttribute("ry", SvgFormat.Number(radius.y))
            );
            return new SvgElement(ellipse);
        }

        public static SvgElement Line(Vector2 pos1, Vector2 pos2)
        {
            var line = new XElement(Svg.Ns + "line",
                new XAttribute("x1", SvgFormat.Number(pos1.x)),
                new XAttribute("y1", SvgFormat.Number(pos1.y)),
                new XAttribute("x2", SvgFormat.Number(pos2.x)),
                new XAttribute("y2", SvgFormat.Number(pos2.y))
            );
            return new SvgElement(line);
        }

        public static SvgElement Polyline(IEnumerable<Vector2> points)
        {
            var element = new XElement(Svg.Ns + "polyline",
                new XAttribute("points", SvgFormat.Points(points))
            );
            return new SvgElement(element);
        }

        public static SvgElement Polygon(IEnumerable<Vector2> points)
        {
            var element = new XElement(Svg.Ns + "polygon",
                new XAttribute("points", SvgFormat.Points(points))
            );
            return new SvgElement(element);
        }
    }
}