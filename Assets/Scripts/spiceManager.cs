using Dune2;
using SuperTiled2Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Search;
using UnityEngine;

public class spiceManager
{
    private Dictionary<Vector2Int, spice> spicesList;
    private List<Vector3Int> spiceBombsList;
    private const int SPICE_MAX = 1000;
    private List<Vector2Int> searchList;
    private Vector2Int mapSize;
    private Dictionary<string, Sprite> spritesMap;
    public void Init(Vector2Int pMapSize) {
        mapSize = pMapSize;
        spiceBombsList = new List<Vector3Int>();
        searchList = new List<Vector2Int>();
        Sprite[] sprites = Resources.LoadAll<Sprite>("atlas");
        spritesMap = new Dictionary<string, Sprite>();
        foreach (Sprite sprite in sprites)
        {
            spritesMap[sprite.name] = sprite;
        }
        var spice = GameObject.Find("map/Grid/spice");
        var spices = spice.GetComponentsInChildren<Transform>();
        foreach (var sp in spices)
        {
            if (spice != sp.gameObject)
            {
                AddSpiceAt(tools.RoundPosX(sp.transform.position.x),
                    tools.RoundPosY(sp.transform.position.y));
            }
        }
        RefreshSpicesConfig();
    }

    public void RefreshSpicesConfig()
    {
        //check corners
        foreach (var sp in spicesList.Values)
        {
            bool left = false;
            bool right = false;
            bool up = false;
            bool down = false;
            bool upleft = false;
            bool downleft = false;
            bool upright = false;
            bool downright = false;

            if (sp.pos.x == 0) {
                left = true;
                upleft = true;
                downleft = true;
            }
            else if (sp.pos.x == mapSize.x - 1) {
                right = true;
                upright = true;
                downright = true;
            }

            if (sp.pos.y == 0) {
                up = true;
                upleft = true;
                upright = true;
            }
            else if (sp.pos.y == mapSize.y - 1) {
                down = true;
                downleft = true;
                downright = true;
            }

            foreach (var sp2 in spicesList.Values)
            {
                if (sp2 != sp)
                {
                    if ((sp2.pos.x == sp.pos.x + 1 && sp2.pos.y == sp.pos.y))
                    {
                        right = true;
                    }
                    else if ((sp2.pos.x == sp.pos.x - 1 && sp2.pos.y == sp.pos.y))
                    {
                        left = true;
                    }
                    else if ((sp2.pos.x == sp.pos.x && sp2.pos.y == sp.pos.y - 1) )
                    {
                        up = true;
                    }
                    else if ((sp2.pos.x == sp.pos.x && sp2.pos.y == sp.pos.y + 1) )
                    {
                        down = true;
                    }
                    else if (sp2.pos.x == sp.pos.x - 1 && sp2.pos.y == sp.pos.y - 1)
                    {
                        upleft = true;
                    }
                    else if (sp2.pos.x == sp.pos.x - 1 && sp2.pos.y == sp.pos.y + 1)
                    {
                        downleft = true;
                    }
                    else if (sp2.pos.x == sp.pos.x + 1 && sp2.pos.y == sp.pos.y - 1)
                    {
                        upright = true;
                    }
                    else if (sp2.pos.x == sp.pos.x + 1 && sp2.pos.y == sp.pos.y + 1)
                    {
                        downright = true;
                    }

                }
            }

            string spicename = "";
            if (!right && !downright && !down && up && left && upleft)
            {
                //right down
                spicename = "atlas_201";
            }
            else if (!right && !upright && !up && down && left && downleft) {
                //right up
                spicename = "atlas_204";
            }
            else if (!left && !downleft && !down && up && right && upright)
            {
                //left down
                spicename = "atlas_195";
            }
            else if (!left && !upleft && !up && down && right && downright)
            {
                //left up
                spicename = "atlas_198";
            }

            if (!spicename.IsEmpty())
            {
                sp.ChangeTexture(spritesMap[spicename]);
            }
        }

        foreach (var sp in spicesList.Values) {
            
        }
    }

    public void AddSpiceBomb(Vector3 pos, int spices)
    {
        Vector3Int result = new Vector3Int(tools.RoundPosX(pos.x), tools.RoundPosY(pos.y), spices);
        spiceBombsList.Add(result);
    }

    public void CheckSpiceMines(int x, int y)
    {
        foreach (var it in spiceBombsList)
        {
            if (it.x == x && it.y == y)
            {
                ActivateSpiceBomb(x, y, it.z);
                spiceBombsList.Remove(it);
                return;
            }
        }
    }

    public void ActivateSpiceBomb(int x, int y, int spices)
    {
        int sum = SPICE_MAX;
        //spicesList.Add(new Vector2Int(x, y), SPICE_MAX);

        searchList.Clear();
        searchList.Add(new Vector2Int(x + 1, y));
        searchList.Add(new Vector2Int(x, y - 1));
        searchList.Add(new Vector2Int(x + 1, y - 1));
        searchList.Add(new Vector2Int(x, y + 1));
        searchList.Add(new Vector2Int(x + 1, y + 1));
        searchList.Add(new Vector2Int(x - 1, y));
        searchList.Add(new Vector2Int(x - 1, y - 1));
        searchList.Add(new Vector2Int(x - 1, y + 1));
        searchList.Add(new Vector2Int(x + 2, y));
        searchList.Add(new Vector2Int(x - 2, y));
        searchList.Add(new Vector2Int(x, y - 2));
        searchList.Add(new Vector2Int(x, y + 2));
        //while (sum<spices) {
        //    sum += SPICE_MAX;

        //}
    }

    public void AddSpice(spice s)
    {
        spicesList ??= new Dictionary<Vector2Int, spice>();
        spicesList[s.pos] = s;
        //spicesList.Add(new Vector2Int(x, y), spices);
    }

    public void AddSpiceAt(int x, int y)
    {
        spicesList ??= new Dictionary<Vector2Int, spice>();
        GameObject g =
            UnityEngine.Object.Instantiate(Resources.Load("spice", typeof(GameObject))) as
                GameObject;

        var s = g.GetComponent<spice>();
        if (s != null)
        {
            s.Init(x, y);
            spicesList[s.pos] = s;
        }
    }
    public Vector2Int SearchNearestSpice(Vector2Int from)
    {
        Vector2Int result = new Vector2Int();
        Dictionary<Vector2Int, float> ranges = new Dictionary<Vector2Int, float>();
        foreach (var s in spicesList)
        {
            ranges[s.Key] = (s.Key - from).magnitude;
        }

        float min = 999999999.0f;
        foreach (var r in ranges)
        {
            if (min > r.Value)
            {
                min = r.Value;
                result = r.Key;
            }
        }
        return result;
    }

    public bool IsSpiceAtPoint(Vector2Int pos)
    {
        foreach (var s in spicesList)
        {
            if (s.Key.x == pos.x && s.Key.y == pos.y)
            {
                return true;
            }
        }

        return false;
    }
}
