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
        private List<Player> players; // Used to keep track of current player
        private List<ThiefPlayer> thiefPlayers = new List<ThiefPlayer>();
        private PolicePlayer policePlayer;
        private int aliveThiefPlayers = 0;
        private RuleEngine ruleEngine;
        public bool GameRunning { get; set; }
        public int CurrentPlayerIndex { get; private set; }

        public GameController(int nrOfPlayers) {
            if (nrOfPlayers < 2) throw new ArgumentException("Must have at least two players");
            try {
                List<Point> policeSpawnpoints = new List<Point>();
                for (int i = 0; i < nrOfPlayers - 1; ++i) {
                    policeSpawnpoints.Add(board.getUnoccupiedByBlockType(BlockType.PoliceStation));
                    thiefPlayers.Add(new ThiefPlayer(board.getUnoccupiedByBlockType(BlockType.Hideout)));
                    aliveThiefPlayers++;
                }
                // One more police pieces than thieves
                policeSpawnpoints.Add(board.getUnoccupiedByBlockType(BlockType.PoliceStation));
                policePlayer = new PolicePlayer(policeSpawnpoints);
            }
            catch(Exception e) {
                throw new ArgumentException("Attempted to add more thieves than there are hideouts");
            }
            players = new List<Player>(thiefPlayers);
            players.Add(policePlayer);
            ruleEngine = new RuleEngine(board);
            GameRunning = true;
        }

        public void movePiece(Piece p, Point pt) {
            if (!players[CurrentPlayerIndex].allowedToMovePiece(p)) return;
            else if (!ruleEngine.canMoveTo(p, pt)) return;
            else if (ruleEngine.canArrestAt(p, pt))
            {
                Thief arrestTarget = (Thief)board.getPieceAt(pt);
                ruleEngine.arrest(arrestTarget, p);
                if (arrestTarget.ArrestCount == RuleEngine.MAX_ARRESTS)
                {
                    ruleEngine.removePieceFromGame(p);
                    ruleEngine.removePieceFromGame(arrestTarget);

                }
            }
            else p.Position = pt;
            p.TurnsOnCurrentPosition++;
        }

        public void endTurn()
        {
            nextPlayer();
            // If all thieves arrested or no thief pieces on the board, end the game
            GameRunning = thiefPlayers.Any(s => s.Piece.Alive && s.Piece.ArrestTurns == 0);
        }

        public void nextPlayer()
        {
            // TODO: Check if thief is standing on escape, is arrestable, and should be removed from the game
            CurrentPlayerIndex = (CurrentPlayerIndex+1) % players.Count;
        }
    }
}
