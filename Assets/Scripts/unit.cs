using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using Assets.Scripts;
using Dune2;
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

    public int GetPlayer() {
        return player;
    }

    public int GetDirection() {
        return direction;
    }

    public Vector2Int getTilePos() {
        return tilePos;
    }

    public void setTilePos(int x, int y) {
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
        if (player == color) {
            return;
        }

        switch (color) {
            case 1:
                
            break;
            case 2:

                break;
        }
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

    public void turnLeft() {
        direction--;
        if (direction < 0)
            direction = 15;
        applyDirection();

    }

    public void turnRight()
    {
        direction++;
        if (direction >= 16)
            direction = 0;
        applyDirection();
    }

    protected abstract void applyDirection();

    public bool isRect(float x, float y) {
        if (x > pos.x -0.32f && x < pos.x + 0.32f && y>pos.y - 0.32f && y<pos.y + 0.32f) {
            return true;
        }

        return false;
    }

    public void select() {
        pickRenderer.enabled = true;
        //GetComponentInChildren<SpriteRenderer>().enabled = true;
    }

    public void unselect() {
        pickRenderer.enabled = false;
    }
}
