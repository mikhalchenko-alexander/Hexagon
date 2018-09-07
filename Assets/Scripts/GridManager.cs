using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;

public class GridManager : MonoBehaviour {

	[SerializeField] private Grid _grid;

	private Tilemap _board;
	private List<Vector3Int> boardTilesPositionsXY = new List<Vector3Int>();
	
	void Start () {
		InitializeBoard();
	}

	private void InitializeBoard() {
		var tilemaps = _grid.GetComponentsInChildren<Tilemap>();
		foreach (var tileMap in tilemaps) {
			if (tileMap.name == "Board") {
				_board = tileMap;
			}
		}

		_board.CompressBounds();

		foreach (var position in _board.cellBounds.allPositionsWithin) {
			if (_board.GetTile(position) != null) {
				boardTilesPositionsXY.Add(CoordinateUtils.OffsetToAxial(position));
			}
		}
	}
}
