using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessDLC {
    public static partial class ChessBoard {
        public static int WIDTH = 12;
        public static int HEIGHT = 12;
        public static Rect[,] map;
        public static List<Piece> pieceList = new List<Piece>();

        public static Rect GetRect(int x, int y) {
            if (x < 0 || x >= WIDTH) {
                return new Rect(Terrain.Void);
            }
            if (y < 0 || y >= HEIGHT) {
                return new Rect(Terrain.Void);
            }
            return map[x, y];
        }

        public static Rect GetRect((int x, int y) pos) {
            if (pos.x < 0 || pos.x >= WIDTH) {
                return new Rect(Terrain.Void);
            }
            if (pos.y < 0 || pos.y >= HEIGHT) {
                return new Rect(Terrain.Void);
            }
            return map[pos.x, pos.y];
        }

        public static void PieceMove(int fromX, int fromY, int toX, int toY) {
            if (fromX == toX && fromY == toY)
                return;
            if (GetRect(fromX, fromY).piece == null)
                return;
            GetRect(toX, toY).piece = GetRect(fromX, fromY).piece;
            GetRect(toX, toY).piece.position = (toX, toY);
            GetRect(fromX, fromY).piece = null;
        }
        public static void PieceMove(Piece piece, int x, int y) {
            PieceMove(piece.position.x, piece.position.y, x, y);
        }

        public static void PieceMove(Piece piece, (int x, int y) nextPosition) {
            PieceMove(piece.position.x, piece.position.y, nextPosition.x, nextPosition.y);
        }

        public static void PieceSwap(Piece piece1, Piece piece2) {
            if (piece1 == null || piece2 == null) {
                return;
            }
            (int x, int y) pos1 = piece1.position;
            (int x, int y) pos2 = piece2.position;

            GetRect(pos1).piece = piece2;
            GetRect(pos2).piece = piece1;

            GetRect(pos1).piece.position = pos1;
            GetRect(pos2).piece.position = pos2;
        }

        public static void PieceRemove(int x, int y) {
            GetRect(x, y).piece = null;
        }

        public static void PiecePlace(int x, int y, Piece piece) {
            GetRect(x, y).piece = piece;
            piece.position = (x, y);
        }
    }
}
