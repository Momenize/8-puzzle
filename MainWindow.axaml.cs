using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using MsgBox;

namespace Puzzle;

public partial class MainWindow : Window
{
    private GameState _gameState;

    public MainWindow()
    {
        InitializeComponent();
        _gameState = new GameState();
        RenderBoard();
    }

    private void RenderBoard()
    {
        GameBoard.Children.Clear();
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                var button = new Button
                {
                    Content = _gameState.Board[i, j] == 0 ? "" : _gameState.Board[i, j].ToString(),
                    FontSize = 24,
                    Background = Brushes.LightGray,
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
        RenderBoard();
        if (!_gameState.IsSolved()) return;
        const string text = "You win!";
        const string title = "Congratulations!";
        MessageBox.Show(this, text, title, MessageBox.MessageBoxButtons.Ok);
    }

    private void Shuffle_Click(object sender, RoutedEventArgs e)
    {
        _gameState.ShuffleBoard();
        RenderBoard();
    }

    private void Reset_Click(object sender, RoutedEventArgs e)
    {
        _gameState.ResetBoard();
        RenderBoard();
    }
}