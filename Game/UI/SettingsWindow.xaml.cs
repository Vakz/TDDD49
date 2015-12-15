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
using System.Windows.Shapes;

namespace Game.UI
{
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        public SettingsWindow(int currentNrOfPlayers, bool PoliceCurrentlyAI)
        {
            InitializeComponent();
            AIPolice.IsChecked = PoliceCurrentlyAI;
            NrOfPlayers.SelectedIndex = currentNrOfPlayers - 1;
            Save.Click += closeWindow;
        }

        public void closeWindow(object Sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        public int NumberOfPlayers
        {
            get
            {
                return NrOfPlayers.SelectedIndex + 1;
            }
        }
    }
}
