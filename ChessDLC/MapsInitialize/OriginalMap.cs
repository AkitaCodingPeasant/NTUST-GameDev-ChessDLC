using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessDLC {
    class OriginalMap : MapInitialize {

        public override void GetMapSize() {
            ChessBoard.WIDTH = 13;
            ChessBoard.HEIGHT = 13;
        }

        int[,] mapMatrix = {
            {2,2},
            {5,3},
            {6,3},
            {7,3},
            {10,2},

            {0,6},
            {3,6},
            {9,6},
            {12,6},

            {2,10},
            {5,9},
            {6,9},
            {7,9},
            {10,10},
        };

        public override void CreateTerrain() {
            for (int i = 0; i < mapMatrix.GetLength(0); i++) {
                ChessBoard.map[mapMatrix[i, 0], mapMatrix[i, 1]].terrain = Terrain.Wall;
            }
        }

        public override void CreatePiece() {
            // Red
            CreatePiece(new Ruler(Faction.Red, 2), 6, 0);
            CreatePiece(new Saber(Faction.Red, 2), 3, 0);
            CreatePiece(new Archer(Faction.Red, 2), 4, 0);
            CreatePiece(new Lancer(Faction.Red, 2), 5, 0);
            CreatePiece(new Caster(Faction.Red, 2), 7, 0);
            CreatePiece(new Rider(Faction.Red, 2), 8, 0);
            CreatePiece(new Assassin(Faction.Red, 2), 9, 0);
            CreatePiece(new Berserker(Faction.Red, 2), 6, 2);

            CreatePiece(new Archer(Faction.Red, 1), 0, 0);
            CreatePiece(new Rider(Faction.Red, 1), 1, 0);
            CreatePiece(new Caster(Faction.Red, 1), 2, 0);
            CreatePiece(new Archer(Faction.Red, 1), 12, 0);
            CreatePiece(new Rider(Faction.Red, 1), 11, 0);
            CreatePiece(new Caster(Faction.Red, 1), 10, 0);
            
            CreatePiece(new Saber(Faction.Red, 1), 3, 2);
            CreatePiece(new Assassin(Faction.Red, 1), 4, 2);
            CreatePiece(new Lancer(Faction.Red, 1), 5, 2);

            CreatePiece(new Saber(Faction.Red, 1), 9, 2);
            CreatePiece(new Assassin(Faction.Red, 1), 8, 2);
            CreatePiece(new Lancer(Faction.Red, 1), 7, 2);

            CreatePiece(new Saber(Faction.Red, 0), 0, 1);
            CreatePiece(new Archer(Faction.Red, 0), 1, 1);
            CreatePiece(new Lancer(Faction.Red, 0), 2, 1);

            CreatePiece(new Caster(Faction.Red, 0), 10, 1);
            CreatePiece(new Rider(Faction.Red, 0), 11, 1);
            CreatePiece(new Assassin(Faction.Red, 0), 12, 1);

            CreatePiece(new Berserker(Faction.Red, 0), 8, 3);
            CreatePiece(new Caster(Faction.Red, 0), 9, 3);
            CreatePiece(new Rider(Faction.Red, 0), 10, 3);
            CreatePiece(new Assassin(Faction.Red, 0), 11, 3);
            CreatePiece(new Berserker(Faction.Red, 1), 12, 3);

            CreatePiece(new Berserker(Faction.Red, 1), 0, 3);
            CreatePiece(new Saber(Faction.Red, 0), 1, 3);
            CreatePiece(new Archer(Faction.Red, 0), 2, 3);
            CreatePiece(new Lancer(Faction.Red, 0), 3, 3);
            CreatePiece(new Berserker(Faction.Red, 0), 4, 3);

            // Blue
            CreatePiece(new Saber(Faction.Blue, 2), 9, 12);
            CreatePiece(new Archer(Faction.Blue, 2), 8, 12);
            CreatePiece(new Lancer(Faction.Blue, 2), 7, 12);
            CreatePiece(new Ruler(Faction.Blue, 2), 6, 12);
            CreatePiece(new Caster(Faction.Blue, 2), 5, 12);
            CreatePiece(new Rider(Faction.Blue, 2), 4, 12);
            CreatePiece(new Assassin(Faction.Blue, 2), 3, 12);
            CreatePiece(new Berserker(Faction.Blue, 2), 6, 10);

            CreatePiece(new Archer(Faction.Blue, 1), 12, 12);
            CreatePiece(new Rider(Faction.Blue, 1), 11, 12);
            CreatePiece(new Caster(Faction.Blue, 1), 10, 12);
            CreatePiece(new Archer(Faction.Blue, 1), 0, 12);
            CreatePiece(new Rider(Faction.Blue, 1), 1, 12);
            CreatePiece(new Caster(Faction.Blue, 1), 2, 12);

            CreatePiece(new Saber(Faction.Blue, 1), 9, 10);
            CreatePiece(new Assassin(Faction.Blue, 1), 8, 10);
            CreatePiece(new Lancer(Faction.Blue, 1), 7, 10);

            CreatePiece(new Saber(Faction.Blue, 1), 3, 10);
            CreatePiece(new Assassin(Faction.Blue, 1), 4, 10);
            CreatePiece(new Lancer(Faction.Blue, 1), 5, 10);

            CreatePiece(new Saber(Faction.Blue, 0), 12, 11);
            CreatePiece(new Archer(Faction.Blue, 0), 11, 11);
            CreatePiece(new Lancer(Faction.Blue, 0), 10, 11);

            CreatePiece(new Caster(Faction.Blue, 0), 2, 11);
            CreatePiece(new Rider(Faction.Blue, 0), 1, 11);
            CreatePiece(new Assassin(Faction.Blue, 0), 0, 11);

            CreatePiece(new Berserker(Faction.Blue, 0), 4, 9);
            CreatePiece(new Caster(Faction.Blue, 0), 3, 9);
            CreatePiece(new Rider(Faction.Blue, 0), 2, 9);
            CreatePiece(new Assassin(Faction.Blue, 0), 1, 9);
            CreatePiece(new Berserker(Faction.Blue, 1), 0, 9);

            CreatePiece(new Berserker(Faction.Blue, 1), 12, 9);
            CreatePiece(new Saber(Faction.Blue, 0), 11, 9);
            CreatePiece(new Archer(Faction.Blue, 0), 10, 9);
            CreatePiece(new Lancer(Faction.Blue, 0), 9, 9);
            CreatePiece(new Berserker(Faction.Blue, 0), 8, 9);
        }

        public override void CreatePiece(Piece piece, int x, int y) {
            ChessBoard.pieceList.Add(piece);
            ChessBoard.PiecePlace(x, y, piece);
        }
    }
}
