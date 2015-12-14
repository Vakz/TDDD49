using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Drawing;

namespace Game.UI
{
    static class ExtensionMethods
    {
        public static ImageSource toImageSource(this Bitmap b)
        {
            return System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                   b.GetHbitmap(),
                   IntPtr.Zero,
                   System.Windows.Int32Rect.Empty,
                   BitmapSizeOptions.FromWidthAndHeight(50, 50));
        }

        public static Game.Model.DataStructures.Point toGamePoint(this System.Windows.Point pt) {
            return new Game.Model.DataStructures.Point((int)pt.X, (int)pt.Y);
        }
    }
}
