using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Module.MapGenerator.Core.Runtime;
using Module.SvgUtility;
using UnityEngine;

namespace Module.MapGenerator.Svg;

public static class MapSvgExporter
{
    public static SvgDocument Export(Map targetMap)
    {
        return Export(targetMap, ExportSetting.Default);
    }

    public static SvgDocument Export(Map targetMap, ExportSetting setting)
    {
        if (targetMap.RoomList.Count == 0)
        {
            return EmptySvg();
        }

        var document = CreateSvgDocument(targetMap, setting)
            .DrawRoomList(targetMap, setting)
            .DrawConnectionList(targetMap, setting);

        return document;
    }

    private static SvgDocument CreateSvgDocument(Map targetMap, ExportSetting setting)
    {
        var bounds = targetMap.ToBounds();
        var margin = Vector2.one * (setting.CellSize + setting.Margin);
        var leftDown = bounds.Position.AsF() * setting.CellSize - margin;
        var rightUp = (bounds.Size + bounds.Position).AsF() * setting.CellSize + margin;

        var root = new SvgDocument(leftDown, rightUp);

        return root;
    }

    private static SvgDocument DrawRoomList(this SvgDocument document, Map targetMap, ExportSetting setting)
    {
        foreach (var room in targetMap.RoomList)
        {
            var centerPosition = setting.GetGridCenter(room.Position);
            // セルの中心 - 左下のパディング
            var leftDownPos = centerPosition - setting.PaddedCellSize * Vector2.one / 2;
            // サイズ分のセルサイズ - 四隅のパディング
            var size = room.Size.AsF() * setting.CellSize - Vector2.one * setting.RoomPadding;

            var element = SvgElementFactory.Rect(leftDownPos, size)
                .Fill(Color.white)
                .Stroke(Color.black);

            document.AddElement(element);
        }

        return document;
    }

    private static SvgDocument DrawConnectionList(this SvgDocument document, Map targetMap, ExportSetting setting)
    {
        foreach (var connector in targetMap.RoomList.SelectMany(x => x.ConnectorList)
                     .Where(x => x.ConnectionState == ConnectionStateType.Connected))
        {
            var connPos = connector.AsPosition();
            var cellCenter = setting.GetGridCenter(connPos);

            // 接続方向取得
            var offsetX = connector.ConnectTo switch
            {
                ConnectVecType.Left => -setting.CellSize,
                ConnectVecType.Right => setting.CellSize,
                _ => throw new InvalidEnumArgumentException()
            };
            var offset = new Vector2(offsetX / 2, 0);

            var circleCenter = cellCenter + offset;
            var circle = SvgElementFactory.Circle(circleCenter, 4) // FIXME
                .Fill(Color.green);
            var line = SvgElementFactory.Line(circleCenter, circleCenter + offset);
            document.AddElement(circle);
            document.AddElement(line);
        }

        return document;
    }

    private static SvgDocument EmptySvg()
    {
        return new SvgDocument(Vector2Int.zero, Vector2Int.zero);
    }
}

/// <summary>
/// SVG出力用の設定オプションを管理する構造体
/// </summary>
public readonly record struct ExportSetting(float CellSize, float Margin, float RoomPadding)
{
    public static ExportSetting Default => new ExportSetting(64, 16, 4);
    public float PaddedCellSize => CellSize - RoomPadding;
}

public static class Extensions
{
    /// <summary>
    /// グリッド座標の中心座標に変換する
    /// </summary>
    internal static Vector2 GetGridCenter(this ExportSetting setting, Vector2Int cellPos)
    {
        return cellPos.AsF() * setting.CellSize;
    }

    internal static Bounds2D ToBounds(this Map self)
    {
        var minX = self.RoomList.Min(x => x.Position.x);
        var minY = self.RoomList.Min(x => x.Position.y);

        var maxX = self.RoomList.Max(x => x.Position.x + x.Size.x - 1);
        var maxY = self.RoomList.Max(x => x.Position.y + x.Size.y - 1);

        var position = new Vector2Int(minX, minY);
        var size = new Vector2Int(maxX, maxY) - position;

        return new Bounds2D(position, size);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static Vector2 AsF(this Vector2Int self)
    {
        return new Vector2(self.x, self.y);
    }
}