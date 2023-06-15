using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dune2 {
    public class concrete : MonoBehaviour {
        // Start is called before the first frame update

        private RectInt rect;
        private int player;
        private Vector2Int pos;

        public void Init(int x, int y, int w, int h, int pPlayer) {
            rect = new RectInt(x, y, w, h);
            pos = new Vector2Int(x, y);
            this.player = pPlayer;
            transform.position = tools.iPos2PosB(x, y);
        }

        public Vector2Int GetTilePos() {
            return pos;
        }

        public int GetPlayer() {
            return player;
        }

        public RectInt GetRect() {
            return rect;
        }

    }
}