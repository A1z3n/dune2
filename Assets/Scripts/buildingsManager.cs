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
    private bool[][] concretes;
    private building selectedBuilding;
    private const float cameraDistance = 0.64f;
    
    //private astar mAstar;

    public buildingsManager()
    {
        buildings = new List<building>();
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
    }

    // Update is called once per frame
    public void Update(float dt)
    {
        
    }

    public building Build(int x,int y, eBuildingType type, int player) {
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
                return b;
            }
            case eBuildingType.kConcreteBig: {
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

        return null;
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
    
}
