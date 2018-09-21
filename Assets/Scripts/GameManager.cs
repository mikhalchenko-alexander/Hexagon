using System;
using System.Collections;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager> {

	[SerializeField] private TextMeshProUGUI _redGemCountText;
	[SerializeField] private TextMeshProUGUI _blueGemCountText;
	[SerializeField] private WinnerBoard _winnerBoard;
	[SerializeField] private HexPins _hexPinsPrefab;
	
	private int _blueGemsCount;
	private int _redGemsCount;
	private bool _moveInProgress;

	private GemType _currentPlayer = GemType.Red;

	private Vector3Int? _selectedTile;

	public void SwitchPlayer() {
		if (_currentPlayer == GemType.None) return;

		_currentPlayer = _currentPlayer == GemType.Red ? GemType.Blue : GemType.Red;
		_selectedTile = null;
	}
	
	public void BoardInitialized() {
		UpdateGemCountersText();
		HighlightCurrentPlayerGems();
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

	public void TileClicked(Vector3Int cubeCoordinates) {
		if (_currentPlayer == GemType.None ||
		    !CanMoveFrom(cubeCoordinates) ||
		    _moveInProgress) return;

		if (GemPlacementManager.Instance.GemTypeAt(cubeCoordinates) == _currentPlayer) {
			GridManager.Instance.DeselectAllTiles();
			GridManager.Instance.SelectGemTile(cubeCoordinates, _currentPlayer);
			_selectedTile = cubeCoordinates;
		} else if (_selectedTile != null && CurrentPlayerCanMoveTo(_selectedTile.Value, cubeCoordinates)) {
			_moveInProgress = true;
			StartCoroutine(DoMove(_selectedTile.Value, cubeCoordinates));
		}
	}

	public void BackToMenu() {
		SceneManager.LoadScene("Menu");
	}

	private IEnumerator DoMove(Vector3Int from, Vector3Int to) {
		switch (CoordinateUtils.CubeDistance(from, to)) {
			case 1:
				yield return DoSplitMove(to);
				break;
			case 2:
				yield return DoJumpMove(from, to);
				break;
		}
		
		UpdateGemCountersText();
		CheckWinner();
		SwitchPlayer();
		HighlightCurrentPlayerGems();
		
		_moveInProgress = false;
	}

	private void HighlightCurrentPlayerGems() {
		var highlightCoordinates = GemPlacementManager.Instance.GetGemCoordinates(_currentPlayer)
			.Where(CanMoveFrom).ToList();
		GridManager.Instance.ShowCellBorders(highlightCoordinates);
	}

	private void UpdateGemCountersText() {
		_redGemCountText.text = _redGemsCount.ToString();
		_blueGemCountText.text = _blueGemsCount.ToString();
	}

	private void CheckWinner() {
		if (BoardHasNoEmptyCell() || AnyPlayerHasNoGems()) {
			if (_redGemsCount > _blueGemsCount) {
				FinishGame(GemType.Red);
			} else if (_blueGemsCount > _redGemsCount) {
				FinishGame(GemType.Blue);
			} else {
				FinishGame(GemType.None);
			}
		} else if (!NextPlayerHasMoves()) {
			FinishGame(_currentPlayer);
		}
	}

	private bool BoardHasNoEmptyCell() {
		return GridManager.Instance.CellCount() - (_redGemsCount + _blueGemsCount) == 0;
	}

	private bool AnyPlayerHasNoGems() {
		return _redGemsCount == 0 || _blueGemsCount == 0;
	}

	private bool NextPlayerHasMoves() {
		var nextPlayer = _currentPlayer == GemType.Red ? GemType.Blue : GemType.Red;

		var nextPlayerGemPositions = GemPlacementManager.Instance.GetGemCoordinates(nextPlayer);
		return nextPlayerGemPositions.Any(CanMoveFrom);
	}

	private bool CanMoveFrom(Vector3Int pos) {
		var neighbours = CoordinateUtils.Neighbours(pos).Where(GridManager.Instance.CellInBounds).ToList();
		if (neighbours.Any(n => GemPlacementManager.Instance.GemTypeAt(n) == GemType.None)) {
			return true;
		}

		var jumpNeighbours = neighbours.SelectMany(CoordinateUtils.Neighbours).Where(GridManager.Instance.CellInBounds);
		return jumpNeighbours.Any(n => GemPlacementManager.Instance.GemTypeAt(n) == GemType.None);
	}

	private void FinishGame(GemType winner) {
		_currentPlayer = GemType.None;
		_winnerBoard.Show();
		_winnerBoard.SetWinner(winner);
	}

	private IEnumerator DoJumpMove(Vector3Int from, Vector3Int to) {
		GridManager.Instance.DeselectAllTiles();
		EffectsManager.Instance.PlayDisappearSound();
		yield return GemPlacementManager.Instance.RemoveGem(from);
		EffectsManager.Instance.PlayAppearSound();
		yield return GemPlacementManager.Instance.PutGem(_currentPlayer, to);
		yield return AnimateHexPins(to);
		yield return GemPlacementManager.Instance.SwapGemsAround(_currentPlayer, to);
	}

	private IEnumerator DoSplitMove(Vector3Int to) {
		GridManager.Instance.DeselectAllTiles();
		EffectsManager.Instance.PlayAppearSound();
		yield return GemPlacementManager.Instance.PutGem(_currentPlayer, to);
		yield return AnimateHexPins(to);
		yield return GemPlacementManager.Instance.SwapGemsAround(_currentPlayer, to);
	}

	private IEnumerator AnimateHexPins(Vector3Int cubeCoordinates) {
		var neighbours = CoordinateUtils.Neighbours(cubeCoordinates);

		var directions = neighbours.Where(n => GemPlacementManager.Instance.GemTypeAt(n) != GemType.None &&
		                                       GemPlacementManager.Instance.GemTypeAt(n) != _currentPlayer)
			.Select(n => CoordinateUtils.GetDirection(cubeCoordinates, n))
			.ToList();

		if (directions.Count == 0) {
			yield return null;
		} else {
			yield return Instantiate(
					_hexPinsPrefab,
					GridManager.Instance.GetTileCenterPosition(cubeCoordinates),
					Quaternion.identity)
				.Animate(directions, _currentPlayer);
		}
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
