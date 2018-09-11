using System;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GemPlacementManager : Singleton<GemPlacementManager> {
	
	[SerializeField] private Tilemap _tilemap;
	[SerializeField] private Gem _gemPrefab;

	public void PutGem(GemType gemType, Vector3Int axialCoordinates) {
		var offsetCoords = CoordinateUtils.AxialToOffset(axialCoordinates);
		
		var gem = Instantiate(_gemPrefab);

		switch (gemType) {
			case GemType.Blue:
				gem.SetBlue();
				break;
			case GemType.Red:
				gem.SetRed();
				break;
			default:
				throw new ArgumentOutOfRangeException("gemType", gemType, null);
		}
		
		gem.transform.position = _tilemap.GetCellCenterWorld(offsetCoords);
	}
	
}
