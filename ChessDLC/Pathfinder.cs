using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessDLC {
    class Pathfinder {
        /// <summary>
        /// 深度優先搜索，返回所有找到的目標
        /// </summary>
        public static List<(int, int)> ManhattanPathFinder(int startX, int startY, int targetDistance, TargetType targetType, Faction casterFaction, bool ignoresObstacles) {
            // 記錄已訪問的節點
            HashSet<(int, int)> visited = new HashSet<(int, int)>();
            List<(int, int)> target = new List<(int, int)>();

            // 定義方向 (上、下、左、右)
            int[] dx = { 0, 0, -1, 1 };
            int[] dy = { -1, 1, 0, 0 };

            // 遞歸函式
            void Search(int x, int y) {
                int currentDistance = Math.Abs(startX - x) + Math.Abs(startY - y);

                if (currentDistance > targetDistance || ChessBoard.GetRect(x, y).terrain == Terrain.Void)
                    return;

                visited.Add((x, y));

                if (ChessBoard.GetRect(x, y).piece != null) {
                    if (targetType == TargetType.Pieces) {
                        target.Add((x, y));
                    }
                    if (targetType == TargetType.Ally
                        && ChessBoard.GetRect(x, y).piece.faction == casterFaction) {
                        target.Add((x, y));
                    }
                    if (targetType == TargetType.Enemy
                        && ChessBoard.GetRect(x, y).piece.faction != casterFaction) {
                        target.Add((x, y));
                    }
                }
                if (targetType == TargetType.BlankRect && ChessBoard.GetRect(x, y).Moveable())
                    target.Add((x, y));

                if (!ChessBoard.GetRect(x, y).Moveable() && !ignoresObstacles && !(x == startX && y == startY))
                    return;

                for (int i = 0; i < 4; i++) {
                    int nx = x + dx[i];
                    int ny = y + dy[i];
                    // 確保未訪問過
                    if (!visited.Contains((nx, ny))) {
                        Search(nx, ny);
                    }
                }

            }
            // 啟動搜索
            Search(startX, startY);

            return target;
        }

        public static int ManhattanDistance(int x1, int y1, int x2, int y2) {
            return Math.Abs(x1 - x2) + Math.Abs(y1 - y2);
        }

        public static List<(int, int)> CrossPathFinder(int startX, int startY, int targetDistance, TargetType targetType, Faction casterFaction, bool ignoresObstacles) {
            List<(int, int)> target = new List<(int, int)>();

            int[] dx = { 1, -1, 0, 0 };
            int[] dy = { 0, 0, 1, -1 };

            for (int i = 0; i < 4; i++) {
                for (int j = 0; j < targetDistance; j++) {
                    int x = startX + dx[i] * (j + 1);
                    int y = startY + dy[i] * (j + 1);
                    if (ChessBoard.GetRect(x, y).terrain == Terrain.Void) { break; }

                    if (ChessBoard.GetRect(x, y).piece != null) {
                        if (targetType == TargetType.Pieces) { target.Add((x, y)); }
                        if (targetType == TargetType.Ally && ChessBoard.GetRect(x, y).piece.faction == casterFaction) { target.Add((x, y)); }
                        if (targetType == TargetType.Enemy && ChessBoard.GetRect(x, y).piece.faction != casterFaction) { target.Add((x, y)); }
                        if (targetType == TargetType.BlankRect && ChessBoard.GetRect(x, y).Moveable()) { target.Add((x, y)); }
                    }
                    if (targetType == TargetType.BlankRect && ChessBoard.GetRect(x, y).Moveable()) { target.Add((x, y)); }
                    if (!ChessBoard.GetRect(x, y).Moveable() && !ignoresObstacles && !(x == startX && y == startY)) { break; }

                }
            }
            return target;
        }

        public static List<(int, int)> XPathFinder(int startX, int startY, int targetDistance, TargetType targetType, Faction casterFaction, bool ignoresObstacles) {
            List<(int, int)> target = new List<(int, int)>();

            int[] dx = { 1, -1, 1, -1 };
            int[] dy = { 1, 1, -1, -1 };

            for (int i = 0; i < 4; i++) {
                for (int j = 0; j < targetDistance; j++) {
                    int x = startX + dx[i] * (j + 1);
                    int y = startY + dy[i] * (j + 1);
                    if (ChessBoard.GetRect(x, y).terrain == Terrain.Void) { break; }

                    if (ChessBoard.GetRect(x, y).piece != null) {
                        if (targetType == TargetType.Pieces) { target.Add((x, y)); }
                        if (targetType == TargetType.Ally && ChessBoard.GetRect(x, y).piece.faction == casterFaction) { target.Add((x, y)); }
                        if (targetType == TargetType.Enemy && ChessBoard.GetRect(x, y).piece.faction != casterFaction) { target.Add((x, y)); }
                        if (targetType == TargetType.BlankRect && ChessBoard.GetRect(x, y).Moveable()) { target.Add((x, y)); }
                    }
                    if (targetType == TargetType.BlankRect && ChessBoard.GetRect(x, y).Moveable()) { target.Add((x, y)); }
                    if (!ChessBoard.GetRect(x, y).Moveable() && !ignoresObstacles && !(x == startX && y == startY)) { break; }

                }
            }
            return target;
        }

        public static List<(int, int)> OneWayPathFinder(int startX, int startY, int dx, int dy, int targetDistance, TargetType targetType, Faction casterFaction, bool ignoresObstacles) {
            List<(int, int)> target = new List<(int, int)>();

            for (int j = 0; j < targetDistance; j++) {
                int x = startX + dx * (j + 1);
                int y = startY + dy * (j + 1);
                if (ChessBoard.GetRect(x, y).terrain == Terrain.Void) { break; }

                if (ChessBoard.GetRect(x, y).piece != null) {
                    if (targetType == TargetType.Pieces) { target.Add((x, y)); }
                    if (targetType == TargetType.Ally && ChessBoard.GetRect(x, y).piece.faction == casterFaction) { target.Add((x, y)); }
                    if (targetType == TargetType.Enemy && ChessBoard.GetRect(x, y).piece.faction != casterFaction) { target.Add((x, y)); }
                    if (targetType == TargetType.BlankRect && ChessBoard.GetRect(x, y).Moveable()) { target.Add((x, y)); }
                }
                if (targetType == TargetType.BlankRect && ChessBoard.GetRect(x, y).Moveable()) { target.Add((x, y)); }
                if (!ChessBoard.GetRect(x, y).Moveable() && !ignoresObstacles) { break; }
            }
            return target;
        }
    }
}
