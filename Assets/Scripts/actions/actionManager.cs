using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dune2
{
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
                //u.CancelActions();
                u.destPos.x = destX;
                u.destPos.y = destY;

                actionSeq seq = new actionSeq();
                int curDir = u.GetTurnDirection();
                int destDir = tools.GetDirection(next - u.GetTilePos());
                if (destDir != curDir) {
                    rotateAction r = new rotateAction();
                    r.Init(destDir);
                    seq.AddAction(r);
                }

                waitAction w = new waitAction();
                w.Init(0.3f);
                seq.AddAction(w);

                moveAction a = new moveAction();
                Vector2 diff = u.transform.position;
                a.Init(next.x, next.y);
                seq.AddAction(a);
                u.AddActionSeq(seq);
                
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
                if (b != null) {
                    if (b.GetBuildingType() == eBuildingType.kRefinery && u.GetUnitType() == eUnitType.kHarvester) {
                        dest.x = b.GetTilePos().x+2;
                        dest.y = b.GetTilePos().y+1;

                    }
                    else
                    {
                        dest = b.GetTilePosFrom(start);
                    }

                }
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
                int curDir = u.GetTurnDirection();
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
                u.AddActionSeq(seq);
                return true;
            }
            return false;
        }

        public static void Attack(destructableObject u, destructableObject target,scene pScene, bool pForce) {
            int ang = tools.GetDirection(target.GetTilePos() - u.GetTilePos());
            actionSeq seq = new actionSeq();
            if (u.GetTurnDirection() != ang)
            {
                rotateAction r = new rotateAction();
                r.Init(ang);
                seq.AddAction(r);
            }
            attackAction a = new attackAction();
            a.Init(target, pScene,pForce);
            seq.AddAction(a);
            u.AddAction(seq);
        }

        public static rotateAction Rotate(int dest) {
            rotateAction r = new rotateAction();
            r.Init(dest);
            return r;
        }

        public static rotateAction RotatoToObject(destructableObject u, destructableObject target) {
            int startDir = u.GetTurnDirection();
            Vector2Int dest;
            int destDir = 0;
            if (target.isBuilding) {
                 dest = tools.GetNearestFromRect(u.GetTilePos(), (target as building).GetTileRect());
            }
            else {
                dest = target.GetTilePos();
            }
            destDir = tools.GetDirection(target.GetTilePos() - dest);
            if (startDir == destDir) return null;

            rotateAction r = new rotateAction();
            r.Init(destDir);
            return r;
        }

        public static destroyAction DestroyUnit(unit u) {
            destroyAction d = new destroyAction();
            d.Init();
            u.AddAction(d);
            return d;
        }

        public static void Harvest(unit u) {
            Vector2Int dest = gameManager.GetInstance().GetMapManager().SearchNearestSpice(u.GetTilePos());
            moveToPoint(u,dest.x, dest.y);
        }

        public static void CheckHarvester(unit u) {
            harvester h = u as harvester;
            if (h == null) return;
            var builds = gameManager.GetInstance().GetMapManager().GetBuildingsManager().GetBuildingList();
            List<Vector2Int> positions = new List<Vector2Int>();
            foreach (var build in builds) {
                if (build.GetBuildingType() == eBuildingType.kRefinery) {
                    Vector2Int pos = default;
                    pos.x = build.GetTilePos().x+2;
                    pos.y = build.GetTilePos().y + 1;
                    positions.Add(pos);  
                }
            }

            foreach (var p in positions) {
                if (h.GetTilePos().x == p.x && h.GetTilePos().y==p.y)
                {
                    var dst = gameManager.GetInstance().GetMapManager().GetBuildingsManager()
                        .GetBuildAt(h.GetTilePos()) as refinery;
                    if (dst != null)
                    {
                        unloadHarvesterAction unl = new unloadHarvesterAction();
                        unl.Init(dst);
                        h.AddAction(unl);
                    }

                    return;
                }
            }
            
        }
    }
}
