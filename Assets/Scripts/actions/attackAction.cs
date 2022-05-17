using System.Collections;
using System.Collections.Generic;
using Assets.Scripts;
using UnityEngine;

public class attackAction : action {
    private Vector2 targetPos;
    private bool inited = false;
    private destructableObject target;
    private float timer = 0.0f;
    private float speed = 10.0f;
    private float reloadTime = 2.0f;
    private unit attacker;
    private scene mScene;
    private List<bullet> bullets;
    private float flyTime;
    private Vector2 attackerPos;
    private int damage;

    public void Init(destructableObject u, scene pScene) {
        target = u;
        mScene = pScene;
        bullets = new List<bullet>();
        targetPos = new Vector2();
        attackerPos = new Vector2();
        timer = reloadTime;
    }




    public override bool Update(actionBase u, float dt) {
        if (!inited) {
            inited = true;
            attacker = u as unit;
            if (attacker == null) {
                return false;
            }
            attacker.isAttacking = true;
            damage = attacker.attackDamage;
        }
       
        timer += dt;
        if (cancel) return false;
        if (timer > reloadTime) {
            timer = 0.0f;
            if (target == null) {
                u.CancelActions();
                return false;
            }
            if (tools.IsInAttackRange(attacker, target)) {
                Fire();
            }
            else {
                attacker.isAttacking = false;
                return false;
            }
        }
        
        return true;
    }

    public void Fire() {
        GameObject g = scene.Instantiate(Resources.Load("bullet", typeof(GameObject))) as GameObject;
        var b = g.GetComponent<bullet>();

        //GameObject a = (GameObject)mScene.Instantiate(bullet,
        //    attacker.transform.position, // default position
        //    Quaternion.identity); // default rotation
        // set its target
        //targetPos.x = target.transform.position.x;
        //targetPos.y = target.transform.position.y;
        var t = target.GetTargetPos(attacker.GetTilePos());
        targetPos.x = t.x;
        targetPos.y = t.y;
        attackerPos.x = attacker.transform.position.x;
        attackerPos.y = attacker.transform.position.y;
        b.target = target;
        b.inRange = true;
        b.speed = speed;
        b.damage = damage;
        b.transform.position = attackerPos;
        b.Init(attacker,target);
        //bullets.Add(b);
    }
}
