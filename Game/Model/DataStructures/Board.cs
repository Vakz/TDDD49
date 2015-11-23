﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.Model.DataStructures
{
    class Board
    {
        private Block[,] _board;
        private List<Piece> _pieces;
        private Dictionary<BlockType, List<Point>> specialBlocks = new Dictionary<BlockType, List<Point>>();

        public Board(int width, int height) {
            _board = new Block[height,width];
            _pieces = new List<Piece>();

            foreach (BlockType bt in Enum.GetValues(typeof(BlockType)))
            {
                specialBlocks.Add(bt, new List<Point>());
            }
        }

        public Block this[int x, int y]
        {
            get
            {
                return _board[y, x];
            }
            set
            {
                if (_board[y,x] != null && _board[y,x].Type != BlockType.Blocked) throw new ArgumentException("Cannot overwrite non-blocked block");
                if (value.Type != BlockType.Normal)
                {
                    specialBlocks[value.Type].Add(new Point(x, y));
                }
                _board[y, x] = value;
            }
        }

        public void addPiece(PieceType pt) {
            List<Point> spawnPoints = specialBlocks[pt == PieceType.Police ? BlockType.PoliceStation : BlockType.Hideout];
            Point p = spawnPoints.First(s => !isOccupied(s));
            if (p == null) throw new ArgumentException("No available block to add piece");
            _pieces.Add(pt == PieceType.Thief ? new Thief(p) : new Piece(p));
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
            return _pieces.First(s => s.Position == p);
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
    }
}
