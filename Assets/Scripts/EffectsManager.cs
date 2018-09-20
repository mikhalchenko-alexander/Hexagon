using System.Collections.Generic;
using UnityEngine;

public class EffectsManager : Singleton<EffectsManager> {

	[SerializeField] private float _sparkleAnimationPeriod;
	[SerializeField] private int _sparkleAnimationCount;

	[SerializeField] private AudioClip _gemAppearSound;
	[SerializeField] private AudioClip _gemDisappearSound;

	private float _sparkleTimer;
	private AudioSource _audioSource;

	private void Start() {
		_audioSource = GetComponent<AudioSource>();
	}

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

	public void PlayAppearSound() {
		Debug.Log("Appear");
		_audioSource.PlayOneShot(_gemAppearSound);
	}
	
	public void PlayDisappearSound() {
		Debug.Log("Disappear");
		_audioSource.PlayOneShot(_gemDisappearSound);
	}

}
