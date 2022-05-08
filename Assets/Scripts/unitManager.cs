using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts;
using Dune2;
using SuperTiled2Unity;
using UnityEngine;

public class unitManager  {
    private List<unit> units;
    private Camera camera;
    private scene mScene;

    private unit selectedUnit;
    private const float cameraDistance = 0.64f;
    //private astar mAstar;

    public unitManager() {
        units = new List<unit>();
    }
    

    public void Update(float dt) {
        if (Input.GetMouseButtonDown(0)) {
            if (!camera) {
                camera = gameManager.GetInstance().GetCamera();
            }

            selectedUnit = null;
            //TODO: mouse coords to map coords
            var targetPosition =
                camera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, cameraDistance));
            //float mx = Input.mousePosition.x * 0.64f + camera.transform.position.x;
            //float my = Input.mousePosition.y * 0.64f + camera.transform.position.y;

            foreach (var u in units) {
                if (u.isRect(targetPosition.x, targetPosition.y)) {
                    u.select();
                    selectedUnit = u;
                }
                else {
                    u.unselect();
                }
            }
        }

        if (Input.GetMouseButtonDown(1)) {
            if (selectedUnit) {
                var targetPosition =
                    camera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, cameraDistance));
                int x = tools.posX2iPosX(targetPosition.x);
                int y = tools.posY2iPosY(targetPosition.y);
                moveTo(selectedUnit, x, y);
            }
        }

        foreach (var u in units) {
            u.Update();
        }
    }

    public void SetScene(scene pScene) {
        mScene = pScene;
    }

    public unit createUnit(eUnitType type, int x, int y) {
        switch (type) {
            case eUnitType.kTrike: {
                GameObject g = scene.Instantiate(Resources.Load("trike", typeof(GameObject))) as GameObject;
                var u = g.GetComponent<unit>();
                u.setTilePos(x,y);
                (u as trike)?.Create(x,y);
                astar.GetInstance().AddUnit(x,y);
                units.Add(u);
                    return u;
            }
                
            default:
                return null;
        }
    }
    

    public bool moveTo(unit u, int x, int y) {
        Vector2Int start = u.getTilePos();
        Vector2Int dest = new Vector2Int(x, y);
        if (x == start.x && y == start.y) {
            return false;
        }
        var path = astar.GetInstance().FindPath(start.x, start.y, x, y);
        if (!path.IsEmpty()) {

            u.ClearActions();
            path.Reverse();
            if(path[0].x==start.x && path[0].y==start.y)
                path.RemoveAt(0);
            actionSeq seq = new actionSeq();
            Vector2Int prevPos = new Vector2Int(start.x,start.y);
            int prevDir = u.GetDirection();
            foreach (var p in path)
            {
                int destDir = tools.getDirection( p- prevPos);
                if (destDir != prevDir)
                {
                    rotateAction r = new rotateAction();
                    r.Init(destDir);
                    prevDir = destDir;
                    seq.AddAction(r);
                }

                prevPos = p;
                moveAction a = new moveAction();
                Vector2 diff = u.transform.position;
                a.Init(p.x, p.y);
                seq.AddAction(a);
            }
            u.AddAction(seq);
            u.unselect();
            return true;
        }

        u.unselect();
        return false;
    }

}
