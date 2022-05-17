using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts;
using UnityEngine;

public class building : destructableObject
{
    private Dictionary<string, Sprite> spritesMap;
    protected SpriteRenderer render;
    private String radarName;
    private int prevFrame;
    protected RectInt rect;

    public virtual void Init(int x, int y, int player) {
        pos = tools.iPos2PosB( x, y);
        transform.position = pos;
        rect = new RectInt {
            x = x,
            y = y,
            width = 2,
            height =2,
        };
        tilePos = new Vector2Int(x, y);
        this.player = player;
        isBuilding = true;
        var comps = GetComponentsInChildren<SpriteRenderer>();
        foreach (var it in comps)
        {
            if (it.name == "pick_b")
            {
                pickRenderer = it;
                break;
            }
        }

        clickRect = new Rect {
            x = pos.x,
            y = pos.y-rect.height,
            width = rect.width,
            height = rect.height
        };
    }
    // Start is called before the first frame update
    public void Start() {
        switch (player) {
            case 1:
                radarName = "radarblue";
                break;
            case 2:
                radarName = "radarred";
                break;
            case 3:
                radarName = "radargreen";
                break;
            case 4:
                radarName = "radarpink";
                break;
        }
        Sprite[] sprites = Resources.LoadAll<Sprite>(radarName);

        spritesMap = new Dictionary<string, Sprite>();

        foreach (Sprite sprite in sprites)
        {
            spritesMap[sprite.name] = sprite;
        }
        var list = GetComponentsInChildren<SpriteRenderer>();
        foreach (var it in list)
        {
            if (it.name == radarName)
            {
                render = it.GetComponent<SpriteRenderer>();
            }
            else if(it.name.Contains("radar")) {
                it.GetComponent<SpriteRenderer>().enabled = false;
            }
        }
    }

    // Update is called once per frame
    public void Update()
    {
        int t = Time.frameCount / 90;
        t = t % 8;
        if (t != prevFrame)
        {
            render.sprite = spritesMap[radarName+"-" + t];
            prevFrame = t;
        }
    }


    public bool isRect(float x, float y) {
        //if (x > pos.x && x < pos.x + rect.width*0.64f && y > pos.y && y < pos.y - rect.height*0.64f)
        if (clickRect.Contains(new Vector2(x, y)))
        {
            return true;
        }
        return false;
    }



    public RectInt GetTileRect() {
        return rect;
    }

    public Vector2Int GetTilePosFrom(Vector2Int from) {
        return tools.GetNearestFromRect(from,rect);
    }

    public override Vector2 GetTargetPos(Vector2Int from) {
        var n = GetTilePosFrom(from);
        return new Vector2(n.x+0.5f,-n.y-0.5f);
    }
}
