using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts;
using Dune2;
using SuperTiled2Unity;
using UnityEngine;

public class unitManager  {
    private List<unit> units;
    private List<unit> delUnits;
    private scene mScene;
    private Camera camera;
    private unit selectedUnit;
    private const float cameraDistance = 0.64f;
    private int myPlayer;
    //private astar mAstar;

    public unitManager() {
        units = new List<unit>();
        delUnits = new List<unit>();
    }

    public void Update(float dt) {
        /*
        if (Input.GetMouseButtonDown(0)) {
            if (!camera) {
                camera = gameManager.GetInstance().GetCamera();
            }

            selectedUnit = null;
            var targetPosition =
                camera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, cameraDistance));

            foreach (var u in units) {
                if (u.GetPlayer()==myPlayer && u.IsRect(targetPosition.x, targetPosition.y)) {
                    u.Select();
                    selectedUnit = u;
                }
                else {
                    u.Unselect();
                }
            }
        }

        if (Input.GetMouseButtonDown(1)) {
            if (selectedUnit) {
                var targetPosition =
                    camera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, cameraDistance));
                int x = tools.posX2iPosX(targetPosition.x);
                int y = tools.posY2iPosY(targetPosition.y);
                bool attack = false;
                foreach (var u in units) {
                    if (u.IsRect(targetPosition.x, targetPosition.y))
                    {
                        if (u.GetPlayer() != myPlayer) {
                            if (Vector2.Distance(u.transform.position, selectedUnit.transform.position) <= selectedUnit.attackRange) {
                                Attack(selectedUnit,u);
                            }
                            else {
                                MoveToTargetAndAttack(selectedUnit, u);
                            }
                            attack = true;
                        }
                        break;
                    }
                }

                if (!attack) {
                    MoveTo(selectedUnit, x, y);
                }
                selectedUnit.Unselect();
                selectedUnit = null;
            }
        }*/


        foreach (var u in units) {
            u.Update();
            if (u.isDestroying) {
                delUnits.Add(u);
            }
            //if (u.IsIdle())
            //{
            //    CheckIdleAI(u);
            //}
        }

        if (!delUnits.IsEmpty()) {
            foreach (var u in delUnits)
            {
                units.Remove(u);
                //UnityEngine.Object.Destroy(u);
                u.DestroyMe();
            }
            delUnits.Clear();
        }
    }


    public unit GetUnitAt(Vector3 pos) {
        foreach (var u in units)
        {
            if (u.IsRect(pos.x, pos.y)) {
                return u;
            }
        }

        return null;
    }


    private void CheckIdleAI(unit u) {
        foreach (var t in units) {
            if (t.GetPlayer() != u.GetPlayer() && u.canAttack && Vector2.Distance(u.transform.position,t.transform.position)<=u.attackRange) {
               
                break;
            }
        }
    }

    public void Attack(unit u, destructableObject target) {

        unit t = target as unit;
        int ang = 0;
        if (t != null) {

            if (t.isAttacking)
            {
                return;
            }
            ang = tools.GetDirection(t.GetTilePos() - u.GetTilePos());
        }
        else
        {
            building b = target as building;
            if (b != null) {
                ang = tools.GetDirection(tools.GetNearestFromRect(u.GetTilePos(), b.GetTileRect()) - u.GetTilePos());
            }
        }

        actionSeq seq = new actionSeq();
        if (u.GetDirection() != ang)
        {
            rotateAction r = new rotateAction();
            r.Init(ang);
            seq.AddAction(r);
        }
        attackAction a = new attackAction();
        a.Init(target, mScene);
        seq.AddAction(a);
        u.AddActionDelayed(seq);
    }

    public void SetScene(scene pScene) {
        mScene = pScene;
    }

    public unit CreateUnit(eUnitType type, int player, int x, int y) {
        switch (type) {
            case eUnitType.kTrike: {
                GameObject g = UnityEngine.Object.Instantiate(Resources.Load("trike" + player, typeof(GameObject))) as GameObject;
                var u = g.GetComponent<unit>();
                u.SetTilePos(x,y);
                (u as trike)?.Create(x,y,player);
                astar.GetInstance().AddUnit(x,y);
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
        if (actionManager.MoveToTarget(u, target))
        {
            u.Unselect();
            return true;
        }

        u.Unselect();
        return false;
    }
    public void SetPlayer(int pPlayer) {
        myPlayer = pPlayer;
    }
}
