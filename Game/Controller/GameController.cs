using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Game.Model.DataStructures;
using Game.Model;

namespace Game.Controller
{
    class GameController
    {
        private BoardController Game { get; set; }

        public Board Board
        {
            get { return Game.State.Board; }
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
                return AIPolice ? Game.State.Players.Count - 1 : Game.State.Players.Count;
            }
        }

        public bool AIPolice { get; private set; }

        private bool isAITurn() {
            return Game.isPoliceTurn && AIPolice;
        }

        /// <summary>
        /// Creates a game with the set number of players. if AIPolice is true, the
        /// total number of player instances will be nrOfHumans + 1
        /// </summary>
        /// <param name="nrOfHumans">The number of human players</param>
        /// <param name="AIPolice">Should Police be AI-controlled?</param>
        public GameController(int nrOfHumans, bool AIPolice)
        {
            this.AIPolice = AIPolice;
            Game = new BoardController(AIPolice ? nrOfHumans + 1 : nrOfHumans);
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
            return Game.move(src, dest);
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

        public int DiceRoll
        {
            get
            {
                return Game.State.CurrentPlayerDiceRoll;
            }
        }
    }
}
