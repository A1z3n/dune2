using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class destructableObject : actionBase {
    protected int health;
    public bool canAttack = false;
    public float attackRange;
    public int attackDamage;
    public bool isAttacking = false;
    protected Vector2Int tilePos;
    public bool isBuilding = false;
    protected int direction = 0;
    protected int player = 0;
    protected SpriteRenderer pickRenderer = null;
    protected Rect clickRect;
    protected Vector3 pos;

    public int GetPlayer()
    {
        return player;
    }

    public bool CheckPlayer(int pPlayer) {
        return player == pPlayer;
    }

    public int GetHealth() {
        return health;
    }
    public virtual void Damage(int damage) {

    }

    public Vector2Int GetTilePos() {
        return tilePos;
    }

    public void SetTilePos(int x, int y) {
        tilePos.x = x;
        tilePos.y = y;
    }
    public int GetDirection()
    {
        return direction;
    }

    public void Select()
    {
        if(pickRenderer!=null)
            pickRenderer.enabled = true;
        //GetComponentInChildren<SpriteRenderer>().enabled = true;
    }

    public void Unselect()
    {
        if(pickRenderer!=null)
            pickRenderer.enabled = false;
    }
}
