using System.Collections;
using System.Collections.Generic;
using Dune2;
using UnityEngine;
using Assets.Scripts;
using Microsoft.Unity.VisualStudio.Editor;
using Mono.CompilerServices.SymbolWriter;
using TMPro;


public class sBuild {
    public eBuildingType type;
    public float dur;
    private float timer;
    public bool completed;
    public progressBar bar;
    public int cost;
    private float credits;
    private int prevCredits;
    public TextMeshProUGUI text;

    public sBuild() {
        timer = 0.0f;
        completed = false;
        credits = 0;
        prevCredits = 0;
    }

    public bool Update(float dt) {
        if (completed) return false;
        timer += dt;
        if (timer > dur) {
            timer = dur;
            completed = true;
            bar.SetProgress(0.0f);
            gameManager.GetInstance().AddCredits((int)credits-cost);
            text.enabled = true;
            return false;
        }
        
        float dc = cost*dt/dur;
        credits += dc;
        if ((int)credits != prevCredits) {
            gameManager.GetInstance().AddCredits(prevCredits-(int)credits);
            prevCredits = (int)credits;
        }
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
    private int mapWidth;
    private int mapHeight;
    private Dictionary<eBuildingType, string> namesDictionary;
    private Dictionary<eBuildingType, int> buildSizes;

    //private astar mAstar;

    public buildingsManager() {
        buildings = new List<building>();
        buildingCosts = new Dictionary<eBuildingType, int>();
        buildList = new List<sBuild>();
        buildTimes = new Dictionary<eBuildingType, float>();
        namesDictionary = new Dictionary<eBuildingType, string>();
        buildSizes = new Dictionary<eBuildingType, int>();
    }

    // Start is called before the first frame update
    public void Init(int w, int h) {
        mapWidth = w;
        mapHeight = h;
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

        namesDictionary[eBuildingType.kBase] = "base";
        namesDictionary[eBuildingType.kConcrete] = "concrete";
        namesDictionary[eBuildingType.kWindTrap] = "windtrap";
        namesDictionary[eBuildingType.kRefinery] = "refinery";
        namesDictionary[eBuildingType.kRadar] = "radar";
        namesDictionary[eBuildingType.kSilo] = "silo";
        namesDictionary[eBuildingType.kVehicle] = "vehicle";
        namesDictionary[eBuildingType.kBarracks] = "barracks";
        namesDictionary[eBuildingType.kWall] = "wall";
        namesDictionary[eBuildingType.kTurret] = "turret";
        namesDictionary[eBuildingType.kTurretRocket] = "r-turret";
        namesDictionary[eBuildingType.kRepair] = "repair";
        namesDictionary[eBuildingType.kAir] = "air";
        namesDictionary[eBuildingType.kStarPort] = "starport";
        namesDictionary[eBuildingType.kPalace] = "palace";

        buildSizes[eBuildingType.kBase] = 4;
        buildSizes[eBuildingType.kConcrete] = 4;
        buildSizes[eBuildingType.kWindTrap] = 4;
        buildSizes[eBuildingType.kRefinery] = 6;
        buildSizes[eBuildingType.kRadar] = 4;
        buildSizes[eBuildingType.kSilo] = 4;
        buildSizes[eBuildingType.kVehicle] = 6;
        buildSizes[eBuildingType.kBarracks] = 4;
        buildSizes[eBuildingType.kWall] = 1;
        buildSizes[eBuildingType.kTurret] = 1;
        buildSizes[eBuildingType.kTurretRocket] = 1;
        buildSizes[eBuildingType.kRepair] = 6;
        buildSizes[eBuildingType.kAir] = 4;
        buildSizes[eBuildingType.kStarPort] = 9;
        buildSizes[eBuildingType.kPalace] = 9;
    }

    // Update is called once per frame
    public void Update(float dt)
    {
        foreach (var b in buildList) {
            b.Update(dt);
        }
    }

    public void Build(int x,int y, eBuildingType type, int player) {
        if (type == eBuildingType.kConcrete) {
            GameObject g = scene.Instantiate(Resources.Load("concrete", typeof(GameObject))) as GameObject;
            var b = g.GetComponent<concrete>();
            b.Init(x, y, 2, 2, player);
            for (int xx = x; xx < x + 2; xx++)
            {
                for (int yy = y; yy < y + 2; yy++)
                {
                    if (xx > 0 && yy > 0 && xx <= mapWidth && yy <= mapHeight)
                        concretes[xx - 1][yy - 1] = true;
                }
            }
        }
        else {
            GameObject g = scene.Instantiate(Resources.Load(namesDictionary[type], typeof(GameObject))) as GameObject;
            if (g != null) {
                var b = g.GetComponent<baseBuilding>();
                RectInt rect = new RectInt(x, y, 2, 2);
                float c = CheckConcrete(rect);
                b.Init(x, y, player, c);
                astar.GetInstance().AddBuilding(rect);
                buildings.Add(b);
                int s = buildSizes[type];
                int w = 2;
                int h = 2;
                if (s == 9) {
                    w = 3;
                    h = 3;
                }
                else if (s == 6) {
                    w = 3;
                    h = 2;
                }

                for (int xx = x; xx < x + w; xx++) {
                    for (int yy = y; yy < y + h; yy++) {
                        concretes[xx][yy] = false;
                    }
                }
            }
        }


        foreach (var it in buildList) {
            if (it.type==type) {
                it.text.enabled = false;
                buildList.Remove(it);
                break;
            }
        }
        //gameManager.GetInstance().AddCredits(-buildingCosts[type]);

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

        var pb = GameObject.Find("GUI/builds/"+ namesDictionary[type]+"/progressBar");
        var ok = GameObject.Find("GUI/builds/"+ namesDictionary[type]+"/ok");
        var b = new sBuild {
            dur = buildTimes[type],
            type = type,
            bar = pb.GetComponent<progressBar>(),
            cost = buildingCosts[type],
            text = ok.GetComponent<TextMeshProUGUI>()
        };
        buildList.Add(b);
    }
    

    public void CancelBuilding(eBuildingType type) {

    }

    public bool CheckConcrete(int x, int y) {
        if (x >= mapWidth || y >= mapHeight || x<0 || y<0)
        {
            return false;
        }
        return concretes[x][y];
    }

    public int GetBuildSize(eBuildingType type) {
        return buildSizes[type];
    }

    public string GetBuildName(eBuildingType type) {
        return namesDictionary[type];
    }
}
