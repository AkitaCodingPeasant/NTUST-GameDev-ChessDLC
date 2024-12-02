using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessDLC {
    // 職階
    public enum Role {
        Ruler,
        Saber,
        Archer,
        Lancer,
        Rider,
        Caster,
        Assassin,
        Berserker
    }
    // 陣營
    public enum Faction {
        Blue,
        Red
    }
    public enum EffectType {
        QuickStep,
        Invincibility,
        Perseverance,
        Stun,
        Silence,
        Bind,
        Exposed
    }
    public enum TargetType {
        BlankRect,
        Pieces,
        Enemy,
        Ally,
    }
}
