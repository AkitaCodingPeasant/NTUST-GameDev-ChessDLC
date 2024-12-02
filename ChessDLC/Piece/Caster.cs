using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace ChessDLC {
    public class CasterMovement : Skill {
        public CasterMovement(Piece skillCaster) : base(skillCaster) {
            name = "移動";
            skillType = SkillType.Active;
            cooldown = 0;
            positionsNeeded = 1;
            describe = "往斜向四方位移動1格";
        }
        public override void FindValidPosition() {
            XPathFinder(1, TargetType.BlankRect, false);
        }
        public override void Execute() {
            ChessBoard.PieceMove(skillCaster, targetPositions[0]);
        }
    }

    public class CasterNormalAttack : Skill {
        public CasterNormalAttack(Piece skillCaster) : base(skillCaster, 3) {
            name = "波動";
            skillType = SkillType.Active;
            cooldown = 1;
            positionsNeeded = 1;
            describe = $"對距離 3 格內的敵方單體造成 {damage} 傷害\n" +
                $"技能可無視障礙物";
        }
        public override void FindValidPosition() {
            if (skillCaster.level >= 1) {
                ManhattanPathFinder(3, TargetType.Pieces, true);
            }
            else {
                ManhattanPathFinder(3, TargetType.Enemy, true);
            }
        }
        public override void Execute() {
            Piece targetPiece = ChessBoard.GetRect(targetPositions[0]).piece;
            if (targetPiece.faction == skillCaster.faction) {
                targetPiece.Heal(damage);
            }
            else {
                skillCaster.Attack(targetPiece, damage);
            }
        }
    }

    public class CasterSkill : Skill {
        public CasterSkill(Piece skillCaster) : base(skillCaster, 5) {
            name = "折耀";
            skillType = SkillType.Active;
            cooldown = 6;
            positionsNeeded = 1;
            describe = $"和距離 {damage} 格內的單位交換位置";
        }
        public override void FindValidPosition() {
            ManhattanPathFinder(damage, TargetType.Pieces, true);
        }
        public override void Execute() {
            Piece targetPiece = ChessBoard.GetRect(targetPositions[0]).piece;
            if (skillCaster.level >= 1) {
                if (targetPiece.faction == skillCaster.faction) {
                    targetPiece.statusEffect.AddStatusEffect(EffectType.Invincibility, 1);
                }
                else {
                    targetPiece.statusEffect.AddStatusEffect(EffectType.Bind, 2);
                }
            }
            ChessBoard.PieceSwap(skillCaster, targetPiece);
        }
    }

    public class CasterPassive : Skill {
        public CasterPassive(Piece skillCaster) : base(skillCaster, 1) {
            name = "餘波";
            skillType = SkillType.Passive;
            cooldown = 0;
            positionsNeeded = 0;
            describe = "普通攻擊可以選取友方單體 恢復3生命\n對友方單位施放折耀後 賦予無敵 1 回合\n對敵方單位施放折耀後 賦予沉默 2 回合";
        }
        public override void FindValidPosition() {
        }
        public override void Execute() {
            if (skillCaster.statusEffect.HasStatusEffect(EffectType.Silence) || skillCaster.statusEffect.HasStatusEffect(EffectType.Stun)) { return; }
        }
    }

    public class CasterUlt : Skill {
        public CasterUlt(Piece skillCaster) : base(skillCaster, 4) {
            name = "流星雨";
            skillType = SkillType.Active;
            cooldown = 10;
            positionsNeeded = 3;
            describe = $"召喚流星對距離 5 格內敵方單體造成 {damage} 傷害\n" +
                $"總共召喚 3 顆流星 可針對重複目標\n" +
                $"技能可無視障礙物";
        }
        public override void FindValidPosition() {
            ManhattanPathFinder(5, TargetType.Enemy, true);
        }
        public override void Execute() {
            for (int i = 0; i < targetPositions.Count; i++) {
                Piece enemyPiece = ChessBoard.GetRect(targetPositions[0]).piece;
                skillCaster.Attack(enemyPiece, damage);
            }
        }
    }

    public class Caster : Piece {
        public Caster(Faction faction, int level) : base(faction, Role.Caster, level, "🔮") {
            nameOfDiffLv = new string[3] { "學者", "觀星術者", "星夜魔導" };
            Getname();
            maxHpDiffLv[0] = 6;
            maxHpDiffLv[1] = 15;
            maxHpDiffLv[2] = 23;

            if (level > 2) { level = 2; }
            health = maxHpDiffLv[level];
            maxHealth = maxHpDiffLv[level];

            meritNeeded.Add(6);
            meritNeeded.Add(15);
            skillTable.Add(new CasterMovement(this));
            skillTable.Add(new CasterNormalAttack(this));
            skillTable.Add(new CasterSkill(this));
            skillTable.Add(new CasterPassive(this));
            skillTable.Add(new CasterUlt(this));
        }
    }
}
