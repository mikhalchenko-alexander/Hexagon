using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GridManager : Singleton<GridManager> {

	[SerializeField] private Grid _grid;
	[SerializeField] private TileBase _hexTile;
	[SerializeField] private TileBase _borderHexTile;
	[SerializeField] private TileBase _blueBorderHexTile;
	[SerializeField] private TileBase _redBorderHexTile;
	[SerializeField] private TileBase _redGemTile;
	[SerializeField] private TileBase _blueGemTile;

	private readonly List<Vector3Int> _boardTilesCubeCoordinates = new List<Vector3Int>();
	private Tilemap _board;

	public Tilemap Board {
		get { return _board; }
	}

	void Start () {
		ReadBoard();
		SpawnStartingGems();
	}

	public int CellCount() {
		return _boardTilesCubeCoordinates.Count;
	}

	public void SelectGemTile(Vector3Int centerCubeCoordinates, GemType gemType) {
		var colouredBorderTile = gemType == GemType.Blue ? _blueBorderHexTile : _redBorderHexTile;
		SetTile(centerCubeCoordinates, colouredBorderTile);

		var neighbours = CoordinateUtils.Neighbours(centerCubeCoordinates)
			.Where(_boardTilesCubeCoordinates.Contains).ToList();
		
		var jumpNeighbours = neighbours.SelectMany(CoordinateUtils.Neighbours)
			.Where(cell => _boardTilesCubeCoordinates.Contains(cell) && !neighbours.Contains(cell) &&
			               centerCubeCoordinates != cell);
		
		var emptyNeighbours = neighbours.Where(cell => GemPlacementManager.Instance.GemTypeAt(cell) == GemType.None);
		var emptyJumpNeighbours = jumpNeighbours.Where(cell => GemPlacementManager.Instance.GemTypeAt(cell) == GemType.None);
		
		foreach (var cubeCoord in emptyNeighbours) {
			SetTile(cubeCoord, _borderHexTile);
		}
		foreach (var cubeCoord in emptyJumpNeighbours) {
			SetTile(cubeCoord, colouredBorderTile);
		}
	}

	private void SetTile(Vector3Int cubeCoordinates, TileBase hexTile) {
		var offsetCoordinates = CoordinateUtils.CubeToOffset(cubeCoordinates);
		_board.SetTile(offsetCoordinates, hexTile);
	}
	
	public void DeselectTile(Vector3Int cubeCoordinates) {
		_board.SetTile(CoordinateUtils.CubeToOffset(cubeCoordinates), _hexTile);
	}

	public Vector3 GetTileCenterPosition(Vector3Int cubeCoordinates) {
		return Board.GetCellCenterWorld(CoordinateUtils.CubeToOffset(cubeCoordinates));
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
				_boardTilesCubeCoordinates.Add(CoordinateUtils.OffsetToCube(position));
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
			var cubeCoord = CoordinateUtils.OffsetToCube(position);
			
			if (startingPoints.GetTile(position) == _redGemTile) {
				StartCoroutine(GemPlacementManager.Instance.PutGem(GemType.Red, cubeCoord));
			}
			if (startingPoints.GetTile(position) == _blueGemTile) {
				StartCoroutine(GemPlacementManager.Instance.PutGem(GemType.Blue, cubeCoord));
			}
		}
		
		Destroy(startingPoints.gameObject);
	}

	public void DeselectAllTiles() {
		foreach (var cubeCoordinates in _boardTilesCubeCoordinates) {
			DeselectTile(cubeCoordinates);
		}
	}
}
