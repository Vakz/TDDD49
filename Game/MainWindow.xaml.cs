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

namespace Game
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        GameController Game;

        public MainWindow()
        {
            InitializeComponent();

            Image trump = ImageLoader.loadBlock(BlockType.Bank);

            BoardCanvas.Children.Add(trump);
            GameCanvas.SetLeft(trump, 0);
            GameCanvas.SetTop(trump, 0);

            Image t = ImageLoader.loadBlock(BlockType.Bank);

            BoardCanvas.Children.Add(t);
            GameCanvas.SetLeft(t, 300);
            GameCanvas.SetTop(t, 400);
            InitializeGame(1, 1, true);
        }



        private void InitializeGame(int nrOfHumans, int nrOfAI, bool AIPolice)
        {
            Game = new GameController(nrOfHumans, nrOfAI, AIPolice);
            BoardCanvas.MouseLeftButtonDown += CanvasClick;
        }

        private void CanvasClick(object sender, MouseButtonEventArgs e)
        {
            System.Console.WriteLine("test");
        }
    }
}
