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

	private GemType _currentPlayer = GemType.Red;

	private Vector3Int? _selectedTile;

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
	
	public void BoardInitialized() {
		UpdateGemCountersText();
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
		if (_currentPlayer == GemType.None) return;

		if (GemPlacementManager.Instance.GemTypeAt(cubeCoordinates) == _currentPlayer) {
			GridManager.Instance.DeselectAllTiles();
			GridManager.Instance.SelectGemTile(cubeCoordinates, _currentPlayer);
			_selectedTile = cubeCoordinates;
		} else if (_selectedTile != null && CurrentPlayerCanMoveTo(_selectedTile.Value, cubeCoordinates)) {
			StartCoroutine(DoMove(_selectedTile.Value, cubeCoordinates));
		}
	}

	public void BackToMenu() {
		SceneManager.LoadScene("Menu");
	}

	private IEnumerator DoMove(Vector3Int from, Vector3Int to) {
		switch (CoordinateUtils.CubeDistance(from, to)) {
			case 1:
				yield return DoSplitMove(from, to);
				UpdateGemCountersText();
				CheckWinner();
				break;
			case 2:
				yield return DoJumpMove(from, to);
				UpdateGemCountersText();
				break;
		}
	}

	private void UpdateGemCountersText() {
		_redGemCountText.text = _redGemsCount.ToString();
		_blueGemCountText.text = _blueGemsCount.ToString();
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

	private IEnumerator DoJumpMove(Vector3Int from, Vector3Int to) {
		GridManager.Instance.DeselectAllTiles();
		yield return GemPlacementManager.Instance.RemoveGem(from);
		yield return GemPlacementManager.Instance.PutGem(_currentPlayer, to);
		yield return AnimateHexPins(to);
		yield return GemPlacementManager.Instance.SwapGemsAround(_currentPlayer, to);
		SwitchPlayer();
	}

	private IEnumerator DoSplitMove(Vector3Int from, Vector3Int to) {
		GridManager.Instance.DeselectAllTiles();
		yield return GemPlacementManager.Instance.PutGem(_currentPlayer, to);
		yield return AnimateHexPins(to);
		yield return GemPlacementManager.Instance.SwapGemsAround(_currentPlayer, to);
		SwitchPlayer();
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
