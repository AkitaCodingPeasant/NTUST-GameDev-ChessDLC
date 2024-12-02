using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ChessDLC {
    public enum SkillType {
        Active,
        Passive
    }
    public class Skill {
        public string name { get; set; }
        public SkillType skillType { get; set; }
        public int remainingCooldown { get; set; }
        public int cooldown { get; set; }
        public int positionsNeeded { get; set; }
        public Piece skillCaster { get; set; }
        public List<(int x, int y)> validPosition { get; set; } = new List<(int, int)>();
        public List<(int x, int y)> targetPositions { get; set; } = new List<(int, int)>();
        public List<Piece> targetPieces { get; set; }
        public int damage { get; set; }
        public string describe { get; set; }

        public Skill(Piece skillCaster) {
            this.skillCaster = skillCaster;
            positionsNeeded = 0;
            remainingCooldown = 0;
            targetPieces = new List<Piece>();
            damage = 1;
            describe = "尚未撰寫技能敘述";
        }

        public Skill(Piece skillCaster, int damage) : this(skillCaster) {
            this.damage = damage;
        }

        public bool IsCooldownComplete() {
            return remainingCooldown == 0;
        }

        // 使用技能
        public virtual int UseSkill() {
            Console.WriteLine($"\n嘗試使用 {name} 技能");
            // 判斷 Part 1 被動技能無法主動發動
            if (skillType == SkillType.Passive) {
                Console.WriteLine($"{name} 被動技能無法主動發動");
                return -2;
            }
            // 判斷 Part 2 技能冷卻中
            if (!IsCooldownComplete()) {
                Console.WriteLine($"{name} 技能冷卻中");
                return -1;
            }
            // 情況一 需要詠唱 鎖定地塊
            if (positionsNeeded != 0) {
                FindValidPosition();
                Console.WriteLine();
                ChessBoard.DisplayValidPosition(this);
                ChooseTargetPosition();
            }
            // 情況二 不須詠唱 鎖定自身地塊以確認
            else {
                validPosition.Add(skillCaster.position);
                Console.WriteLine();
                Console.WriteLine($"鎖定自身地塊 {skillCaster.position.x} , {skillCaster.position.y} 以確認並施放技能");
                ChessBoard.DisplayValidPosition(this);
                ChooseTargetPosition();
            }
            return 0;
        }

        public virtual bool QuickStepCheck() {
            if (name == "移動" && skillCaster.statusEffect.HasStatusEffect(EffectType.QuickStep)) {
                return true;
            }
            else { return false; }
        }

        public virtual void skillInitialize() {
            validPosition.Clear();
            targetPositions.Clear();
            targetPieces.Clear();
        }
        public virtual void FindValidPosition() { }

        // 將 skillAiming 設為此技能
        public virtual void ChooseTargetPosition() {
            Console.WriteLine("等待使用者下一次目標位置...");
            ChessBoard.skillAiming = this;
        }
        public virtual void Execute() { }

        public virtual void EnterCooldown() {
            remainingCooldown += cooldown;
        }

        public void ManhattanPathFinder(int targetDistance, TargetType targetType, bool ignoresObstacles) {
            validPosition = Pathfinder.ManhattanPathFinder(skillCaster.position.x, skillCaster.position.y, targetDistance, targetType, skillCaster.faction, ignoresObstacles);
        }

        public void CrossPathFinder(int targetDistance, TargetType targetType, bool ignoresObstacles) {
            validPosition = Pathfinder.CrossPathFinder(skillCaster.position.x, skillCaster.position.y, targetDistance, targetType, skillCaster.faction, ignoresObstacles);
        }

        public void XPathFinder(int targetDistance, TargetType targetType, bool ignoresObstacles) {
            validPosition = Pathfinder.XPathFinder(skillCaster.position.x, skillCaster.position.y, targetDistance, targetType, skillCaster.faction, ignoresObstacles);
        }

        public void OneWayPathFinder(int targetDistance, TargetType targetType, bool ignoresObstacles, int dx, int dy) {
            validPosition = Pathfinder.OneWayPathFinder(skillCaster.position.x, skillCaster.position.y, dx, dy, targetDistance, targetType, skillCaster.faction, ignoresObstacles);
        }
    }
}
