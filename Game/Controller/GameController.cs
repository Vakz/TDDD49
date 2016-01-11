using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Game.Model.DataStructures;
using Game.Model;
using Game.AI;

namespace Game.Controller
{
    class GameController
    {
        private BoardController Game { get; set; }
        private PoliceAI AI { get; set; }

        public Board Board
        {
            get { return Game.State.Board; }
        }

        public Action OnReloadedState
        {
            get
            {
                return Game.OnReloadedState;
            }
            set
            {
                Game.OnReloadedState = value;
            }
        }

        public int Width
        {
            get
            {
                return Game.State.Board.Width;
            }
        }

        public int ThiefMoney
        {
            get {
                return Game.EscapedThiefMoney;
            }
            
        }

        public int PoliceMoney
        {
            get
            {
                return Game.PoliceMoney;
            }
        }

        public bool GameRunning
        {
            get
            {
                return Game.State.GameRunning;
            }
        }

        public int Height
        {
            get
            {
                return Game.State.Board.Height;
            }
        }

        public int? PlayerMoneyAtPoint(Point pt)
        {
            if (!pieceExistsAt(pt)) return null;
            return Game.CurrentPlayer.Money;
        }

        public int HumanPlayers
        {
            get
            {
                return Game.State.AIPolice ? Game.State.Players.Count - 1 : Game.State.Players.Count;
            }
        }

        public bool AIPolice
        {
            get
            {
                return Game.State.AIPolice;
            }
            set
            {
                Game.State.AIPolice = value;
            }
        }

        private bool isAITurn {
            get { return Game.isPoliceTurn && Game.State.AIPolice; }
        }

        /// <summary>
        /// Creates a game with the set number of players. if AIPolice is true, the
        /// total number of player instances will be nrOfHumans + 1
        /// </summary>
        /// <param name="nrOfHumans">The number of human players</param>
        /// <param name="AIPolice">Should Police be AI-controlled?</param>
        public void newGame(int nrOfHumans, bool AIPolice)
        {
            Game = new BoardController(AIPolice ? nrOfHumans + 1 : nrOfHumans);
            Game.State.AIPolice = AIPolice;
            if (AIPolice)
            {
                AI = new PoliceAI(this);
                AI.setPathFinderInfo(new Model.Logic.PathFinder(Width, Height));
                AI.think();
                Game.OnTurnEnd += delegate()
                {
                    if (isAITurn) AI.think();
                };
            }
        }

        public bool CurrentPlayerInJail
        {
            get
            {
                if (!(Game.CurrentPlayer.getControlledPieces()[0].Type == PieceType.Thief)) return false;
                return ((ThiefPlayer)Game.CurrentPlayer).Piece.ArrestTurns > 0;
            }
        }

        public GameController()
        {
            Game = new BoardController();
        }

        public List<Point> getCurrentPlayerPositions()
        {
            return Game.CurrentPlayer.getControlledPieces().Select<Piece, Point>(s => s.Position).ToList();
        }

        /// <summary>
        /// Checks if the specified point contains a player piece
        /// </summary>
        /// <param name="pt">The point to check</param>
        /// <returns>True if any piece at pt</returns>
        public bool pieceExistsAt(Point pt)
        {
            return Game.State.Board.getPieceAt(pt) != null;
        }

        /// <summary>
        /// Attempts to move a piece from src to dst
        /// </summary>
        /// <param name="src">Point to move from</param>
        /// <param name="dest">Point to move to</param>
        /// <returns>True if move was successful</returns>
        public bool move(Point src, Point dest)
        {
            return = Game.move(src, dest);
        }

        /// <summary>
        /// Compiles a list of all pieces in the game
        /// </summary>
        /// <returns>IReadOnlyCollection containing all game pieces</returns>
        public IReadOnlyCollection<Piece> getAllPieces()
        {
            return Game.State.Board.Pieces;
        }

        /// <summary>
        /// Attempts to skip turn
        /// </summary>
        public void skip() {
            Game.skipTurn();
        }

        public bool attemptEscape()
        {
            if (!CurrentPlayerInJail) throw new ArgumentException("Current player is not in jail");
            return Game.attemptEscapeJail();
        }

        public int DiceRoll
        {
            get
            {
                return Game.State.CurrentPlayerDiceRoll;
            }
        }
    }
}
