using System;
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
            return (new Random(new DateTime().Millisecond)).Next(1,7); // Not that random, but good enough for dice rolls
        }

        public static bool containedIn(Game.Model.DataStructures.Point p, int height, int width)
        {
            return p  != Game.Model.DataStructures.Point.Error && (p.X >= 0 && p.Y >= 0 && p.X < width && p.Y < height);
        }

        public bool escapingThiefPred(ThiefPlayer tp) {
            return tp.Piece.Type == PieceType.Thief && tp.Piece.Alive && tp.Piece.Arrestable
                            && new[] { BlockType.EscapeAirport, BlockType.EscapeCheap }.Any(b => board[tp.Piece.Position].Type == b);
        }

        public bool isRobableBlock(Block b)
        {
            return new[] { BlockType.Bank, BlockType.TravelAgency }.Any(s => s == b.Type);
        }
    }
}
