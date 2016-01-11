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
using Game.State;

namespace Game.Controller
{
    class BoardController
    {
        
        
        private RuleEngine ruleEngine;
        private LogicEngine logicEngine;
        public Action OnReloadedState { get; set; }
        

        public GameState State { get; set; }
        SaveHandler SaveHandler { get; set; }

        public BoardController()
        {
            SaveHandler = new SaveHandler();
            
            gameFromFile();
            ruleEngine = new RuleEngine(State.Board);
            logicEngine = new LogicEngine(State.Board);
            SaveHandler.OnManualSaveChange += gameFromFile;
        }

        private void gameFromFile() {
            State = SaveHandler.Load();
            if (State == null) newGame(2);
            else if(OnReloadedState != null) OnReloadedState();
        }

        public BoardController(int nrOfPlayers)
        {
            SaveHandler = new SaveHandler();
            newGame(nrOfPlayers);
            SaveHandler.OnManualSaveChange += gameFromFile;
        }

        private void newGame(int nrOfPlayers)
        {
            State = new GameState();
            if (nrOfPlayers < 2) throw new ArgumentException("Must have at least two players");
            State.Board = Loader.LoadBoard();
            addThiefPlayers(nrOfPlayers - 1);
            addPolicePlayer(nrOfPlayers);
            ruleEngine = new RuleEngine(State.Board);
            logicEngine = new LogicEngine(State.Board);
            State.CurrentPlayerDiceRoll = LogicEngine.diceRoll(); // Initial player roll
            State.CurrentPlayerIndex = State.Players.Count - 1;
            if (OnReloadedState != null) OnReloadedState();
        }

        public IPlayer CurrentPlayer {
            get { return State.Players[State.CurrentPlayerIndex]; }
        }

        public bool isPoliceTurn
        {
            get
            {
                return CurrentPlayer.getControlledPieces()[0].Type == PieceType.Police;
            }
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
                    ThiefPlayer tp = new ThiefPlayer(i, State.Board.getUnoccupiedByBlockType(BlockType.Hideout));
                    State.ThiefPlayers.Add(tp);
                    State.Board.addPiece(tp.Piece);
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
            State.PolicePlayer = new PolicePlayer(State.Players.Count);
            for (int i = 0; i < nrOfPolice; ++i)
            {
                State.Board.addPiece(State.PolicePlayer.addPiece(State.Board.getUnoccupiedByBlockType(BlockType.PoliceStation)));
            }
        }

        public int EscapedThiefMoney {
            get
            {
                int sum = 0;
                foreach (ThiefPlayer t in State.ThiefPlayers)
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
                return State.PolicePlayer.Money;
            }
        }

        public bool move(Point src, Point dest)
        {
            if (!State.GameRunning) throw new ApplicationException("Game is not running!");
            Piece p = State.Board.getPieceAt(src);
            if (!CurrentPlayer.allowedToMovePiece(p)) throw new IllegalMoveException("Selected piece is not allowed to move this turn");
            else if (!ruleEngine.canMoveTo(p, dest, State.CurrentPlayerDiceRoll)) throw new IllegalMoveException("Piece cannot move to the selected position");
            else return movePiece(p, dest);
        }

        /// <summary>
        /// Moves a piece to the target destination if allowed
        /// </summary>
        /// <param name="p">Piece to be moved</param>
        /// <param name="pt">Destination</param>
        /// <returns>True if move was successful</returns>
        private bool movePiece(Piece p, Point pt) {
            if (ruleEngine.canArrestAt(p, pt))
            {
                Thief arrestTarget = (Thief)State.Board.getPieceAt(pt);
                makeArrest(p, arrestTarget);
            }
            else
            {
                if (logicEngine.movedByTrain(p.Position, pt)) p.TrainMovementStreak++;
                else p.TrainMovementStreak = 0;
                p.Position = pt;
                p.TurnsOnCurrentPosition = 0;
                if (p.Type == PieceType.Thief) attemptToRobPos((Thief)p, pt);
            }
            p.TurnsOnCurrentPosition++;
            endTurn();
            return true;
        }

        private void makeArrest(Piece police, Thief thief)
        {
            State.PolicePlayer.Money += ruleEngine.arrest(thief, police);
            if (thief.ArrestCount == RuleEngine.MAX_ARRESTS)
            {
                ruleEngine.removePieceFromGame(police);
                ruleEngine.removePieceFromGame(thief);
            }
        }

        private void attemptToRobPos(Thief t, Point pt) {
            Block b = State.Board[pt];
            if (logicEngine.isRobableBlock(State.Board[pt]))
            {
                if ((b.Type == BlockType.Bank || b.Type == BlockType.TravelAgency) && ((Bank)b).Money > 0) ruleEngine.robBank(t, (Bank)b);
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
            if (!State.GameRunning) throw new ApplicationException("Game is not running!");;
            if (!CurrentPlayer.getControlledPieces().TrueForAll(ruleEngine.isAllowedToSkipTurn)) throw new IllegalMoveException("Player is not allowed to skip this turn");
            CurrentPlayer.getControlledPieces().ForEach(s => s.TurnsOnCurrentPosition++);
            endTurn();
        }

        /// <summary>
        /// Makes all checks that should be made at the end of turn and moves the game to the next player
        /// </summary>
        public void endTurn()
        {
            // TODO: Check if any thieves are surrounded


            // When police turn ends, check if any thief is attempting to escape
            // All players have at least one piece, so using index 0 is safe.
            if (isPoliceTurn)
            {
                List<ThiefPlayer> aliveThieves = State.ThiefPlayers.Where(s => s.Piece.Alive).ToList();
                foreach (Thief t in aliveThieves.Select<ThiefPlayer, Piece>(s => s.Piece))
                {
                    if (ruleEngine.isThiefSurrounded(State.Board.SpecialBlocks[BlockType.Hideout], t))
                    {
                        makeArrest(State.PolicePlayer.getControlledPieces().First(), t);
                    }
                }

                List<ThiefPlayer> escaping = aliveThieves.Where((new Func<ThiefPlayer, bool>(logicEngine.escapingThiefPred))).ToList();
                System.Console.WriteLine(aliveThieves.Count);
                System.Console.WriteLine(escaping.Count);
                foreach(ThiefPlayer tp in escaping)
                {
                    int cost = State.Board[tp.Piece.Position].Type == BlockType.EscapeAirport ? 3000 : 1000;
                    tp.Piece.Money -= cost;
                    addTravelAgencyMoney(cost);
                    Piece p = logicEngine.anyPieceTypeOnBlockType(PieceType.Police, BlockType.Telegraph);
                    if (p != null) makeArrest(p, tp.Piece);
                    else
                    {
                        ruleEngine.removePieceFromGame(tp.Piece);
                        ruleEngine.removePieceFromGame(State.PolicePlayer.getControlledPieces().First(s => s.Alive));
                    }
                }
            }
            // At end of thief turn, check if arrested and potentially release
            else {
                Thief t = State.ThiefPlayers[State.CurrentPlayerIndex].Piece;
                if (t.ArrestTurns > 0)
                {
                    t.ArrestTurns--;
                    if (t.ArrestTurns == 0) ruleEngine.release(t);
                }
            }
            nextPlayer();
            SaveHandler.Save(State);
            // If all thieves arrested or no thief pieces on the board, end the game
            State.GameRunning = State.ThiefPlayers.Any(s => s.Piece.Alive && s.Piece.ArrestTurns == 0);
        }

        public void addTravelAgencyMoney(int amount)
        {
            ((TravelAgency)State.Board[State.Board.SpecialBlocks[BlockType.TravelAgency].First()]).addMoney(amount);
        }

        public bool attemptEscapeJail()
        {
            if (LogicEngine.diceRoll() == 6)
            {
                ruleEngine.release(State.ThiefPlayers[State.CurrentPlayerIndex].Piece);
                State.ThiefPlayers[State.CurrentPlayerIndex].Piece.Arrestable = true;
                return true;
            }
            endTurn();
            return false;
        }

        public void nextPlayer()
        {
            State.CurrentPlayerIndex = (State.CurrentPlayerIndex+1) % State.Players.Count;
            if (!State.Players[State.CurrentPlayerIndex].anyInPlay()) nextPlayer(); // Will cause multiple die rolls
            State.CurrentPlayerDiceRoll = LogicEngine.diceRoll();
        }
    }
}
