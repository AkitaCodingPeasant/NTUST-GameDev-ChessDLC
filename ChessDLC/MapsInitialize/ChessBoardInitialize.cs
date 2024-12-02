using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessDLC {
    public static partial class ChessBoard {

        private static MapInitialize mapInitialize = new OriginalMap();
        public static void Initialize() {
            mapInitialize.GetMapSize();
            map = new Rect[WIDTH, HEIGHT];
            for (int i = 0; i < HEIGHT; i++) {
                for (int j = 0; j < WIDTH; j++) {
                    map[j, i] = new Rect(Terrain.Ground);
                }
            }
        }

        public static void GenerateChessBoard() {
            mapInitialize.CreateTerrain();
            mapInitialize.CreatePiece();
            RefreshChessBoardDisplay();
        }
    }

    class MapInitialize {
        public MapInitialize() { }

        public virtual void GetMapSize() {
            ChessBoard.WIDTH = 12;
            ChessBoard.HEIGHT = 12;
        }

        public virtual void CreateTerrain() {
            ChessBoard.map[10, 10].terrain = Terrain.Wall;
        }

        public virtual void CreatePiece() {
            CreatePiece(new Saber(Faction.Blue, 2), 2, 10);
            CreatePiece(new Lancer(Faction.Blue, 2), 4, 10);
            CreatePiece(new Archer(Faction.Blue, 2), 6, 10);
            CreatePiece(new Rider(Faction.Blue, 2), 8, 10);
            CreatePiece(new Caster(Faction.Blue, 2), 2, 8);
            CreatePiece(new Assassin(Faction.Blue, 2), 4, 8);
            CreatePiece(new Berserker(Faction.Blue, 2), 6, 8);
            CreatePiece(new Ruler(Faction.Blue, 2), 8, 8);

            CreatePiece(new Saber(Faction.Red, 2), 2, 1);
            CreatePiece(new Lancer(Faction.Red, 2), 4, 1);
            CreatePiece(new Archer(Faction.Red, 2), 6, 1);
            CreatePiece(new Rider(Faction.Red, 2), 8, 1);
            CreatePiece(new Caster(Faction.Red, 2), 2, 3);
            CreatePiece(new Assassin(Faction.Red, 2), 4, 3);
            CreatePiece(new Berserker(Faction.Red, 2), 6, 3);
            CreatePiece(new Ruler(Faction.Red, 2), 8, 3);
        }

        public virtual void CreatePiece(Piece piece, int x, int y) {
            ChessBoard.pieceList.Add(piece);
            ChessBoard.PiecePlace(x, y, piece);
        }
    }
}
