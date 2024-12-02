using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;
using System.Threading;

namespace ChessDLC {
    public static partial class ChessBoard {
        public static int selectPhase = 0;
        public static (int x, int y) selectedPosition;
        public static Skill skillAiming;
        public static Piece pieceLocked;
        private static bool quickStepChecker = false;

        private static void LockOnPiece(Piece piece, bool LockOrPeek) {
            if (piece == null) {
                LockOnNullPiece(true);
            }
            if (LockOrPeek) {
                pieceLocked = piece;
                DisplayLockOnPiece(piece);
            }
            // 角色資訊展示
            form.Controls["characterText"].Text = $"{piece.icon} {piece.name} {piece.icon}";
            form.Controls["characterText"].ForeColor = PieceColor[piece.GetForeColor()];
            string meritNeededString = (piece.level >= piece.meritNeeded.Count) ? "--" : piece.meritNeeded[piece.level].ToString("D2");
            form.Controls["characterInfoLeft"].Text =
                $"Health: {piece.health:D2} / {piece.maxHealth:D2}\n" +
                $"Merit : {piece.merit:D2} / {meritNeededString}\n";
            form.Controls["characterInfoRight"].Text = $"Role: {piece.role}\nLv  : {piece.level}\n";
            // 狀態效果展示
            form.Controls["characterStatusEffectInfo"].Text = piece.statusEffect.GetDisplayString();
            // 技能展示
            for (int i = 1; i <= 4; i++) {
                if (i > piece.skillTable.Count + 1) { break; }
                string cooldownText = piece.skillTable[i].remainingCooldown == 0 ?
                                      piece.skillTable[i].skillType.ToString() :
                                      $"CD: {piece.skillTable[i].remainingCooldown:D2}";
                cooldownText = piece.level + 2 >= i ? cooldownText : "Locked";
                cooldownText = (!piece.statusEffect.HasStatusEffect(EffectType.Silence)) ? cooldownText : "Silence";
                cooldownText = (!piece.statusEffect.HasStatusEffect(EffectType.Stun)) ? cooldownText : "Stun";
                form.Controls[$"SkillButton{i}"].Text = piece.skillTable[i].name + "\n" + cooldownText;
                form.Controls[$"SkillButton{i}"].Tag = (cooldownText == "Active") ? $"Active,{i}" : $"none,{i}";
                form.Controls[$"SkillButton{i}"].ForeColor = (cooldownText == "Active" && piece.faction == ChessBoard.nowTurnFaction) ? Color.FromArgb(140, 120, 30) : Color.Gray;
            }
        }

        private static void LockOnNullPiece(bool refreshSkillInfo) {
            DisplayLockOnPiece(null);
            pieceLocked = null;
            // 角色資訊展示
            form.Controls["characterText"].Text = $"角色名稱";
            form.Controls["characterText"].ForeColor = Color.Gray;

            form.Controls["characterInfoLeft"].Text = $"Health: -- / --\nMerit : -- / --\n";
            form.Controls["characterInfoRight"].Text = $"Role: --\nLv  : --\n";
            // 狀態效果展示
            form.Controls["characterStatusEffectInfo"].Text = "🥾:- 🛡:- 🗿:- ⚡:- ⏳:- 🕸:- ⚠️:-";
            // 技能展示
            string[] skillTypeName = new string[4] { "普攻", "技能", "被動", "奧義" };
            for (int i = 1; i <= 4; i++) {
                form.Controls[$"SkillButton{i}"].Text = $"{skillTypeName[i - 1]}\n--";
                form.Controls[$"SkillButton{i}"].Tag = "none";
                form.Controls[$"SkillButton{i}"].ForeColor = Color.Gray;
            }
            // 技能描述
            if (refreshSkillInfo) {
                form.Controls[$"SkillInfo"].Text = "";
            }
        }

        private static void SkillDescribeDisplay() {
            form.Controls["skillInfo"].Text = "";
            if (skillAiming != null) {
                // 如果已選取技能，顯示該技能
                SkillDescribeDisplay(skillAiming);
            }
            else if (pieceLocked != null && pieceLocked.skillTable.Count >= 1) {
                // 如果未選取技能，顯示移動
                SkillDescribeDisplay(pieceLocked.skillTable[0]);
                if (pieceLocked.faction == nowTurnFaction) {
                    if (pieceLocked.statusEffect.HasStatusEffect(EffectType.Stun)) { return; }
                    if (pieceLocked.statusEffect.HasStatusEffect(EffectType.Bind)) {
                        form.Controls["skillInfo"].Text += "\n遭到束縛，無法移動";
                        return;
                    }
                    pieceLocked.skillTable[0].UseSkill();
                }
            }
        }

        private static void SkillDescribeDisplay(Skill skill) {
            form.Controls["skillInfo"].Text = "";
            if (skill == null) { return; }
            string text = "";
            string cd = (skill.cooldown <= 0) ? "None" : $"{skill.cooldown}";
            text += skill.skillCaster.icon;
            text += $" {skill.name} || {skill.skillType} || Cooldown : {cd} ";
            text += skill.skillCaster.icon + "\n";
            text += skill.describe;

            form.Controls["skillInfo"].Text = text;
        }

        private static void LowerFrameDisplayPiece(Piece piece) {
            string text = "";
            text += $"{piece.icon} {piece.name} {piece.icon}\n";
            text += $"Faction : {piece.faction}\n";
            text += $"Role : {piece.role}\n ";
            text += $"Level : {piece.level}\n";
            text += $"Health : {piece.health:D2} / {piece.maxHealth:D2}\n";
            string meritNeededString = (piece.level >= piece.meritNeeded.Count) ? "--" : piece.meritNeeded[piece.level].ToString("D2");
            text += $"Merit : {piece.merit:D2} / {meritNeededString}\n";
            text += piece.statusEffect.GetDisplayString();
            form.Controls["skillInfo"].Text = text;
        }

        private static void RectBtnMouseDown(object sender, MouseEventArgs e) {
            // 處理鎖定地塊，將 tag 分割成座標
            Button hoveredButton = sender as Button;
            string[] parts = hoveredButton.Tag.ToString().Split(',');
            int x = int.Parse(parts[0]);
            int y = int.Parse(parts[1]);
            Console.WriteLine($"\n鎖定地塊 x: {x}, y: {y}");

            if (skillAiming == null || !skillAiming.validPosition.Exists(pos => pos.Equals((x, y)))) {
                // 判斷無效選取，重新鎖定新目標
                Console.WriteLine($"判斷無效選取，重新鎖定新目標");
                selectPhase = 0;
                if (skillAiming != null) {
                    RefreshValidPosition(skillAiming);
                    skillAiming.skillInitialize();
                    skillAiming = null;
                }
                Console.WriteLine($"這是新指令");
                if (map[x, y].piece != null) {
                    Console.WriteLine($"這是一名: {map[x, y].piece.role}");
                    // 選取角色
                    LockOnPiece(map[x, y].piece, true);

                    // 實作技能展示
                    SkillDescribeDisplay();

                    // map[x, y].piece.skillTable[0].UseSkill();
                }
                else {
                    Console.WriteLine($"這是空地塊");
                    LockOnNullPiece(true);
                }
                return;
            }
            selectPhase++;
            Console.WriteLine($"這是技能 {skillAiming.name} 的第 {selectPhase} 次詠唱");
            skillAiming.targetPositions.Add((x, y));
            // 需要多次詠唱的技能
            if (selectPhase > 0 && selectPhase != skillAiming.positionsNeeded && skillAiming.positionsNeeded != 0) {
                Console.WriteLine($"繼續詠唱");
                RefreshValidPosition(skillAiming);
                skillAiming.FindValidPosition();
                DisplayValidPosition(skillAiming);
            }
            // 鎖定地塊數量足夠 或 無須詠唱的技能 施放技能
            if (selectPhase >= skillAiming.positionsNeeded || skillAiming.positionsNeeded == 0) {
                Console.WriteLine($"確認施放");
                RefreshValidPosition(skillAiming);
                form.Controls[$"SkillInfo"].Text = "";
                skillAiming.Execute();

                // 施放完成，進行初始化
                RefreshChessBoardDisplay();
                skillAiming.skillInitialize();
                skillAiming.EnterCooldown();

                // 判定疾行技能
                if (skillAiming.QuickStepCheck() && quickStepChecker == false) {
                    // 不消耗回合階段
                    quickStepChecker = true;
                    skillAiming.FindValidPosition();
                    DisplayValidPosition(skillAiming);
                    selectPhase = 0;
                    return;
                }

                nextPhase();
                skillAiming = null;
                LockOnNullPiece(false);
                selectPhase = 0;
                return;
            }
        }

        private static void RectBtnMouseEnter(object sender, EventArgs e) {
            Button hoveredButton = sender as Button;
            string[] parts = hoveredButton.Tag.ToString().Split(',');
            int x = int.Parse(parts[0]);
            int y = int.Parse(parts[1]);

            Piece piece = map[x, y].piece;
            if (pieceLocked != null && (piece == null || pieceLocked == piece)) {
                // case 1 有已選棋子 且當前地塊不存在棋子
                // case 2 有已選棋子 且與當前地塊上的棋子相同
                // 重新展示已選棋子技能
                SkillDescribeDisplay();

                return;
            }
            if (pieceLocked == null && piece != null) {
                // 沒有已選棋子 且當前地塊存在棋子
                // 在大格顯示棋子資訊
                LockOnPiece(map[x, y].piece, false);

                return;
            }
            if (piece == null && pieceLocked == null) {
                LockOnNullPiece(true);
                form.Controls["skillInfo"].Text = "";
                return;
            }
            if (piece == null) {
                form.Controls["skillInfo"].Text = "";
                return;
            }
            // 角色資訊下方展示
            LowerFrameDisplayPiece(piece);
        }

        private static void SkillBtnMouseDown(object sender, MouseEventArgs e) {
            if (pieceLocked == null) { return; }
            // 分割tag
            Button hoveredButton = sender as Button;
            string[] parts = hoveredButton.Tag.ToString().Split(',');
            int tagPart2 = int.Parse(parts[1]);

            if (parts[0] == "Active" && pieceLocked.faction == nowTurnFaction) {
                // 若是可使用技能則使用
                if (skillAiming != null) {
                    // 取消以選取技能
                    RefreshChessBoardDisplay();
                    skillAiming.skillInitialize();
                }
                selectPhase = 0;
                skillAiming = pieceLocked.skillTable[tagPart2];
                SkillDescribeDisplay();
                skillAiming.UseSkill();
                return;
            }
            else {
                // 若不是可使用的技能則展示
                SkillDescribeDisplay(pieceLocked.skillTable[tagPart2]);
                return;
            }

        }

        private static void skillCancellBtmMouseDown(object sender, MouseEventArgs e) {
            selectPhase = 0;
            if (skillAiming != null) {
                // 取消已選取技能
                RefreshChessBoardDisplay();
                skillAiming.skillInitialize();
            }
        }

        private static void SkipPhaseBtmMouseDown(object sender, MouseEventArgs e) {
            // 跳過回合
            selectPhase = 0;
            nextTurn();
            if (skillAiming != null) {
                // 取消已選取技能
                RefreshChessBoardDisplay();
                skillAiming.skillInitialize();
            }
            LockOnNullPiece(true);
        }
    }
}
