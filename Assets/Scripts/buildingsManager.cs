using System.Collections;
using System.Collections.Generic;
using Dune2;
using UnityEngine;
using Assets.Scripts;

public class buildingsManager
{
    private List<building> buildings;
    private Camera camera;
    private scene mScene;

    private building selectedBuilding;
    private const float cameraDistance = 0.64f;
    //private astar mAstar;

    public buildingsManager()
    {
        buildings = new List<building>();
    }
    // Start is called before the first frame update
    public void Init()
    {
        
    }

    // Update is called once per frame
    public void Update(float dt)
    {
        //if (Input.GetMouseButtonDown(0))
        //{
        //    if (!camera)
        //    {
        //        camera = gameManager.GetInstance().GetCamera();
        //    }

        //    selectedBuilding = null;
        //    var targetPosition =
        //        camera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, cameraDistance));

        //    foreach (var b in buildings)
        //    {
        //        if (b.isRect(targetPosition.x, targetPosition.y))
        //        {
        //            b.Select();
        //            selectedBuilding = b;
        //        }
        //        else
        //        {
        //            b.Unselect();
        //        }
        //    }
        //}
        //foreach (var u in buildings)
        //{
        //    //u.Update();
        //}
    }

    public building Build(int x,int y, eBuildingType type, int player) {
        switch (type) {
            case eBuildingType.kBase:
                GameObject g = scene.Instantiate(Resources.Load("base", typeof(GameObject))) as GameObject;
                var b = g.GetComponent<building>();
                b.Init(x,y, player);
                astar.GetInstance().AddBuilding(new RectInt(x,y,2,2));
                buildings.Add(b);
                return b;
        }

        return null;
    }

    public building GetBuildAt(Vector3 pos) {
        foreach (var b in buildings) {
            if (b.isRect(pos.x, pos.y)) {
                return b;
            }
        }

        return null;
    }
    
}
