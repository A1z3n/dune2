using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dune2 {
    public class destructableObject : actionBase {
        protected int health;
        protected int fullHealth;
        public bool canAttack = false;
        public float attackRange;
        public int attackDamage;
        public float attackSpeed;
        public float reloadTime;
        public bool isAttacking = false;
        protected Vector2Int tilePos;
        public bool isBuilding = false;
        public int direction = 0;
        protected int player = 0;
        protected SpriteRenderer pickRenderer = null;
        protected Rect clickRect;
        protected Vector3 pos;
        public destructableObject target = null;
        public int bulletNum = 1;

        public int GetPlayer() {
            return player;
        }

        public bool CheckPlayer(int pPlayer) {
            return player == pPlayer;
        }

        public int GetHealth() {
            return health;
        }

        public virtual void Damage(int damage) {

        }

        public Vector2Int GetTilePos() {
            return tilePos;
        }

        public void SetTilePos(int x, int y) {
            tilePos.x = x;
            tilePos.y = y;
        }

        public int GetDirection() {
            return direction;
        }

        public int GetTurnDirection() {
            if (direction > 8)
                return direction - 16;
            return direction;
        }

        public virtual void Select() {
            if (pickRenderer != null)
                pickRenderer.enabled = true;
            //GetComponentInChildren<SpriteRenderer>().enabled = true;
        }

        public virtual void Unselect() {
            if (pickRenderer != null)
                pickRenderer.enabled = false;
        }

        public virtual Vector2 GetTargetPos(Vector2Int from) {
            return pos;
        }
    }


}