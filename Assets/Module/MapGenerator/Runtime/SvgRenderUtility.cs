using System.IO;
using Module.SvgUtility;
using Unity.VectorGraphics;
using UnityEngine.UIElements;

namespace Module.MapGenerator.Runtime
{
    public static class SvgRenderUtility
    {
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