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
        BoardPoint Selected { get; set; }
        int BlockSize { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            InitializeGame(1, true);
            
            Selected = BoardPoint.Error;
        }

        private void InitializeGame(int nrOfHumans, bool AIPolice)
        {
            Game = new GameController(nrOfHumans, AIPolice);
            BoardCanvas.MouseLeftButtonDown += CanvasClick;
        }

        private void CanvasClick(object sender, MouseButtonEventArgs e)
        {
            
            var clicked = pixelCoordsToBlockCoords(e.GetPosition(BoardCanvas).toGamePoint());

            System.Console.WriteLine("Enter click-function");
            // No currently selected tile
            
            if (Selected == BoardPoint.Error)
            {
                System.Console.WriteLine("No piece!");
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
                    if (Game.move(Selected, clicked)) Selected = BoardPoint.Error;
                }
                catch(Game.Exceptions.IllegalMoveException ime)
                {
                    Error.Text = ime.Message;
                }
               
            }
        }

        private BoardPoint pixelCoordsToBlockCoords(BoardPoint p) {
            int blockSize = (int)BoardCanvas.ActualHeight / Game.Height;
            return new BoardPoint(
                p.X / blockSize,
                p.Y / blockSize
            );
        }
    }
}
