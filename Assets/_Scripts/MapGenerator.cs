using Module.MapGenerator.Core.Runtime;
using Module.MapGenerator.Svg;
using Module.SvgUtility;
using UnityEngine;
using UnityEngine.UIElements;

namespace _Scripts
{
    [RequireComponent(typeof(PanelRenderer))]
    public class MapGenerator : MonoBehaviour
    {
        [SerializeField] private int roomCount;

        private PanelRenderer? _panelRenderer;
        private Image? _image;

        private void Start()
        {
            var map = Logic.CreateMap(roomCount);
            var svgDocument = MapSvgExporter.Export(map);
            svgDocument.Save("./new_map.svg");
            Debug.Log("on start");
            _image!.ApplySvg(svgDocument);
        }

        private void OnEnable()
        {
            _panelRenderer = GetComponent<PanelRenderer>();
            _panelRenderer.RegisterUIReloadCallback(OnUIReload);
        }

        private void OnDisable()
        {
            _panelRenderer!.UnregisterUIReloadCallback(OnUIReload);
        }

        private void OnUIReload(PanelRenderer renderer, VisualElement root, int version)
        {
            Debug.Log("on reload");
            _image = root.Q<Image>("Image");
        }
    }
}