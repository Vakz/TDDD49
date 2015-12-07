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
    class BoardController
    {
        public Board Board {get; private set;}
        public List<Player> Players { get; private set; } // Used to keep track of current player
        private List<ThiefPlayer> thiefPlayers = new List<ThiefPlayer>();
        private PolicePlayer policePlayer;
        private int aliveThiefPlayers = 0;
        public int CurrentPlayerDiceRoll { get; private set; }
        private RuleEngine ruleEngine;
        private LogicEngine logicEngine;
        public bool GameRunning { get; set; }
        public int CurrentPlayerIndex { get; private set; }

        public BoardController(int nrOfPlayers) {
            if (nrOfPlayers < 2) throw new ArgumentException("Must have at least two players");
            Players = new List<Player>();
            Board = BoardReader.readBoard("board.txt");
            addThiefPlayers(nrOfPlayers - 1);
            addPolicePlayer(nrOfPlayers);
            ruleEngine = new RuleEngine(Board);
            logicEngine = new LogicEngine(Board);
            GameRunning = true;
        }

        /// <summary>
        /// Adds new thief players to the game, at the first available hideout
        /// </summary>
        /// <param name="nrOfPlayers">The number of thief players to add</param>
        private void addThiefPlayers(int nrOfPlayers)
        {
            try
            {
                for (int i = 0; i < nrOfPlayers - 1; ++i)
                {
                    ThiefPlayer tp = new ThiefPlayer(Board.getUnoccupiedByBlockType(BlockType.Hideout));
                    thiefPlayers.Add(tp);
                    Players.Add(tp);
                    aliveThiefPlayers++;
                }
                // One more police pieces than thieves
            }
            catch (Exception)
            {
                throw new ArgumentException("Attempted to add more thieves than there are hideouts");
            }           
        }

        /// <summary>
        /// Adds new police pieces on first available point in the police station.
        /// </summary>
        /// <param name="nrOfPolice">The number of police pieces to be added</param>
        private void addPolicePlayer(int nrOfPolice)
        {
            List<Point> spawnpoints = new List<Point>();
            for (int i = 0; i < nrOfPolice; ++i)
            {
                spawnpoints.Add(Board.getUnoccupiedByBlockType(BlockType.PoliceStation));
            }
            policePlayer = new PolicePlayer(spawnpoints);
            Players.Add(policePlayer);

        }

        public bool move(Point src, Point dest)
        {
            try
            {
                Piece p = Board.getPieceAt(src);
                return movePiece(p, dest);
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Moves a piece to the target destination if allowed
        /// </summary>
        /// <param name="p">Piece to be moved</param>
        /// <param name="pt">Destination</param>
        /// <returns>True if move was successful</returns>
        public bool movePiece(Piece p, Point pt) {
            if (!Players[CurrentPlayerIndex].allowedToMovePiece(p)) return false;
            else if (!ruleEngine.canMoveTo(p, pt, CurrentPlayerDiceRoll)) return false;
            else if (ruleEngine.canArrestAt(p, pt))
            {
                Thief arrestTarget = (Thief)Board.getPieceAt(pt);
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
                if (p.Type == PieceType.Thief) attemptToRobPos((Thief)p, pt);
            }
            p.TurnsOnCurrentPosition++;
            endTurn();
            return true;
        }

        private void attemptToRobPos(Thief t, Point pt) {
            if (logicEngine.isRobableBlock(Board[pt]))
            {
                if (Board[pt].Type == BlockType.Bank)
                {
                    ruleEngine.robBank(t, (Bank)Board[pt]);
                }
                else if (Board[pt].Type == BlockType.TravelAgency && ((TravelAgency)Board[pt]).Money > 0)
                {
                    ruleEngine.robBank(t, (TravelAgency)Board[pt]);
                }
                t.Arrestable = true;
            }
        }

        /// <summary>
        /// Skips turn, if allowed
        /// </summary>
        /// <param name="p">Piece requesting skip</param>
        /// <returns>True if skip was allowed and turn ended</returns>
        public bool skipTurn(Piece p)
        {
            if (ruleEngine.isAllowedToSkipTurn(p))
            {
                p.TurnsOnCurrentPosition++;
                endTurn();
                return true;
            }
            return false;
        }

        /// <summary>
        /// Checks if the move from origin to dest was made by train
        /// </summary>
        /// <param name="origin">Point the piece originated from</param>
        /// <param name="dest">Point the piece moved to</param>
        /// <returns>True if move was made by train, else returns false</returns>
        private bool movedByTrain(Point origin, Point dest)
        {
            return Board[origin].Type == BlockType.TrainStop && Board[dest].Type == BlockType.TrainStop;
        }

        /// <summary>
        /// Makes all checks that should be made at the end of turn and moves the game to the next player
        /// </summary>
        public void endTurn()
        {
            // When police turn ends, check if any thief is attempting to escape
            if (CurrentPlayerIndex == Players.Count - 1)
            {
                try
                {
                    ruleEngine.removePieceFromGame(thiefPlayers.First((new Func<ThiefPlayer, bool>(logicEngine.escapingThiefPred))).Piece);
                }
                catch (Exception)
                {
                    // No thief matching
                }
            }
            // At end of thief turn, check if arrested and potentially release
            else {
                decrementArrestTime(thiefPlayers[CurrentPlayerIndex].Piece);
            }
            nextPlayer();
            // If all thieves arrested or no thief pieces on the board, end the game
            GameRunning = thiefPlayers.Any(s => s.Piece.Alive && s.Piece.ArrestTurns == 0);
        }

        private void decrementArrestTime(Thief t) {
            if (t.ArrestTurns > 0) {
                t.ArrestTurns--;
                if (t.ArrestTurns == 0) ruleEngine.release(t);
            }
        }

        public void attemptEscapeJail()
        {
            if (LogicEngine.diceRoll() == 6)
            {
                ruleEngine.release(thiefPlayers[CurrentPlayerIndex].Piece);
                thiefPlayers[CurrentPlayerIndex].Piece.Arrestable = true;
            }
        }

        public void nextPlayer()
        {
            CurrentPlayerIndex = (CurrentPlayerIndex+1) % Players.Count;
            CurrentPlayerDiceRoll = LogicEngine.diceRoll();
        }
    }
}
