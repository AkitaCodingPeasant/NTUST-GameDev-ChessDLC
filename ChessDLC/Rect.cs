using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessDLC {
    public enum Terrain {
        Ground,
        Wall,
        Void
    }

    public class Rect {
        public Piece piece { get; set; }
        public Terrain terrain { get; set; }

        public Rect() {
            piece = null;
            terrain = Terrain.Ground;
        }
        public Rect(Terrain terrain) {
            piece = null;
            this.terrain = terrain;
        }

        public bool Moveable() {
            if (terrain == Terrain.Wall || terrain == Terrain.Void) {
                return false;
            }
            if (piece != null) {
                return false;
            }
            return true;
        }
    }
}
