using System;
using TMPro;
using UnityEngine;

public class GameManager : Singleton<GameManager> {

	[SerializeField] private TextMeshProUGUI _redGemCountText;
	[SerializeField] private TextMeshProUGUI _blueGemCountText;
	
	private int _blueGemsCount;
	private int _redGemsCount;

	private GemType _currentPlayer = GemType.Red;

	private Vector3Int? _selectedTile;

	private int BlueGemsCount {
		get { return _blueGemsCount; }
		set {
			_blueGemsCount = value;
			_blueGemCountText.text = value.ToString();
		}
	}

	private int RedGemsCount {
		get { return _redGemsCount; }
		set {
			_redGemsCount = value;
			_redGemCountText.text = value.ToString();
		}
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

		_selectedTile = null;
	}
	
	public void GemAdded(GemType gemType) {
		switch (gemType) {
			case GemType.Blue:
				BlueGemsCount++;
				break;
			case GemType.Red:
				RedGemsCount++;
				break;
			default:
				throw new ArgumentOutOfRangeException("gemType", gemType, null);
		}
	}
	
	public void GemRemoved(GemType gemType) {
		switch (gemType) {
			case GemType.Blue:
				BlueGemsCount--;
				break;
			case GemType.Red:
				RedGemsCount--;
				break;
			default:
				throw new ArgumentOutOfRangeException("gemType", gemType, null);
		}
	}

	public void TileClicked(Vector3Int cubeCoordinates) {
		if (GemPlacementManager.Instance.GemTypeAt(cubeCoordinates) == _currentPlayer) {
			GridManager.Instance.DeselectAllTiles();
			GridManager.Instance.SelectGemTile(cubeCoordinates, _currentPlayer);
			_selectedTile = cubeCoordinates;
		} else if (_selectedTile != null && CurrentPlayerCanMoveTo(_selectedTile.Value, cubeCoordinates)) {
			DoMove(_selectedTile.Value, cubeCoordinates);
		}
	}

	private void DoMove(Vector3Int from, Vector3Int to) {
		switch (CoordinateUtils.CubeDistance(from, to)) {
			case 1:
				DoSplitMove(from, to);
				break;
			case 2:
				DoJumpMove(from, to);
				break;
		}

		
	}

	private void DoJumpMove(Vector3Int from, Vector3Int to) {
		GridManager.Instance.DeselectAllTiles();
		GemPlacementManager.Instance.RemoveGem(from);
		GemPlacementManager.Instance.PutGem(_currentPlayer, to);
		GemPlacementManager.Instance.SwapGemsAround(_currentPlayer, to);
		SwitchPlayer();
	}

	private void DoSplitMove(Vector3Int from, Vector3Int to) {
		GridManager.Instance.DeselectAllTiles();
		GemPlacementManager.Instance.PutGem(_currentPlayer, to);
		GemPlacementManager.Instance.SwapGemsAround(_currentPlayer, to);
		SwitchPlayer();
	}

	private bool CurrentPlayerCanMoveTo(Vector3Int from, Vector3Int to) {
		return TargetHexIsEmpty(to) &&
		       TargetHexInMoveRange(from, to);
	}

	private static bool TargetHexIsEmpty(Vector3Int cubeCoordinates) {
		return GemPlacementManager.Instance.GemTypeAt(cubeCoordinates) == GemType.None;
	}

	private static bool TargetHexInMoveRange(Vector3Int from, Vector3Int to) {
		return InSplitRange(from, to) || InJumpRange(from, to);
	}

	private static bool InJumpRange(Vector3Int from, Vector3Int to) {
		return CoordinateUtils.CubeDistance(from, to) == 1;
	}

	private static bool InSplitRange(Vector3Int from, Vector3Int to) {
		return CoordinateUtils.CubeDistance(from, to) == 2;
	}
}
