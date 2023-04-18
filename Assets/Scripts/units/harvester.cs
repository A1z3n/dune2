using System;
using System.Collections;
using System.Collections.Generic;
using Dune2;
using UnityEditor;
using UnityEngine;
using UnityEngine.U2D;

public class harvester : unit
{
    // Start is called before the first frame update
    private Dictionary<string, Sprite> spritesMap;
    public void Create(int x, int y, int player) {
        base.Create(x,y,player);
        Sprite[] sprites = Resources.LoadAll<Sprite>("harvester"+player);
        spritesMap = new Dictionary<string, Sprite>();

        foreach (Sprite sprite in sprites) {
            spritesMap[sprite.name] = sprite;
        }

        canAttack = false;
        attackDamage = 0;
        attackRange = 0.0f;
        attackSpeed = 0.0f;
        health = 300;
        fullHealth = 300;
        moveSpeed = 0.3f;
        reloadTime = 0.0f;
        bulletNum = 0;
        unitType = eUnitType.kHarvester;
    }


    // Update is called once per frame
    void Update()
    {
        base.Update();
    }

    protected override void applyDirection() {
        if (direction % 2 == 1) return;
        String t = ""+direction/2;
        render.sprite = spritesMap[t];
    }
}
