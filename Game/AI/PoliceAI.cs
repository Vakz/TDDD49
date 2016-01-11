using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Game.Model.DataStructures;
using Game.Controller;

using Game.Model.Logic;

namespace Game.AI
{
    class PoliceAI
    {
        private GameController game_controller;

        public PoliceAI(GameController game_controller)
        {
            this.game_controller = game_controller;
            if (!game_controller.AIPolice)
            {
                throw new Exception("cannot use AI when AI is disabled.");
            }
        }

        private int manhattan_dist(Point a, Point b)
        {
            if (a == Point.Error || b == Point.Error) return int.MaxValue;
            return Math.Abs(a.X - b.X) + Math.Abs(a.Y - b.Y);
        }
        
        private List<Point> getPreferredTargets(){
            IReadOnlyCollection<Piece> pieces = game_controller.getAllPieces();
            List<Point> preferred_targets = new List<Point>();
            List<Point> nonarrestable     = new List<Point>();
            foreach ( Piece p in pieces ){
                if ( p.Type != PieceType.Thief ) continue;
                if ( ((Thief)p).Arrestable ){
                    preferred_targets.Add(p.Position);
                } else {
                    nonarrestable.Add(p.Position);
                }
            }

            if (preferred_targets.Count != 0){
                return preferred_targets;
            } else {
                return nonarrestable;
            }
        }

        public void setPathFinderInfo() { }

        public void think() {
            List<Point> pieces = game_controller.getCurrentPlayerPositions();
            List<Point> targets = getPreferredTargets();

            Dictionary<Point, Point> closest_thieves = new Dictionary<Point, Point>();
            foreach (Point police in pieces){
                Point closest_thief = Point.Error;
                foreach (Point thief in targets){
                    if (manhattan_dist(police, thief) < manhattan_dist(police, closest_thief)){
                        closest_thief = thief;
                    }
                }
                closest_thieves[police] = closest_thief;
            }

            Point from = Point.Error;
            Point to   = Point.Error;

            // testa om någon polis kan nå en tjuv direkt:
            foreach ( Point p in closest_thieves.Keys ) {
                
            }
            // testa om någon polis kan närma sig en tjuv:

            game_controller.move(from, to);
            // annars rör sig polisen inte:
            game_controller.skip();
        }

    }
}
