using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Game.Model.DataStructures;
using Game.Controller;

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

        public void think()
        {
            List<Point> pieces = game_controller.getCurrentPlayerPositions();
            List<Point> targets = getPreferredTargets();

            Dictionary<Point, Point> closest_target = new Dictionary<Point, Point>();
            foreach (Point p in pieces){
                Point closest = Point.Error;
                foreach (Point t in targets){
                    if (manhattan_dist(p, t) < manhattan_dist(p, closest)){
                        closest = t;
                    }
                    
                }
            }
        }

    }
}
