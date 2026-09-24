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
    private Dictionary<Vector2Int, spice> fakeList;
    public void Init(Vector2Int pMapSize) {
        mapSize = pMapSize;
        spiceBombsList = new List<Vector3Int>();
        searchList = new List<Vector2Int>();
        fakeList = new Dictionary<Vector2Int, spice>();
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
        if (spicesList == null || spicesList.Count == 0) return;

        // Simplified approach - determine tile type based on neighbors
        foreach (var spicePair in spicesList) {
            var sp = spicePair.Value;
            if (sp.type == spice.eSpiceType.kNone) continue;
            bool current = IsSpiceAtPoint(sp.pos);
            // Check 4 main directions around the current spice tile
            bool north = IsSpiceAtPoint(sp.pos + new Vector2Int(0, -1));
            bool east = IsSpiceAtPoint(sp.pos + new Vector2Int(1, 0));
            bool south = IsSpiceAtPoint(sp.pos + new Vector2Int(0, 1));
            bool west = IsSpiceAtPoint(sp.pos + new Vector2Int(-1, 0));

            // Determine sprite name based on neighbors
           /* string spriteName = DetermineSpriteName(current, north, east, south, west);

            // Apply the texture change if sprite exists in atlas
            if (!string.IsNullOrEmpty(spriteName) && spritesMap.ContainsKey(spriteName)) {
                sp.ChangeTexture(spritesMap[spriteName]);
            }*/

            // Update spice type
            UpdateSpiceType(sp, north, east, south, west);

            /*for (int x = -1; x < 2; x++)
            {
                for (int y = -1; y < 2; y++)
                {
                    Vector2Int p = new Vector2Int(x + sp.pos.x, y + sp.pos.y);
                    if (x < 0 || y < 0 || x >= mapSize.x || y > +mapSize.y)
                    {
                        continue;
                    }
                    current = GetSpiceCountAt(sp.pos);
                    if (current > 0)
                    {
                        north = GetSpiceCountAt(sp.pos + new Vector2Int(0, -1));
                        east = GetSpiceCountAt(sp.pos + new Vector2Int(1, 0));
                        south = GetSpiceCountAt(sp.pos + new Vector2Int(0, 1));
                        west = GetSpiceCountAt(sp.pos + new Vector2Int(-1, 0));
                        spriteName = DetermineSpriteName(0, north, east, south, west);
                        // Apply the texture change if sprite exists in atlas
                        if (!string.IsNullOrEmpty(spriteName) && spritesMap.ContainsKey(spriteName))
                        {
                            spice sp2 = AddSpiceAt(p);
                            sp2.SetFake();
                            sp2.ChangeTexture(spritesMap[spriteName]);
                            Debug.LogFormat("addFakeSpice at %d %d",x,y);
                            fakeList[p] = sp2;
                        }
                    }
                }
            }*/
        }
    }

    public void RemoveFakeSpice(Vector2Int pos)
    {
        fakeList.TryGetValue(pos, out var spice);
        if (spice != null)
        {
            fakeList.Remove(pos);
        }
    }

    private int GetSpiceCountAt(Vector2Int pos) {
        // Check if position is within map bounds
        if (pos.x < 0 || pos.x >= mapSize.x || pos.y < 0 || pos.y >= mapSize.y) {
            return 0; // Treat map boundaries as having spice
        }

        // Check if there's a spice at this position
        if (spicesList.TryGetValue(pos, out var spice))
        {
            return spice.GetCount();
            //return spice.type != spice.eSpiceType.kNone;
        }

        return 0;
    }

    private string DetermineSpriteName(bool current, bool north, bool east, bool south, bool west) {
        // Map different neighbor configurations to sprite names from atlas
        // Using the existing atlas sprites that were in the commented code

        if (!east && !south && north && west) {// Up-left
            if (current)
                return "atlas_66";
            return "atlas_70"; 
            
        }
        if (!east && !north && south && west) {// Down-left
            if (current)
                return "atlas_67";
            return "atlas_71";
        }
        if (!west && !south && north && east) {// Up-Right
            if (current)
                return "atlas_64";
            return "atlas_68";
        }
        if (!west && !north && south && east) {// Down-Right
            if (current)
                return "atlas_65";
            return "atlas_69";
        }
        if (north && !east && !south && !west)// Only up
        {
            if (current )
                return "atlas_177";
            return "atlas_193";
        }

        if (!north && east && !south && !west)// Only right
        {
            if (current)
                return "atlas_178";
            return "atlas_194";
        }


        if (!north && !east && south  && !west)// Only down
        {
            if (current)
                return "atlas_180";
            return "atlas_196";
        }

        if (!north && !east && !south && west)// Only left
        {
            if (current)
                return "atlas_184";
            return "atlas_200";
        }


        if (north && !east && south && !west)// Up-down
        {
            if (current)
                return "atlas_181";
            return "atlas_197";
        }

        if (north && east && !south && west)// Left-Right
        {
            if (current)
                return "atlas_186";
            return "atlas_202";
        }

        if (!north && east && south && west) {// Up free
            if (current)
                return "atlas_176";
            return "atlas_190";
        }
        if (north && !east && south && west) {// Right free
            if (current)
                return "atlas_176";
            return "atlas_205";
        }
        if (north && east && !south && west) { // Down free
            if (current)
                return "atlas_176";
            return "atlas_203";
        }
        if (north && east && south && !west) {// left free
            if (current)
                return "atlas_176";
            return "atlas_199";
        }

        // Isolated piece
        if (!north && !east && !south && !west) {
            return "atlas_176"; // Use corner sprite for isolated pieces
        }

        // Default full tile (surrounded by spice or other configurations)
        return null; // Keep original texture
    }

    private void UpdateSpiceType(spice sp, bool north, bool east, bool south, bool west) {
        // Check for corner types
        if (!east && !south && north && west) {
            sp.type = spice.eSpiceType.kRightDown;
        }
        else if (!east && !north && south && west) {
            sp.type = spice.eSpiceType.kRightUp;
        }
        else if (!west && !south && north && east) {
            sp.type = spice.eSpiceType.kLeftDown;
        }
        else if (!west && !north && south && east) {
            sp.type = spice.eSpiceType.kLeftUp;
        }
        else {
            sp.type = spice.eSpiceType.kFull;
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
                if (result < 0.0f) {
                    DestroySpice(s.Value);
                }
                return result;
            }
        }
        return 0;
    }

    public void AddFakeSpiceAt(Vector2Int pos)
    {
        fakeList.TryGetValue(pos, out var s);
        if (s == null)
        {
            var g = UnityEngine.Object.Instantiate(Resources.Load("spice", typeof(GameObject))) as
                GameObject;
            if (g != null)
            {
                s = g.GetComponent<spice>();
                if (s)
                {
                    s.Init(pos.x, pos.y);
                    fakeList[s.pos] = s;
                }
            }
        }
    }

    public spice GetFakeSpiceAt(Vector2Int pos)
    {
        fakeList.TryGetValue(pos, out var s);
        return s;
    }

    public void RemoveFakeSpiceAt(Vector2Int pos)
    {
        fakeList.Remove(pos);
    }
}
