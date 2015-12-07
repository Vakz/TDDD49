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
using Game.Model.DataStructures;
using Game.Controller;

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
        }

        private void InitializeGame(int nrOfHumans, int nrOfAI, bool AIPolice)
        {
            Game = new GameController(nrOfHumans, nrOfAI, AIPolice);
        }
    }
}
