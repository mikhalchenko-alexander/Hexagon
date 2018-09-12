using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GemPlacementManager : Singleton<GemPlacementManager> {
	
	[SerializeField] private Tilemap _tilemap;
	[SerializeField] private Gem _gemPrefab;

	private Dictionary<Vector3Int, GemType> _gems = new Dictionary<Vector3Int, GemType>();
	
	public void PutGem(GemType gemType, Vector3Int cubeCoordinates) {
		var offsetCoords = CoordinateUtils.CubeToOffset(cubeCoordinates);
		
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
		_gems[cubeCoordinates] = gemType;
	}

	public GemType GemTypeAt(Vector3Int cubeCoordinates) {
		return _gems.ContainsKey(cubeCoordinates) ? _gems[cubeCoordinates] : GemType.None;
	}
}
