using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Dune2;
using UnityEditor;
using UnityEngine;
using UnityEngine.U2D;

public class harvester : unit
{
    // Start is called before the first frame update
    private Dictionary<string, Sprite> spritesMap;
    private Vector2Int destPos = new Vector2Int();
    private SpriteRenderer harvestRender;
    private Transform spiceTransform;
    private int prevFrame = 0;
    [SerializeField] private int spices = 0;
    private int prevSpices = 0;
    //[SerializeField] private const int HARVEST_SPEED = 50;
    [SerializeField] private const int MAX_SPICES = 700;
    private bool isHarvest = false;
    public void Create(int x, int y, int player)
    {
        base.Create(x,y,player);
        Sprite[] sprites = Resources.LoadAll<Sprite>("harvester"+player);
        spritesMap = new Dictionary<string, Sprite>();

        foreach (Sprite sprite in sprites) {
            spritesMap[sprite.name] = sprite;
        }

        sprites = Resources.LoadAll<Sprite>("spice");
        foreach (Sprite sprite in sprites)
        {
            spritesMap[sprite.name] = sprite;
        }

        var lSpices = GetComponentsInChildren<Transform>();
        foreach (var it in lSpices)
        {
            if (it.name == "harvest")
            {
                spiceTransform = it;
                break;
            }
        }
        var list = GetComponentsInChildren<SpriteRenderer>();
        foreach (var it in list) {
            if (it.name == "harvest") {
                harvestRender = it;
                break;
            }
        }


        canAttack = false;
        attackDamage = 0;
        attackRange = 0.0f;
        attackSpeed = 0.0f;
        health = 300;
        fullHealth = 300;
        moveSpeed = 0.5f;
        reloadTime = 0.0f;
        bulletNum = 0;
        unitType = eUnitType.kHarvester;
        direction = 12;
        actionManager.Harvest(this);
        harvestRender.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        base.Update();

        if (isHarvest) {
            
            int t = Time.frameCount / 90;
            t = t % 3;
            if (t != prevFrame) {
                harvestRender.sprite = spritesMap["spice_" + t];
                prevFrame = t;
            }

        }
    }

    protected override void ApplyDirection() {
        if (direction % 2 == 1) return;
        String t = "harvester_0"+direction/2;
        if(render)  
            render.sprite = spritesMap[t];
        var spicePos = new Vector3(0,0,0);
        switch (direction/2) {
            case 0:
                spicePos.x = -0.16f;
                spicePos.y = 0;
                break;
            case 1:
                spicePos.x = -0.113f;
                spicePos.y = 0.113f;
                break;
            case 2:
                spicePos.x = 0;
                spicePos.y = 0.16f;
                break;
            case 3:
                spicePos.x = 0.113f;
                spicePos.y = 0.113f;
                break;
            case 4:
                spicePos.x = 0.16f;
                spicePos.y = 0;
                break;
            case 5:
                spicePos.x = 0.113f;
                spicePos.y = -0.113f;
                break; 
            case 6:
                spicePos.x = 0;
                spicePos.y = -0.16f;
                break;
            case 7:
                spicePos.x = -0.113f;
                spicePos.y = -0.113f;
                break;

        }

        spiceTransform.localPosition = spicePos;
    }

   

    public Vector2Int SearchSpice() {
        return gameManager.GetInstance().GetMapManager().SearchNearestSpice(tilePos);
    }

    public override void PositionChanged() {

        if (GetSpiceCount()<MAX_SPICES && gameManager.GetInstance().GetMapManager().IsSpiceAtPoint(tilePos)) {
            //ChangeHarvestAnimation(true); 
            this.ClearActionsType(eActionType.kHarvestAction);
            harvestAction a = new harvestAction();
            this.AddAction(a);
        }
        else {
            ChangeHarvestAnimation(false);
        }
    }


    public void ChangeHarvestAnimation(bool active) {
        harvestRender.enabled = active;
        isHarvest = true;
    }


    public int GetSpiceCount() {
        return spices;
    }

    public int GetMaxSpices() {
        return MAX_SPICES;
    }

    public SpriteRenderer GetHarvestRender() {
        return harvestRender;
    }

    public void AddSpice(int count) {
        spices += count;
        if (spices < 0) spices = 0;
        if (spices > MAX_SPICES) spices = MAX_SPICES;
    }

}
