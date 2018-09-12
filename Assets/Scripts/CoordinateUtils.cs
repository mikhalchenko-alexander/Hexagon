using System;
using System.Collections.Generic;
using UnityEngine;

public static class CoordinateUtils {
    public static Vector3Int OffsetToCube(Vector3Int offset) {
        var x = offset.y;
        var z = -offset.x - (offset.y + (offset.y & 1)) / 2;
        var y = -offset.y - z;

        return new Vector3Int(x, y, z);
    }

    public static Vector3Int CubeToOffset(Vector3Int cube) {
        var y = cube.x;
        var x = cube.z + (cube.x + (cube.x & 1)) / 2;
        return new Vector3Int(-x, y, 0);
    }

    public static List<Vector3Int> Neighbours(Vector3Int cubeCoordinates) {
        var result = new List<Vector3Int> {
            new Vector3Int(cubeCoordinates.x + 1, cubeCoordinates.y - 1, cubeCoordinates.z),
            new Vector3Int(cubeCoordinates.x - 1, cubeCoordinates.y + 1, cubeCoordinates.z),
            new Vector3Int(cubeCoordinates.x, cubeCoordinates.y + 1, cubeCoordinates.z - 1),
            new Vector3Int(cubeCoordinates.x, cubeCoordinates.y - 1, cubeCoordinates.z + 1),
            new Vector3Int(cubeCoordinates.x + 1, cubeCoordinates.y, cubeCoordinates.z - 1),
            new Vector3Int(cubeCoordinates.x - 1, cubeCoordinates.y, cubeCoordinates.z + 1)
        };
        return result;
    }

    public static int CubeDistance(Vector3Int a, Vector3Int b) {
        return (Math.Abs(a.x - b.x) + Math.Abs(a.y - b.y) + Math.Abs(a.z - b.z)) / 2;
    }
}
