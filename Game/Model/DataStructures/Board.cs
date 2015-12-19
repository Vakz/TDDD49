using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.Model.DataStructures
{
    public class Board
    {
        private Block[,] _board;
        private List<Piece> _pieces;
        public Dictionary<BlockType, List<Point>> SpecialBlocks { get; private set; }



        public Board(int width, int height) {
            _board = new Block[height,width];
            _pieces = new List<Piece>();

            SpecialBlocks = new Dictionary<BlockType, List<Point>>();
            foreach (BlockType bt in Enum.GetValues(typeof(BlockType)))
            {
                SpecialBlocks.Add(bt, new List<Point>());
            }
        }

        public Block this[int x, int y]
        {
            get
            {
                Block b = _board[y, x];
                return b;
            }
            set
            {
                if (_board[y,x] != null && _board[y,x].Type != BlockType.Blocked) throw new ArgumentException("Cannot overwrite non-blocked block");
                if (value.Type != BlockType.Normal)
                {
                    SpecialBlocks[value.Type].Add(new Point(x, y));
                }
                _board[y, x] = value;
            }
        }

        public void addPiece(Piece p) {
            _pieces.Add(p);
        }

        public void addPiece(List<Piece> p)
        {
            _pieces.AddRange(p);
        }

        public Block this[Point p]
        {
            get
            {
                return this[p.X, p.Y];
            }
            set
            {
                this[p.X, p.Y] = value;
            }
        }

        public bool isOccupied(Point p)
        {
            return _pieces.Any(s => p == s.Position);
        }

        public IReadOnlyCollection<Piece> Pieces
        {
            get
            {
                return _pieces.AsReadOnly();
            }
        }

        public Piece getPieceAt(Point p) {
            return _pieces.FirstOrDefault(s => s.Position == p);
        }

        public int Height
        {
            get
            {
                return _board.GetLength(0);
            }
        }

        public int Width
        {
            get
            {
                return _board.GetLength(1);
            }
        }

        public Point getUnoccupiedByBlockType(BlockType type)
        {
            return SpecialBlocks[type].FirstOrDefault(b => !isOccupied(b));
        }

        public bool isEscape(Point p)
        {
            return (p.X >= 0 && p.Y >= 0) && (this[p].Type == BlockType.EscapeCheap || this[p].Type == BlockType.EscapeAirport);
        }
    }
}
