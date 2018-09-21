using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public struct AiMove {
    public Vector3Int From;
    public Vector3Int To;

    public AiMove(Vector3Int from, Vector3Int to) {
        From = from;
        To = to;
    }

    public override string ToString() {
        return string.Format("[{0}; {1}]", From, To);
    }
}

public static class Ai {

    public static AiMove GetMove(List<Vector3Int> grid, List<Vector3Int> redGems, List<Vector3Int> blueGems) {
        var firstMovableGem = blueGems.First(gem => GetEmptyNeighbours(gem).Count > 0);

        var firstAvailableMove = GetEmptyNeighbours(firstMovableGem)[0];
        
        return new AiMove(firstMovableGem, firstAvailableMove);
    }

    private static List<Vector3Int> GetEmptyNeighbours(Vector3Int cell) {
        var neighbours = CoordinateUtils.Neighbours(cell)
            .Where(GridManager.Instance.CellInBounds)
            .ToList();

        var jumpNeighbours = neighbours.SelectMany(CoordinateUtils.Neighbours)
            .Where(GridManager.Instance.CellInBounds)
            .Where(n => n != cell && !neighbours.Contains(n));

        var emptyNeighbours = neighbours.Concat(jumpNeighbours)
            .Where(n => GemPlacementManager.Instance.GemTypeAt(n) == GemType.None)
            .ToList();
            
        return emptyNeighbours;
    }
    
}