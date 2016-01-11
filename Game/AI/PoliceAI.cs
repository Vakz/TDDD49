using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Game.Model.DataStructures;
using Game.Controller;

using Game.Model.Logic;
using Game.Model.Rules;

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

        private PathFinder pathFinder;
        public void setPathFinderInfo( PathFinder pathFinder ){
            this.pathFinder = pathFinder;
        }

        public void think() {
            List<Point> pieces = game_controller.getCurrentPlayerPositions();
            List<Point> targets = getPreferredTargets();


            Dictionary<Point, Point> closest_thieves = new Dictionary<Point, Point>();
            foreach (Point police in pieces){
                RuleEngine.PieceCanPassPoint canPass = new RuleEngine.PieceCanPassPoint(
                    game_controller.Board.getPieceAt(police), game_controller.Board);
                
                Point closest_thief = pathFinder.getClosestPointOfInterest( police, targets, canPass );
                closest_thieves[police] = closest_thief;
            }

            // testa om någon polis kan nå en tjuv direkt:
            foreach ( Point police in closest_thieves.Keys ) {
                RuleEngine.PieceCanPassPoint canPass = new RuleEngine.PieceCanPassPoint(
                    game_controller.Board.getPieceAt(police), game_controller.Board);

                List<Point> path = pathFinder.getPathWithExactCost( police,
                                                                    closest_thieves[police],
                                                                    game_controller.DiceRoll,
                                                                    canPass );
                if (path != null) {
                    game_controller.move(police, closest_thieves[police]);
                    return;
                }
            }
            // testa om någon polis kan närma sig en tjuv:
            foreach (Point police in closest_thieves.Keys) {
                RuleEngine.PieceCanPassPoint canPass = new RuleEngine.PieceCanPassPoint(
                    game_controller.Board.getPieceAt(police), game_controller.Board);

                // kolla vilka platser polisen kan röra sig till:
                List<Point> points = pathFinder.getPointsWithExactCost(police, game_controller.DiceRoll, canPass);
                // kolla vilken av dessa platser som är närmast tjuven som letas:
                Point point = pathFinder.getClosestPointOfInterest(closest_thieves[police], points, canPass);
                if (point != Point.Error) {
                    game_controller.move( police, point );
                    return;
                }
            }
            // annars rör sig polisen inte:
            game_controller.skip();
        }
    }
}
