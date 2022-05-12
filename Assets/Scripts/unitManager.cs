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
                selectedUnit = null;
            }
        }

        foreach (var u in units) {
            u.Update();
        }
    }

    public void SetScene(scene pScene) {
        mScene = pScene;
    }

    public unit createUnit(eUnitType type, int player, int x, int y) {
        switch (type) {
            case eUnitType.kTrike: {
                GameObject g = scene.Instantiate(Resources.Load("trike", typeof(GameObject))) as GameObject;
                var u = g.GetComponent<unit>();
                u.setTilePos(x,y);
                (u as trike)?.Create(x,y,player);
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
        if (actionManager.moveToPoint(u, start.x, start.y, x, y)) {
            u.unselect();
            return true;
        }
       
        u.unselect();
        return false;
    }

}
