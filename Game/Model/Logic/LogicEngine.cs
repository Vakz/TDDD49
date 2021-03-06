﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Game.Model.DataStructures;
using Game.Model;

namespace Game.Model.Logic
{
    class LogicEngine
    {
        Board board;

        public LogicEngine(Board b)
        {
            board = b;
        }

        public static int diceRoll()
        {
            return (new Random(DateTime.Now.Millisecond)).Next(1,7); // Not that random, but good enough for dice rolls
        }

        public static bool containedIn(Game.Model.DataStructures.Point p, int height, int width)
        {
            return p  != Game.Model.DataStructures.Point.Error && (p.X >= 0 && p.Y >= 0 && p.X < width && p.Y < height);
        }

        public bool escapingThiefPred(ThiefPlayer tp) {
            BlockType escapeType = board[tp.Piece.Position].Type;
            if (escapeType == BlockType.EscapeAirport && tp.Money < 3000) return false;
            if (escapeType == BlockType.EscapeCheap && tp.Money < 1000) return false;
            return tp.Piece.Type == PieceType.Thief && tp.Piece.Alive && tp.Piece.Arrestable
                            && new[] { BlockType.EscapeAirport, BlockType.EscapeCheap }.Any(b => board[tp.Piece.Position].Type == b);
        }

        public Piece anyPieceTypeOnBlockType(PieceType pt, BlockType bt)
        {
            IReadOnlyCollection<Piece> pieces = board.Pieces.Where(s => s.Alive && s.Type == pt).ToList().AsReadOnly();
            return pieces.FirstOrDefault(s => board[s.Position].Type == bt);
        }

        

        public bool isRobableBlock(Block b)
        {
            return new[] { BlockType.Bank, BlockType.TravelAgency }.Any(s => s == b.Type);
        }

        /// <summary>
        /// Checks if the move from origin to dest was made by train
        /// </summary>
        /// <param name="origin">Point the piece originated from</param>
        /// <param name="dest">Point the piece moved to</param>
        /// <returns>True if move was made by train, else returns false</returns>
        public bool movedByTrain(Point origin, Point dest)
        {
            return board[origin].Type == BlockType.TrainStop && board[dest].Type == BlockType.TrainStop;
        }
    }
}
