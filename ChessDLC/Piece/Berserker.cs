using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace ChessDLC {
    public class BerserkerMovement : Skill {
        public BerserkerMovement(Piece skillCaster) : base(skillCaster) {
            name = "移動";
            skillType = SkillType.Active;
            cooldown = 1;
            positionsNeeded = 1;
            describe = "往八方位移動 1 格";
        }
        public override void FindValidPosition() {
            int[] dx = { -1, 0, 1, -1, 1, -1, 0, 1 };
            int[] dy = { -1, -1, -1, 0, 0, 1, 1, 1 };
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

    public class BerserkerNormalAttack : Skill {
        public BerserkerNormalAttack(Piece skillCaster) : base(skillCaster, 4) {
            name = "撕碎";
            skillType = SkillType.Active;
            cooldown = 1;
            positionsNeeded = 1;
            describe = $"對八方位1格內敵方單體造成 {damage} 傷害";
        }
        public override void FindValidPosition() {
            int[] dx = { -1, 0, 1, -1, 1, -1, 0, 1 };
            int[] dy = { -1, -1, -1, 0, 0, 1, 1, 1 };

            for (int i = 0; i < dx.Length; i++) {
                (int x, int y) pos = (skillCaster.position.x + dx[i], skillCaster.position.y + dy[i]);

                Piece targetPiece = ChessBoard.GetRect(pos).piece;
                if (targetPiece != null && targetPiece.faction != skillCaster.faction) {
                    validPosition.Add(pos);
                }
            }
        }
        public override void Execute() {
            Piece targetPiece = ChessBoard.GetRect(targetPositions[0]).piece;
            skillCaster.Attack(targetPiece, damage);
            if (skillCaster.level >= 1) {
                skillCaster.skillTable[3].Execute();
            }
        }
    }
    public class BerserkerSkill : Skill {
        public BerserkerSkill(Piece skillCaster) : base(skillCaster, 2) {
            name = "戰吼";
            skillType = SkillType.Active;
            cooldown = 5;
            positionsNeeded = 0;
            describe = $"賦予自身毅力 {damage} 回合\n賦予八方位一格內敵方全體沉默 {damage} 回合";
        }
        public override void FindValidPosition() {

        }
        public override void Execute() {
            skillCaster.statusEffect.AddStatusEffect(EffectType.Perseverance, damage);
            int[] dx = { -1, 0, 1, -1, 1, -1, 0, 1 };
            int[] dy = { -1, -1, -1, 0, 0, 1, 1, 1 };

            for (int i = 0; i < dx.Length; i++) {
                (int x, int y) pos = (skillCaster.position.x + dx[i], skillCaster.position.y + dy[i]);

                Piece targetPiece = ChessBoard.GetRect(pos).piece;
                if (targetPiece != null && targetPiece.faction != skillCaster.faction) {
                    targetPiece.statusEffect.AddStatusEffect(EffectType.Silence, damage);
                }
            }
        }
    }

    public class BerserkerPassive : Skill {
        public BerserkerPassive(Piece skillCaster) : base(skillCaster, 3) {
            name = "嗜血";
            skillType = SkillType.Passive;
            cooldown = 0;
            positionsNeeded = 0;
            describe = $"攻擊時 恢復自身 {damage} 生命值";
        }
        public override void FindValidPosition() {
        }
        public override void Execute() {
            if (skillCaster.statusEffect.HasStatusEffect(EffectType.Silence) || skillCaster.statusEffect.HasStatusEffect(EffectType.Stun)) { return; }
            skillCaster.Heal(damage);
        }
    }

    public class BerserkerUlt : Skill {
        public BerserkerUlt(Piece skillCaster) : base(skillCaster, 7) {
            name = "飛身躍擊";
            skillType = SkillType.Active;
            cooldown = 6;
            positionsNeeded = 1;
            describe = $"往距離 2 格內的空地塊移動\n並對原始位置和目標位置距離1的敵方全體造成 {damage} 傷害\n技能可無視障礙物";
        }
        public override void FindValidPosition() {
            ManhattanPathFinder(2, TargetType.BlankRect, true);
        }
        public override void Execute() {
            (int x, int y) originalPos = skillCaster.position;

            (int x, int y) targetPos = targetPositions[0];
            ChessBoard.PieceMove(skillCaster, targetPos);

            int[] dx = { 1, -1, 0, 0 };
            int[] dy = { 0, 0, 1, -1 };

            for (int i = 0; i < dx.Length; i++) {
                // 檢查原始位置距離 1 格內的敵人
                (int x, int y) adjacentPosOriginal = (originalPos.x + dx[i], originalPos.y + dy[i]);
                Piece enemyOriginal = ChessBoard.GetRect(adjacentPosOriginal).piece;
                skillCaster.Attack(enemyOriginal, damage);

                if (enemyOriginal != null && enemyOriginal.faction != skillCaster.faction && skillCaster.level >= 1) {
                    skillCaster.skillTable[3].Execute();
                }

                // 檢查目標位置距離 1 格內的敵人
                (int x, int y) adjacentPosTarget = (targetPos.x + dx[i], targetPos.y + dy[i]);
                Piece enemyTarget = ChessBoard.GetRect(adjacentPosTarget).piece;
                skillCaster.Attack(enemyTarget, damage);

                if (enemyTarget != null && enemyTarget.faction != skillCaster.faction && skillCaster.level >= 1) {
                    skillCaster.skillTable[3].Execute();
                }
            }
        }
    }

    public class Berserker : Piece {
        public Berserker(Faction faction, int level) : base(faction, Role.Berserker, level, "🔪") {
            nameOfDiffLv = new string[3] { "莽夫", "狂戰士", "嗜血惡煞" };
            Getname();
            maxHpDiffLv[0] = 13;
            maxHpDiffLv[1] = 27;
            maxHpDiffLv[2] = 40;

            if (level > 2) { level = 2; }
            health = maxHpDiffLv[level];
            maxHealth = maxHpDiffLv[level];

            meritNeeded.Add(12);
            meritNeeded.Add(18);
            skillTable.Add(new BerserkerMovement(this));
            skillTable.Add(new BerserkerNormalAttack(this));
            skillTable.Add(new BerserkerSkill(this));
            skillTable.Add(new BerserkerPassive(this));
            skillTable.Add(new BerserkerUlt(this));
        }
    }
}
