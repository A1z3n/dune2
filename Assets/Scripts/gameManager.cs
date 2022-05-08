using System.Collections;
using System.Collections.Generic;
using Assets.Scripts;
using SuperTiled2Unity;
using SuperTiled2Unity.Editor;
using UnityEngine;

// The AutoCustomTmxImporterAttribute will force this importer to always be applied.
// Leave this attribute off if you want to choose a custom importer from a drop-down?list instead.[AutoCustomTmxImporter()]

public class gameManager  {
    public Camera mainCamera;
    private static readonly gameManager instance = new gameManager();
    private int youColor;
    private mapManager mMap;
    public string Date { get; private set; }

    private gameManager() {
        youColor = 1;
        mainCamera = Camera.main;
        var mm = GameObject.Find("mapManager");
        mMap = mm.GetComponent<mapManager>();
    }

    public static gameManager GetInstance()
    {
        return instance;
    }

    public Vector2Int GetMapSize() {
        if(mMap!=null)
            return mMap.GetMapSize();
        return new Vector2Int(0, 0);
    }

    public Vector2Int GetTileSize() {
        if(mMap!=null)
            return mMap.GetTileSize();
        return new Vector2Int(0, 0);
    }
    public Vector2 GetCellSize()
    {
        if (mMap != null)
            return mMap.GetCellSize();
        return new Vector2(0, 0);
    }

    public Camera GetCamera() {
        return mainCamera;
    }
    public unitManager GetUnitManager()
    {
        if (mMap != null)
            return mMap.GetUnitManager();
        return null;
    }

    public SuperMap getMap()
    {
        if (mMap != null)
            return mMap.getMap();
        return null;
    }

    public int getYourColor() {
        return youColor;
    }

}