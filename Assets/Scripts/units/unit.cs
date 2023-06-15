using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using Dune2;
using SuperTiled2Unity;
using UnityEditor;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UIElements;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;


public abstract class unit : destructableObject {

    protected Camera cam;
    protected SpriteRenderer render;
    protected SpriteAtlas atlas;
    protected Sprite[] sprites = new Sprite[16];
    public Vector2Int destPos = new Vector2Int();
    public bool isDestroying = false;
    protected float moveSpeed;
    protected eUnitType unitType { get; set; }

    // Start is called before the first frame update
    void Start() {
        cam = Camera.main;
        render = GetComponent<SpriteRenderer>();
        var comps = GetComponentsInChildren<SpriteRenderer>();
        foreach (var it in comps) {
            if (it.name == "pick") {
                pickRenderer = it;
                break;
            }
        }

    }

    public void Create(int x, int y, int pPlayer) {
        tilePos = new Vector2Int();
        tilePos.x = x;
        tilePos.y = y;
        pos = tools.iPos2Pos(x, y);
        transform.position = pos;
        this.player = pPlayer;
        clickRect = new Rect {
            x = pos.x - 0.32f,
            y = pos.y - 0.32f,
            width = 0.64f,
            height = 0.64f
        };
    }

    private void SetColor(int color) {
      
    }

    // Update is called once per frame
    public void Update() {
        base.Update();
        pos.x = transform.position.x;
        pos.y = transform.position.y;
        clickRect.x = pos.x-0.32f;
        clickRect.y = pos.y - 0.32f;
    }

    public void OnMouseDown() {
        Debug.Log("CLICK!");
        //gameManager.GetInstance().GetUnitManager().moveTo(this,tilePos.x + 1, tilePos.y);
    }

    public void TurnLeft() {
        direction--;
        if (direction < 0)
            direction = 16-1;
        applyDirection();

    }

    public void TurnRight()
    {
        direction++;
        if (direction >= 16)
            direction = 0;
        applyDirection();
    }

    protected abstract void applyDirection();

    public bool IsRect(float x, float y) {
        //if (x > pos.x -0.32f && x < pos.x + 0.32f && y>pos.y - 0.32f && y<pos.y + 0.32f) {
        if(clickRect.Contains(new Vector2(x,y))){
            return true;
        }

        return false;
    }
    
    public bool IsIdle() {
        return !IsActions() && !isAttacking;
    }

    public void Attack() {
        isAttacking = true;
    }

    public override void Damage(int damage) {
        health -= damage;
        if (health <= 0) {
            DestroyUnit();
        }
    }

    public void DestroyUnit()
    {
        isDestroying = true;
        actionManager.DestroyUnit(this);
    }

    public void DestroyMe() {
        Destroy(gameObject);
    }

    public float GetMoveSpeed() {
        return moveSpeed;
    }
    


}
