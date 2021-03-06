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

public class Board {
    public readonly List<Vector3Int> Grid;
    public readonly List<Vector3Int> RedGems;
    public readonly List<Vector3Int> BlueGems;

    public Board(List<Vector3Int> grid, List<Vector3Int> redGems, List<Vector3Int> blueGems) {
        Grid = grid;
        RedGems = redGems;
        BlueGems = blueGems;
    }
    
    public List<Vector3Int> GetEmptyNeighbours(Vector3Int cell) {
        var neighbours = ValidNeighbours(cell);

        var jumpNeighbours = neighbours.SelectMany(CoordinateUtils.Neighbours)
            .Where(Grid.Contains)
            .Where(n => n != cell && !neighbours.Contains(n));

        var emptyNeighbours = neighbours.Concat(jumpNeighbours)
            .Where(n => !RedGems.Contains(n) && !BlueGems.Contains(n))
            .ToList();
            
        return emptyNeighbours;
    }

    public int RedGemsCountAround(Vector3Int cell) {
        return ValidNeighbours(cell).Count(RedGems.Contains);
    }

    private List<Vector3Int> ValidNeighbours(Vector3Int cell) {
        return CoordinateUtils.Neighbours(cell)
            .Where(Grid.Contains)
            .ToList();
    }
}

public static class Ai {

    public static AiMove GetMove(Board board) {
        return BestMove(board);
    }

    private static AiMove BestMove(Board board) {
        var allMoves = GetAllMoves(board);
        var estimatedMoves = Estimate(board, allMoves);
        return estimatedMoves.OrderByDescending(kv => kv.Value).First().Key;
    }

    private static List<KeyValuePair<AiMove, int>> Estimate(Board board, List<AiMove> moves) {
        return moves.Select(move => {
            var value = CoordinateUtils.CubeDistance(move.From, move.To) == 1 ? 1 : 0;
            var enemySwapCount = board.RedGemsCountAround(move.To);
            value += enemySwapCount;
            return new KeyValuePair<AiMove, int>(move, value);
        }).ToList();
    }

    private static List<AiMove> GetAllMoves(Board board) {
        return board.BlueGems
            .SelectMany(from => board.GetEmptyNeighbours(from).Select(to => new AiMove(from, to)))
            .ToList();
    }
}