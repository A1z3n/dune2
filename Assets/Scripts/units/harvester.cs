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

    public void Create(int x, int y, int player)
    {
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

        }
    }

    protected override void ApplyDirection() {
        if (direction % 2 == 1) return;
        String t = ""+direction/2;
        if(render)  
            render.sprite = spritesMap[t];
    }

    private void ChangeState(eHarvesterState s) {
        switch (s) {
            case eHarvesterState.kBuilded: {
                    destPos = SearchSpice();
                    gameManager.GetInstance().GetUnitManager().MoveTo(this, destPos.x, destPos.y);
                    ChangeState(eHarvesterState.kSeekSpice);
                }
                break;
            case eHarvesterState.kSeekSpice:
                break;
            case eHarvesterState.kHarvesting:
                break;
            case eHarvesterState.kReturnBase:
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
        state = eHarvesterState.kHarvesting;

    }
}
