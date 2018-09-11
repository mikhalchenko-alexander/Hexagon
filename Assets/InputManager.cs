using UnityEngine;

public class InputManager : MonoBehaviour {

	void Update () {
		if (Input.GetMouseButtonDown(0))
		{
			Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			var board = GridManager.Instance.Board;
			var tilePosition = board.WorldToCell(pos);
			var tile = board.GetTile(tilePosition);
 
			if (tile != null)
			{
				GridManager.Instance.SelectTile(tilePosition, GemType.Blue);
			}
		}
	}

}
