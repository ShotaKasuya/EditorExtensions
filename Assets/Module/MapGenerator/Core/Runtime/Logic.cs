using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Module.MapGenerator.Core.Runtime;

public static class Logic
{
    private enum GenerateSelectionType
    {
        OneCell,
        Right,
        Left,
        Up,
        Down,
        Count,
    }

    private const int RandomRoleMax = 1 << 8;

    public static Map CreateMap(int roomCount)
    {
        var map = new Map();
        var starterRoom = StarterRoom();
        map.AddRoom(starterRoom);

        for (int i = 0; i < roomCount; i++)
        {
            var unusedRoomList = map.GetUnusedConnector();
            
            // ランダムにつなげる場所を選ぶ
            var randomIndex = Random.Range(0, unusedRoomList.Count);
            var selectedConnection = unusedRoomList[randomIndex];
            var generatePosition = selectedConnection.LookAt();
            
            // 規定回数リトライする
            for (int j = 0; j < RandomRoleMax; j++)
            {
                var selection = (GenerateSelectionType)Random.Range(0, (int)GenerateSelectionType.Count);
                
                // 部屋が伸びる方向が接合部に向かうならリトライ
                var conflictLeft = selection == GenerateSelectionType.Left &
                                   selectedConnection.ConnectTo == ConnectVecType.Right;
                var conflictRight = selection == GenerateSelectionType.Right &
                                   selectedConnection.ConnectTo == ConnectVecType.Left;
                if (conflictRight|conflictLeft)
                {
                    continue;
                }

                // 複数マスの場合の空きマスチェック
                var checkPosition = generatePosition + selection switch
                {
                    GenerateSelectionType.Left => Vector2Int.left,
                    GenerateSelectionType.Right => Vector2Int.right,
                    GenerateSelectionType.Up => Vector2Int.up,
                    GenerateSelectionType.Down => Vector2Int.down,
                    GenerateSelectionType.OneCell => Vector2Int.zero,
                    _ => throw new ArgumentOutOfRangeException($"{roomCount}")
                };
                var isOverlap = map.IsOverlap(checkPosition);
                if (isOverlap)
                {
                    continue;
                }

                var newRoom = selection switch
                {
                    GenerateSelectionType.OneCell => Room.Create1X1(generatePosition),
                    GenerateSelectionType.Right => Room.Create2X1(generatePosition),
                    GenerateSelectionType.Left => Room.Create2X1(generatePosition + Vector2Int.left),
                    GenerateSelectionType.Up => Room.Create1X2(generatePosition),
                    GenerateSelectionType.Down => Room.Create1X2(generatePosition + Vector2Int.down),
                    _ => throw new ArgumentOutOfRangeException($"{selection}")
                };
                
                map.AddRoom(newRoom);
                // 生成成功時はリトライしない
                break;
            }
        }

        return map;
    }

    private static Room StarterRoom()
    {
        var room = Room.Create1X1(Vector2Int.zero);

        for (int i = 0; i < room.ConnectorList.Count; i++)
        {
            if (room.ConnectorList[i].ConnectTo==ConnectVecType.Left)
            {
                room.ConnectorList[i] = room.ConnectorList[i]with
                {
                    ConnectionState = ConnectionStateType.Removed,
                };
            }
        }

        return room;
    }
}