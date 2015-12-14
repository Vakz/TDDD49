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

            InitializeGame(1, 1, true);
        }

        private void InitializeGame(int nrOfHumans, int nrOfAI, bool AIPolice)
        {
            Game = new GameController(nrOfHumans, nrOfAI, AIPolice);
            
            BoardCanvas.MouseLeftButtonDown += CanvasClick;
        }

        private void CanvasClick(object sender, MouseButtonEventArgs e)
        {
            var clicked = pixelCoordsToBlockCoords(e.GetPosition(BoardCanvas)).toGamePoint();

            if (Selected == null)
            {
                Selected = Game.pieceExistsAt(clicked) ? clicked : BoardPoint.Error;
            }
            else if (Selected != null)
            {

            }
           
        }

        private Point pixelCoordsToBlockCoords(System.Windows.Point p) {
            int blockSize = (int)BoardCanvas.ActualHeight / Game.Height;
            return new Point(
                (int)p.X / blockSize,
                (int)p.Y / blockSize
            );
        }
    }
}
