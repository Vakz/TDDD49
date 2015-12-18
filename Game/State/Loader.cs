using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Game.Model.DataStructures;
using System.Xml.Linq;
using Game.Model;
using System.IO;

namespace Game.State
{
    static class Loader
    {
        static string filename = Directory.GetCurrentDirectory() + "\\Resources\\save.xml";

        public static Board LoadBoard()
        {
            return Game.Model.Logic.BoardReader.readBoard(Directory.GetCurrentDirectory() + "\\Resources\\board.txt");
        }

        public static GameState Load()
        {
            if (!File.Exists(filename)) return null;
            XDocument doc = XDocument.Load(filename);
            if (!Convert.ToBoolean(doc.Root.Element("GameState").Element("GameRunning").Value)) return null;
            return GameState(doc.Root);
        }

        private static GameState GameState(XElement state)
        {
            var gs = from g in state.Elements("GameState")
                                        select new
                                        {
                                            AIPolice = Convert.ToBoolean(g.Element("AIPolice").Value),
                                            ThiefPlayers = ThiefPlayers(g.Element("ThiefPlayers")),
                                            PolicePlayer = PolicePlayer(g.Element("PolicePlayer")),
                                            CurrentPlayerDiceRoll = Convert.ToInt32(g.Element("CurrentDiceRoll").Value),
                                            CurrentPlayerIndex = Convert.ToInt32(g.Element("CurrentPlayerIndex").Value),
                                            TravelAgency = Convert.ToInt32(g.Element("TravelAgency").Attribute("Money").Value),
                                            GameRunning = Convert.ToBoolean(g.Element("GameRunning").Value)
                                        };
            GameState s = new GameState();
            foreach (var g in gs)
            {
                s.Board = LoadBoard();
                s.AIPolice = g.AIPolice;
                s.ThiefPlayers = g.ThiefPlayers;
                s.PolicePlayer = g.PolicePlayer;
                s.CurrentPlayerDiceRoll = g.CurrentPlayerDiceRoll;
                s.CurrentPlayerIndex = g.CurrentPlayerIndex;
                s.GameRunning = g.GameRunning;
                ((TravelAgency)s.Board[s.Board.SpecialBlocks[BlockType.TravelAgency][0].X, s.Board.SpecialBlocks[BlockType.TravelAgency][0].Y]).Money = g.TravelAgency;


                s.Board.addPiece(s.ThiefPlayers.Select<ThiefPlayer, Piece>(player => player.Piece).ToList());
                s.Board.addPiece(s.PolicePlayer.getControlledPieces());
            }
            return s;
        }

        private static List<Piece> PolicePieces(XElement pieces)
        {
            IEnumerable<Piece> t = from p in pieces.Elements("Piece")
                                   select new Piece(Convert.ToInt32(p.Attribute("ID").Value), Position(p.Element("Position")))
                                   {
                                       Alive = Convert.ToBoolean(p.Element("Alive").Value),
                                       TurnsOnCurrentPosition = Convert.ToInt32(p.Element("TurnsOnCurrentPosition").Value),
                                       TrainMovementStreak = Convert.ToInt32(p.Element("TrainMovementStreak").Value)
                                   };
            return t.ToList();
        }

        private static PolicePlayer PolicePlayer(XElement player)
        {
            return new PolicePlayer(Convert.ToInt32(player.Attribute("ID").Value))
                                          {
                                              Money = Convert.ToInt32(player.Attribute("Money").Value),
                                              Pieces = PolicePieces(player.Element("Pieces"))
                                          };
        }

        private static List<ThiefPlayer> ThiefPlayers(XElement players)
        {
            IEnumerable<ThiefPlayer> tp = from t in players.Elements("ThiefPlayer")
                                          select new ThiefPlayer(Convert.ToInt32(t.Attribute("ID").Value), Point.Error)
                                          {
                                              Piece = ThiefPiece(t.Element("Piece"))
                                          };
            return tp.ToList();
        }

        private static Thief ThiefPiece(XElement piece)
        {
            return new Thief(Convert.ToInt32(piece.Attribute("ID").Value), Position(piece.Element("Position")))
            {
                Arrestable = Convert.ToBoolean(piece.Element("Arrestable").Value),
                Alive = Convert.ToBoolean(piece.Element("Alive").Value),
                TurnsOnCurrentPosition = Convert.ToInt32(piece.Element("TurnsOnCurrentPosition").Value),
                TrainMovementStreak = Convert.ToInt32(piece.Element("TrainMovementStreak").Value),
                Money = Convert.ToInt32(piece.Element("Money").Value),
                ArrestTurns = Convert.ToInt32(piece.Element("ArrestTurns").Value),
                ArrestCount = Convert.ToInt32(piece.Element("ArrestCount").Value),
                HiddenMoney = HiddenMoney(piece.Element("HiddenMoney"))
            };
        }

        private static Dictionary<Point, int> HiddenMoney(XElement hidden)
        {
            IEnumerable<KeyValuePair<Point, int>> points = from h in hidden.Elements()
                                                           select new KeyValuePair<Point, int>(Position(h.Element("Position")), Convert.ToInt32(h.Element("Amount").Value));
            return points.ToDictionary(x => x.Key, x => x.Value);
        }

        private static Point Position(XElement point)
        {
           return new Point((Convert.ToInt32(point.Attribute("X").Value)), ((Convert.ToInt32(point.Attribute("Y").Value))));
        }
    }
}
