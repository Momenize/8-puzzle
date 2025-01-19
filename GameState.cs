using System;
using System.Collections;
using System.Linq;
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
}