using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;

namespace ChessDLC {
    public static partial class ChessBoard {
        public static int turnCount = 1;
        public static int turnPhase = 1;
        public static Faction nowTurnFaction = Faction.Blue;
        public static void CreatePiece(Piece piece, int x, int y) {
            pieceList.Add(piece);
            PiecePlace(x, y, piece);
        }

        public static void nextPhase() {
            turnPhase++;
            if (turnPhase > 2) {
                nextTurn();
            }
            else {
                form.Controls["PhaseText"].Text = $"Phase {turnPhase}";
            }
        }

        public static void nextTurn() {
            turnPhase = 1;
            if (nowTurnFaction == Faction.Blue) {
                nowTurnFaction = Faction.Red;
                form.Controls["TurnText"].Text = "Red";
                form.Controls["TurnText"].ForeColor = Color.Crimson;
            }
            else {
                nowTurnFaction = Faction.Blue;
                turnCount++;
                form.Controls["TurnText"].Text = "BLUE";
                form.Controls["TurnText"].ForeColor = Color.DeepSkyBlue;
            }
            form.Controls["PhaseText"].Text = $"Phase {turnPhase}";
            form.Controls["TurnConutText"].Text = $"Turn {turnCount}";
            for (int i = 0; i < pieceList.Count; i++) {
                pieceList[i].RoundEnd();
            }
            quickStepChecker = false;
        }
        public static void GameEnd(Faction loser) {
            for (int i = pieceList.Count - 1; i >= 0; i--) {
                if (pieceList[i].faction == loser) {
                    (int x, int y) pos = pieceList[i].position;
                    pieceList.Remove(pieceList[i]);
                    PieceRemove(pos.x, pos.y);
                }
            }
        }
    }
}
