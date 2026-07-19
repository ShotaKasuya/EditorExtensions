using System.Xml.Linq;
using UnityEngine;

namespace Module.SvgUtility
{
    /// <summary>
    /// XDocumentをラップし、SVGファイル全体を持つクラス
    /// ルート要素(svg)の生成・保存・文字列化を担当する
    /// </summary>
    public class SvgDocument
    {
        private XDocument Document { get; }
        private XElement Root { get; }

        public SvgDocument(Vector2 leftDown, Vector2 rightUp)
        {
            var size = rightUp - leftDown;
            var viewBox = $"{SvgFormat.Number(leftDown.x)} {SvgFormat.Number(leftDown.y)} {SvgFormat.Number(size.x)} {SvgFormat.Number(size.y)}";
            Root = new XElement(Svg.Ns + "svg",
                new XAttribute("xmlns", Svg.Ns.NamespaceName),
                new XAttribute(XNamespace.Xmlns + "xlink", Svg.XlinkNs.NamespaceName),
                new XAttribute("width", SvgFormat.Number(size.x)),
                new XAttribute("height", SvgFormat.Number(size.y)),
                new XAttribute("viewBox", viewBox)
            );
            Document = new XDocument(new XDeclaration("1.0", "UTF-8", null), Root);
        }

        /// <summary>
        /// ルート要素に子要素(図形など)を追加する。
        /// メソッドチェーン用にthisを返す。
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public SvgDocument AddElement(SvgElement element)
        {
            Root.Add(element.Element);
            return this;
        }

        /// <summary>
        /// Svgを保存する
        /// </summary>
        public void Save(string path)
        {
            Document.Save(path);
        }

        public override string ToString() => Document.ToString();
    }
}