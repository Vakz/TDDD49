using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Game.Model.DataStructures;
using System.Windows.Media;
using System.Windows.Controls;
using System.Drawing;


namespace Game.UI
{

    static class ImageLoader
    {
        private static Dictionary<BlockType, ImageSource> imageSources = new Dictionary<BlockType, ImageSource>() { };
        private static Dictionary<BlockType, Bitmap> availableBitmaps = new Dictionary<BlockType, Bitmap>() {
        };

        public static System.Windows.Controls.Image loadBlock(BlockType bt)
        {
            System.Windows.Controls.Image i = new System.Windows.Controls.Image();
            i.Source = loadImageSourceFromResource(bt);
            return i;
        }

        private static ImageSource loadImageSourceFromResource(BlockType bt)
        {
            Bitmap b = availableBitmaps[bt];
            if (!imageSources.ContainsKey(bt))
            {
                imageSources[bt] = b.toImageSource();
            }
            return imageSources[bt];
        }
    }

    enum AvailableTextures
    {
        Bank,
        Blocked,
        Normal,
        Hideout,
        Telegraph,
        Airport,
        Train,
        PoliceStation,
        TrainStop,
        Thief,
        Police
    }
}
