using System;
using UnityEngine;

public class GameManager : Singleton<GameManager> {

	private int _blueGemsCount;
	private int _redGemsCount;

	private GemType _currentPlayer = GemType.Red;

	public GemType CurrentPlayer {
		get { return _currentPlayer; }
	}

	public void SwitchPlayer() {
		switch (_currentPlayer) {
			case GemType.Blue:
				_currentPlayer = GemType.Red;
				break;
			case GemType.Red:
				_currentPlayer = GemType.Blue;
				break;
			default:
				throw new ArgumentOutOfRangeException();
		}
	}
	
	public void GemAdded(GemType gemType) {
		switch (gemType) {
			case GemType.Blue:
				_blueGemsCount++;
				break;
			case GemType.Red:
				_redGemsCount++;
				break;
			default:
				throw new ArgumentOutOfRangeException("gemType", gemType, null);
		}
	}
	
	public void GemRemoved(GemType gemType) {
		switch (gemType) {
			case GemType.Blue:
				_blueGemsCount--;
				break;
			case GemType.Red:
				_redGemsCount--;
				break;
			default:
				throw new ArgumentOutOfRangeException("gemType", gemType, null);
		}
	}

	public void TileClicked(Vector3Int axialCoordinates) {
		GridManager.Instance.SelectTile(axialCoordinates, _currentPlayer);
		SwitchPlayer();
	}
}
