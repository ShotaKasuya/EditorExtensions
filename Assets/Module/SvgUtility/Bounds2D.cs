using UnityEngine;

namespace Module.SvgUtility;

public readonly record struct Bounds2D(Vector2Int Position, Vector2Int Size)
{
    public int XMin => Position.x;
    public int YMin => Position.y;

    public int XMax => Position.x + Size.x;
    public int YMax => Position.y + Size.y;
}