using System;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Threading;
using MsgBox;
using SkiaSharp;

namespace Puzzle;

public partial class MainWindow : Window
{
    private GameState _gameState;

    public MainWindow()
    {
        InitializeComponent();
        _gameState = new GameState();
        RenderBoard(_gameState.Board);
    }

    private void RenderBoard(int[, ] board)
    {
        GameBoard.Children.Clear();
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                var button = new Button
                {
                    Content = board[i, j] == 0 ? "  " : board[i, j].ToString(),
                    FontSize = 24,
                    Background = Brushes.Yellow,
                    Tag = (i, j) // Store row and column as tag
                };
                button.Click += Tile_Click;
                GameBoard.Children.Add(button);
            }
        }
    }

    private void Tile_Click(object sender, RoutedEventArgs e)
    {
        if (sender is not Button { Tag: (int row, int col) }) return;
        if (!_gameState.MoveTile(row, col)) return;
        RenderBoard(_gameState.Board);
        if (!_gameState.IsSolved()) return;
        const string text = "You win!";
        const string title = "Congratulations!";
        MessageBox.Show(this, text, title, MessageBox.MessageBoxButtons.Ok);
    }

    private void Shuffle_Click(object sender, RoutedEventArgs e)
    {
        _gameState.ShuffleBoard();
        RenderBoard(_gameState.Board);
    }

    private void Reset_Click(object sender, RoutedEventArgs e)
    {
        _gameState.ResetBoard();
        RenderBoard(_gameState.Board);
    }

    private async void Solve_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            await _gameState.SolveWithBranchAndBound(RenderBoard);
            const string text = "You win!";
            const string title = "Congratulations!";
            MessageBox.Show(this, text, title, MessageBox.MessageBoxButtons.Ok);
        }
        catch
        {
            return;
        }
    }
}