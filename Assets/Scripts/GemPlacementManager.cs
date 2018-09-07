using UnityEngine;
using UnityEngine.Tilemaps;

public class GemPlacementManager : MonoBehaviour {
	
	[SerializeField] private Tilemap _tilemap;

	public void PutGem(Gem gem, Vector3Int axialCoordinates) {
		var offsetCoords = CoordinateUtils.AxialToOffset(axialCoordinates);
		gem.transform.position = _tilemap.GetCellCenterWorld(offsetCoords);
	}
	
}
