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
            BoardCanvas.addBlock(UI.Controls.GameCanvas.BlockType.Special, new BoardPoint(6, 2));
            BoardCanvas.addBlock(UI.Controls.GameCanvas.BlockType.Path, new BoardPoint(6, 1));
            BoardCanvas.addBlock(UI.Controls.GameCanvas.BlockType.Path, new BoardPoint(5, 1));
            BoardCanvas.addBlock(UI.Controls.GameCanvas.BlockType.Path, new BoardPoint(4, 1));
            BoardCanvas.addBlock(UI.Controls.GameCanvas.BlockType.Path, new BoardPoint(3, 1));
            BoardCanvas.addBlock(UI.Controls.GameCanvas.BlockType.Path, new BoardPoint(2, 1));
            BoardCanvas.addBlock(UI.Controls.GameCanvas.BlockType.Path, new BoardPoint(1, 1));
            BoardCanvas.MouseLeftButtonDown += CanvasClick;
            Skip.Click += SkipClick;
            SettingsButton.Click += SettingsClick;
            NewGameButton.Click += RestartClick;
        }

        public void EndTurn()
        {
            string endMessage = "";
            int thiefMoney = Game.ThiefMoney;
            int policeMoney = Game.PoliceMoney;
            if (thiefMoney == policeMoney) endMessage = String.Format("It's a tie! Both sides have {0} monies.", thiefMoney);
            else
            {
                string winner = thiefMoney > policeMoney ? "thieves" : "police";
                string winnerMoney = (thiefMoney > policeMoney ? thiefMoney : policeMoney).ToString();
                string loser = thiefMoney < policeMoney ? "thieves" : "police";
                string loserMoney = (thiefMoney < policeMoney ? thiefMoney : policeMoney).ToString();
                endMessage = String.Format("The game is over! The {0} won against the {1}, with {2} monies over {3} monies.", winner, loser, winnerMoney, loserMoney);
            }
            
            MessageBox.Show(endMessage, "Game Over", MessageBoxButton.OK, MessageBoxImage.None);   
            // Game has ended
            if (!Game.GameRunning)
            {
                MessageBox.Show(endMessage, "Game Over", MessageBoxButton.OK, MessageBoxImage.None);   
            }
        }

        public void SettingsClick(object Sender, RoutedEventArgs e)
        {
            SettingsWindow w = new SettingsWindow(Game.HumanPlayers, Game.AIPolice);
            bool? save = w.ShowDialog();
            if (save.Value)
            {
                InitializeGame(w.NumberOfPlayers, w.AIPolice.IsChecked.Value);
            }
        }

        public void RestartClick(object Sender, RoutedEventArgs e)
        {
            EndTurn();
            InitializeGame(Game.HumanPlayers, Game.AIPolice);
        }

        private void InitializeGame(int nrOfHumans, bool AIPolice)
        {
            Game = new GameController(nrOfHumans, AIPolice);
            Selected = BoardPoint.Error;
            Dice.Data = Game.DiceRoll.ToString();
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
            // No currently selected tile
            if (Selected == BoardPoint.Error) Selected = Game.pieceExistsAt(clicked) ? clicked : BoardPoint.Error;
            // Clicked same tile, deselect
            else if (Selected == clicked) Selected = BoardPoint.Error;
            // Selected exists, new tile selected. Attempt to move, and if successful, deselect
            else if (Selected != BoardPoint.Error)
            {
                try
                {
                    if (Game.move(Selected, clicked)) Selected = BoardPoint.Error;
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
