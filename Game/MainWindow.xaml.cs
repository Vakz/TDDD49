using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Game.Controller;
using Game.UI;
using BoardPoint = Game.Model.DataStructures.Point;

namespace Game
{

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        GameController Game;
        BoardPoint _selected = BoardPoint.Error;
        BoardPoint Selected
        {
            get
            {
                return _selected;
            }
            set
            {
                _selected = value;
                BoardCanvas.BoardSelection = value;
            }
        }

        public MainWindow()
        {
            InitializeComponent();
            InitializeGame(1, true);
            System.Console.WriteLine(Game.getAllPieces().Count);
            System.Console.WriteLine(Game.getAllPieces().First(s => s.Type == Model.DataStructures.PieceType.Thief).Position.ToString());
            BoardCanvas.addBlock(UI.Controls.GameCanvas.BlockType.Special, new BoardPoint(6, 2));
            BoardCanvas.addBlock(UI.Controls.GameCanvas.BlockType.Path, new BoardPoint(6, 1));
            BoardCanvas.addBlock(UI.Controls.GameCanvas.BlockType.Path, new BoardPoint(5, 1));
            BoardCanvas.addBlock(UI.Controls.GameCanvas.BlockType.Path, new BoardPoint(4, 1));
            BoardCanvas.addBlock(UI.Controls.GameCanvas.BlockType.Path, new BoardPoint(3, 1));
            BoardCanvas.addBlock(UI.Controls.GameCanvas.BlockType.Path, new BoardPoint(2, 1));
            BoardCanvas.addBlock(UI.Controls.GameCanvas.BlockType.Path, new BoardPoint(1, 1));
            Dice.Data = Game.DiceRoll.ToString();
            Selected = BoardPoint.Error;
            BoardCanvas.MouseLeftButtonDown += CanvasClick;
            Skip.Click += SkipClick;
        }

        private void InitializeGame(int nrOfHumans, bool AIPolice)
        {
            Game = new GameController(nrOfHumans, AIPolice);
            
        }

        private void SkipClick(object Sender, RoutedEventArgs e)
        {
            try
            {
                Game.skip();
                Error.Visibility = System.Windows.Visibility.Hidden;
                Selected = BoardPoint.Error;
            }
            catch (Exceptions.IllegalMoveException ime)
            {
                setError(ime.Message);
            }
        }

        private void CanvasClick(object sender, MouseButtonEventArgs e)
        {
            if (Error.Visibility == System.Windows.Visibility.Visible) Error.Visibility = System.Windows.Visibility.Hidden;
            var clicked = pixelCoordsToBlockCoords(e.GetPosition(BoardCanvas).toGamePoint());
            System.Console.WriteLine("Enter click-function");
            // No currently selected tile
            System.Console.WriteLine(clicked);
            if (Selected == BoardPoint.Error)
            {
                System.Console.WriteLine("No piece previously selected");
                Selected = Game.pieceExistsAt(clicked) ? clicked : BoardPoint.Error;
            }
            // Clicked same tile, deselect
            else if (Selected == clicked)
            {
                System.Console.WriteLine("Deselected");
                Selected = BoardPoint.Error;
            }
            // Selected exists, new tile selected. Attempt to move, and if successful, deselect
            else if (Selected != BoardPoint.Error)
            {
                System.Console.WriteLine("Attempting to move");
                try
                {
                    if (Game.move(Selected, clicked))
                    {
                        Selected = BoardPoint.Error;
                    }
                }
                catch(Game.Exceptions.IllegalMoveException ime)
                {
                    setError(ime.Message);
                }
            }
        }

        private void setError(string message)
        {
            Error.Data = message;
            Error.Visibility = System.Windows.Visibility.Visible;
        }

        private BoardPoint pixelCoordsToBlockCoords(BoardPoint p) {
            return new BoardPoint(
                p.X / BoardCanvas.TileSize,
                p.Y / BoardCanvas.TileSize
            );
        }
    }
}
