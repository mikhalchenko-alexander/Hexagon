using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;

public class GridManager : Singleton<GridManager> {

	[SerializeField] private Grid _grid;
	[SerializeField] private TileBase _hexTile;
	[SerializeField] private TileBase _borderHexTile;
	[SerializeField] private TileBase _blueBorderHexTile;
	[SerializeField] private TileBase _redBorderHexTile;
	[SerializeField] private TileBase _redGemTile;
	[SerializeField] private TileBase _blueGemTile;

	private List<Vector3Int> _boardTilesAxialCoordinates = new List<Vector3Int>();
	private Tilemap _board;

	public Tilemap Board {
		get { return _board; }
	}

	void Start () {
		ReadBoard();
		SpawnStartingGems();
	}

	public void SelectGemTile(Vector3Int axialCoordinates, GemType gemType) {
		switch (gemType) {
			case GemType.Blue:
				SetTile(axialCoordinates, _blueBorderHexTile);
				break;
			case GemType.Red:
				SetTile(axialCoordinates, _redBorderHexTile);
				break;
		}

		var neighbours = CoordinateUtils.Neighbours(axialCoordinates).Where(BoardContains).ToList();
		foreach (var axialCoord in neighbours) {
			SetTile(axialCoord, _borderHexTile);
		}
	}

	private void SetTile(Vector3Int axialCoordinates, TileBase hexTile) {
		var offsetCoordinates = CoordinateUtils.AxialToOffset(axialCoordinates);
		_board.SetTile(offsetCoordinates, hexTile);
	}

	private bool BoardContains(Vector3Int axialCoordinates) {
		return _boardTilesAxialCoordinates.Exists(c =>
			c.x == axialCoordinates.x && c.y == axialCoordinates.y && c.z == axialCoordinates.z);
	}
	
	public void DeselectTile(Vector3Int axialCoordinates) {
		_board.SetTile(CoordinateUtils.AxialToOffset(axialCoordinates), _hexTile);
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
				_boardTilesAxialCoordinates.Add(CoordinateUtils.OffsetToAxial(position));
			}
		}
	}
	
	private void SpawnStartingGems() {
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
			var axialPos = CoordinateUtils.OffsetToAxial(position);
			
			if (startingPoints.GetTile(position) == _redGemTile) {
				GemPlacementManager.Instance.PutGem(GemType.Red, axialPos);
			}
			if (startingPoints.GetTile(position) == _blueGemTile) {
				GemPlacementManager.Instance.PutGem(GemType.Blue, axialPos);
			}
		}
		
		Destroy(startingPoints.gameObject);
	}

	public void DeselectAllTiles() {
		foreach (var axialCoord in _boardTilesAxialCoordinates) {
			DeselectTile(axialCoord);
		}
	}
}
