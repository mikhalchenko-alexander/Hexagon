using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GridManager : MonoBehaviour {

	[SerializeField] private Grid _grid;
	[SerializeField] private Gem gem;
	[SerializeField] private TileBase _redGemTile;
	[SerializeField] private TileBase _blueGemTile;

	private List<Vector3Int> _boardTilesPositionsXy = new List<Vector3Int>();
	private List<Vector3Int> _startingPointsRed = new List<Vector3Int>();
	private List<Vector3Int> _startingPointsBlue = new List<Vector3Int>();
	
	void Start () {
		ReadBoard();
		ReadStartingPoints();
		SpawnStartingGems();
	}

	private void SpawnStartingGems() {
		foreach (var pos in _startingPointsRed) {
			var redGem = Instantiate(gem);
			redGem.SetRed();
			GemPlacementManager.Instance.PutGem(redGem, pos);
		}
		
		foreach (var pos in _startingPointsBlue) {
			var blueGem = Instantiate(gem);
			blueGem.SetBlue();
			GemPlacementManager.Instance.PutGem(blueGem, pos);
		}
	}

	private void ReadBoard() {
		var tilemaps = _grid.GetComponentsInChildren<Tilemap>();
		Tilemap board = null;
		foreach (var tileMap in tilemaps) {
			if (tileMap.name == "Board") {
				board = tileMap;
			}
		}
		
		if (board == null) {
			throw new Exception("No Board tilemap found");
		}

		board.CompressBounds();

		foreach (var position in board.cellBounds.allPositionsWithin) {
			if (board.GetTile(position) != null) {
				_boardTilesPositionsXy.Add(CoordinateUtils.OffsetToAxial(position));
			}
		}
	}
	
	private void ReadStartingPoints() {
		var tilemaps = _grid.GetComponentsInChildren<Tilemap>();
		Tilemap startingPoints = null;
		foreach (var tileMap in tilemaps) {
			if (tileMap.name == "StartingPoints") {
				startingPoints = tileMap;
			}
		}

		if (startingPoints == null) {
			throw new Exception("No StartingPoints tilemap found");
		}

		startingPoints.CompressBounds();

		foreach (var position in startingPoints.cellBounds.allPositionsWithin) {
			if (startingPoints.GetTile(position) == _redGemTile) {
				_startingPointsRed.Add(CoordinateUtils.OffsetToAxial(position));
			}
			if (startingPoints.GetTile(position) == _blueGemTile) {
				_startingPointsBlue.Add(CoordinateUtils.OffsetToAxial(position));
			}
		}
		
		Destroy(startingPoints.gameObject);
	}
}
