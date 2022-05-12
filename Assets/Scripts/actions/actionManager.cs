using System.Collections;
using System.Collections.Generic;
using Assets.Scripts;
using UnityEngine;

namespace Assets.Scripts {
    static class actionManager {
        public static bool moveToPoint(unit u, int startX, int startY, int destX, int destY) {
            Vector2Int start = u.getTilePos();
            Vector2Int dest = new Vector2Int(destX, destY);
            if (destX == start.x && destY == start.y) {
                return false;
            }

            Vector2Int next = new Vector2Int();
            if (astar.GetInstance().FindNext(start.x, start.y, destX, destY, out next)) {
                //u.ClearActions();
                u.CancelActions();
                u.destPos.x = destX;
                u.destPos.y = destY;
                actionSeq seq = new actionSeq();
                int curDir = u.GetDirection();
                int destDir = tools.getDirection(next - u.getTilePos());
                if (destDir != curDir) {
                    rotateAction r = new rotateAction();
                    r.Init(destDir);
                    seq.AddAction(r);
                }

                moveAction a = new moveAction();
                Vector2 diff = u.transform.position;
                a.Init(next.x, next.y);
                seq.AddAction(a);
                u.AddActionLazy(seq);
                return true;
            }

            return false;
        }
    }
}
