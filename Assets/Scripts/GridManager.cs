using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GridManager : MonoBehaviour {

	[SerializeField] private Grid _grid;

	private Tilemap _board;
	
	void Start () {
		BuildBoard();
	}

	private void BuildBoard() {
		var tilemaps = _grid.GetComponentsInChildren<Tilemap>();
		foreach (var tilemap in tilemaps) {
			if (tilemap.name == "Board") {
				_board = tilemap;
			}
		}

		_board.CompressBounds();

		List<Vector3Int> boardTilesPositionsXY = new List<Vector3Int>();

		foreach (var position in _board.cellBounds.allPositionsWithin) {
			if (_board.GetTile(position) != null) {
				boardTilesPositionsXY.Add(position);
			}
		}
	}
}
