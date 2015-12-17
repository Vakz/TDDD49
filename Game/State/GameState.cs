using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Game.Model;

namespace Game.Model.DataStructures
{
    public class GameState
    {
        /// <summary>
        /// A list of all the players in the game. The police player can be expected to be the last element.
        /// </summary>
        public List<IPlayer> Players
        {
            get
            {
                List<IPlayer> p = new List<IPlayer>(ThiefPlayers);
                p.Add(PolicePlayer);
                return p;
            }
        }

        public Game.Model.DataStructures.Board Board { get; set; }

        public List<ThiefPlayer> ThiefPlayers { get; set; }

        public PolicePlayer PolicePlayer { get; set; }

        public int CurrentPlayerDiceRoll { get; set; }

        public int CurrentPlayerIndex { get; set; }

        public bool AIPolice { get; set; }

        public bool GameRunning { get; set; }

        public GameState()
        {
            ThiefPlayers = new List<ThiefPlayer>();
            GameRunning = true;
        }
    }
}
