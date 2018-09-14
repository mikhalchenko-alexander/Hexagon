using TMPro;
using UnityEngine;

public class WinnerBoard : MonoBehaviour {

	[SerializeField] private GameObject _redGem;
	[SerializeField] private GameObject _blueGem;
	[SerializeField] private TextMeshProUGUI _text;
	[SerializeField] private Color32 _redOutlineColor;
	[SerializeField] private Color32 _blueOutlineColor;
	
	public void Show() {
		gameObject.SetActive(true);
	}

	public void SetWinner(GemType winner) {
		if (winner != GemType.None) {
			if (winner == GemType.Red) {
				_redGem.SetActive(true);
				_blueGem.SetActive(false);
				_text.SetText("Rubies wins");
				_text.outlineColor = _redOutlineColor;
			} else {
				_blueGem.SetActive(true);
				_redGem.SetActive(false);
				_text.SetText("Sapphires wins");
				_text.outlineColor = _blueOutlineColor;
			}
		} else {
			_redGem.SetActive(false);
			_blueGem.SetActive(false);
			_text.SetText("Draw");
		}
		
	}
}
