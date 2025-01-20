using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Puzzle;

public class GameState
{
    public int[,] Board { get; private set; } = new int[3, 3];
    public (int Row, int Col) EmptyTile { get; private set; }
    
    public GameState()
    {
        ResetBoard();
    }

    

    public void ResetBoard()
    {
        // Initialize tiles in order
        int counter = 1;
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                Board[i, j] = counter;
                counter++;
            }
        }
        Board[2, 2] = 0; // Empty space
        EmptyTile = (2, 2);
    }

    public void ShuffleBoard()
    {
        var rand = new Random();
        var emptyTile = (rand.Next(0, 2), rand.Next(0, 2));
        Board[emptyTile.Item1, emptyTile.Item2] = 0;
        EmptyTile = emptyTile;
        var tiles = new ArrayList();
        for (int i = 1; i <= 8; i++)
        {
            tiles.Add(i);
        }
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                if (i == emptyTile.Item1 && j == emptyTile.Item2) continue;
                if (tiles.Count > 0)
                {
                    var num = rand.Next(0, tiles.Count - 1);
                    Board[i, j] = (int)tiles[num]!;
                    tiles.RemoveAt(num);    
                }
            }
        }


        var linear = new int[9];
        var counter = 0;
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                linear[counter++] = Board[i, j];
            }
        }

        var invCount = 0;
        int iIndex = 0, jIndex = 0;
        for (int i = 0; i < 8; i++)
        {
            if (i == emptyTile.Item1) continue;
            for (int j = i + 1; j < 9; j++)
            {
                if (j == emptyTile.Item2) continue;
                if (linear[i] > linear[j])
                {
                    invCount++;
                    iIndex = i;
                    jIndex = j;
                }
                
            }
        }

        if (invCount % 2 == 1)
        {
            (Board[iIndex / 3, iIndex % 3], Board[jIndex / 3, jIndex % 3]) = 
                (Board[jIndex / 3, jIndex % 3], Board[iIndex / 3, iIndex % 3]);
        }
    }
    
    public bool MoveTile(int row, int col)
    {
        // Check if the clicked tile is adjacent to the empty space
        if (Math.Abs(row - EmptyTile.Row) + Math.Abs(col - EmptyTile.Col) == 1)
        {
            Board[EmptyTile.Row, EmptyTile.Col] = Board[row, col];
            Board[row, col] = 0;
            EmptyTile = (row, col);
            return true;
        }
        return false;
    }
    
    public bool IsSolved()
    {
        int counter = 1;
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                if (i == 2 && j == 2) return Board[i, j] == 0; // Last tile should be empty
                if (Board[i, j] != counter++) return false;
            }
        }
        return true;
    }
    
    public async Task SolveWithBranchAndBound(Action<int[,]> displayState, int delayMs = 600)
    {
        var startState = new Node(Board, EmptyTile, 0, CalculateManhattan(Board), null);
        var priorityQueue = new PriorityQueue<Node, int>();
        priorityQueue.Enqueue(startState, startState.Cost);

        var visited = new HashSet<string>();
        visited.Add(GetStateString(Board));

        while (priorityQueue.Count > 0)
        {
            var currentNode = priorityQueue.Dequeue();

            // Display the current state
            displayState(currentNode.Board);
            await Task.Delay(delayMs);

            // Check if the current node is the solution
            if (IsGoal(currentNode.Board))
            {
                // Optionally, reconstruct and display the solution path
                displayState(currentNode.Board);
                return; // Exit immediately once the first solution is found
            }

            foreach (var neighbor in GetNeighbors(currentNode))
            {
                var stateString = GetStateString(neighbor.Board);
                if (!visited.Contains(stateString))
                {
                    visited.Add(stateString);
                    priorityQueue.Enqueue(neighbor, neighbor.Cost);
                }
            }
        }
    }


    private IEnumerable<Node> GetNeighbors(Node node)
    {
        var directions = new (int Row, int Col)[]
        {
            (-1, 0), (1, 0), (0, -1), (0, 1)
        };

        foreach (var dir in directions)
        {
            var newRow = node.EmptyTile.Row + dir.Row;
            var newCol = node.EmptyTile.Col + dir.Col;

            if (newRow >= 0 && newRow < 3 && newCol >= 0 && newCol < 3)
            {
                var newBoard = CloneBoard(node.Board);
                newBoard[node.EmptyTile.Row, node.EmptyTile.Col] = newBoard[newRow, newCol];
                newBoard[newRow, newCol] = 0;

                yield return new Node(newBoard, (newRow, newCol), node.G + 1, CalculateManhattan(newBoard), node);
            }
        }
    }

    private bool IsGoal(int[,] board)
    {
        int counter = 1;
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                if (i == 2 && j == 2) return board[i, j] == 0;
                if (board[i, j] != counter++) return false;
            }
        }
        return true;
    }

    private int CalculateManhattan(int[,] board)
    {
        int distance = 0;
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                if (board[i, j] != 0)
                {
                    int value = board[i, j] - 1;
                    int targetRow = value / 3;
                    int targetCol = value % 3;
                    distance += Math.Abs(i - targetRow) + Math.Abs(j - targetCol);
                }
            }
        }
        return distance;
    }

    private int[,] CloneBoard(int[,] board)
    {
        var newBoard = new int[3, 3];
        Array.Copy(board, newBoard, board.Length);
        return newBoard;
    }

    private string GetStateString(int[,] board)
    {
        return string.Join(",", board.Cast<int>());
    }

    private class Node
    {
        public int[,] Board { get; }
        public (int Row, int Col) EmptyTile { get; }
        public int G { get; } // Cost from start
        public int H { get; } // Heuristic cost
        public int Cost => G + H; // Total cost
        public Node Previous { get; }

        public Node(int[,] board, (int Row, int Col) emptyTile, int g, int h, Node previous)
        {
            Board = board;
            EmptyTile = emptyTile;
            G = g;
            H = h;
            Previous = previous;
        }
    }
}