using System;
using Dune2;
using SuperTiled2Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Search;
using UnityEngine;
using static UnityEditor.PlayerSettings;

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
            if (spice != sp.gameObject) {
                AddSpiceAt(tools.RoundPosX(sp.transform.position.x),
                    tools.RoundPosY(sp.transform.position.y)); 
            }
        }
        RefreshSpicesConfig();
    }

    public void RefreshSpicesConfig() {
      
    

        return;
        //TODO: Advanved Tile System for Spices.
        /*
        //check corners
        foreach (var sp in spicesList.Values) {
            if (sp.type == spice.eSpiceType.kNone) continue;
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

            foreach (var sp2 in spicesList.Values) {
                if (sp2.type == spice.eSpiceType.kNone) continue;
                if (sp2 != sp) {
                    if ((sp2.pos.x == sp.pos.x + 1 && sp2.pos.y == sp.pos.y)) {
                        right = true;
                    }
                    else if ((sp2.pos.x == sp.pos.x - 1 && sp2.pos.y == sp.pos.y)) {
                        left = true;
                    }
                    else if ((sp2.pos.x == sp.pos.x && sp2.pos.y == sp.pos.y - 1)) {
                        up = true;
                    }
                    else if ((sp2.pos.x == sp.pos.x && sp2.pos.y == sp.pos.y + 1)) {
                        down = true;
                    }
                    else if (sp2.pos.x == sp.pos.x - 1 && sp2.pos.y == sp.pos.y - 1) {
                        upleft = true;
                    }
                    else if (sp2.pos.x == sp.pos.x - 1 && sp2.pos.y == sp.pos.y + 1) {
                        downleft = true;
                    }
                    else if (sp2.pos.x == sp.pos.x + 1 && sp2.pos.y == sp.pos.y - 1) {
                        upright = true;
                    }
                    else if (sp2.pos.x == sp.pos.x + 1 && sp2.pos.y == sp.pos.y + 1) {
                        downright = true;
                    }

                }
            }

            string spicename = "";
            spice.eSpiceType t = spice.eSpiceType.kFull;
            if (!right && !downright && !down && up && left && upleft) {
                //right down
                spicename = "atlas_201";
                t = spice.eSpiceType.kRightDown;
            }
            else if (!right && !upright && !up && down && left && downleft) {
                //right up
                spicename = "atlas_204";
                t = spice.eSpiceType.kRightUp;
            }
            else if (!left && !downleft && !down && up && right && upright) {
                //left down
                spicename = "atlas_195";
                t = spice.eSpiceType.kLeftDown;
            }
            else if (!left && !upleft && !up && down && right && downright) {
                //left up
                spicename = "atlas_198";
                t = spice.eSpiceType.kLeftUp;
            }

            if (!spicename.IsEmpty()) {
                sp.ChangeTexture(spritesMap[spicename]);
                sp.type = t;
            }
        }

        //outer corners
        List<(Vector2Int, string)> addList = new List<(Vector2Int, string)>();
        foreach (var sp in spicesList.Values) {
            bool left = false;
            bool right = false;
            bool up = false;
            bool down = false;
            bool upleft = false;
            bool downleft = false;
            bool upright = false;
            bool downright = false;
            if (sp.type == spice.eSpiceType.kFull) {
                foreach (var sp2 in spicesList.Values) {
                    if (sp2 != sp && sp2.type != spice.eSpiceType.kFull && sp2.type != spice.eSpiceType.kNone) {
                        if ((sp2.pos.x == sp.pos.x + 1 && sp2.pos.y == sp.pos.y)) {
                            right = true;
                        }
                        else if ((sp2.pos.x == sp.pos.x - 1 && sp2.pos.y == sp.pos.y)) {
                            left = true;
                        }
                        else if ((sp2.pos.x == sp.pos.x && sp2.pos.y == sp.pos.y - 1)) {
                            up = true;
                        }
                        else if ((sp2.pos.x == sp.pos.x && sp2.pos.y == sp.pos.y + 1)) {
                            down = true;
                        }
                        else if (sp2.pos.x == sp.pos.x - 1 && sp2.pos.y == sp.pos.y - 1) {
                            upleft = true;
                        }
                        else if (sp2.pos.x == sp.pos.x - 1 && sp2.pos.y == sp.pos.y + 1) {
                            downleft = true;
                        }
                        else if (sp2.pos.x == sp.pos.x + 1 && sp2.pos.y == sp.pos.y - 1) {
                            upright = true;
                        }
                        else if (sp2.pos.x == sp.pos.x + 1 && sp2.pos.y == sp.pos.y + 1) {
                            downright = true;
                        }
                    }
                }
            }

            string spicename = "";
            Vector2Int pos = sp.pos;
            if (right && !downright && down) {
                //right down
                spicename = "atlas_66";
                pos.x++;
                pos.y++;
            }
            else if (right && !upright && up ) {
                //right up
                spicename = "atlas_67";
                pos.x++;
                pos.y--;
            }
            else if (left && !downleft && down ) {
                //left down
                spicename = "atlas_64";
                pos.x--;
                pos.y++;
            }
            else if (left && !upleft && up) {
                //left up
                spicename = "atlas_65";
                pos.x--;
                pos.y--;
            }
            else if (!right && (up || pos.y==0) && down) {
                //right
                spicename = "atlas_200";
                pos.x++;
            }
            else if (right && (left || pos.x==0) && !up) {
                //up
                spicename = "atlas_196";
                pos.y--;
            }
            else if (left && (right || pos.x==mapSize.x-1) && !down) {
                //down
                spicename = "atlas_193";
                pos.y++;
            }
            else if (up && (down || pos.y==mapSize.y-1) && !left) {
                //left
                spicename = "atlas_194";
                pos.x--;
            }

            if (!spicename.IsEmpty() && pos.x >=0 && pos.y>=0 && pos.x<mapSize.x && pos.y<mapSize.y) {
                addList.Add((pos,spicename));
            }
        }

        foreach (var sp in addList) {

            var s = AddSpiceAt(sp.Item1);
            if (s) {
                s.ChangeTexture(spritesMap[sp.Item2]);
                s.type = spice.eSpiceType.kNone;
            }
        }
        */
        
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

    public spice AddSpiceAt(Vector2Int pos) {
        return AddSpiceAt(pos.x, pos.y);
    }
    public spice AddSpiceAt(int x, int y)
    {
        spicesList ??= new Dictionary<Vector2Int, spice>();

        foreach (var sp in spicesList) {
            if (sp.Key.x == x && sp.Key.y==y) {
                return null;
            }
        }
        
        GameObject g =
            UnityEngine.Object.Instantiate(Resources.Load("spice", typeof(GameObject))) as
                GameObject;

        var s = g.GetComponent<spice>();
        if (s != null)
        {
            s.Init(x, y);
            spicesList[s.pos] = s;
            return s;
        }

        return null;
    }


    public void DestroySpice(spice s) {
        foreach (var sp in spicesList) {
            if (sp.Value == s) {
                spicesList.Remove(sp.Key);
                return;
            }
        }
    }
    public void DestroySpiceAt(int x, int y)
    {
        foreach (var s in spicesList)
        {
            if (s.Key.x == x && s.Key.y == y)
            {
                spicesList.Remove(s.Key);
                return;
            }
        }
    }
    public void DestroySpiceAt(Vector2Int pos)
    {
        foreach (var s in spicesList)
        {
            if (s.Key == pos)
            {
                spicesList.Remove(s.Key);
                return;
            }
        }
    }

    public spice GetSpiceAt(int x, int y) {
        spicesList.TryGetValue(new Vector2Int(x, y), out spice s);
        return s;
    }
    public spice GetSpiceAt(Vector2Int pos)
    {
        spicesList.TryGetValue(pos, out spice s);
        return s;
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

    public int AddSpiceCountAt(int x, int y, int count) {
        foreach (var s in spicesList) {
            if (s.Key.x == x && s.Key.y == y) {
                int result = s.Value.AddCount(count);
                if (result < 0) {
                    DestroySpice(s.Value);
                }
                return result;
            }
        }
        return 0;
    }
}
