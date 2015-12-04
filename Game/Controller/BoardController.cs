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
            addThiefPlayers(nrOfPlayers - 1);
            addPolicePlayer(nrOfPlayers);
            ruleEngine = new RuleEngine(board);
            GameRunning = true;
        }

        private void addThiefPlayers(int nrOfPlayers)
        {
            try
            {
                for (int i = 0; i < nrOfPlayers - 1; ++i)
                {
                    ThiefPlayer tp = new ThiefPlayer(board.getUnoccupiedByBlockType(BlockType.Hideout));
                    thiefPlayers.Add(tp);
                    players.Add(tp);
                    aliveThiefPlayers++;
                }
                // One more police pieces than thieves
            }
            catch (Exception e)
            {
                throw new ArgumentException("Attempted to add more thieves than there are hideouts");
            }           
        }

        private void addPolicePlayer(int nrOfPolice)
        {
            List<Point> spawnpoints = new List<Point>();
            for (int i = 0; i < nrOfPolice; ++i)
            {
                spawnpoints.Add(board.getUnoccupiedByBlockType(BlockType.PoliceStation));
            }
            policePlayer = new PolicePlayer(spawnpoints);
            players.Add(policePlayer);

        }

        /// <summary>
        /// Moves a piece to the target destination if allowed
        /// </summary>
        /// <param name="p">Piece to be moved</param>
        /// <param name="pt">Destination</param>
        /// <returns>True if move was successful</returns>
        public bool movePiece(Piece p, Point pt) {
            if (!players[CurrentPlayerIndex].allowedToMovePiece(p)) return false;
            else if (!ruleEngine.canMoveTo(p, pt)) return false;
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
            else
            {
                if (movedByTrain(p.Position, pt)) p.TrainMovementStreak++;
                p.Position = pt;
            }
            p.TurnsOnCurrentPosition++;
            return true;
        }

        private bool movedByTrain(Point origin, Point dest)
        {
            return board[origin].Type == BlockType.TrainStop && board[dest].Type == BlockType.TrainStop;
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
