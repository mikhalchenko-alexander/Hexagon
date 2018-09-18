using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GemPlacementManager : Singleton<GemPlacementManager> {
	
	[SerializeField] private Gem _gemPrefab;

	private readonly Dictionary<Vector3Int, Gem> _gems = new Dictionary<Vector3Int, Gem>();
	
	public IEnumerator PutGem(GemType gemType, Vector3Int cubeCoordinates) {
		var gem = Instantiate(_gemPrefab);
		gem.transform.position = GridManager.Instance.GetTileCenterPosition(cubeCoordinates);
		_gems[cubeCoordinates] = gem;
		
		switch (gemType) {
			case GemType.Blue: case GemType.Red:
				yield return gem.SetGemType(gemType);
				break;
			
			default:
				throw new ArgumentOutOfRangeException("gemType", gemType, null);
		}
		
		GameManager.Instance.GemAdded(gemType);
	}
	
	public IEnumerator RemoveGem(Vector3Int cubeCoordinates) {
		var gemTypeAt = GemTypeAt(cubeCoordinates);
		var gem = _gems[cubeCoordinates];

		yield return gem.AnimateDisappear();
		
		Destroy(gem.gameObject);
		_gems.Remove(cubeCoordinates);
		GameManager.Instance.GemRemoved(gemTypeAt);
	}

	public GemType GemTypeAt(Vector3Int cubeCoordinates) {
		return _gems.ContainsKey(cubeCoordinates) ? _gems[cubeCoordinates].GemType : GemType.None;
	}

	public IEnumerator SwapGemsAround(GemType gemType, Vector3Int cubeCoordinates) {
		var neighbours = CoordinateUtils.Neighbours(cubeCoordinates);
		foreach (var neighbour in neighbours) {
			var gemTypeAt = GemTypeAt(neighbour);
			if (gemTypeAt != GemType.None && gemTypeAt != gemType) {
				yield return RemoveGem(neighbour);
				yield return PutGem(gemType, neighbour);
			}
		}
	}
}
