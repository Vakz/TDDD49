using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows;


namespace Game.UI
{
    class GameCanvas : Canvas
    {


        protected override void OnRender(System.Windows.Media.DrawingContext dc)
        {
 	        base.OnRender(dc);
            
            Image trumpie = new Image();
            trumpie.Source = new BitmapImage(new Uri(@"pack://application:,,,/Resources/trumpman.png"));




            //BitmapImage img = new BitmapImage (new Uri ("c:\\demo.jpg"));
            dc.DrawImage (trumpie.Source, new Rect (0, 0, 50, 50));
            
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


/*
        public void drawImage( Point pos, Image img ){

            Image trump = img;

            BoardCanvas.Children.Add(trump);
            Canvas.SetLeft(img, 0);
            Canvas.SetTop(trump, 0);

            Image t = ImageLoader.loadBlock(BlockType.Bank);

            BoardCanvas.Children.Add(t);
            Canvas.SetLeft(t, new Point(_tileSize*pos.x, _tileSize*pos.y));
            Canvas.SetTop(t, 400);










        }


*/