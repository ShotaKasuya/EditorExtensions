using System.Xml.Linq;

namespace Module.SvgUtility
{
    /// <summary>
    /// SVGの要素(rect, circle, g...)を表すラッパークラス
    /// ライブラリ利用者は常にこの`SvgElement`を介して図形を組み立てる
    /// </summary>
    public sealed class SvgElement
    {
        internal XElement Element { get; }
        
        internal SvgElement(XElement element)
        {
            Element = element;
        }

        /// <summary>
        /// この要素に子要素(図形など)を追加する
        /// </summary>
        /// <param name="child"></param>
        /// <returns></returns>
        public SvgElement Add(SvgElement child)
        {
            Element.Add(child);
            return this;
        }

        /// <summary>
        /// この要素に子要素(図形など)を追加する
        /// </summary>
        public SvgElement AddAttr(XAttribute attribute)
        {
            Element.SetAttributeValue(attribute.Name, attribute.Value);
            return this;
        }

        /// <summary>
        /// この要素のテキスト内容を設定する
        /// </summary>
        public SvgElement Content(string text)
        {
            Element.Add(text);
            return this;
        }

        public override string ToString() => Element.ToString();
    }
}