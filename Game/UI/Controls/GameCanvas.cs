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
using Game.Model.Logic;
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

        private class LinePathPassCheck : PathFinder.CanPass {
            Dictionary<GamePoint, BlockType> blocks;
            public LinePathPassCheck( Dictionary<GamePoint, BlockType> blocks ){
                this.blocks = blocks;
            }
        }

        public void addBlock(BlockType block, GamePoint p)
        {
            BoardBlocks[p] = block;
            this.InvalidateVisual();

        }

        public void addLine(Color c, List<GamePoint> stations)
        {
            BoardStations[c] = stations;
            BoardLines[c] = new List<GamePoint>();
            //TODO: kalla på pathfinding för att lägga till BoardLines


            this.InvalidateVisual();
        }

        public void setPieces(Dictionary<GamePoint, PieceType> pieces)
        {
            BoardPieces = pieces;
            this.InvalidateVisual();
        }

        public void setPathFinder( int width, int height )
        {
            path_finder = new PathFinder( width, height );
        }

        private Dictionary<GamePoint, BlockType>    BoardBlocks;
        private Dictionary<GamePoint, PieceType>    BoardPieces;
        private Dictionary<Color, List<GamePoint>>  BoardLines;
        private Dictionary<Color, List<GamePoint>>  BoardStations;
        private GamePoint _boardSelection = GamePoint.Error;
        public GamePoint                           BoardSelection {
            set {
                _boardSelection = value;
                this.InvalidateVisual();
            }
            get {
                return _boardSelection;
            }
        }

        private PathFinder path_finder;

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

            BoardSelection = new GamePoint(1, 1);
        }

        protected override void OnRender(System.Windows.Media.DrawingContext dc)
        {
 	        base.OnRender(dc);
            //ImageSource trumpie = new BitmapImage( new Uri( Directory.GetCurrentDirectory()+"\\Resources\\trumpman.jpg") );
            //dc.DrawImage( trumpie, new Rect(0, 0, _tileSize, _tileSize ) );

            // draw blocks:
            foreach ( GamePoint p in BoardBlocks.Keys ){
                BlockType bt = BoardBlocks[p];
                dc.DrawImage(BlockBitmaps[bt], new Rect(p.X*_tileSize, p.Y*_tileSize, _tileSize, _tileSize));

                // draw black outline:
                Brush outline_brush = new SolidColorBrush(Color.FromRgb(0x00, 0x00, 0x00));
                Pen outline_pen = new Pen(outline_brush, 0.5);

                Point start = new Point(p.X * _tileSize, p.Y * _tileSize);
                Point[] points = new Point[]{
                    new Point( start.X, start.Y ),
                    new Point( start.X+_tileSize, start.Y ),
                    new Point( start.X+_tileSize, start.Y+_tileSize ),
                    new Point( start.X, start.Y+_tileSize )
                };

                for (int i = 0; i < points.Length; i++)
                {
                    dc.DrawLine(outline_pen, points[i], points[(i + 1) % points.Length]);
                }
            }

            // draw lines:
            foreach ( Color c in BoardLines.Keys ){
                Brush brush = new SolidColorBrush( c );
                Pen   pen   = new Pen( brush, 1.5 );

                for (int i = 1; i < BoardLines[c].Count; i++ )
                {
                    Point a = new Point( BoardLines[c][i-1].X, BoardLines[c][i-1].Y );
                    Point b = new Point( BoardLines[c][ i ].X, BoardLines[c][ i ].Y );

                    dc.DrawLine( pen, a, b );
                }
            }

            // draw stations:
            foreach (Color c in BoardStations.Keys){
                Brush brush = new SolidColorBrush(c);
                Pen pen = new Pen(brush, 1.0);

                foreach ( GamePoint p in BoardStations[c] ){
                    Point center = new Point( p.X, p.Y );
                    dc.DrawEllipse( brush, pen, center, ((double)_tileSize)/2.0, ((double)_tileSize)/2.0 );
                }
            }

            // draw pieces:
            foreach (GamePoint p in BoardPieces.Keys){
                PieceType pt = BoardPieces[p];
                dc.DrawImage(PieceBitmaps[pt], new Rect(p.X * _tileSize, p.Y * _tileSize, _tileSize, _tileSize));
            }

            // TODO: Refactor to function, so it can be reused to draw border around current player piece(s)
            // draw selection:
            if (BoardSelection != GamePoint.Error){
                Brush selection_brush = new SolidColorBrush(Color.FromRgb(0xff, 0xff, 0x00));
                Pen   selection_pen   = new Pen(selection_brush, 2.0);

                Point start = new Point( BoardSelection.X*_tileSize, BoardSelection.Y*_tileSize );
                Point[] points = new Point[]{
                    new Point( start.X, start.Y ),
                    new Point( start.X+_tileSize, start.Y ),
                    new Point( start.X+_tileSize, start.Y+_tileSize ),
                    new Point( start.X, start.Y+_tileSize )
                };
                
                for (int i = 0; i < points.Length; i++ )
                {
                    dc.DrawLine( selection_pen, points[i], points[(i + 1) % points.Length] );
                }
            }
        }

        private int _tileSize = 30;
        public int TileSize{
            get {
                return _tileSize;
            }
            set {
                if (value <= 0) throw new ArgumentException("tile size must be greater than zero");
                _tileSize = value;
                this.InvalidateVisual();
            }
        }
    }
}
