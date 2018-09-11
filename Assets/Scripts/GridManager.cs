using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;

public class GridManager : Singleton<GridManager> {

	[SerializeField] private Grid _grid;
	[SerializeField] private Gem gem;
	[SerializeField] private TileBase _hexTile;
	[SerializeField] private TileBase _borderHexTile;
	[SerializeField] private TileBase _blueBorderHexTile;
	[SerializeField] private TileBase _redBorderHexTile;
	[SerializeField] private TileBase _redGemTile;
	[SerializeField] private TileBase _blueGemTile;

	private List<Vector3Int> _boardTilesPositionsXy = new List<Vector3Int>();
	private List<Vector3Int> _startingPointsRed = new List<Vector3Int>();
	private List<Vector3Int> _startingPointsBlue = new List<Vector3Int>();
	private Tilemap _board;

	public Tilemap Board {
		get { return _board; }
	}

	void Start () {
		ReadBoard();
		ReadStartingPoints();
		SpawnStartingGems();
	}

	public void SelectTile(Vector3Int pos, GemType gemType) {
		switch (gemType) {
			case GemType.Blue:
				_board.SetTile(pos, _blueBorderHexTile);
				break;
			case GemType.Red:
				_board.SetTile(pos, _redBorderHexTile);
				break;
			case GemType.None:
				_board.SetTile(pos, _borderHexTile);
				break;
		}
	}

	public void DeselectTile(Vector3Int pos) {
		_board.SetTile(pos, _hexTile);
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
		foreach (var tileMap in tilemaps) {
			if (tileMap.name == "Board") {
				_board = tileMap;
				break;
			}
		}
		
		if (_board == null) {
			throw new Exception("No Board tilemap found");
		}

		_board.CompressBounds();

		foreach (var position in _board.cellBounds.allPositionsWithin) {
			if (_board.GetTile(position) != null) {
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
