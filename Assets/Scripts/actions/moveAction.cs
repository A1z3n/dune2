using System.Collections;
using System.Collections.Generic;
using Assets.Scripts;
using UnityEngine;

public class moveAction : action {
    private Vector2 startPos;
    private Vector2 destPos;
    private int destX;
    private int destY;
    private float timer = 0.0f;
    private float duration;
    private bool inited = false;
    public void Init(int x, int y) {
        duration = 1.0f;
        destX = x;
        destY = y;
        destPos = tools.iPos2Pos(x,y);
    }
    

    public override bool Update(actionBase u, float dt) {
        if (!inited) {
            inited = true;
            startPos = u.transform.position;
        }
        timer += dt;
        if (timer >= duration) {
            u.transform.position = destPos;
            var un = u as unit;
            if (un != null)
            {
                astar.GetInstance().ChangeUnitPos(un.getTilePos().x,un.getTilePos().y,destX,destY);
                un.setTilePos(destX, destY);
            }

            //OnEndCallback();
            return false;
        }

        float p = timer / duration;
        u.transform.position = startPos + (destPos - startPos) * p;
        return true;
    }

}
