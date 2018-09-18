using System;
using System.Collections;
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
	
	[SerializeField] private float _animationTime;

	private SpriteRenderer _glowRenderer;
	private SpriteRenderer _bgRenderer;
	private SpriteRenderer _gemRenderer;
	private float _minScale = 0f;
	private float _maxScale;
	
	void Awake () {
		_glowRenderer = FindRenderer("Glow");
		_bgRenderer = FindRenderer("Background");
		_gemRenderer = FindRenderer("Gem");
		_maxScale = transform.localScale.y;
		transform.localScale = new Vector3(transform.localScale.x, 0, transform.localScale.z);
	}

	private SpriteRenderer FindRenderer(String rendererName) {
		return GetComponentsInChildren<SpriteRenderer>().First(go => go.name.Equals(rendererName));
	}

	private GemType _gemType = GemType.None;

	public GemType GemType {
		get { return _gemType; }
		set { _gemType = value; }
	}

	public IEnumerator SetGemType(GemType gemType) {
		if (GemType != GemType.None) {
			yield return AnimateDisappear();
		}
		
		GemType = gemType;
		
		yield return AnimateAppear(gemType);
	}

	public IEnumerator AnimateAppear(GemType gemType) {
		_glowRenderer.sprite = gemType == GemType.Red ? _gemGlowRed : _gemGlowBlue;
		_bgRenderer.sprite = gemType == GemType.Red ? _gemBgRed : _gemBgBlue;
		_gemRenderer.sprite = gemType == GemType.Red ? _gemRed : _gemBlue;
		
		transform.localScale = new Vector3(transform.localScale.x, 0, transform.localScale.z);
		var time = 0f;
		var currentScale = _minScale;
		
		while (currentScale < _maxScale) {
			time += Time.deltaTime;
			
			currentScale = _maxScale * time / _animationTime;
			currentScale = currentScale > _maxScale ? _maxScale : currentScale;
			
			transform.localScale = new Vector3(transform.localScale.x, currentScale, transform.localScale.z);
			
			yield return null;
		}
	}
	
	public IEnumerator AnimateDisappear() {
		transform.localScale = new Vector3(transform.localScale.x, _maxScale, transform.localScale.z);
		var time = 0f;
		var currentScale = _maxScale;
		
		while (currentScale > _minScale) {
			time += Time.deltaTime;
			
			currentScale = _maxScale - _maxScale * time / _animationTime;
			currentScale = currentScale < _minScale ? _minScale : currentScale;
			
			transform.localScale = new Vector3(transform.localScale.x, currentScale, transform.localScale.z);
			
			yield return null;
		}
	}

}
