using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using Assets.Scripts;
using Dune2;
using SuperTiled2Unity;
using UnityEditor;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UIElements;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;


public abstract class unit : actionBase {

    protected Camera cam;
    protected SpriteRenderer render;
    protected int direction = 0;
    protected SpriteAtlas atlas;
    protected Vector2Int tilePos;
    protected Vector3 pos;
    protected SpriteRenderer pickRenderer;
    protected Sprite[] sprites = new Sprite[16];
    protected int player = 0;
    public Vector2Int destPos = new Vector2Int();
    public bool canAttack = false;
    public float attackRange;
    public int attackDamage;
    public int health;
    public bool isAttacking = false;
    public bool isDestroying = false;

    public int GetPlayer() {
        return player;
    }

    public int GetDirection() {
        return direction;
    }

    public Vector2Int GetTilePos() {
        return tilePos;
    }

    public void SetTilePos(int x, int y) {
        tilePos.x = x;
        tilePos.y = y;
    }
    // Start is called before the first frame update
    void Start() {
        cam = Camera.main;
        render = GetComponent<SpriteRenderer>();
        base.Init();
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

    }

    private void SetColor(int color) {
      
    }

    // Update is called once per frame
    public void Update() {
        base.Update();
        pos.x = transform.position.x;
        pos.y = transform.position.y;
    }

    public void OnMouseDown() {
        Debug.Log("CLICK!");
        //gameManager.GetInstance().GetUnitManager().moveTo(this,tilePos.x + 1, tilePos.y);
    }

    public void TurnLeft() {
        direction--;
        if (direction < 0)
            direction = 15;
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
        if (x > pos.x -0.32f && x < pos.x + 0.32f && y>pos.y - 0.32f && y<pos.y + 0.32f) {
            return true;
        }

        return false;
    }

    public void Select() {
        pickRenderer.enabled = true;
        //GetComponentInChildren<SpriteRenderer>().enabled = true;
    }

    public void Unselect() {
        pickRenderer.enabled = false;
    }

    public bool IsIdle() {
        return !IsActions() && !isAttacking;
    }

    public void Attack(unit target) {
        isAttacking = true;
    }

    public void Damage(int damage) {
        health -= damage;
        if (health <= 0) {
            DestroyUnit();
        }
    }

    public void DestroyUnit() {
        isDestroying = true;
    }
}
