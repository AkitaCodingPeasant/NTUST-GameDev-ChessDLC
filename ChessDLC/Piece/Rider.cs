using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace ChessDLC {
    public class RiderMovement : Skill {
        public RiderMovement(Piece skillCaster) : base(skillCaster) {
            name = "移動";
            skillType = SkillType.Active;
            cooldown = 0;
            positionsNeeded = 1;
            describe = "往四方位移動 2 格\n再往另外一座標軸的兩方位移動 1 格";
        }
        public override void FindValidPosition() {
            int[] dx = { 2, 2, -2, -2, 1, -1, 1, -1 };
            int[] dy = { 1, -1, 1, -1, 2, 2, -2, -2 };
            for (int i = 0; i < dx.Length; i++) {
                (int x, int y) pos = (skillCaster.position.x + dx[i], skillCaster.position.y + dy[i]);
                if (ChessBoard.GetRect(pos).Moveable()) {
                    validPosition.Add(pos);
                }
            }
        }
        public override void Execute() {
            ChessBoard.PieceMove(skillCaster, targetPositions[0]);
        }
    }

    public class RiderNormalAttack : Skill {
        public RiderNormalAttack(Piece skillCaster) : base(skillCaster, 5) {
            name = "揮戟";
            skillType = SkillType.Active;
            cooldown = 1;
            positionsNeeded = 1;
            describe = $"對四方位 1 格內敵方單體造成 {damage} 傷害\n" +
                $"再對目標兩側單位造成 {(damage + 1) / 2} 傷害";
        }
        public override void FindValidPosition() {
            CrossPathFinder(1, TargetType.Enemy, false);
        }
        public override void Execute() {
            Piece enemyPiece = ChessBoard.GetRect(targetPositions[0]).piece;
            skillCaster.Attack(enemyPiece, damage);
            int dx = skillCaster.position.y - targetPositions[0].y;
            int dy = skillCaster.position.x - targetPositions[0].x;
            Piece enemyPiece1 = ChessBoard.GetRect(targetPositions[0].x + dx, targetPositions[0].y + dy).piece;
            Piece enemyPiece2 = ChessBoard.GetRect(targetPositions[0].x - dx, targetPositions[0].y - dy).piece;
            skillCaster.Attack(enemyPiece1, (damage + 1) / 2);
            skillCaster.Attack(enemyPiece2, (damage + 1) / 2);
        }
    }

    public class RiderSkill : Skill {
        public RiderSkill(Piece skillCaster) : base(skillCaster, 6) {
            name = "衝鋒";
            skillType = SkillType.Active;
            cooldown = 4;
            positionsNeeded = 1;
            describe = $"將八方位 2~3 格的敵方單體設為目標\n" +
                $"往目標方向移動至目標前 1 格\n" +
                $"對目標單位造成 {damage} 傷害\n" +
                $"並對目標後方單位造成 {(damage + 1) / 2} 傷害" +
                $"但對自身賦予束縛 1 回合"+
                $"\n傷害加成：LV.1 150% || LV.2 200%";
        }
        public override void FindValidPosition() {
            int targetDistance = 3;

            int[] dx = new int[8] { -1, 0, 1, -1, 1, -1, 0, 1 };
            int[] dy = new int[8] { -1, -1, -1, 0, 0, 1, 1, 1 };
            int startX = skillCaster.position.x;
            int startY = skillCaster.position.y;

            for (int i = 0; i < dx.Length; i++) {
                for (int j = 0; j < targetDistance; j++) {
                    int x = startX + dx[i] * (j + 1);
                    int y = startY + dy[i] * (j + 1);

                    if (ChessBoard.GetRect(x, y).piece != null && ChessBoard.GetRect(x, y).piece.faction != skillCaster.faction && j >= 1) {
                        validPosition.Add((x, y));
                        break;
                    }
                    if (!ChessBoard.GetRect(x, y).Moveable()) {
                        break;
                    }
                }
            }
        }
        public override void Execute() {
            int Sign(int x) { return (x > 0 ? 1 : (x < 0 ? -1 : 0)); }
            int unitDirX = Sign(targetPositions[0].x - skillCaster.position.x);
            int unitDirY = Sign(targetPositions[0].y - skillCaster.position.y);
            ChessBoard.PieceMove(skillCaster, targetPositions[0].x - unitDirX, targetPositions[0].y - unitDirY);
            Piece enemyPiece1 = ChessBoard.GetRect(targetPositions[0].x, targetPositions[0].y).piece;
            Piece enemyPiece2 = ChessBoard.GetRect(targetPositions[0].x + unitDirX, targetPositions[0].y + unitDirY).piece;

            int totalDamage = damage + (damage * skillCaster.level / 2);

            skillCaster.Attack(enemyPiece1, totalDamage);
            skillCaster.Attack(enemyPiece2, (totalDamage + 1) / 2);

            skillCaster.statusEffect.AddStatusEffect(EffectType.Bind, 1);
        }
    }

    public class RiderPassive : Skill {
        public RiderPassive(Piece skillCaster) : base(skillCaster, 2) {
            name = "號角";
            skillType = SkillType.Passive;
            cooldown = 0;
            positionsNeeded = 0;
            describe = $"擊殺敵人後\n對八方位 1 格內的友軍全體賦予疾行 {damage} 回合";
        }
        public override void FindValidPosition() {
        }
        public override void Execute() {
            if (skillCaster.statusEffect.HasStatusEffect(EffectType.Silence) || skillCaster.statusEffect.HasStatusEffect(EffectType.Stun)) { return; }
            int[] dx = new int[8] { -1, 0, 1, -1, 1, -1, 0, 1 };
            int[] dy = new int[8] { -1, -1, -1, 0, 0, 1, 1, 1 };
            for (int i = 0; i < dx.Length; i++) {
                Rect rectToDetect = ChessBoard.GetRect(skillCaster.position.x + dx[i], skillCaster.position.y + dy[i]);
                if (rectToDetect.piece != null && skillCaster.faction == rectToDetect.piece.faction) {
                    rectToDetect.piece.statusEffect.AddStatusEffect(EffectType.QuickStep, damage);
                }
            }
            skillCaster.statusEffect.AddStatusEffect(EffectType.QuickStep, damage);
        }
    }

    public class RiderUlt : Skill {
        public RiderUlt(Piece skillCaster) : base(skillCaster, 6) {
            name = "縱橫沙場";
            skillType = SkillType.Active;
            cooldown = 7;
            positionsNeeded = 1;
            describe = $"向四方位移動 6 格\n並對距離路徑 1 格內的敵方全體造成 {damage} 傷害";
        }
        public override void FindValidPosition() {
            CrossPathFinder(6, TargetType.BlankRect, false);
        }
        public override void Execute() {
            int Sign(int x) { return (x > 0 ? 1 : (x < 0 ? -1 : 0)); }
            int unitDirX = Sign(targetPositions[0].x - skillCaster.position.x);
            int unitDirY = Sign(targetPositions[0].y - skillCaster.position.y);

            int[] dx = { 1, -1, 0, 0 };
            int[] dy = { 0, 0, 1, -1 };

            (int x, int y) currentPos = skillCaster.position;

            while (skillCaster.position != targetPositions[0]) {
                // 計算下一步的位置
                currentPos.x += unitDirX;
                currentPos.y += unitDirY;

                // 在棋盤上移動施放者到當前位置
                ChessBoard.PieceMove(skillCaster, currentPos);

                // 對路徑周圍 1 格內的敵方單體造成傷害
                for (int i = 0; i < 4; i++) {
                    (int x, int y) adjacentPos = (currentPos.x + dx[i], currentPos.y + dy[i]);

                    // 確認目標位置上有敵方棋子
                    Piece enemyPiece = ChessBoard.GetRect(adjacentPos).piece;
                    skillCaster.Attack(enemyPiece, damage);
                }
            }
        }
    }

    public class Rider : Piece {
        public override void GetKill(Piece target) {
            skillTable[3].Execute();
        }
        public Rider(Faction faction, int level) : base(faction, Role.Rider, level, "🐴") {
            nameOfDiffLv = new string[3] { "騎兵", "先鋒", "金甲驕雄" };
            Getname();
            maxHpDiffLv[0] = 10;
            maxHpDiffLv[1] = 21;
            maxHpDiffLv[2] = 32;

            if (level > 2) { level = 2; }
            health = maxHpDiffLv[level];
            maxHealth = maxHpDiffLv[level];

            meritNeeded.Add(15);
            meritNeeded.Add(25);
            skillTable.Add(new RiderMovement(this));
            skillTable.Add(new RiderNormalAttack(this));
            skillTable.Add(new RiderSkill(this));
            skillTable.Add(new RiderPassive(this));
            skillTable.Add(new RiderUlt(this));
        }
    }
}
