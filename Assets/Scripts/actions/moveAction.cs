using System.Collections;
using System.Collections.Generic;
using Dune2;
using UnityEngine;

namespace Dune2 {
    public class moveAction : action {
        private Vector2 startPos;
        private Vector2 destPos;
        private int destX;
        private int destY;
        private int startX;
        private int startY;
        private float timer = 0.0f;
        private float duration;
        private bool inited = false;
        private destructableObject target;

        public void Init(int x, int y) {
            duration = 1.0f;
            destX = x;
            destY = y;
            destPos = tools.iPos2Pos(x, y);
        }

        public void InitToTarget(destructableObject pTarget, int x, int y) {
            duration = 1.0f;
            destX = x;
            destY = y;
            destPos = tools.iPos2Pos(destX, destY);
            this.target = pTarget;
        }

        public override bool Update(actionBase u, float dt) {
            if (!inited) {
                inited = true;
                startPos = u.transform.position;
                var obj = u as destructableObject;
                startX = obj.GetTilePos().x;
                startY = obj.GetTilePos().y;
                obj.SetTilePos(destX, destY);
                astar.GetInstance().ChangeUnitPos(startX, startY, destX, destY);
                var un = u as unit;
                if (un != null) {
                    duration = 1 / un.GetMoveSpeed();
                    un.PositionChanged();
                }
            }

            if (cancel) {
                return false;
            }

            timer += dt;
            if (timer >= duration) {
                u.transform.position = destPos;

                var un = u as unit;
                if (un != null) {
                    gameManager.GetInstance().GetMapManager().CheckSpiceMines(destX, destY);
                    if (un.destPos.x == destX && un.destPos.y == destY) {
                        return false;
                    }

                    if (target != null) {
                        if (tools.IsInAttackRange(un, target)) {
                            gameManager.GetInstance().GetUnitManager().Attack(un, target, true);
                            return false;
                        }

                        actionManager.MoveToTarget(un, target);
                        return false;
                    }

                    actionManager.moveToPoint(un, un.destPos.x, un.destPos.y);
                    return false;

                }

                //OnEndCallback();
                return false;
            }

            float p = timer / duration;
            u.transform.position = startPos + (destPos - startPos) * p;
            return true;
        }

        public float GetProgress() {
            return timer / duration;
        }

        public override eActionType GetActionType() {
            return eActionType.kMoveAction;
        }
    }
}
