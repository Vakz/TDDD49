﻿using System;
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
            public bool check(GamePoint position)
            {
                if (!blocks.ContainsKey(position)) throw new ArgumentOutOfRangeException("invalid index");
                return blocks[position] == BlockType.Path; 
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
            BoardLines[c] = new List<List<GamePoint>>();

            LinePathPassCheck can_pass = new LinePathPassCheck(BoardBlocks);
            for ( int i = 1; i < BoardStations[c].Count; i++ ){
                List<GamePoint> path = path_finder.getShortestPath(BoardStations[c][i-1], BoardStations[c][i], can_pass);
                BoardLines[c].Add(path);
            }

            this.InvalidateVisual();
        }

        public void addText(string s, GamePoint p)
        {
            BoardText[p] = s;
            InvalidateVisual();
        }

        public void setPieces(Dictionary<GamePoint, PieceType> pieces)
        {
            BoardPieces = pieces;
            this.InvalidateVisual();
        }

        private PathFinder path_finder;
        public void setPathFinder( int width, int height )
        {
            path_finder = new PathFinder( width, height );
        }

        private Dictionary<GamePoint, string>               BoardText;
        private Dictionary<GamePoint, BlockType>            BoardBlocks;
        private Dictionary<GamePoint, PieceType>            BoardPieces;
        private Dictionary<Color, List<List<GamePoint>>>    BoardLines;
        private Dictionary<Color, List<GamePoint>>          BoardStations;
        public SimpleEventDictionary<GamePoint, Color> MarkedSquares = new SimpleEventDictionary<GamePoint, Color>();
        
        private GamePoint _boardSelection;
        public GamePoint BoardSelection {
            set {
                _boardSelection = value;
                this.InvalidateVisual();
            }
            get {
                return _boardSelection;
            }
        }

        public void clearDrawData(){
            BoardText      = new Dictionary<GamePoint, string>();
            BoardBlocks    = new Dictionary<GamePoint,BlockType>();
            BoardPieces    = new Dictionary<GamePoint,PieceType>();
            BoardLines     = new Dictionary<Color,List<List<GamePoint>>>();
            BoardStations  = new Dictionary<Color, List<GamePoint>>();
            
            BoardSelection = GamePoint.Error;
        }

        Dictionary<BlockType,ImageSource> BlockBitmaps;
        Dictionary<PieceType,ImageSource> PieceBitmaps;

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
            MarkedSquares.OnChange += delegate() { this.InvalidateVisual(); };
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

                drawOutline(Color.FromRgb(0x00, 0x00, 0x00),p,dc);
            }

            // draw lines:
            foreach ( Color c in BoardLines.Keys ){
                Color transparent = Color.FromArgb(0x40, c.R, c.G, c.B);
                Brush brush = new SolidColorBrush( transparent );
                Pen   pen   = new Pen( brush, _tileSize/2 );

                foreach (List<GamePoint> line in BoardLines[c]){
                    for (int i = 1; i < line.Count; i++){
                        Point a = new Point(_tileSize * line[i - 1].X + _tileSize / 2, _tileSize * line[i - 1].Y + _tileSize / 2);
                        Point b = new Point(_tileSize * line[i].X + _tileSize / 2, _tileSize * line[i].Y + _tileSize / 2);
                        dc.DrawLine(pen, a, b);
                    }
                }
            }

            // draw stations:
            foreach (Color c in BoardStations.Keys){
                Color transparent = Color.FromArgb(0x80, c.R, c.G, c.B);
                Brush brush = new SolidColorBrush(transparent);
                Pen pen = new Pen(brush, 1.0);

                foreach ( GamePoint p in BoardStations[c] ){
                    Point center = new Point(p.X * _tileSize + _tileSize / 2, p.Y * _tileSize + _tileSize / 2);
                    dc.DrawEllipse( brush, pen, center, ((double)_tileSize)/4.0, ((double)_tileSize)/4.0 );
                }
            }

            // draw pieces:
            foreach (GamePoint p in BoardPieces.Keys){
                PieceType pt = BoardPieces[p];
                if ( p != GamePoint.Error )
                    dc.DrawImage(PieceBitmaps[pt], new Rect(p.X * _tileSize, p.Y * _tileSize, _tileSize, _tileSize));
            }

            // draw marked squares:
            foreach (KeyValuePair<GamePoint, Color> kvp in MarkedSquares)
            {
                drawOutline(kvp.Value, kvp.Key, dc);
            }

            // draw selection:
            if (BoardSelection != GamePoint.Error)
            {
                drawOutline(Color.FromRgb(0xff, 0xff, 0x00), BoardSelection, dc);
            }

            // draw text:
            foreach (GamePoint p in BoardText.Keys){
                Brush back = new SolidColorBrush(Color.FromRgb(0x00, 0x00, 0x00));
                Brush b = new SolidColorBrush(Color.FromRgb(0xff, 0xff, 0x00));

                System.Globalization.CultureInfo cinf = new System.Globalization.CultureInfo(0x0409);
                Typeface tf = new Typeface("Verdana");
                double size = 12.0;
                FormattedText btext = new FormattedText(BoardText[p], cinf, FlowDirection.LeftToRight, tf, size, back);
                FormattedText ftext = new FormattedText(BoardText[p], cinf, FlowDirection.LeftToRight, tf, size, b);
                Point text_centerpos = new Point( TileSize*(p.X+0.5), TileSize*(p.Y+0.5) );
                Point draw_origin = new Point( text_centerpos.X-ftext.Width/2, text_centerpos.Y-ftext.Height/2 );

                int text_border = 1;
                dc.DrawText(btext, new Point(draw_origin.X + text_border, draw_origin.Y + text_border));
                dc.DrawText(btext, new Point(draw_origin.X + text_border, draw_origin.Y - text_border));
                dc.DrawText(btext, new Point(draw_origin.X - text_border, draw_origin.Y + text_border));
                dc.DrawText(btext, new Point(draw_origin.X - text_border, draw_origin.Y - text_border));

                dc.DrawText( ftext, draw_origin );
            }
        }

        private void drawOutline(Color c, GamePoint p, DrawingContext dc )
        {
            if (p == GamePoint.Error) return;
            Brush selection_brush = new SolidColorBrush(c);
            Pen selection_pen = new Pen(selection_brush, 2.0);

            Point start = new Point(p.X * _tileSize, p.Y * _tileSize);
            Point[] points = new Point[]
            {
                new Point( start.X, start.Y ),
                new Point( start.X+_tileSize, start.Y ),
                new Point( start.X+_tileSize, start.Y+_tileSize ),
                new Point( start.X, start.Y+_tileSize )
            };

            for (int i = 0; i < points.Length; i++)
            {
                dc.DrawLine(selection_pen, points[i], points[(i + 1) % points.Length]);
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
