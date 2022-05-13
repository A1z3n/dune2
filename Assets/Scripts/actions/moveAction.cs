using System.Collections;
using System.Collections.Generic;
using Assets.Scripts;
using UnityEngine;

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
    private unit target;
    public void Init(int x, int y) {
        duration = 1.0f;
        destX = x;
        destY = y;
        destPos = tools.iPos2Pos(x,y);
    }

    public void InitToTarget(unit u) {
        duration = 1.0f;
        destX = u.GetTilePos().x;
        destY = u.GetTilePos().y;
        destPos = tools.iPos2Pos(destX, destY);
        target = u;
    }

    public override bool Update(actionBase u, float dt) {
        if (!inited) {
            inited = true;
            startPos = u.transform.position;
            var un = u as unit;
            startX = un.GetTilePos().x;
            startY = un.GetTilePos().y;
            un.SetTilePos(destX, destY);
            astar.GetInstance().ChangeUnitPos(startX, startY, destX, destY);
        }
        timer += dt;
        if (timer >= duration) {
            u.transform.position = destPos;
            if (cancel) {
                return false;
            }
            var un = u as unit;
            if (un != null)
            {
                if(un.destPos.x==destX && un.destPos.y ==destY) {
                    return false;
                }

                if (target != null) {
                    if (Vector2.Distance(target.transform.position, u.transform.position) <= un.attackRange) {
                        
                    }
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


}
