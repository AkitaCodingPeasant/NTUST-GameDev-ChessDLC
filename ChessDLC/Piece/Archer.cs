using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace ChessDLC {
    public class ArcherMovement : Skill {
        public ArcherMovement(Piece skillCaster) : base(skillCaster) {
            name = "移動";
            skillType = SkillType.Active;
            cooldown = 0;
            positionsNeeded = 1;
            describe = "往斜向四方位移動 1 格";
        }
        public override void FindValidPosition() {
            XPathFinder(1, TargetType.BlankRect, false);
        }
        public override void Execute() {
            ChessBoard.PieceMove(skillCaster, targetPositions[0]);
        }
    }

    public class ArcherNormalAttack : Skill {
        public ArcherNormalAttack(Piece skillCaster) : base(skillCaster, 4) {
            name = "射擊";
            skillType = SkillType.Active;
            cooldown = 0;
            positionsNeeded = 1;
            describe = "對四方位距離 4 格內敵方單體造成 4 傷害\n攻擊不可越過敵人";
        }
        public override void FindValidPosition() {
            CrossPathFinder(4, TargetType.Enemy, false);
        }
        public override void Execute() {
            Piece enemyPiece = ChessBoard.GetRect(targetPositions[0]).piece;
            skillCaster.Attack(enemyPiece, damage);

        }
    }

    public class ArcherSkill : Skill {
        public ArcherSkill(Piece skillCaster) : base(skillCaster, 6) {
            name = "鉤爪";
            skillType = SkillType.Active;
            cooldown = 3;
            positionsNeeded = 1;
            describe = $"四方位延伸 {damage} 格內的不可越過地塊為鉤爪目標\n往目標方向移動至目標前 1 格";
        }
        public override void FindValidPosition() {
            int targetDistance = damage;

            int[] dx = { 1, -1, 0, 0 };
            int[] dy = { 0, 0, 1, -1 };
            int startX = skillCaster.position.x;
            int startY = skillCaster.position.y;

            for (int i = 0; i < 4; i++) {
                for (int j = 0; j < targetDistance; j++) {
                    int x = startX + dx[i] * (j + 1);
                    int y = startY + dy[i] * (j + 1);
                    if (!ChessBoard.GetRect(x, y).Moveable()) {
                        validPosition.Add((x - dx[i], y - dy[i]));
                        break;
                    }
                }
            }
        }
        public override void Execute() {
            ChessBoard.PieceMove(skillCaster, targetPositions[0]);
        }
    }

    public class ArcherPassive : Skill {
        public ArcherPassive(Piece skillCaster) : base(skillCaster, 6) {
            name = "後躍射";
            skillType = SkillType.Active;
            cooldown = 0;
            positionsNeeded = 2;
            describe = $"對四方位距離 1 格內敵方單體造成 {damage} 傷害\n並將自身往返方向移動 2 格\n攻擊不可越過敵人";
        }
        public override void FindValidPosition() {
            if (targetPositions.Count == 0) {
                // 第一次判定
                CrossPathFinder(1, TargetType.Enemy, false);
            }
            else {
                // 第二次判定
                int dx = skillCaster.position.x - targetPositions[0].x;
                int dy = skillCaster.position.y - targetPositions[0].y;
                OneWayPathFinder(2, TargetType.BlankRect, false, dx, dy);
            }
        }
        public override void Execute() {
            Piece enemyPiece = ChessBoard.GetRect(targetPositions[0]).piece;
            skillCaster.Attack(enemyPiece, damage);
            if (!skillCaster.statusEffect.HasStatusEffect(EffectType.Bind)) {
                ChessBoard.PieceMove(skillCaster, targetPositions[1]);
            }
        }
    }

    public class ArcherUlt : Skill {
        public ArcherUlt(Piece skillCaster) : base(skillCaster, 7) {
            name = "弱點狙擊";
            skillType = SkillType.Active;
            cooldown = 7;
            positionsNeeded = 1;
            describe = $"對正前方無限距離敵方單體造成 {damage} 點傷害\n若目標生命低於 50% 造成雙倍傷害\n範圍無視牆體和單位";
        }
        public override void FindValidPosition() {
            int dy = (skillCaster.faction == Faction.Blue) ? -1 : 1;
            OneWayPathFinder(20, TargetType.Enemy, true, 0, dy);
        }
        public override void Execute() {
            Piece enemyPiece = ChessBoard.GetRect(targetPositions[0]).piece;
            if (enemyPiece.health * 2 >= enemyPiece.maxHealth) {
                skillCaster.Attack(enemyPiece, damage);
            }
            else {
                skillCaster.Attack(enemyPiece, damage * 2);
            }
        }
    }

    public class Archer : Piece {
        public Archer(Faction faction, int level) : base(faction, Role.Archer, level, "🏹") {
            nameOfDiffLv = new string[3] { "弓兵", "遊俠", "尋蹤猛禽" };
            Getname();
            maxHpDiffLv[0] = 6;
            maxHpDiffLv[1] = 13;
            maxHpDiffLv[2] = 20;

            if (level > 2) { level = 2; }
            health = maxHpDiffLv[level];
            maxHealth = maxHpDiffLv[level];

            meritNeeded.Add(6);
            meritNeeded.Add(16);
            skillTable.Add(new ArcherMovement(this));
            skillTable.Add(new ArcherNormalAttack(this));
            skillTable.Add(new ArcherSkill(this));
            skillTable.Add(new ArcherPassive(this));
            skillTable.Add(new ArcherUlt(this));
        }
    }
}
