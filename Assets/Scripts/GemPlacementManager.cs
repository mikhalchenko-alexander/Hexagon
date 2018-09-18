using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
		var neighboursToSwap = CoordinateUtils.Neighbours(cubeCoordinates)
			.Where(n => {
				var gemTypeAt = GemTypeAt(n);
				return gemTypeAt != GemType.None && gemTypeAt != gemType;
			})
			.ToList();

		foreach (var coroutine in neighboursToSwap.Select(n => StartCoroutine(RemoveGem(n))).ToList()) {
			yield return coroutine;
		}
		
		foreach (var coroutine in neighboursToSwap.Select(n => StartCoroutine(PutGem(gemType, n))).ToList()) {
			yield return coroutine;
		}
	}
}
