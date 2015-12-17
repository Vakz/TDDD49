using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

using Game.Model.DataStructures;
using Game.UI.Controls;
using GamePoint = Game.Model.DataStructures.Point;

namespace Game.UI
{
    class BoardCanvasTranslator
    {
        private GameCanvas canvas;
        private Board      board;

        public BoardCanvasTranslator(GameCanvas canvas, Board board)
        {
            this.canvas = canvas;
            this.board  = board;

            canvas.setPathFinder(board.Width, board.Height);
            addStaticsToCanvas();
            update();
        }

        public void update(){
            // update pieces:
            Dictionary<GamePoint,GameCanvas.PieceType> pieces = new Dictionary<GamePoint,GameCanvas.PieceType>();
            foreach ( Piece p in board.Pieces ){
                switch (p.Type) {
                    case PieceType.Thief:
                        pieces[p.Position] = GameCanvas.PieceType.Thief;
                        break;
                    case PieceType.Police:
                        pieces[p.Position] = GameCanvas.PieceType.Police;
                        break;
                }
            }
            canvas.setPieces(pieces);
        }

        private void addStaticsToCanvas()
        {
            canvas.clearDrawData();
            Dictionary<Color, List<GamePoint>> stations = new Dictionary<Color,List<GamePoint>>();
            List<Color> station_colors = new List<Color>
            {
                Color.FromRgb( 0xff, 0x00, 0x00 ),  //red
                Color.FromRgb( 0xff, 0xff, 0x00 ),  //yellow
                Color.FromRgb( 0x00, 0xff, 0x00 ),  //green
                Color.FromRgb( 0x00, 0x00, 0xff ),  //blue

                Color.FromRgb( 0x90, 0x00, 0x90 ), //purple?
                Color.FromRgb( 0x00, 0xff, 0xff )  //cyan
            };

            for (int x = 0; x < board.Width; x++){
                for (int y = 0; y < board.Height; y++){
                    Point position = new Point(x,y);
                    Block block = board[x, y];
                    switch ( block.Type )
                    {
                        // BlockType.Path:
                        case BlockType.TrainStop:
                            TrainStop station = (TrainStop)block;
                            foreach ( int line in station.getLines() ){
                                Color c = station_colors[ line % station_colors.Count ];
                                if (!stations.ContainsKey(c)){
                                    stations[c] = new List<GamePoint>();
                                }
                                stations[c].Add(position);
                            }
                            canvas.addBlock(GameCanvas.BlockType.Path, position);
                            break;

                        case BlockType.Normal:
                            canvas.addBlock(GameCanvas.BlockType.Path, position);
                            break;

                        // BlockType.Blocked:
                        case BlockType.Blocked:
                            canvas.addBlock(GameCanvas.BlockType.Blocked, position);
                            break;

                        // BlockType.Special with text:
                        case BlockType.Bank:
                            canvas.addText("Bank "+((Bank)block).Money.ToString(), position);
                            canvas.addBlock(GameCanvas.BlockType.Special, position);
                            break;
                        case BlockType.Hideout:
                            canvas.addText("Hideout", position);
                            canvas.addBlock(GameCanvas.BlockType.Special, position);
                            break;
                        case BlockType.Telegraph:
                            canvas.addText("Telegraph", position);
                            canvas.addBlock(GameCanvas.BlockType.Special, position);
                            break;
                        case BlockType.EscapeAirport:
                            canvas.addText("Airport (Escape)", position);
                            canvas.addBlock(GameCanvas.BlockType.Special, position);
                            break;
                        case BlockType.EscapeCheap:
                            canvas.addText("Escape", position);
                            canvas.addBlock(GameCanvas.BlockType.Special, position);
                            break;

                        // BlockType.Special without text:
                        case BlockType.TravelAgency:
                            canvas.addText("Travel Agency", position);
                            canvas.addBlock(GameCanvas.BlockType.Special, position);
                            break;
                        case BlockType.PoliceStation:
                            canvas.addBlock(GameCanvas.BlockType.Special, position);
                            break;
                    }
                }
            }
            //Dictionary<Color, List<GamePoint>> stations = new Dictionary<Color,List<GamePoint>>();
            foreach (Color c in stations.Keys){
                canvas.addLine(c, stations[c]);
            }
        }
    }
}
