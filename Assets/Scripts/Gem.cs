﻿using System;
using System.Linq;
using UnityEngine;

public enum GemType {
	Blue, Red, None
}

public class Gem : MonoBehaviour {

	[SerializeField] private Sprite _gemGlowRed;
	[SerializeField] private Sprite _gemBgRed;
	[SerializeField] private Sprite _gemRed;
	
	[SerializeField] private Sprite _gemGlowBlue;
	[SerializeField] private Sprite _gemBgBlue;
	[SerializeField] private Sprite _gemBlue;

	private SpriteRenderer _glowRenderer;
	private SpriteRenderer _bgRenderer;
	private SpriteRenderer _gemRenderer;
	
	void Awake () {
		_glowRenderer = FindRenderer("Glow");
		_bgRenderer = FindRenderer("Background");
		_gemRenderer = FindRenderer("Gem");
	}

	private SpriteRenderer FindRenderer(String rendererName) {
		return GetComponentsInChildren<SpriteRenderer>().First(go => go.name.Equals(rendererName));
	}

	private float _time;
	private GemType _gemType = GemType.None;
	
	void Update () {
		_time += Time.deltaTime;

		if (_time > 1) {
			Flip();
			_time = 0;
		}
	}

	private void Flip() {
		if (_gemType == GemType.Red) {
			SetBlue();
		} else if (_gemType == GemType.Blue) {
			SetRed();
		}
	}

	public void SetRed() {
		_glowRenderer.sprite = _gemGlowRed;
		_bgRenderer.sprite = _gemBgRed;
		_gemRenderer.sprite = _gemRed;
		_gemType = GemType.Red;
	}
	
	public void SetBlue() {
		_glowRenderer.sprite = _gemGlowBlue;
		_bgRenderer.sprite = _gemBgBlue;
		_gemRenderer.sprite = _gemBlue;
		_gemType = GemType.Blue;
	}

}
