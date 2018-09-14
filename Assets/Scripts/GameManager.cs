using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager> {

	[SerializeField] private TextMeshProUGUI _redGemCountText;
	[SerializeField] private TextMeshProUGUI _blueGemCountText;
	[SerializeField] private WinnerBoard _winnerBoard;
	
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
		if (_currentPlayer == GemType.None) return;

		if (GemPlacementManager.Instance.GemTypeAt(cubeCoordinates) == _currentPlayer) {
			GridManager.Instance.DeselectAllTiles();
			GridManager.Instance.SelectGemTile(cubeCoordinates, _currentPlayer);
			_selectedTile = cubeCoordinates;
		} else if (_selectedTile != null && CurrentPlayerCanMoveTo(_selectedTile.Value, cubeCoordinates)) {
			DoMove(_selectedTile.Value, cubeCoordinates);
		}
	}

	public void BackToMenu() {
		SceneManager.LoadScene("Menu");
	}

	private void DoMove(Vector3Int from, Vector3Int to) {
		switch (CoordinateUtils.CubeDistance(from, to)) {
			case 1:
				DoSplitMove(from, to);
				CheckWinner();
				break;
			case 2:
				DoJumpMove(from, to);
				break;
		}
	}

	private void CheckWinner() {
		var emptyCellCount = GridManager.Instance.CellCount() - (_redGemsCount + _blueGemsCount);
		if (emptyCellCount == 0 || _redGemsCount == 0 || _blueGemsCount == 0) {
			if (_redGemsCount > _blueGemsCount) {
				FinishGame(GemType.Red);
			} else if (_blueGemsCount > _redGemsCount) {
				FinishGame(GemType.Blue);
			} else {
				FinishGame(GemType.None);
			}
		}
	}

	private void FinishGame(GemType winner) {
		_currentPlayer = GemType.None;
		_winnerBoard.Show();
		_winnerBoard.SetWinner(winner);
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
