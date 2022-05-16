using System.Collections;
using System.Collections.Generic;
using Assets.Scripts;
using UnityEngine;

namespace Assets.Scripts {
    static class actionManager {
        public static bool moveToPoint(unit u, int destX, int destY) {
            Vector2Int start = u.GetTilePos();
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
                int destDir = tools.GetDirection(next - u.GetTilePos());
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

        public static bool MoveToTarget(unit u, destructableObject target)
        {
            Vector2Int start = u.GetTilePos();
            Vector2Int dest = target.GetTilePos();
            if (target.isBuilding) {
                var b = target as building;
                if(b!=null)
                    dest = b.GetTilePosFrom(start);
            }
            if (dest.x == start.x && dest.y == start.y)
            {
                return false;
            }

            Vector2Int next = new Vector2Int();
            if (astar.GetInstance().FindNext(start.x, start.y, dest.x, dest.y, out next))
            {
                u.CancelActions();
                u.destPos.x = dest.x;
                u.destPos.y = dest.y;

                actionSeq seq = new actionSeq();
                int curDir = u.GetDirection();
                int destDir = tools.GetDirection(next - u.GetTilePos());
                if (destDir != curDir)
                {
                    rotateAction r = new rotateAction();
                    r.Init(destDir);
                    seq.AddAction(r);
                }

                moveAction a = new moveAction();
                Vector2 diff = u.transform.position;
                a.InitToTarget(target,next.x,next.y);
                seq.AddAction(a);
                u.AddActionLazy(seq);
                return true;
            }
            return false;
        }

        public static bool Attack(destructableObject u, destructableObject target,scene pScene) {
            int ang = tools.GetDirection(target.GetTilePos() - u.GetTilePos());
            actionSeq seq = new actionSeq();
            if (u.GetDirection() != ang)
            {
                rotateAction r = new rotateAction();
                r.Init(ang);
                seq.AddAction(r);
            }
            attackAction a = new attackAction();
            a.Init(target, pScene);
            seq.AddAction(a);
            u.AddAction(seq);
            return true;
        }
    }
}
