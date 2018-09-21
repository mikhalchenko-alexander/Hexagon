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

public struct Board {
    public readonly List<Vector3Int> Grid;
    public readonly List<Vector3Int> RedGems;
    public readonly List<Vector3Int> BlueGems;

    public Board(List<Vector3Int> grid, List<Vector3Int> redGems, List<Vector3Int> blueGems) {
        Grid = grid;
        RedGems = redGems;
        BlueGems = blueGems;
    }
}

public static class Ai {

    public static AiMove GetMove(Board board) {
        var firstMovableGem = board.BlueGems.First(gem => GetEmptyNeighbours(gem, board).Count > 0);

        var firstAvailableMove = GetEmptyNeighbours(firstMovableGem, board)[0];
        
        return new AiMove(firstMovableGem, firstAvailableMove);
    }

    private static List<Vector3Int> GetEmptyNeighbours(Vector3Int cell, Board board) {
        var neighbours = CoordinateUtils.Neighbours(cell)
            .Where(board.Grid.Contains)
            .ToList();

        var jumpNeighbours = neighbours.SelectMany(CoordinateUtils.Neighbours)
            .Where(board.Grid.Contains)
            .Where(n => n != cell && !neighbours.Contains(n));

        var emptyNeighbours = neighbours.Concat(jumpNeighbours)
            .Where(n => !board.RedGems.Contains(n) && !board.BlueGems.Contains(n))
            .ToList();
            
        return emptyNeighbours;
    }
    
}