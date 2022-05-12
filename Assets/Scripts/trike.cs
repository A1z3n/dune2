using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.U2D;

public class trike : unit
{
    // Start is called before the first frame update
    private Dictionary<string, Sprite> spritesMap;
    public void Create(int x, int y, int player) {
        base.Create(x,y,player);
        //atlas = AssetDatabase.LoadAssetAtPath("Assets/Sprites/Units/Trike/trike.spriteatlas",
        //  typeof(SpriteAtlas)) as SpriteAtlas;
        //render.sprite = atlas.GetSprite("trike_00");
        Sprite[] sprites = Resources.LoadAll<Sprite>("trike");
        spritesMap = new Dictionary<string, Sprite>();

        foreach (Sprite sprite in sprites) {
            spritesMap[sprite.name] = sprite;
        }


        //var sprite1 = spritesDict["sprite01"];
    }


    // Update is called once per frame
    void Update()
    {
        base.Update();
    }

    protected override void applyDirection() {
        String t;
        if (direction > 9)
        {
            t = "trike_" + direction;
        }
        else
        {
            t = "trike_0" + direction;
        }

        render.sprite = spritesMap[t];
        ////render.sprite = atlas.GetSprite(t);
        //var rot = transform.rotation;
        //rot.z = direction * (float)Math.PI / 8.0f;
        //transform.rotation = rot;
    }
}
