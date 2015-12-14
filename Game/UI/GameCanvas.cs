using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows;
using System.IO;


namespace Game.UI
{
    class GameCanvas : Canvas
    {
        enum BlockTypes{
            
        }

        Dictionary<BlockType,ImageSource> bitmaps;

        public GameCanvas()
        {


        }


        public void updateCanvasData(){ }

        protected override void OnRender(System.Windows.Media.DrawingContext dc)
        {
 	        base.OnRender(dc);
            
            //@"pack://application:,,,/Resources/trumpman.png"
            ImageSource trumpie = new BitmapImage( new Uri( Directory.GetCurrentDirectory()+"\\Resources\\trumpman.jpg") );

            

            dc.DrawImage(trumpie, new Rect (0, 0, 50, 50));
            
        }

        private int _tileSize = 50;
        public int TileSize{
            get {
                return _tileSize;
            }
            set {
                if (value <= 0) throw new ArgumentException("tile size must be greater than zero");
                _tileSize = value;
            }
        }
    }
}
