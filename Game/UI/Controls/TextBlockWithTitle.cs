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

namespace Game.UI.Controls
{
    public class TextBlockWithTitle : TextBlock
    {
        private string _label = "";
        private string _data = "";
        public string Label
        {
            get
            {
                return _label;
            }
            set
            {
                _label = value;
                base.Text = format();
            }
        }

        public string Data
        {
            get
            {
                return _data;
            }
            set
            {
                _data = value;
                base.Text = format();
            }
        }

        private string format()
        {
 	         return String.Format("{0}: {1}", _label, _data);
        }
    }
}
