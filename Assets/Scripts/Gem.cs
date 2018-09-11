using System;
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

	private float time = 0;
	private bool red = true;
	
	void Update () {
		time += Time.deltaTime;

		if (time > 1) {
			Flip();
			time = 0;
		}
	}

	private void Flip() {
		if (red) {
			SetBlue();
		} else {
			SetRed();
		}
	}

	public void SetRed() {
		_glowRenderer.sprite = _gemGlowRed;
		_bgRenderer.sprite = _gemBgRed;
		_gemRenderer.sprite = _gemRed;
		red = true;
	}
	
	public void SetBlue() {
		_glowRenderer.sprite = _gemGlowBlue;
		_bgRenderer.sprite = _gemBgBlue;
		_gemRenderer.sprite = _gemBlue;
		red = false;
	}

}
