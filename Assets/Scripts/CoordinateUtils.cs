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
	
}
