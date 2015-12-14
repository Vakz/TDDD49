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
using GamePoint = Game.Model.DataStructures.Point;

namespace Game.UI.Controls
{
    class GameCanvas : Canvas
    {
        public enum BlockType {
            Default,
            Path,
            Blocked,
            Special
        }

        public enum PieceType {
            Thief,
            Police
        }

        public void addBlock(BlockType block, GamePoint p)
        {
            BoardBlocks[p] = block;
        }

        public void addLine(Color c, GamePoint[] stations)
        {
            //TODO: kalla på pathfinding för att lägga till BoardLines
        }

        public void setPieces(Dictionary<GamePoint, PieceType> pieces)
        {
            BoardPieces = pieces;
        }

        private Dictionary<GamePoint, BlockType>    BoardBlocks;
        private Dictionary<GamePoint, PieceType>    BoardPieces;
        private Dictionary<Color, List<GamePoint>>  BoardLines;
        private Dictionary<Color, List<GamePoint>>  BoardStations;
        private GamePoint                           BoardSelection;


        public void clearDrawData(){
            //private BlockType[,]                       
            BoardBlocks    = new Dictionary<GamePoint,BlockType>();
            BoardPieces    = new Dictionary<GamePoint,PieceType>();
            BoardLines     = new Dictionary<Color,List<GamePoint>>();
            BoardStations  = new Dictionary<Color, List<GamePoint>>();
            
            BoardSelection = GamePoint.Error;
        }

        Dictionary<BlockType,ImageSource> BlockBitmaps;// = new Dictionary<BlockType,ImageSource>();
        Dictionary<PieceType,ImageSource> PieceBitmaps;// = new Dictionary<PieceType,ImageSource>();

        public GameCanvas()
        {
            // init bitmaps:
            BlockBitmaps = new Dictionary<BlockType, ImageSource>
            {
                 { BlockType.Default, new BitmapImage(new Uri(Directory.GetCurrentDirectory() + "\\Resources\\trumpman.jpg")) },
                 { BlockType.Path,    new BitmapImage(new Uri(Directory.GetCurrentDirectory() + "\\Resources\\path.png"))     },
                 { BlockType.Blocked, new BitmapImage(new Uri(Directory.GetCurrentDirectory() + "\\Resources\\blocked.png"))  },
                 { BlockType.Special, new BitmapImage(new Uri(Directory.GetCurrentDirectory() + "\\Resources\\special.png"))  }
            };

            PieceBitmaps = new Dictionary<PieceType, ImageSource>
            {
                { PieceType.Thief,  new BitmapImage(new Uri(Directory.GetCurrentDirectory() + "\\Resources\\thief.png"))  },
                { PieceType.Police, new BitmapImage(new Uri(Directory.GetCurrentDirectory() + "\\Resources\\police.png")) }
            };

            clearDrawData();
        }


        public void updateCanvasData(){ }

        protected override void OnRender(System.Windows.Media.DrawingContext dc)
        {
 	        base.OnRender(dc);
            
            ImageSource trumpie = new BitmapImage( new Uri( Directory.GetCurrentDirectory()+"\\Resources\\trumpman.jpg") );
            
            dc.DrawImage( trumpie, new Rect(0, 0, _tileSize, _tileSize ) );

            // draw blocks:
            foreach ( GamePoint p in BoardBlocks.Keys ){
                BlockType bt = BoardBlocks[p];
                dc.DrawImage(BlockBitmaps[bt], new Rect(p.X * _tileSize, p.Y * _tileSize, _tileSize, _tileSize));
            }

            // draw stations and lines:
            //BoardLines = new Dictionary<Color, List<GamePoint>>();
            //BoardStations = new Dictionary<Color, List<GamePoint>>();

            //

            // draw pieces:
            foreach (GamePoint p in BoardPieces.Keys){
                PieceType pt = BoardPieces[p];
                dc.DrawImage(PieceBitmaps[pt], new Rect(p.X * _tileSize, p.Y * _tileSize, _tileSize, _tileSize));
            }

            // draw selection:
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
