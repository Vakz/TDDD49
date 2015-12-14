using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Game.Model.DataStructures;

namespace Game.Controller
{
    class GameController
    {
        private BoardController Game { get; set; }

        public int Width
        {
            get
            {
                return Game.Board.Width;
            }
        }

        public int Height
        {
            get
            {
                return Game.Board.Height;
            }
        }

        public bool AIPolice { get; private set; }
        public int HumanPlayers { get; set; }

        private bool isAITurn() {
            if (Game.CurrentPlayerIndex == Game.Players.Count - 1 && AIPolice) return true;
            if (Game.CurrentPlayerIndex > HumanPlayers - 1) return true;
            return false;
        }

        public GameController(int nrOfHumans, int nrOfAI, bool AIPolice)
        {
            HumanPlayers = nrOfHumans;
            this.AIPolice = AIPolice;
            Game = new BoardController(nrOfHumans + nrOfAI);
        }

        public bool pieceExistsAt(int x, int y)
        {
            return Game.Board.getPieceAt(new Point(x, y)) != null;
        }
    }
}
