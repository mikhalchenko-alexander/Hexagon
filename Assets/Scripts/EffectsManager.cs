using System.Collections.Generic;
using UnityEngine;

public class EffectsManager : MonoBehaviour {

	[SerializeField] private float _sparkleAnimationPeriod;
	[SerializeField] private int _sparkleAnimationCount;

	private float _sparkleTimer;
	
	void Update () {
		_sparkleTimer += Time.deltaTime;

		if (!(_sparkleTimer >= _sparkleAnimationPeriod)) return;

		_sparkleTimer = 0;
		
		var indicies = new List<int>(_sparkleAnimationCount);

		var gems = GemPlacementManager.Instance.GetGems();
		var range = new List<int>(gems.Count);
		for (var i = 0; i < gems.Count; i++) {
			range.Add(i);
		}
		while (indicies.Count < _sparkleAnimationCount && indicies.Count < gems.Count) {
			var idx = Random.Range(0, range.Count);
			indicies.Add(range[idx]);
			range.RemoveAt(idx);
		}

		foreach (var index in indicies) {
			gems[index].PlaySparkleAnimation();
		}
	}
}
