using System;
using System.Collections;
using System.Collections.Generic;
using Dune2;
using UnityEditor;
using UnityEngine;
using UnityEngine.U2D;

public class harvester : unit
{
    enum eHarvesterState {
        kBuilded,
        kSeekSpice,
        kHarvesting,
        kReturnBase,
        kUnload,
        kNone
    }
    // Start is called before the first frame update
    private Dictionary<string, Sprite> spritesMap;
    private eHarvesterState state;
    private Vector2Int destPos = new Vector2Int();
    private SpriteRenderer harvestRender;
    private Transform spiceTransform;
    private int prevFrame = 0;

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

        var spices = GetComponentsInChildren<Transform>();
        foreach (var it in spices)
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
        ChangeState(eHarvesterState.kBuilded);
    }


    // Update is called once per frame
    void Update()
    {
        base.Update();

        if (tilePos.x == destPos.x && tilePos.y == destPos.y)
        {
            if (state == eHarvesterState.kSeekSpice) {
                ChangeState(eHarvesterState.kHarvesting);
            }
        }

        if (state == eHarvesterState.kHarvesting) {
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

    private void ChangeState(eHarvesterState s)
    {
        state = s;
        switch (s) {
            case eHarvesterState.kBuilded: {
                    destPos = SearchSpice();
                    gameManager.GetInstance().GetUnitManager().MoveTo(this, destPos.x, destPos.y);
                    harvestRender.enabled = false;
                    ChangeState(eHarvesterState.kSeekSpice);
                }
                break;
            case eHarvesterState.kSeekSpice:
                break;
            case eHarvesterState.kHarvesting:
                harvestRender.enabled = true;
                break;
            case eHarvesterState.kReturnBase:
                harvestRender.enabled = false;
                break;
            case eHarvesterState.kUnload:
                break;
            case eHarvesterState.kNone:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(s), s, null);
        }
    }

    private Vector2Int SearchSpice() {
        return gameManager.GetInstance().GetMapManager().SearchNearestSpice(tilePos);
    }

    public override void PositionChanged() {
        if (gameManager.GetInstance().GetMapManager().IsSpiceAtPoint(tilePos)) {
            Harvest();
        }
    }

    private void Harvest() {
        //state = eHarvesterState.kHarvesting;
        ChangeState(eHarvesterState.kHarvesting);
    }
}
