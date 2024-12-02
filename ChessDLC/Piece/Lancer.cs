using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace ChessDLC {
    public class LancerMovement : Skill {
        public LancerMovement(Piece skillCaster) : base(skillCaster) {
            name = "移動";
            skillType = SkillType.Active;
            cooldown = 0;
            positionsNeeded = 1;
            describe = "往距離2格內的地塊移動";
        }
        public override void FindValidPosition() {
            ManhattanPathFinder(1, TargetType.BlankRect, false);
        }
        public override void Execute() {
            ChessBoard.PieceMove(skillCaster, targetPositions[0]);
        }
    }

    public class LancerNormalAttack : Skill {
        public LancerNormalAttack(Piece skillCaster) : base(skillCaster, 6) {
            name = "戳刺";
            skillType = SkillType.Active;
            cooldown = 1;
            positionsNeeded = 1;
            describe = $"對自身斜前方 1 格內的敵方單體造成 {damage} 傷害";
        }
        public override void FindValidPosition() {
            int[] dxBlue = new int[2] { -1, 1 };
            int[] dyBlue = new int[2] { -1, -1 };

            int[] dxRed = new int[2] { -1, 1, };
            int[] dyRed = new int[2] { 1, 1, };

            for (int i = 0; i < 2; i++) {
                int[] dx = skillCaster.faction == Faction.Blue ? dxBlue : dxRed;
                int[] dy = skillCaster.faction == Faction.Blue ? dyBlue : dyRed;
                (int x, int y) pos = (skillCaster.position.x + dx[i], skillCaster.position.y + dy[i]);
                if (ChessBoard.GetRect(pos).piece != null && ChessBoard.GetRect(pos).piece.faction != skillCaster.faction) {
                    validPosition.Add(pos);
                }
            }
        }
        public override void Execute() {
            skillCaster.Attack(ChessBoard.GetRect(targetPositions[0]).piece, damage);
        }
    }
    public class LancerSkill : Skill {
        public LancerSkill(Piece skillCaster) : base(skillCaster, 8) {
            name = "守望";
            skillType = SkillType.Active;
            cooldown = 3;
            positionsNeeded = 1;
            describe = $"賦予距離1格內的友方單體無敵1回合\n並治療 {damage} 生命值";
        }
        public override void FindValidPosition() {
            ManhattanPathFinder(1, TargetType.Ally, false);
        }
        public override void Execute() {
            ChessBoard.GetRect(targetPositions[0]).piece.statusEffect.AddStatusEffect(EffectType.Invincibility, damage);
            ChessBoard.GetRect(targetPositions[0]).piece.Heal(damage);
        }
    }

    public class LancerPassive : Skill {
        public LancerPassive(Piece skillCaster) : base(skillCaster, 12) {
            name = "榮譽";
            skillType = SkillType.Passive;
            cooldown = 0;
            positionsNeeded = 0;
            describe = $"死亡時 賦予九宮格內友方全體毅力 3 回合\n並治療 {damage} 生命值";
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
                    rectToDetect.piece.statusEffect.AddStatusEffect(EffectType.Perseverance, 3);
                    rectToDetect.piece.Heal(damage);
                }
            }
        }
    }

    public class LancerUlt : Skill {
        public LancerUlt(Piece skillCaster) : base(skillCaster, 7) {
            name = "最終防線";
            skillType = SkillType.Active;
            cooldown = 15;
            positionsNeeded = 1;
            describe = "賦予垂直座標於自身前方兩格內的敵軍全體\n破綻 1 回合與束縛 2 回合";
        }
        public override void FindValidPosition() {
            validPosition.Add(skillCaster.position);
            for (int i = 1; i <= 2; i++) {
                for (int j = 0; j < ChessBoard.WIDTH; j++) {
                    int dy = i * ((skillCaster.faction == Faction.Red) ? 1 : -1);
                    if (ChessBoard.GetRect(j, skillCaster.position.y + dy).terrain == Terrain.Ground) {
                        validPosition.Add((j, skillCaster.position.y + dy));
                    }
                    Piece targetP = ChessBoard.GetRect(j, skillCaster.position.y + dy).piece;
                    if (targetP != null && targetP.faction != skillCaster.faction) {
                        targetPieces.Add(targetP);
                    }
                }
            }
        }
        public override void Execute() {
            for (int i = 0; i < targetPieces.Count; i++) {
                targetPieces[i].statusEffect.AddStatusEffect(EffectType.Exposed, 1);
                targetPieces[i].statusEffect.AddStatusEffect(EffectType.Bind, 2);
            }
        }
    }

    public class Lancer : Piece {
        public override void Dead(int damage, Piece killer) {
            if (statusEffect.HasStatusEffect(EffectType.Perseverance)) {
                health = 1;
                statusEffect.RemoveStatusEffect(EffectType.Perseverance);
                ChessBoard.form.Controls[$"SkillInfo"].Text += $" {name} 持有毅力狀態，死裡逃生\n";
                return;
            }
            ChessBoard.form.Controls[$"SkillInfo"].Text += $" {name} 死前發動了被動 榮譽\n";
            skillTable[3].Execute();

            state = "Dead";
            ChessBoard.pieceList.Remove(this);
            ChessBoard.PieceRemove(position.x, position.y);
        }
        public Lancer(Faction faction, int level) : base(faction, Role.Lancer, level, "🔱") {
            nameOfDiffLv = new string[3] { "槍兵", "守望者", "禁衛勇將" };
            Getname();
            maxHpDiffLv[0] = 12;
            maxHpDiffLv[1] = 25;
            maxHpDiffLv[2] = 35;

            if (level > 2) { level = 2; }
            health = maxHpDiffLv[level];
            maxHealth = maxHpDiffLv[level];

            meritNeeded.Add(6);
            meritNeeded.Add(12);
            skillTable.Add(new LancerMovement(this));
            skillTable.Add(new LancerNormalAttack(this));
            skillTable.Add(new LancerSkill(this));
            skillTable.Add(new LancerPassive(this));
            skillTable.Add(new LancerUlt(this));
        }
    }
}
