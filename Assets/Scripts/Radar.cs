using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Radar : MonoBehaviour
{
    private Dictionary<string, Sprite> spritesMap;
    protected SpriteRenderer render;

    private int prevFrame;
    // Start is called before the first frame update

    void Start()
    {
        Sprite[] sprites = Resources.LoadAll<Sprite>("radarred");

        spritesMap = new Dictionary<string, Sprite>();

        foreach (Sprite sprite in sprites)
        {
            spritesMap[sprite.name] = sprite;
        }
        var list = GetComponentsInChildren<SpriteRenderer>();
        foreach (var it in list) {
            if (it.name == "radarred") {
                render = it.GetComponent<SpriteRenderer>();
                break;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        int t = Time.frameCount/90;
        t = t % 8;
        if (t != prevFrame) {
            render.sprite = spritesMap["radarred-" + t];
            prevFrame = t;
        }
    }
}
