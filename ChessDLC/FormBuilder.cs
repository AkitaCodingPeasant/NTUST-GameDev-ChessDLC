using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Drawing;
using System.Windows.Forms;

namespace ChessDLC {
    public static partial class ChessBoard {
        static public Button[,] buttonMatrix;
        static public Form form;

        static readonly Color[] PieceColor = new Color[]
        {
            Color.SkyBlue,
            Color.CornflowerBlue,
            Color.DeepSkyBlue,
            Color.LightCoral,
            Color.FromArgb(255,90,90),
            Color.Crimson
        };
        public static void BuildForm(Form inputForm) {
            form = inputForm;
            form.BackColor = Color.Black;
            BuildChessBoardDisplayFrame(form);
            BuildOtherDisplayFrame(form);
        }
        public static void BuildChessBoardDisplayFrame(Form form) {

            for (int i = 0; i < HEIGHT; i++) {
                for (int j = 0; j < WIDTH; j++) {
                    map[j, i] = new Rect(Terrain.Ground);
                }
            }

            int buttonHeight = 640 / HEIGHT;
            int buttonWidth = 640 / WIDTH;
            int LocationX = 20;
            int LocationY = 20;
            buttonMatrix = new Button[WIDTH, HEIGHT];

            for (int i = 0; i < HEIGHT; i++) {
                for (int j = 0; j < WIDTH; j++) {
                    buttonMatrix[j, i] = new Button();
                    buttonMatrix[j, i].Location = new Point(LocationX, LocationY);              // 位置
                    buttonMatrix[j, i].Size = new Size(buttonWidth, buttonHeight);              // 大小
                    buttonMatrix[j, i].Tag = j + "," + i;                                       // 標記座標
                    buttonMatrix[j, i].BackColor = Color.Black;                                 // 背景顏色
                    buttonMatrix[j, i].Text = "";                                               // 文字內容
                    buttonMatrix[j, i].FlatStyle = FlatStyle.Flat;
                    buttonMatrix[j, i].FlatAppearance.BorderColor = Color.FromArgb(50, 50, 50); // 邊框顏色
                    buttonMatrix[j, i].FlatAppearance.BorderSize = 3;                           // 邊框粗細
                    buttonMatrix[j, i].FlatAppearance.MouseOverBackColor = Color.Gray;          // 滑鼠懸停背景色
                    buttonMatrix[j, i].Font = new Font("Impact", 16);                           // 字體與大小
                    buttonMatrix[j, i].TextAlign = ContentAlignment.MiddleCenter;               // 置中

                    buttonMatrix[j, i].MouseDown += RectBtnMouseDown;
                    buttonMatrix[j, i].MouseEnter += RectBtnMouseEnter;
                    form.Controls.Add(buttonMatrix[j, i]);

                    LocationX += buttonWidth;
                }
                LocationY += buttonHeight;
                LocationX = 20;
            }
            Console.WriteLine("Chess Board Build Done");
        }

        public static void BuildOtherDisplayFrame(Form form) {
            // 回合顯示器
            Label TurnText = new Label() {
                Name = "TurnText",
                Text = "BLUE",
                Font = new Font("Impact", 40),
                ForeColor = Color.DeepSkyBlue,
                Size = new Size(240, 70),
                Location = new Point(690, 25),
                TextAlign = ContentAlignment.MiddleRight,
                BackColor = Color.Transparent
            };
            form.Controls.Add(TurnText);

            // 回合階段顯示器
            Label PhaseText = new Label() {
                Name = "PhaseText",
                Text = $"Phase {1}",
                Font = new Font("Impact", 15),
                ForeColor = Color.Gray,
                Size = new Size(240, 35),
                Location = new Point(930, 30),
                TextAlign = ContentAlignment.MiddleLeft,
                BackColor = Color.Transparent
            };
            form.Controls.Add(PhaseText);

            // 回合數顯示器
            Label TurnCountText = new Label() {
                Name = "TurnConutText",
                Text = "Turn 1",
                Font = new Font("Impact", 15),
                ForeColor = Color.Gray,
                Size = new Size(240, 35),
                Location = new Point(930, 55),
                TextAlign = ContentAlignment.MiddleLeft,
                BackColor = Color.Transparent
            };
            form.Controls.Add(TurnCountText);

            // 回合顯示框
            Label TurnFrame = new Label() {
                Size = new Size(500, 90),
                Location = new Point(680, 15),
                TextAlign = ContentAlignment.MiddleCenter,
            };
            TurnFrame.Paint += Label_Paint;
            form.Controls.Add(TurnFrame);

            // 四個角色技能
            Button[] SkillButton;
            SkillButton = new Button[4];
            string[] skillTypeName = new string[4] { "普攻", "技能", "被動", "奧義" };
            for (int i = 0; i < 4; i++) {
                SkillButton[i] = new Button();
                SkillButton[i].Name = $"SkillButton{i + 1}";
                SkillButton[i].Text = $"{skillTypeName[i]}\n--";
                SkillButton[i].Tag = "none";
                SkillButton[i].Font = new Font("Microsoft JhengHei", 15, FontStyle.Bold);
                SkillButton[i].ForeColor = Color.Gray;
                SkillButton[i].Location = new System.Drawing.Point(695 + 120 * i, 300);
                SkillButton[i].Size = new System.Drawing.Size(110, 80);
                SkillButton[i].FlatStyle = FlatStyle.Flat;
                SkillButton[i].Enabled = true;
                SkillButton[i].FlatAppearance.BorderColor = Color.FromArgb(50, 50, 50);
                SkillButton[i].FlatAppearance.BorderSize = 5;
                SkillButton[i].TextAlign = ContentAlignment.MiddleCenter;
                SkillButton[i].MouseDown += SkillBtnMouseDown;
                form.Controls.Add(SkillButton[i]);
            };

            // 角色名稱
            Label characterText = new Label() {
                Name = "characterText",
                Text = "角色名稱",
                Font = new Font("Microsoft JhengHei", 20, FontStyle.Bold),
                ForeColor = Color.DeepSkyBlue,
                Location = new Point(690, 130),
                Size = new Size(480, 40),
                TextAlign = ContentAlignment.MiddleCenter,
                BackColor = Color.Transparent
            };
            form.Controls.Add(characterText);

            // 角色詳細資訊 左
            Label characterInfoLeft = new Label() {
                Name = "characterInfoLeft",
                Text = "Health: -- / --\nMerit : -- / --\n",
                Font = new Font("consolas", 15, FontStyle.Bold),
                ForeColor = Color.Gray,
                Location = new Point(710, 170),
                Size = new Size(220, 50),
                TextAlign = ContentAlignment.TopLeft,
                BackColor = Color.Transparent
            };
            form.Controls.Add(characterInfoLeft);

            // 角色詳細資訊 右
            Label characterInfoRight = new Label() {
                Name = "characterInfoRight",
                Text = "Role: --\nLv  : -\n",
                Font = new Font("consolas", 15, FontStyle.Bold),
                ForeColor = Color.Gray,
                Location = new Point(930, 170),
                Size = new Size(220, 50),
                TextAlign = ContentAlignment.TopLeft,
                BackColor = Color.Transparent
            };
            form.Controls.Add(characterInfoRight);

            // 角色狀態效果
            Label characterStatusEffectInfo = new Label() {
                Name = "characterStatusEffectInfo",
                Text = "🥾:- 🛡:- 🗿:- ⚡:- ⏳:- 🕸:- ⚠️:-",
                Font = new Font("consolas", 15, FontStyle.Bold),
                ForeColor = Color.Gray,
                Location = new Point(710, 225),
                Size = new Size(440, 30),
                TextAlign = ContentAlignment.MiddleCenter,
                BackColor = Color.Transparent
            };
            form.Controls.Add(characterStatusEffectInfo);

            // 技能資訊 / 下方顯示器
            Label skillInfo = new Label() {
                Name = "skillInfo",
                Location = new Point(710, 405),
                Size = new Size(440, 170),
                ForeColor = Color.Gray,
                Font = new Font("Microsoft JhengHei", 14, FontStyle.Bold),
                TextAlign = ContentAlignment.TopLeft,
            };
            form.Controls.Add(skillInfo);

            // 技能資訊框 內部
            Label skillInfoFrame = new Label() {
                Location = new Point(695, 390),
                Size = new Size(470, 200),
            };
            skillInfoFrame.Paint += Label_Paint;
            form.Controls.Add(skillInfoFrame);

            // 角色資訊框
            Label characterFrame = new Label() {
                Location = new Point(680, 120),
                Size = new Size(500, 150),
                TextAlign = ContentAlignment.MiddleCenter,
            };
            characterFrame.Paint += Label_Paint;
            form.Controls.Add(characterFrame);

            // 跳過回合按鈕
            Button SkipPhaseButton = new Button() {
                Name = "SkipPhaseButton",
                Text = "跳過回合",
                Font = new Font("Microsoft JhengHei", 20, FontStyle.Bold),
                ForeColor = Color.Gray,
                Location = new System.Drawing.Point(935, 600),
                Size = new System.Drawing.Size(230, 50),
                FlatStyle = FlatStyle.Flat,
                Enabled = true
            };
            SkipPhaseButton.FlatAppearance.BorderColor = Color.FromArgb(50, 50, 50);
            SkipPhaseButton.FlatAppearance.BorderSize = 5;
            SkipPhaseButton.MouseDown += SkipPhaseBtmMouseDown;
            form.Controls.Add(SkipPhaseButton);

            // 技能取消按鈕
            Button skillCancellButton = new Button() {
                Name = "skillCancellButton",
                Text = "取消技能",
                Font = new Font("Microsoft JhengHei", 20, FontStyle.Bold),
                ForeColor = Color.Gray,
                Location = new System.Drawing.Point(695, 600),
                Size = new System.Drawing.Size(230, 50),
                FlatStyle = FlatStyle.Flat,
                Enabled = true
            };
            skillCancellButton.FlatAppearance.BorderColor = Color.FromArgb(50, 50, 50);
            skillCancellButton.FlatAppearance.BorderSize = 5;
            skillCancellButton.MouseDown += skillCancellBtmMouseDown;
            form.Controls.Add(skillCancellButton);

            // 技能展示框
            Label skillFrame = new Label() {
                Location = new Point(680, 285),
                Size = new Size(500, 380),
                TextAlign = ContentAlignment.MiddleCenter,
            };
            skillFrame.Paint += Label_Paint;
            form.Controls.Add(skillFrame);

            // 棋盤框
            Button chessBoardFrame = new Button() {
                Size = new System.Drawing.Size(650, 650),
                Location = new System.Drawing.Point(15, 15),
                FlatStyle = FlatStyle.Flat
            };
            chessBoardFrame.FlatAppearance.BorderColor = Color.FromArgb(50, 50, 50);
            chessBoardFrame.FlatAppearance.BorderSize = 10;
            chessBoardFrame.Enabled = false;
            form.Controls.Add(chessBoardFrame);

            Console.WriteLine("Other Diplayer Frame Build Done");
        }

        private static void Label_Paint(object sender, PaintEventArgs e) {
            Label label = sender as Label;
            if (label == null) return;

            // 設置邊框顏色和粗細
            using (Pen borderPen = new Pen(Color.FromArgb(50, 50, 50), 12)) // 設置紅色邊框，寬度為3
            {
                Rectangle rect = new Rectangle(0, 0, label.Width - 1, label.Height - 1);
                e.Graphics.DrawRectangle(borderPen, rect);
            }
        }
    }
}

