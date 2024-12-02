using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Drawing;
using System.Windows.Forms;

namespace ChessDLC {
    public static partial class ChessBoard {

        static Piece lastPieceLocked = null;
        public static void DisplayLockOnPiece(Piece piece) {
            if (lastPieceLocked != null) {
                buttonMatrix[lastPieceLocked.position.x, lastPieceLocked.position.y].FlatAppearance.BorderColor = Color.FromArgb(50, 50, 50);
            }
            if (piece != null) {
                lastPieceLocked = piece;
                (int x, int y) Pos = piece.position;
                buttonMatrix[Pos.x, Pos.y].FlatAppearance.BorderColor = Color.FromArgb(255, 204, 0);
            }
        }
        public static void DisplayValidPosition(Skill skill) {
            (int x, int y) casterPos = skill.skillCaster.position;
            buttonMatrix[casterPos.x, casterPos.y].FlatAppearance.BorderColor = Color.FromArgb(255, 204, 0);
            foreach ((int x, int y) position in skill.validPosition) {
                buttonMatrix[position.x, position.y].BackColor = Color.FromArgb(80, 70, 30);
            }
        }
        public static void RefreshValidPosition(Skill skill) {
            (int x, int y) casterPos = skill.skillCaster.position;
            buttonMatrix[casterPos.x, casterPos.y].FlatAppearance.BorderColor = Color.FromArgb(50, 50, 50);
            foreach ((int x, int y) position in skill.validPosition) {
                buttonMatrix[position.x, position.y].BackColor = Color.Black;
            }
            skill.validPosition.Clear();
        }
        public static void RefreshChessBoardDisplay() {
            for (int i = 0; i < HEIGHT; i++) {
                for (int j = 0; j < WIDTH; j++) {
                    buttonMatrix[j, i].BackColor = (map[j, i].terrain == Terrain.Ground) ? Color.Black : Color.FromArgb(50, 50, 50);
                    if (map[j, i].piece != null) {
                        buttonMatrix[j, i].Text = map[j, i].piece.icon;
                        int colorNum = map[j, i].piece.GetForeColor();
                        buttonMatrix[j, i].ForeColor = PieceColor[colorNum];
                    }
                    else {
                        buttonMatrix[j, i].Text = "";
                    }
                }
            }
        }
    }
}

