using System;
using System.Collections.Generic;
using UnityEngine;

public class GemPlacementManager : Singleton<GemPlacementManager> {
	
	[SerializeField] private Gem _gemPrefab;

	private readonly Dictionary<Vector3Int, Gem> _gems = new Dictionary<Vector3Int, Gem>();
	
	public void PutGem(GemType gemType, Vector3Int cubeCoordinates) {
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
		
		gem.transform.position = GridManager.Instance.GetTileCenterPosition(cubeCoordinates);
		_gems[cubeCoordinates] = gem;
		GameManager.Instance.GemAdded(gemType);
	}
	
	public void RemoveGem(Vector3Int cubeCoordinates) {
		var gemTypeAt = GemTypeAt(cubeCoordinates);
		Destroy(_gems[cubeCoordinates].gameObject);
		_gems.Remove(cubeCoordinates);
		GameManager.Instance.GemRemoved(gemTypeAt);
	}

	public GemType GemTypeAt(Vector3Int cubeCoordinates) {
		return _gems.ContainsKey(cubeCoordinates) ? _gems[cubeCoordinates].GemType : GemType.None;
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
