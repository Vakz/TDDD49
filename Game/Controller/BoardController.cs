using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Game.Model.DataStructures;
using Game.Model.Logic;
using Game.Model.Rules;
using Game.Model;

namespace Game.Controller;
{
    class GameController
    {
        private Board board = BoardReader.readBoard("board.txt");
        private List<Player> players = new List<Player>();
        private RuleEngine ruleEngine;

        public GameController(int nrOfPlayers) {

        }
    }
}
