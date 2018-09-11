using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GemPlacementManager : Singleton<GemPlacementManager> {
	
	[SerializeField] private Tilemap _tilemap;
	[SerializeField] private Gem _gemPrefab;

	private Dictionary<Vector3Int, GemType> _gems = new Dictionary<Vector3Int, GemType>();
	
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
		_gems[axialCoordinates] = gemType;
	}

	public GemType GemTypeAt(Vector3Int axialCoordinates) {
		return _gems.ContainsKey(axialCoordinates) ? _gems[axialCoordinates] : GemType.None;
	}
}
