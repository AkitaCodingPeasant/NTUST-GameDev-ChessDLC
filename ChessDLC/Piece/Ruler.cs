using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace ChessDLC {
    public class RulerMovement : Skill {
        public RulerMovement(Piece skillCaster) : base(skillCaster) {
            name = "移動";
            skillType = SkillType.Active;
            cooldown = 0;
            positionsNeeded = 1;
        }
        public override void FindValidPosition() {
            int[] dx = new int[8] { -1, 0, 1, -1, 1, -1, 0, 1 };
            int[] dy = new int[8] { -1, -1, -1, 0, 0, 1, 1, 1 };
            for (int i = 0; i < dx.Length; i++) {
                int x = skillCaster.position.x + dx[i];
                int y = skillCaster.position.y + dy[i];
                Rect rectToDetect = ChessBoard.GetRect(x, y);
                if (rectToDetect.Moveable()) {
                    validPosition.Add((x, y));
                }
            }
        }
        public override void Execute() {
            ChessBoard.PieceMove(skillCaster, targetPositions[0]);
        }
    }

    public class RulerNormalAttack : Skill {
        public RulerNormalAttack(Piece skillCaster) : base(skillCaster, 1) {
            name = "看破";
            skillType = SkillType.Active;
            cooldown = 1;
            positionsNeeded = 1;
            describe = $"對距離 1 格內敵方單體賦予破綻 {damage} 回合";
        }
        public override void FindValidPosition() {
            ManhattanPathFinder(1, TargetType.Enemy, false);
        }
        public override void Execute() {
            ChessBoard.GetRect(targetPositions[0]).piece.statusEffect.AddStatusEffect(EffectType.Exposed, damage);
        }
    }
    public class RulerSkill : Skill {
        public RulerSkill(Piece skillCaster) : base(skillCaster, 99) {
            name = "冊命";
            skillType = SkillType.Active;
            cooldown = 10;
            positionsNeeded = 1;
            describe = $"賦予距離 1 格內友方單體無敵 2 回合\n並回復 {damage} 生命值";
        }
        public override void FindValidPosition() {
        }
        public override void Execute() {
            ChessBoard.GetRect(targetPositions[0]).piece.statusEffect.AddStatusEffect(EffectType.Invincibility, 2);
            ChessBoard.GetRect(targetPositions[0]).piece.Heal(damage);
        }
    }

    public class RulerPassive : Skill {
        public RulerPassive(Piece skillCaster) : base(skillCaster, 1) {
            name = "激昂";
            skillType = SkillType.Active;
            cooldown = 1;
            positionsNeeded = 0;
            describe = $"賦予八方位1格內友方全體疾行 {damage} 回合";
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

    public class RulerUlt : Skill {
        public RulerUlt(Piece skillCaster) : base(skillCaster, 3) {
            name = "聖劍出鞘";
            skillType = SkillType.Active;
            cooldown = 99;
            positionsNeeded = 1;
            describe = $"對距離 {damage} 格內全體敵人造成當前生命值 50% 傷害";
        }
        public override void FindValidPosition() {
            ManhattanPathFinder(3, TargetType.Everything, true);
        }
        public override void Execute() {
            targetPositions.Clear();
            targetPositions = Pathfinder.ManhattanPathFinder(skillCaster.position.x, skillCaster.position.y, damage, TargetType.Enemy, skillCaster.faction, true);
            for (int i = 0; i < targetPositions.Count; i++) {
                Piece enemyPiece = ChessBoard.GetRect(targetPositions[i]).piece;
                skillCaster.Attack(enemyPiece, enemyPiece.health / 2);
            }
        }
    }

    public class Ruler : Piece {
        public override void Dead(int damage, Piece killer) {
            if (statusEffect.HasStatusEffect(EffectType.Perseverance)) {
                health = 1;
                statusEffect.RemoveStatusEffect(EffectType.Perseverance);
                ChessBoard.form.Controls[$"SkillInfo"].Text += $" {name} 持有毅力狀態，死裡逃生\n";
                return;
            }
            state = "Dead";
            ChessBoard.pieceList.Remove(this);
            ChessBoard.PieceRemove(position.x, position.y);
            ChessBoard.GameEnd(faction);
        }
        public Ruler(Faction faction, int level) : base(faction, Role.Ruler, level, "👑") {
            nameOfDiffLv = new string[3] { "君王", "君王", "君王" };
            Getname();
            maxHpDiffLv[0] = 30;
            maxHpDiffLv[1] = 30;
            maxHpDiffLv[2] = 30;

            if (level > 2) { level = 2; }
            health = maxHpDiffLv[level];
            maxHealth = maxHpDiffLv[level];

            skillTable.Add(new RulerMovement(this));
            skillTable.Add(new RulerNormalAttack(this));
            skillTable.Add(new RulerSkill(this));
            skillTable.Add(new RulerPassive(this));
            skillTable.Add(new RulerUlt(this));
        }
    }
}
