using System;
using System.Collections;
using System.Collections.Generic;
using Dune2;
using SuperTiled2Unity;
using UnityEngine;

namespace Dune2 {
    public class unitManager {
        private List<unit> units;
        private List<unit> delUnits;
        private scene mScene;

        public unitManager() {
            units = new List<unit>();
            delUnits = new List<unit>();
        }

        public void Update(float dt) {



            foreach (var u in units) {
                u.Update();
                if (u.isDestroying) {
                    delUnits.Add(u);
                }

                if (u.IsIdle()) {
                    CheckIdleAI(u);
                }
            }

            if (!delUnits.IsEmpty()) {
                foreach (var u in delUnits) {
                    units.Remove(u);
                    //UnityEngine.Object.Destroy(u);
                    u.DestroyMe();
                }

                delUnits.Clear();
            }
        }


        public unit GetUnitAt(Vector3 pos) {
            foreach (var u in units) {
                if (u.IsRect(pos.x, pos.y)) {
                    return u;
                }
            }

            return null;
        }


        private void CheckIdleAI(unit u) {
            foreach (var t in units) {
                if (!u.isAttacking && t.GetPlayer() != u.GetPlayer() && u.canAttack && tools.IsInAttackRange(u, t)) {
                    bool idle = u.IsIdle();
                    Attack(u, t, false);
                }
            }
        }

        public void Attack(unit u, destructableObject target, bool pForce) {

            unit t = target as unit;
            u.isAttacking = true;
            int ang = 0;
            if (t != null) {
                ang = tools.GetDirection(t.GetTilePos() - u.GetTilePos());
            }
            else {
                building b = target as building;
                if (b != null) {
                    ang = tools.GetDirection(tools.GetNearestFromRect(u.GetTilePos(), b.GetTileRect()) -
                                             u.GetTilePos());
                }
            }

            actionSeq seq = new actionSeq();
            if (u.GetTurnDirection() != ang) {
                rotateAction r = new rotateAction();
                r.Init(ang);
                seq.AddAction(r);
            }

            attackAction a = new attackAction();
            a.Init(target, mScene, pForce);
            seq.AddAction(a);
            u.AddActionDelayed(seq);
        }

        public void SetScene(scene pScene) {
            mScene = pScene;
        }

        public unit CreateUnit(eUnitType type, int player, int x, int y) {
            switch (type) {
                case eUnitType.kTrike: {
                    GameObject g =
                        UnityEngine.Object.Instantiate(Resources.Load("trike" + player, typeof(GameObject))) as
                            GameObject;
                    var u = g.GetComponent<unit>();
                    u.SetTilePos(x, y);
                    (u as trike)?.Create(x, y, player);
                    astar.GetInstance().AddUnit(x, y);
                    units.Add(u);
                    return u;
                }

                case eUnitType.kHarvester: {
                    GameObject g =
                        UnityEngine.Object.Instantiate(Resources.Load("harvester" + player, typeof(GameObject))) as
                            GameObject;
                    var u = g.GetComponent<unit>();
                    u.SetTilePos(x, y);
                    (u as harvester)?.Create(x, y, player);
                    astar.GetInstance().AddUnit(x, y);
                    units.Add(u);
                    return u;
                }
                default:
                    return null;
            }
        }

        public bool MoveTo(unit u, int x, int y) {
            if (actionManager.moveToPoint(u, x, y)) {
                u.Unselect();
                return true;
            }

            u.Unselect();
            return false;
        }

        public bool MoveToTargetAndAttack(unit u, destructableObject target) {
            if (actionManager.MoveToTarget(u, target)) {
                u.Unselect();
                return true;
            }

            u.Unselect();
            return false;
        }
    }
}