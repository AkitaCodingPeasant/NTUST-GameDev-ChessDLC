using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace ChessDLC {
    public class AssassinMovement : Skill {
        public AssassinMovement(Piece skillCaster) : base(skillCaster) {
            name = "移動";
            skillType = SkillType.Active;
            cooldown = 0;
            positionsNeeded = 1;
            describe = "技能描述";
        }
        public override void FindValidPosition() {
            ManhattanPathFinder(2, TargetType.BlankRect, false);
        }
        public override void Execute() {
            ChessBoard.PieceMove(skillCaster, targetPositions[0]);
        }
    }

    public class AssassinNormalAttack : Skill {
        public AssassinNormalAttack(Piece skillCaster) : base(skillCaster, 4) {
            name = "突襲";
            skillType = SkillType.Active;
            cooldown = 1;
            positionsNeeded = 1;
            describe = $"將四方位 2 格內的敵方單體設為目標\n" +
                $"往目標方向移動至目標前 1 格\n" +
                $"對目標單位造成 {damage} 傷害";
        }
        public override void FindValidPosition() {
            CrossPathFinder(2, TargetType.Enemy, false);
        }
        public override void Execute() {
            int Sign(int x) { return (x > 0 ? 1 : (x < 0 ? -1 : 0)); }
            int unitDirX = Sign(targetPositions[0].x - skillCaster.position.x);
            int unitDirY = Sign(targetPositions[0].y - skillCaster.position.y);
            ChessBoard.PieceMove(skillCaster, targetPositions[0].x - unitDirX, targetPositions[0].y - unitDirY);
            Piece enemyPiece = ChessBoard.GetRect(targetPositions[0].x, targetPositions[0].y).piece;
            skillCaster.Attack(enemyPiece, damage);
        }
    }

    public class AssassinSkill : Skill {
        public AssassinSkill(Piece skillCaster) : base(skillCaster, 2) {
            name = "奪命";
            skillType = SkillType.Active;
            cooldown = 5;
            positionsNeeded = 2;
            describe = $"將距離 3 格內的敵方單體設為目標\n" +
                $"對目標造成 {damage} 傷害\n" +
                $"再往敵方四方位 1 格移動\n" +
                $"若目標生命低於 {damage * 2} 造成雙倍傷害" +
                $"\n傷害加成：LV.1 150 % || LV.2 200 % ";
        }
        public override void FindValidPosition() {
            if (targetPositions.Count == 0) {
                ManhattanPathFinder(3, TargetType.Enemy, false);
            }
            else {
                validPosition = Pathfinder.CrossPathFinder(targetPositions[0].x, targetPositions[0].y, 1, TargetType.BlankRect, skillCaster.faction, false);
            }
        }
        public override void Execute() {
            Piece enemyPiece = ChessBoard.GetRect(targetPositions[0]).piece;

            int totalDamage = damage + (damage * skillCaster.level / 2);

            if (enemyPiece.health <= totalDamage * 2) {
                skillCaster.Attack(enemyPiece, totalDamage * 2);
            }
            else {
                skillCaster.Attack(enemyPiece, totalDamage);
            }
            ChessBoard.PieceMove(skillCaster, targetPositions[1]);
        }
    }

    public class AssassinPassive : Skill {
        public AssassinPassive(Piece skillCaster) : base(skillCaster, 2) {
            name = "無蹤";
            skillType = SkillType.Passive;
            cooldown = 0;
            positionsNeeded = 0;
            describe = $"擊殺敵人後 賦予自身疾行 {damage} 回合\n並重製技能 奪命 的冷卻時間";
        }
        public override void FindValidPosition() {
        }
        public override void Execute() {
            if (skillCaster.statusEffect.HasStatusEffect(EffectType.Silence) || skillCaster.statusEffect.HasStatusEffect(EffectType.Stun)) { return; }
            skillCaster.skillTable[2].remainingCooldown = -skillCaster.skillTable[2].cooldown;
            Console.WriteLine($"{skillCaster.skillTable[2].name} 減少冷卻 {skillCaster.skillTable[2].remainingCooldown}");
            skillCaster.statusEffect.AddStatusEffect(EffectType.QuickStep, 2);
        }
    }

    public class AssassinUlt : Skill {
        public AssassinUlt(Piece skillCaster) : base(skillCaster, 5) {
            name = "暗殺步法";
            skillType = SkillType.Active;
            cooldown = 10;
            positionsNeeded = 2;
            describe = $"將距離 15 格內的敵方單體設為目標\n對目標造成 {damage} 傷害\n再往敵方八方位 1 格移動";
        }
        public override void FindValidPosition() {
            if (targetPositions.Count == 0) {
                ManhattanPathFinder(15, TargetType.Enemy, false);
            }
            else {
                int[] dx = { 1, 1, 1, 0, -1, -1, -1, 0 };
                int[] dy = { 1, 0, -1, -1, -1, 0, 1, 1 };

                for (int i = 0; i < 8; i++) {
                    (int x, int y) adjacentPos = (targetPositions[0].x + dx[i], targetPositions[0].y + dy[i]);
                    if (ChessBoard.GetRect(adjacentPos).Moveable()) {
                        validPosition.Add(adjacentPos);
                    }
                }
            }
        }
        public override void Execute() {
            Piece enemyPiece = ChessBoard.GetRect(targetPositions[0]).piece;
            if (enemyPiece != null) {
                skillCaster.Attack(enemyPiece, damage);
            }
            if (targetPositions.Count > 1) {
                ChessBoard.PieceMove(skillCaster, targetPositions[1]);
            }
        }
    }

    public class Assassin : Piece {
        public override void GetKill(Piece target) {
            skillTable[3].Execute();
        }
        public Assassin(Faction faction, int level) : base(faction, Role.Assassin, level, "🗡") {
            nameOfDiffLv = new string[3] { "刺客", "影武者", "緋紅之刃" };
            Getname();
            maxHpDiffLv[0] = 7;
            maxHpDiffLv[1] = 17;
            maxHpDiffLv[2] = 23;

            if (level > 2) { level = 2; }
            health = maxHpDiffLv[level];
            maxHealth = maxHpDiffLv[level];

            meritNeeded.Add(10);
            meritNeeded.Add(25);
            skillTable.Add(new AssassinMovement(this));
            skillTable.Add(new AssassinNormalAttack(this));
            skillTable.Add(new AssassinSkill(this));
            skillTable.Add(new AssassinPassive(this));
            skillTable.Add(new AssassinUlt(this));
        }
    }
}
