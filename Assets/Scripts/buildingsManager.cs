using System.Collections;
using System.Collections.Generic;
using Dune2;
using UnityEngine;
using Assets.Scripts;
using Microsoft.Unity.VisualStudio.Editor;


public struct sBuild {
    public eBuildingType type;
    public float dur;
    public float timer;
    public bool completed;
    public progressBar bar;

    public bool Update(float dt) {
        timer += dt;
        if (timer > dur) {
            timer = dur;
            completed = true;
            //bar.transform.position.Scale(new Vector3(1, 1, 1));
            bar.SetProgress(1.0f);
            return false;
        }
        
        //bar.transform.position.Scale(new Vector3(timer / dur, 1, 1));
        bar.SetProgress(timer/dur);
        return true;
    }
}
public class buildingsManager
{
    private List<building> buildings;
    private Camera camera;
    private scene mScene;
    private bool[][] concretes;
    private building selectedBuilding;
    private const float cameraDistance = 0.64f;
    private Dictionary<eBuildingType, int> buildingCosts;
    private Dictionary<eBuildingType, float> buildTimes;
    private List<sBuild> buildList;

    //private astar mAstar;

    public buildingsManager() {
        buildings = new List<building>();
        buildingCosts = new Dictionary<eBuildingType, int>();
        buildList = new List<sBuild>();
        buildTimes = new Dictionary<eBuildingType, float>();
    }

    // Start is called before the first frame update
    public void Init(int w, int h)
    {
        
        concretes = new bool[w][];
        for (int x = 0; x < w; x++)
        {
            concretes[x] = new bool[h];
            for (int y = 0; y < h; y++)
            {
                concretes[x][y] = false;
            }
        }

        buildingCosts[eBuildingType.kBase] = 0;
        buildingCosts[eBuildingType.kConcrete] = 15;
        buildingCosts[eBuildingType.kWindTrap] = 300;
        buildingCosts[eBuildingType.kRefinery] = 400;
        buildingCosts[eBuildingType.kRadar] = 400;
        buildingCosts[eBuildingType.kSilo] = 150;
        buildingCosts[eBuildingType.kVehicle] = 400;
        buildingCosts[eBuildingType.kBarracks] = 300;
        buildingCosts[eBuildingType.kWall] = 50;
        buildingCosts[eBuildingType.kTurret] = 125;
        buildingCosts[eBuildingType.kTurretRocket] = 250;
        buildingCosts[eBuildingType.kRepair] = 700;
        buildingCosts[eBuildingType.kAir] = 250;
        buildingCosts[eBuildingType.kStarPort] = 500;
        buildingCosts[eBuildingType.kPalace] = 500;

        buildTimes[eBuildingType.kBase] = 0;
        buildTimes[eBuildingType.kConcrete] = 3.0f;
        buildTimes[eBuildingType.kWindTrap] = 10.0f;
        buildTimes[eBuildingType.kRefinery] = 15.0f;
        buildTimes[eBuildingType.kRadar] = 15.0f;
        buildTimes[eBuildingType.kSilo] = 15.0f;
        buildTimes[eBuildingType.kVehicle] = 15.0f;
        buildTimes[eBuildingType.kBarracks] = 15.0f;
        buildTimes[eBuildingType.kWall] = 5.0f;
        buildTimes[eBuildingType.kTurret] = 15.0f;
        buildTimes[eBuildingType.kTurretRocket] = 15.0f;
        buildTimes[eBuildingType.kRepair] = 15.0f;
        buildTimes[eBuildingType.kAir] = 15.0f;
        buildTimes[eBuildingType.kStarPort] = 15.0f;
        buildTimes[eBuildingType.kPalace] = 15.0f;
    }

    // Update is called once per frame
    public void Update(float dt)
    {
        foreach (var b in buildList) {
            b.Update(dt);

        }
    }

    public void Build(int x,int y, eBuildingType type, int player) {
        switch (type) {
            case eBuildingType.kBase: {
                GameObject g = scene.Instantiate(Resources.Load("base", typeof(GameObject))) as GameObject;
                var b = g.GetComponent<baseBuilding>();
                RectInt rect = new RectInt(x, y, 2, 2);
                float h = CheckConcrete(rect);
                b.Init(x, y, player,h);
                astar.GetInstance().AddBuilding(rect);
                buildings.Add(b);
                for (int xx = x; xx < x + 2; xx++)
                {
                    for (int yy = y; yy < y + 2; yy++)
                    {
                        concretes[xx][yy] = false;
                    }
                }
            }
                break;
            case eBuildingType.kConcrete: {
                GameObject g = scene.Instantiate(Resources.Load("concrete", typeof(GameObject))) as GameObject;
                var b = g.GetComponent<concrete>();
                b.Init(x, y,2,2, player);
                for (int xx = x; xx < x + 2; xx++) {
                    for (int yy = y; yy < y + 2; yy++) {
                        concretes[xx][yy] = true;
                    }
                }
                
                break;
            }
        }
        gameManager.GetInstance().AddCredits(-buildingCosts[type]);
        
    }

    private float CheckConcrete(RectInt rect) {
        float result = 0.5f;
        for (int x = rect.x; x < rect.x + rect.width; x++) {
            for(int y = rect.y; y < rect.y + rect.height; y++) {
                if (concretes[x][y]) {
                    result += 0.125f;
                }
            }
        }

        return result;
    }

    public building GetBuildAt(Vector3 pos) {
        foreach (var b in buildings) {
            if (b.isRect(pos.x, pos.y)) {
                return b;
            }
        }

        return null;
    }

    public int GetBuildCost(eBuildingType type) {
        return buildingCosts[type];
    }

    public void StartBuilding(eBuildingType type) {
        if (buildList.Count>0) {
            foreach (var t in buildList) {
                if (t.type == type) {
                    if(!t.completed)
                        CancelBuilding(type);
                    else 
                        gameManager.GetInstance().GetMapManager().ActivateBuildMode(type);
                    return;
                }
            }
        }

        var pb = GameObject.Find("GUI/builds/concrete/progressBar");
        var b = new sBuild {
            timer = 0.0f,
            dur = buildTimes[type],
            type = type,
            completed = false,
            bar = pb.GetComponent<progressBar>()

        };
        buildList.Add(b);
    }
    

    public void CancelBuilding(eBuildingType type) {

    }
}
