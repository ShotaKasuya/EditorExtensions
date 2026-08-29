using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


namespace Module.MapGenerator.Core.Runtime;

public enum ConnectVecType
{
    Left,
    Right,
}

public enum ConnectionStateType
{
    NotUsed,
    Connected,
    Removed,
}

public readonly record struct RoomConnector(
    Room Parent,
    Vector2Int Offset,
    ConnectVecType ConnectTo,
    ConnectionStateType ConnectionState
)
{
    public static RoomConnector CreateLeft(Room parent, Vector2Int offset)
    {
        return new RoomConnector(parent, offset, ConnectVecType.Left, ConnectionStateType.NotUsed);
    }

    public static RoomConnector CreateRight(Room parent, Vector2Int offset)
    {
        return new RoomConnector(parent, offset, ConnectVecType.Right, ConnectionStateType.NotUsed);
    }
}

public class Map
{
    public readonly List<Room> RoomList = new();

    /// <summary>
    /// 該当座標が`roomList`と重なっている
    /// </summary>
    public bool IsOverlap(Vector2Int pos)
    {
        foreach (var room in RoomList)
        {
            if (room.IsOverlap(pos))
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// 使用されていない`Connection`を取得する
    /// </summary>
    public List<RoomConnector> GetUnusedConnector()
    {
        return RoomList.SelectMany(x => x.ConnectorList)
            .Where(x => x.ConnectionState == ConnectionStateType.NotUsed)
            .ToList();
    }

    /// <summary>
    /// `Room`を追加し、`RoomConnector`を更新する
    /// </summary>
    public void AddRoom(Room room)
    {
        RoomList.Add(room);
        var unusedConnectorList = GetUnusedConnector();
        for (int i = 0; i < unusedConnectorList.Count; i++)
        {
            for (int j = 0; j < room.ConnectorList.Count; j++)
            {
                if (IsPair(unusedConnectorList[i], room.ConnectorList[j]))
                {
                    room.Use(room.ConnectorList[j]);
                    unusedConnectorList[i].Parent.Use(unusedConnectorList[i]);
                }
            }
        }
    }

    /// <summary>
    /// Enumerates all connected room pairs in the map.
    /// Returns each connection as a tuple of (Room A, its connector, Room B, its connector).
    /// Each connection appears only once.
    /// </summary>
    public IEnumerable<(Room roomA, RoomConnector connectorA, Room roomB, RoomConnector connectorB)> GetAllConnections()
    {
        var visited = new HashSet<(RoomConnector, RoomConnector)>(new ConnectorPairComparer());
        foreach (var room in RoomList)
        {
            foreach (var connector in room.ConnectorList)
            {
                if (connector.ConnectionState != ConnectionStateType.Connected) continue;
                // Find the matching connector in other rooms
                foreach (var otherRoom in RoomList)
                {
                    foreach (var otherConnector in otherRoom.ConnectorList)
                    {
                        if (connector == otherConnector) continue;
                        if (otherConnector.ConnectionState != ConnectionStateType.Connected) continue;

                        if (IsPair(connector, otherConnector))
                        {
                            var pair = (connector, otherConnector);
                            if (!visited.Contains(pair))
                            {
                                visited.Add(pair);
                                yield return (connector.Parent, connector, otherConnector.Parent, otherConnector);
                            }
                        }
                    }
                }
            }
        }
    }

    // Comparer to treat connector pairs as unordered
    private class ConnectorPairComparer : IEqualityComparer<(RoomConnector, RoomConnector)>
    {
        public bool Equals((RoomConnector, RoomConnector) x, (RoomConnector, RoomConnector) y)
        {
            return (x.Item1 == y.Item1 && x.Item2 == y.Item2) ||
                   (x.Item1 == y.Item2 && x.Item2 == y.Item1);
        }

        public int GetHashCode((RoomConnector, RoomConnector) obj)
        {
            // Order-independent hash code
            int h1 = obj.Item1.GetHashCode();
            int h2 = obj.Item2.GetHashCode();
            return h1 ^ h2;
        }
    }

    /// <summary>
    /// 二つの`Connector`が向かい合っているか
    /// </summary>
    private static bool IsPair(RoomConnector lhs, RoomConnector rhs)
    {
        var lhsCanConnect = lhs.LookAt() == rhs.AsPosition();
        var rhsCanConnect = rhs.LookAt() == lhs.AsPosition();

        return lhsCanConnect & rhsCanConnect;
    }
}

public class Room
{
    public readonly List<RoomConnector> ConnectorList = new();
    public readonly Vector2Int Position;
    public readonly Vector2Int Size;

    private Room(Vector2Int position, Vector2Int size)
    {
        Position = position;
        Size = size;
    }

    public static Room Create1X1(Vector2Int position)
    {
        var newEntity = new Room(position, Vector2Int.one);

        var leftConnector = RoomConnector.CreateLeft(newEntity, Vector2Int.zero);
        var rightConnector = RoomConnector.CreateRight(newEntity, Vector2Int.zero);

        newEntity.ConnectorList.Add(leftConnector);
        newEntity.ConnectorList.Add(rightConnector);

        return newEntity;
    }

    public static Room Create2X1(Vector2Int position)
    {
        var newEntity = new Room(position, new Vector2Int(2, 1));

        var leftConnector = RoomConnector.CreateLeft(newEntity, Vector2Int.zero);
        var rightConnector = RoomConnector.CreateRight(newEntity, Vector2Int.right);

        newEntity.ConnectorList.Add(leftConnector);
        newEntity.ConnectorList.Add(rightConnector);

        return newEntity;
    }

    public static Room Create1X2(Vector2Int position)
    {
        var newEntity = new Room(position, new Vector2Int(1, 2));

        var leftConnector = RoomConnector.CreateLeft(newEntity, Vector2Int.zero);
        var rightConnector = RoomConnector.CreateRight(newEntity, Vector2Int.zero);

        var leftConnector2 = RoomConnector.CreateLeft(newEntity, Vector2Int.up);
        var rightConnector2 = RoomConnector.CreateRight(newEntity, Vector2Int.up);

        newEntity.ConnectorList.Add(leftConnector);
        newEntity.ConnectorList.Add(rightConnector);
        newEntity.ConnectorList.Add(leftConnector2);
        newEntity.ConnectorList.Add(rightConnector2);

        return newEntity;
    }

    /// <summary>
    /// connectorを使用済みに変更
    /// </summary>
    public void Use(RoomConnector connector)
    {
        for (int i = 0; i < ConnectorList.Count; i++)
        {
            if (connector == ConnectorList[i])
            {
                ConnectorList[i] = connector with
                {
                    ConnectionState = ConnectionStateType.Connected,
                };
            }
        }
    }

    /// <summary>
    /// 該当座標と重なっているか
    /// </summary>
    public bool IsOverlap(Vector2Int pos)
    {
        var x = pos.x >= Position.x & pos.x <= (Position.x + Size.x - 1);
        var y = pos.y >= Position.y & pos.y <= (Position.y + Size.y - 1);

        return x & y;
    }

    public override string ToString()
    {
        var builder = new StringBuilder(nameof(Room));

        builder.Append($"position: {Position}, size: {Size}");
        return builder.ToString();
    }
}

public static class Extension
{
    public static Vector2Int AsPosition(this RoomConnector connector)
    {
        return connector.Parent.Position + connector.Offset;
    }

    public static Vector2Int LookAt(this RoomConnector connector)
    {
        var connectionPoint = connector.AsPosition();
        var lookAt = connector.ConnectTo switch
        {
            ConnectVecType.Left => Vector2Int.left,
            ConnectVecType.Right => Vector2Int.right,
            _ => throw new ArgumentOutOfRangeException($"{connector}")
        };

        return connectionPoint + lookAt;
    }
}