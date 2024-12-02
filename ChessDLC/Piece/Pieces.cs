using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessDLC {
    public class Piece {
        // 棋子的屬性
        public Faction faction { get; set; }            // 陣營
        public Role role { get; set; }                  // 職業
        public string name { get; set; }                // 名稱
        public (int x, int y) position { get; set; }    // 位置 (例如棋盤上的座標)
        public int health { get; set; }                 // 當前生命值
        public int maxHealth { get; set; }              // 最大生命值
        public StatusEffect statusEffect { get; set; }  // 狀態效果 (如: 中毒、暈眩等)
        public int level { get; set; }                  // 等級
        public int merit { get; set; }                  // 功勳
        public List<int> meritNeeded { get; set; } = new List<int>();     // 晉升所需功勳
        public string state { get; set; }               // 狀態 (如: 活躍、已死亡)
        public string icon { get; set; }
        public string[] nameOfDiffLv { get; set; }

        public int[] maxHpDiffLv { get; set; }
        public List<Skill> skillTable { get; set; } = new List<Skill>();

        // 建構函式，初始化棋子
        public Piece(Faction faction, Role role, int level, string icon) {
            this.faction = faction;
            this.role = role;
            this.position = (0, 0);
            this.maxHealth = maxHealth;
            this.health = maxHealth;
            this.statusEffect = new StatusEffect();
            this.level = level;
            this.merit = 0;
            this.state = "Active";
            this.icon = icon;
            this.nameOfDiffLv = new string[3] { "nullName lv 1", "nullName lv 2", "nullName lv 3" };
            this.maxHpDiffLv = new int[3] { 10, 20, 30 };
        }

        // 受傷害
        public virtual int TakeDamage(int damage, Piece skillCaster) {
            if (statusEffect.HasStatusEffect(EffectType.Invincibility)) {
                // 執行無敵判定
                ChessBoard.form.Controls[$"SkillInfo"].Text += faction == Faction.Blue ? "藍方" : "紅方";
                ChessBoard.form.Controls[$"SkillInfo"].Text += $" {name} 持有無敵狀態，無視攻擊傷害\n";
                return -1;
            }
            if (statusEffect.HasStatusEffect(EffectType.Exposed)) {
                // 執行破綻判定
                health = 0;
                ChessBoard.form.Controls[$"SkillInfo"].Text += faction == Faction.Blue ? "藍方" : "紅方";
                ChessBoard.form.Controls[$"SkillInfo"].Text += $" {name} 受到破綻傷害，扣除所有生命值\n";
                Console.WriteLine($"{name} 受到 {skillCaster.name} 攻擊");
                Console.WriteLine("受到破綻傷害，棋子已死亡！");
                Dead(damage, skillCaster);
                return 1;
            }
            health -= damage;
            Console.WriteLine($"{name} 受到 {skillCaster.name} 攻擊");
            Console.WriteLine($"受到 {damage} 點傷害，剩餘生命值：{health}");
            ChessBoard.form.Controls[$"SkillInfo"].Text += faction == Faction.Blue ? "藍方" : "紅方";
            ChessBoard.form.Controls[$"SkillInfo"].Text += $" {name} 受到 {damage} 點傷害，剩餘生命值：{health}\n";
            if (health <= 0) {
                health = 0;
                Console.WriteLine("棋子已死亡！");
                Dead(damage, skillCaster);
                return 1;
            }
            return 0;
        }

        // 死亡
        public virtual void Dead(int damage, Piece killer) {
            if (statusEffect.HasStatusEffect(EffectType.Perseverance)) {
                health = 1;
                statusEffect.RemoveStatusEffect(EffectType.Perseverance);
                ChessBoard.form.Controls[$"SkillInfo"].Text += $" {name} 持有毅力狀態，死裡逃生\n";
                return;
            }
            state = "Dead";
            ChessBoard.pieceList.Remove(this);
            ChessBoard.PieceRemove(position.x, position.y);
        }

        // 攻擊
        public void Attack(Piece target, int damage) {
            if (target == null) {
                Console.WriteLine("目標無效！");
                return;
            }
            if (target.faction == faction) {
                Console.WriteLine("目標無效！");
                return;
            }
            if(target.TakeDamage(damage, this) == 1) {
                // 成功擊殺
                GetKill(target);
            }
            Console.WriteLine($"{name} 攻擊了 {target.name}，預計造成 {damage} 點傷害！");
            Console.WriteLine($"{name} 獲得 {damage} 點功勳！");
            AddMerit(damage);
        }

        public virtual void GetKill(Piece target) {

        }

        // 提升功勳
        public void AddMerit(int meritPoints) {
            merit += meritPoints;
            if (level >= meritNeeded.Count) {
                return;
            }
            if (merit >= meritNeeded[level]) {
                merit -= meritNeeded[level];
                level++;
                health += maxHpDiffLv[level - 1] - maxHpDiffLv[level];
                maxHealth = maxHpDiffLv[level];
                Console.WriteLine($"{name} 已經晉升至 LV.{level}");
                Console.WriteLine($"晉升成為 {Getname()}");
                // ChessBoard.form.Controls[$"SkillInfo"].Text += $"晉升成為 {Getname()}\n";
            }
        }

        // 治療
        public void Heal(int healAmount) {
            Heal(healAmount, true);
        }

        public void Heal(int healAmount, bool display) {
            if (state == "Dead") {
                if (display) Console.WriteLine("無法恢復生命值，棋子已死亡！");
                return;
            }
            health += healAmount;
            if (health > maxHealth) {
                health = maxHealth;
            }
            if (display) Console.WriteLine($"棋子恢復了 {healAmount} 點生命值，當前生命值：{health}");
            if (display) ChessBoard.form.Controls[$"SkillInfo"].Text += faction == Faction.Blue ? "藍方" : "紅方";
            if (display) ChessBoard.form.Controls[$"SkillInfo"].Text += $" {name} 恢復 {healAmount} 生命值，當前生命值：{health}/{maxHealth}\n";
        }

        protected string Getname() {
            name = (level >= 0 && level <= 2) ? nameOfDiffLv[level] : nameOfDiffLv[2];
            return name;
        }

        public int GetForeColor() {
            return (int)faction * 3 + level;
        }

        public void CooldownReduce() {
            for (int i = 0; i < skillTable.Count; i++) {
                if (skillTable[i].remainingCooldown >= 1) {
                    skillTable[i].remainingCooldown--;
                }
            }
        }

        // 回合結束
        public virtual void RoundEnd() {
            if (ChessBoard.nowTurnFaction != faction) {
                CooldownReduce();
                Heal(1, false);
            }
            else {
                statusEffect.DurationReduce();
            }
        }
    }
}
