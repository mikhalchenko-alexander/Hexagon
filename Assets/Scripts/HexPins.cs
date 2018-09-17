using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum Direction {
	Top, TopRight, BottomRight, Bottom, BottomLeft, TopLeft
}

public class HexPins : MonoBehaviour {

	[SerializeField] private Sprite _redPinSprite;
	[SerializeField] private Sprite _bluePinSprite;
	[SerializeField] private float _minScale;
	[SerializeField] private float _maxScale;
	[SerializeField] private float _animationTime;
	
	private Dictionary<Direction, GameObject> _pins = new Dictionary<Direction, GameObject>();

	void Start() {
		foreach (var d in Enum.GetValues(typeof(Direction)).Cast<Direction>()) {
			var pin = transform.Find(d.ToString()).gameObject;
			_pins.Add(d, pin);
		}
	}

	public IEnumerator Animate(List<Direction> directions, GemType gemType) {
		var sprite = gemType == GemType.Red ? _redPinSprite : _bluePinSprite;
		foreach (var direction in directions) {
			var pin = _pins[direction];
			pin.SetActive(true);
			pin.GetComponent<SpriteRenderer>().sprite = sprite;
			pin.transform.localScale = new Vector3(pin.transform.localScale.x, _minScale, pin.transform.localScale.z);
		}

		var time = 0f;
		var currentScale = _minScale;
		
		while (currentScale < _maxScale) {
			time += Time.deltaTime;
			
			currentScale = _maxScale * time / _animationTime;
			currentScale = currentScale > _maxScale ? _maxScale : currentScale;
			
			foreach (var direction in directions) {
				var pin = _pins[direction];
				var pinScale = pin.transform.localScale;
				pin.transform.localScale = new Vector3(pinScale.x, currentScale, pinScale.z);
			}

			yield return null;
		}

		yield return null;
	}
	
}
