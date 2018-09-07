using UnityEngine;

public class CoordinateUtils {

	public static Vector3Int OffsetToAxial(Vector3Int offset) {
		var x = offset.x;
		var z = offset.y - (offset.x - (offset.x & 1)) / 2;
		var y = -x - z;
		
		return new Vector3Int(x, y, z);
	}

	public static Vector3Int AxialToOffset(Vector3Int axial) {
		var x = axial.x;
		var y = axial.z + (axial.x - (axial.x & 1)) / 2;
		return new Vector3Int(x, y, 0);
	}
	
}
