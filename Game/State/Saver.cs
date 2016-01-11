using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Game.Model.DataStructures;
using System.Xml.Linq;
using Game.Model;
using System.IO;
using System.Xml;

namespace Game.State
{
    static class Saver
    {

        public static void Save(string filename, GameState state)
        {

            // Purge with fire
            while (true)
            {
                try
                {
                    XDocument x = new XDocument();
                    x.Add(new XElement("Save", XmlGameState(state)));
                    x.Save(filename);
                    break;
                }
                catch (Exception) {}
            }
        }

        private static XElement XmlGameState(GameState state)
        {
            XElement s = new XElement("GameState");
            s.Add(new XElement("AIPolice", state.AIPolice));
            s.Add(XmlThiefPlayers(state.ThiefPlayers));
            s.Add(XmlPolicePlayer(state.PolicePlayer));
            s.Add(new XElement("CurrentDiceRoll", state.CurrentPlayerDiceRoll));
            s.Add(new XElement("GameRunning", state.GameRunning));
            s.Add(new XElement("CurrentPlayerIndex", state.CurrentPlayerIndex));
            s.Add(new XElement("TravelAgency", new XAttribute("Money", ((TravelAgency)state.Board[state.Board.SpecialBlocks[BlockType.TravelAgency][0]]).Money)));
            return s;
        }

        private static XElement XmlPolicePlayer(PolicePlayer pp)
        {
            XElement policePlayer = new XElement("PolicePlayer");
            policePlayer.Add(new XAttribute("ID", pp.ID));
            policePlayer.Add(new XAttribute("Money", pp.Money));
            XElement pieces = new XElement("Pieces");
            foreach(Piece p in pp.Pieces)
            {
                pieces.Add(XmlPiece(p));
            }
            policePlayer.Add(pieces);
            return policePlayer;
        }

        private static XElement XmlThiefPlayers(List<ThiefPlayer> tp)
        {
            XElement thieves = new XElement("ThiefPlayers");
            foreach(ThiefPlayer t in tp) {
                thieves.Add(
                    new XElement("ThiefPlayer",
                        new XAttribute("ID", t.ID),
                        XmlThiefPiece(t.Piece)
                    )
                );
            }
            return thieves;
        }

        private static XElement XmlThiefPiece(Thief t)
        {
            XElement tp = XmlPiece(t);
            tp.Add(new XElement("Arrestable", t.Arrestable));
            tp.Add(new XElement("Money", t.Money));
            tp.Add(new XElement("ArrestTurns", t.ArrestTurns));
            tp.Add(new XElement("ArrestCount", t.ArrestCount));
            tp.Add(XmlHiddenMoney(t.HiddenMoney));

            return tp;
        }

        private static XElement XmlPiece(Piece p)
        {
            XElement piece = new XElement("Piece");
            piece.Add(new XAttribute("ID", p.ID));
            piece.Add(XmlPoint(p.Position));
            piece.Add(new XElement("Alive", p.Alive));
            piece.Add(new XElement("Active", p.Active));
            piece.Add(new XElement("TurnsOnCurrentPosition", p.TurnsOnCurrentPosition));
            piece.Add(new XElement("TrainMovementStreak", p.TrainMovementStreak));
            return piece;
        }

        private static XElement XmlHiddenMoney(Dictionary<Point, int> hiddenMoney) {
            XElement HiddenMoney = new XElement("HiddenMoney");
            foreach (KeyValuePair<Point, int> kvp in hiddenMoney)
            {
                HiddenMoney.Add(
                    new XElement("Hidden", XmlPoint(kvp.Key)),
                    new XElement("Amount", kvp.Value)
                );
            }
            return HiddenMoney;
        }

        private static XElement XmlPoint(Point p)
        {
            XElement pt = new XElement("Position");
            pt.Add(new XAttribute("X", p.X));
            pt.Add(new XAttribute("Y", p.Y));
            return pt;
        }
    }
}
