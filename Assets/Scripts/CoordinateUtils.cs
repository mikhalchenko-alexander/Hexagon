using System.Collections.Generic;
using UnityEngine;

public class CoordinateUtils {

	public static Vector3Int OffsetToAxial(Vector3Int offset) {
		var x = offset.y;
		var z = -offset.x - (offset.y + (offset.y & 1)) / 2;
		var y = -offset.y - z;
		
		return new Vector3Int(x, y, z);
	}

	public static Vector3Int AxialToOffset(Vector3Int axial) {
		var y = axial.x;
		var x = axial.z + (axial.x + (axial.x & 1)) / 2;
		return new Vector3Int(-x, y, 0);
	}

	public static List<Vector3Int> Neighbours(Vector3Int axialCoordinates) {
		var result = new List<Vector3Int> {
			new Vector3Int(axialCoordinates.x + 1, axialCoordinates.y - 1, axialCoordinates.z),
			new Vector3Int(axialCoordinates.x - 1, axialCoordinates.y + 1, axialCoordinates.z),
			new Vector3Int(axialCoordinates.x, axialCoordinates.y + 1, axialCoordinates.z - 1),
			new Vector3Int(axialCoordinates.x, axialCoordinates.y - 1, axialCoordinates.z + 1),
			new Vector3Int(axialCoordinates.x + 1, axialCoordinates.y, axialCoordinates.z - 1),
			new Vector3Int(axialCoordinates.x - 1, axialCoordinates.y, axialCoordinates.z + 1)
		};
		return result;
	}
	
}
