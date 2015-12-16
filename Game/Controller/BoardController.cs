using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Game.Model.DataStructures;
using Game.Model.Logic;
using Game.Model.Rules;
using Game.Model;
using System.IO;
using Game.Exceptions;

namespace Game.Controller
{
    class BoardController
    {
        public Board Board {get; private set;}
        public List<IPlayer> Players { get; private set; } // Used to keep track of current player
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
            Players = new List<IPlayer>();
            Board = BoardReader.readBoard( Directory.GetCurrentDirectory()+"\\Resources\\board.txt" );
            addThiefPlayers(nrOfPlayers - 1);
            addPolicePlayer(nrOfPlayers);
            ruleEngine = new RuleEngine(Board);
            logicEngine = new LogicEngine(Board);
            CurrentPlayerDiceRoll = LogicEngine.diceRoll(); // Initial player roll
            CurrentPlayerIndex = Players.Count - 1;
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
                for (int i = 0; i < nrOfPlayers; ++i)
                {
                    ThiefPlayer tp = new ThiefPlayer(Board.getUnoccupiedByBlockType(BlockType.Hideout));
                    thiefPlayers.Add(tp);
                    Board.addPiece(tp.Piece);
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
            Board.addPiece(policePlayer.getControlledPieces());
        }

        public int EscapedThiefMoney {
            get
            {
                int sum = 0;
                foreach (ThiefPlayer t in thiefPlayers)
                {
                    if (!t.Piece.Alive)
                    {
                        sum += t.Piece.Money;
                    }
                }
                return sum;
            }            
        }

        public int PoliceMoney
        {
            get
            {
                return policePlayer.Money;
            }
        }

        public bool move(Point src, Point dest)
        {
            Piece p = Board.getPieceAt(src);
            return movePiece(p, dest);
        }

        /// <summary>
        /// Moves a piece to the target destination if allowed
        /// </summary>
        /// <param name="p">Piece to be moved</param>
        /// <param name="pt">Destination</param>
        /// <returns>True if move was successful</returns>
        public bool movePiece(Piece p, Point pt) {
            if (!GameRunning) throw new ApplicationException("Game is not running!");
            if (!Players[CurrentPlayerIndex].allowedToMovePiece(p)) throw new IllegalMoveException("Selected piece is not allowed to move this turn");
            else if (!ruleEngine.canMoveTo(p, pt, CurrentPlayerDiceRoll)) throw new IllegalMoveException("Piece cannot move to the selected position");
            else if (ruleEngine.canArrestAt(p, pt))
            {
                Thief arrestTarget = (Thief)Board.getPieceAt(pt);
                policePlayer.Money += ruleEngine.arrest(arrestTarget, p);
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
        public void skipTurn()
        {
            if (!GameRunning) throw new ApplicationException("Game is not running!");
            IPlayer p = Players[CurrentPlayerIndex];
            if (!p.getControlledPieces().TrueForAll(ruleEngine.isAllowedToSkipTurn)) throw new IllegalMoveException("Player is not allowed to skip this turn");
            p.getControlledPieces().ForEach(s => s.TurnsOnCurrentPosition++);
            endTurn();
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
            // All players have at least one piece, so using index 0 is safe.
            if (Players[CurrentPlayerIndex].getControlledPieces()[0].Type == PieceType.Police)
            {
                List<ThiefPlayer> escaping = thiefPlayers.Where((new Func<ThiefPlayer, bool>(logicEngine.escapingThiefPred))).ToList();
                foreach(ThiefPlayer tp in escaping)
                {
                    ruleEngine.removePieceFromGame(tp.Piece);
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
            if (!Players[CurrentPlayerIndex].anyInPlay()) nextPlayer(); // Will cause multiple die rolls
            CurrentPlayerDiceRoll = LogicEngine.diceRoll();
        }
    }
}
