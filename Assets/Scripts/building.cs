using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts;
using UnityEngine;

public class building : MonoBehaviour
{
    private Dictionary<string, Sprite> spritesMap;
    public int colour { get; set; }
    protected SpriteRenderer render;
    protected Vector3 pos;
    private String radarName;

    private int prevFrame;
    protected SpriteRenderer pickRenderer;

    public void Init(int x, int y, int color) {
        pos = tools.iPos2PosB( x, y);
        transform.position = pos;
        colour = color;
    }
    // Start is called before the first frame update
    void Start() {
        switch (colour) {
            case 1:
                radarName = "radarred";
                break;
            case 2:
                radarName = "radarblue";
                break;
            case 3:
                radarName = "radarred";
                break;
            case 4:
                radarName = "radarred";
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
                break;
            }
        }
    }

    // Update is called once per frame
    void Update()
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
        Vector2 pos;
        pos.x = transform.position.x;
        pos.y = transform.position.y;
        if (x > pos.x - 0.32f && x < pos.x + 0.32f && y > pos.y - 0.32f && y < pos.y + 0.32f)
        {
            return true;
        }
        return false;
    }

    public void select()
    {
        //pickRenderer.enabled = true;
        //GetComponentInChildren<SpriteRenderer>().enabled = true;
    }

    public void unselect()
    {
        //pickRenderer.enabled = false;
    }
}
