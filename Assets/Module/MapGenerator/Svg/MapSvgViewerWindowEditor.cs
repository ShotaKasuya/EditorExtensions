using Module.MapGenerator.Core.Runtime;
using Module.SvgUtility;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Module.MapGenerator.Svg;

public class MapSvgViewerWindowEditor : EditorWindow
{
    private int roomCount = 10;
    private VisualElement? previewContainer;
    private VisualElement? svgDisplayElement;
    private Label? statusLabel;

    private const string ViewerName = "Map Svg Viewer";

    [MenuItem("Tools/Map Svg Viewer")]
    public static void ShowWindow()
    {
        var window = GetWindow<MapSvgViewerWindowEditor>(ViewerName);
        window.minSize = new Vector2(400, 450);
    }

    private void CreateGUI()
    {
        // Set up main style
        VisualElement root = rootVisualElement;
        root.style.flexDirection = FlexDirection.Column;
        root.style.backgroundColor = new StyleColor(Color.gray2);
        root.style.paddingLeft = 10;
        root.style.paddingRight = 10;
        root.style.paddingTop = 10;
        root.style.paddingBottom = 10;

        // Title
        var titleLabel = new Label(ViewerName);
        titleLabel.style.fontSize = 20;
        titleLabel.style.unityFontStyleAndWeight = FontStyle.Bold;
        titleLabel.style.color = new StyleColor(Color.whiteSmoke);
        titleLabel.style.marginBottom = 20;
        root.Add(titleLabel);

        // Settings Section Container
        var settingsContainer = new VisualElement();
        settingsContainer.style.backgroundColor = new StyleColor(Color.gray3);
        settingsContainer.style.paddingLeft = 8;
        settingsContainer.style.paddingRight = 8;
        settingsContainer.style.paddingTop = 8;
        settingsContainer.style.paddingBottom = 8;
        settingsContainer.style.borderTopLeftRadius = 6;
        settingsContainer.style.borderTopRightRadius = 6;
        settingsContainer.style.borderBottomLeftRadius = 6;
        settingsContainer.style.borderBottomRightRadius = 6;
        settingsContainer.style.marginBottom = 10;
        root.Add(settingsContainer);

        // Room Count Field
        var roomCountField = new SliderInt("Room Count", 1, 50);
        roomCountField.value = roomCount;
        roomCountField.style.color = new StyleColor(Color.gray8);
        roomCountField.RegisterValueChangedCallback(evt => roomCount = evt.newValue);
        settingsContainer.Add(roomCountField);

        // Generate Button
        var generateButton = new Button(GenerateAndDisplayMap);
        generateButton.text = "Generate Map & Display SVG";
        generateButton.style.height = 32;
        generateButton.style.marginBottom = 8;
        generateButton.style.backgroundColor = new StyleColor(Color.darkGreen);
        generateButton.style.color = new StyleColor(Color.white);
        generateButton.style.unityFontStyleAndWeight = FontStyle.Bold;
        generateButton.style.borderTopLeftRadius = 4;
        generateButton.style.borderTopRightRadius = 4;
        generateButton.style.borderBottomLeftRadius = 4;
        generateButton.style.borderBottomRightRadius = 4;
        settingsContainer.Add(generateButton);

        // Status Label
        statusLabel = new Label("Ready to generate.");
        statusLabel.style.color = new StyleColor(Color.gray6);
        statusLabel.style.fontSize = 11;
        statusLabel.style.marginTop = 4;
        settingsContainer.Add(statusLabel);

        previewContainer = new VisualElement();
        previewContainer.style.flexGrow = 1;
        previewContainer.style.backgroundColor = new StyleColor(Color.slateGray);
        var borderColor = new StyleColor(Color.gray2);
        previewContainer.style.borderLeftColor = borderColor;
        previewContainer.style.borderRightColor = borderColor;
        previewContainer.style.borderTopColor = borderColor;
        previewContainer.style.borderBottomColor = borderColor;
        previewContainer.style.borderLeftWidth = 1;
        previewContainer.style.borderRightWidth = 1;
        previewContainer.style.borderTopWidth = 1;
        previewContainer.style.borderBottomWidth = 1;
        previewContainer.style.borderTopLeftRadius = 6;
        previewContainer.style.borderTopRightRadius = 6;
        previewContainer.style.borderBottomLeftRadius = 6;
        previewContainer.style.borderBottomRightRadius = 6;
        previewContainer.style.justifyContent = Justify.Center;
        previewContainer.style.alignItems = Align.Center;
        previewContainer.style.overflow = Overflow.Hidden;
        root.Add(previewContainer);

        // SVG Display Element inside Container
        svgDisplayElement = new VisualElement();
        svgDisplayElement.style.width = Length.Percent(95);
        svgDisplayElement.style.height = Length.Percent(95);
        svgDisplayElement.style.backgroundSize = new BackgroundSize(BackgroundSizeType.Contain);
        svgDisplayElement.style.backgroundPositionX = new BackgroundPosition(BackgroundPositionKeyword.Center);
        svgDisplayElement.style.backgroundPositionY = new BackgroundPosition(BackgroundPositionKeyword.Center);
        svgDisplayElement.style.backgroundRepeat = new BackgroundRepeat(Repeat.NoRepeat, Repeat.NoRepeat);
        previewContainer.Add(svgDisplayElement);
    }

    private void GenerateAndDisplayMap()
    {
        statusLabel!.text = "Generating map data...";
        statusLabel.style.color = new StyleColor(Color.orange);

        // 1. Generate map logic
        Map generatedMap = Logic.CreateMap(roomCount);

        // 2. Export to SVG string
        var svgDocument = MapSvgExporter.Export(generatedMap);
        var svgString = svgDocument.ToString();
        var svgImage = SvgStyleExtension.CreateVectorImage(svgString);
        svgDisplayElement!.style.backgroundImage = new StyleBackground(svgImage);
        statusLabel.text = $"Successfully generated and rendered SVG with {roomCount} rooms.";
        statusLabel.style.color = new StyleColor(Color.green);
    }
}