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
        public RulerNormalAttack(Piece skillCaster) : base(skillCaster, 4) {
            name = "";
            skillType = SkillType.Active;
            cooldown = 0;
            positionsNeeded = 1;
        }
        public override void FindValidPosition() {
        }
        public override void Execute() {
        }
    }
    public class RulerSkill : Skill {
        public RulerSkill(Piece skillCaster) : base(skillCaster, 4) {
            name = "";
            skillType = SkillType.Active;
            cooldown = 0;
            positionsNeeded = 10;
        }
        public override void FindValidPosition() {
        }
        public override void Execute() {
            skillCaster.Attack(ChessBoard.GetRect(targetPositions[0]).piece, damage);
        }
    }

    public class RulerPassive : Skill {
        public RulerPassive(Piece skillCaster) : base(skillCaster, 1) {
            name = "";
            skillType = SkillType.Passive;
            cooldown = 0;
            positionsNeeded = 0;
        }
        public override void FindValidPosition() {
        }
        public override void Execute() {
            if (!skillCaster.statusEffect.HasStatusEffect(EffectType.Silence) && !skillCaster.statusEffect.HasStatusEffect(EffectType.Stun)) { return; }
        }
    }

    public class RulerUlt : Skill {
        public RulerUlt(Piece skillCaster) : base(skillCaster, 7) {
            name = "";
            skillType = SkillType.Active;
            cooldown = 0;
            positionsNeeded = 0;
        }
        public override void FindValidPosition() {
        }
        public override void Execute() {
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
