using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Game.Model.DataStructures;
using Game.Model.Logic;
using Game.Model.Rules;
using Game.Model;

namespace Game.Controller
{
    class GameController
    {
        private Board board = BoardReader.readBoard("board.txt");
        private List<Player> players = new List<Player>();
        private RuleEngine ruleEngine;
        public int CurrentPlayerIndex { get; private set; }

        public GameController(int nrOfPlayers) {
            if (nrOfPlayers < 2) throw new ArgumentException("Must have at least two players");
            try {
                List<Point> policeSpawnpoints = new List<Point>();
                for (int i = 0; i < nrOfPlayers - 1; ++i) {
                    policeSpawnpoints.Add(board.getUnoccupiedByBlockType(BlockType.PoliceStation));
                    players.Add(new ThiefPlayer(board.getUnoccupiedByBlockType(BlockType.Hideout)));
                }
                // One more police pieces than thieves
                policeSpawnpoints.Add(board.getUnoccupiedByBlockType(BlockType.PoliceStation));
                players.Add(new PolicePlayer(policeSpawnpoints));
                CurrentPlayerIndex = players.Count - 1; // Police goes first
            }
            catch(Exception e) {
                throw new ArgumentException("Attempted to add more thieves than there are hideouts");
            }
            ruleEngine = new RuleEngine(board);
        }

        public void movePiece(Piece p, Point pt) {
            if (!players[CurrentPlayerIndex].allowedToMovePiece(p)) return;
            else if (!ruleEngine.canMoveTo(p, pt)) return;
            else if (ruleEngine.canArrestAt(p, pt)) ruleEngine.arrest((Thief)board.getPieceAt(pt), p);
            else p.Position = pt;
        }
    }
}
