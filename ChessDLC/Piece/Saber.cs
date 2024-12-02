using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace ChessDLC {
    public class SaberMovement : Skill {
        public SaberMovement(Piece skillCaster) : base(skillCaster) {
            name = "移動";
            skillType = SkillType.Active;
            cooldown = 0;
            positionsNeeded = 1;
            describe = "往距離 2 格內的地塊移動";
        }
        public override void FindValidPosition() {
            ManhattanPathFinder(2, TargetType.BlankRect, false);
        }
        public override void Execute() {
            ChessBoard.PieceMove(skillCaster, targetPositions[0]);
        }
    }

    public class SaberNormalAttack : Skill {
        public SaberNormalAttack(Piece skillCaster) : base(skillCaster, 6) {
            name = "劈斬";
            skillType = SkillType.Active;
            cooldown = 1;
            positionsNeeded = 1;
            describe = $"對距離 1 格內的敵方單體造成 {damage} 傷害";
        }
        public override void FindValidPosition() {
            ManhattanPathFinder(1, TargetType.Enemy, false);
        }
        public override void Execute() {
            skillCaster.Attack(ChessBoard.GetRect(targetPositions[0]).piece, damage);
        }
    }

    public class SaberSkill : Skill {
        (int x, int y) lastPos;
        public SaberSkill(Piece skillCaster) : base(skillCaster, 10) {
            name = "行雲";
            skillType = SkillType.Active;
            cooldown = 4;
            positionsNeeded = 6;
            describe = $"最多鎖定 6 個目標地塊 \n" +
                $"將敵方後方 1 格作為下一個跳躍位置\n" +
                $"每次突進對路徑中間的敵方單體造成 {damage} 點傷害\n" +
                $"連續選取同一地塊 立即完成快速施放";
        }
        public override void FindValidPosition() {
            // 雙擊確認重複施放
            if (targetPositions.Count > 1 && lastPos == targetPositions[targetPositions.Count - 1]) {
                ChessBoard.selectPhase = 10;
                Console.WriteLine($"連續鎖定地塊，快速詠唱");
                return;
            }

            // 計算攻擊目標並更新上次選取的位置 
            if (ChessBoard.selectPhase == 0) {
                lastPos = skillCaster.position;
            }
            if (ChessBoard.selectPhase >= 1) {
                int targetPiecesX = (lastPos.x + targetPositions[targetPositions.Count - 1].x) / 2;
                int targetPiecesY = (lastPos.y + targetPositions[targetPositions.Count - 1].y) / 2;
                targetPieces.Add(ChessBoard.GetRect(targetPiecesX, targetPiecesY).piece);
                Console.WriteLine($"將 {targetPiecesX} {targetPiecesY} 設為攻擊目標");
                Console.WriteLine($"目前有 {targetPieces.Count} 個攻擊目標");
                lastPos = targetPositions[targetPositions.Count - 1];
            }
            Console.WriteLine($"最後位置 {lastPos.x} {lastPos.y}");

            // 找敵人
            validPosition = Pathfinder.ManhattanPathFinder(lastPos.x, lastPos.y, 1, TargetType.Enemy, skillCaster.faction, false);

            // 找敵人往後一格
            for (int i = validPosition.Count - 1; i >= 0; i--) {
                // Console.WriteLine($"找到敵人 {validPosition[i].x} {validPosition[i].y}");
                // 計算敵人後一格
                (int x, int y) nextPos = validPosition[i];
                nextPos = (nextPos.x * 2 - lastPos.x, nextPos.y * 2 - lastPos.y);

                // 判斷該格是否符合 Part 1 敵人被選取過
                if (targetPieces.Contains(ChessBoard.GetRect(validPosition[i]).piece)) {
                    validPosition.RemoveAt(i);
                    continue;
                }
                // 判斷該格是否符合 Part 2 回到原點
                if (nextPos == skillCaster.position) {
                    validPosition[i] = nextPos;
                    continue;
                }
                // 判斷該格是否符合 Part 3 能否移動
                if (ChessBoard.GetRect(nextPos).Moveable()) {
                    validPosition[i] = nextPos;
                }
                else {
                    validPosition.RemoveAt(i);
                    continue;
                }
            }
            // 將自身地塊加入可詠唱
            if (ChessBoard.selectPhase >= 1) {
                validPosition.Add(lastPos);
            }
        }
        public override void Execute() {
            for (int i = 0; i < targetPositions.Count; i++) {
                ChessBoard.PieceMove(skillCaster, targetPositions[i]);
                if (i < targetPieces.Count) {
                    skillCaster.Attack(targetPieces[i], damage);
                }
            }
        }
    }

    public class SaberPassive : Skill {
        public SaberPassive(Piece skillCaster) : base(skillCaster, 2) {
            name = "決心";
            skillType = SkillType.Passive;
            cooldown = 0;
            positionsNeeded = 0;
            describe = $"回合結束時 根據八方位 1 格內敵方單位數量\n恢復自身生命值\n恢復量公式: 敵方單位數量 * {damage}";
        }
        public override void FindValidPosition() {
        }
        public override void Execute() {
            if (!skillCaster.statusEffect.HasStatusEffect(EffectType.Silence) && !skillCaster.statusEffect.HasStatusEffect(EffectType.Stun)) { return; }
            int enemyCount = 0;
            for (int dx = -1; dx < 2; dx++) {
                for (int dy = -1; dy < 2; dy++) {
                    Rect rectToDetect = ChessBoard.GetRect(skillCaster.position.x + dx, skillCaster.position.y + dy);
                    if (rectToDetect.piece != null && skillCaster.faction != rectToDetect.piece.faction) { enemyCount++; }
                }
            }
            if (enemyCount > 0) {
                Console.WriteLine($"Saber 周圍存在 {enemyCount} 位敵人，恢復生命值");
                skillCaster.Heal(enemyCount * damage, false);
            }
            else {
                Console.WriteLine($"Saber 周圍不存在敵人");
            }
        }
    }

    public class SaberUlt : Skill {
        public SaberUlt(Piece skillCaster) : base(skillCaster, 10) {
            name = "劍舞";
            skillType = SkillType.Active;
            cooldown = 6;
            positionsNeeded = 0;
            describe = $"對八方位 1 格內敵方全體造成 {damage} 傷害";
        }
        public override void FindValidPosition() {
        }
        public override void Execute() {
            int enemyCount = 0;
            for (int dx = -1; dx < 2; dx++) {
                for (int dy = -1; dy < 2; dy++) {
                    Rect rectToDetect = ChessBoard.GetRect(skillCaster.position.x + dx, skillCaster.position.y + dy);
                    if (rectToDetect.piece != null && skillCaster.faction != rectToDetect.piece.faction) {
                        enemyCount++;
                        skillCaster.Attack(rectToDetect.piece, damage);
                    }
                }
            }
            if (enemyCount > 0) { }
            else {
                Console.WriteLine($"Saber 周圍不存在敵人");
            }
        }
    }

    public class Saber : Piece {
        public Saber(Faction faction, int level) : base(faction, Role.Saber, level, "⚔️") {
            nameOfDiffLv = new string[3] { "劍士", "浪客", "劍術宗師" };
            Getname();
            maxHpDiffLv[0] = 10;
            maxHpDiffLv[1] = 20;
            maxHpDiffLv[2] = 30;

            if (level > 2) { level = 2; }
            health = maxHpDiffLv[level];
            maxHealth = maxHpDiffLv[level];

            meritNeeded.Add(6);
            meritNeeded.Add(18);
            skillTable.Add(new SaberMovement(this));
            skillTable.Add(new SaberNormalAttack(this));
            skillTable.Add(new SaberSkill(this));
            skillTable.Add(new SaberPassive(this));
            skillTable.Add(new SaberUlt(this));
        }

        public override void RoundEnd() {
            if (ChessBoard.nowTurnFaction != faction) {
                CooldownReduce();
                Heal(1, false);
                skillTable[3].Execute();
            }
            else {
                statusEffect.DurationReduce();
            }
        }
    }
}
