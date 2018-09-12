using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GemPlacementManager : Singleton<GemPlacementManager> {
	
	[SerializeField] private Tilemap _tilemap;
	[SerializeField] private Gem _gemPrefab;

	private Dictionary<Vector3Int, GemType> _gems = new Dictionary<Vector3Int, GemType>();
	private Dictionary<Vector3Int, Gem> _gemInstances = new Dictionary<Vector3Int, Gem>();
	
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
		_gemInstances[cubeCoordinates] = gem;
	}
	
	public void RemoveGem(Vector3Int cubeCoordinates) {
		_gems.Remove(cubeCoordinates);
		Destroy(_gemInstances[cubeCoordinates].gameObject);
	}

	public GemType GemTypeAt(Vector3Int cubeCoordinates) {
		return _gems.ContainsKey(cubeCoordinates) ? _gems[cubeCoordinates] : GemType.None;
	}

	public void SwapGemsAround(GemType gemType, Vector3Int cubeCoordinates) {
		var neighbours = CoordinateUtils.Neighbours(cubeCoordinates);
		foreach (var neighbour in neighbours) {
			var gemTypeAt = GemTypeAt(neighbour);
			if (gemTypeAt != GemType.None && gemTypeAt != gemType) {
				RemoveGem(neighbour);
				PutGem(gemType, neighbour);
			}
		}
	}
}
