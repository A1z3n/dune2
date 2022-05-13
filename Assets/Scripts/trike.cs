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
        Sprite[] sprites = Resources.LoadAll<Sprite>("trike"+player);
        spritesMap = new Dictionary<string, Sprite>();

        foreach (Sprite sprite in sprites) {
            spritesMap[sprite.name] = sprite;
        }

        canAttack = true;
        attackDamage = 20;
        attackRange = 3.0f;
        health = 100;
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
    }
}