using System.Collections;
using System.Collections.Generic;
using Dune2;
using UnityEngine;

namespace Dune2 {
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
        private int direction;
        private bool force = false;
        private int bulletNum = 1;

        public void Init(destructableObject u, scene pScene, bool pForce) {
            force = pForce;
            target = u;
            mScene = pScene;
            bullets = new List<bullet>();
            targetPos = new Vector2();
            attackerPos = new Vector2();
            timer = 0;
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
                reloadTime = attacker.reloadTime;
                direction = attacker.GetTurnDirection();
                attacker.target = target;
                bulletNum = attacker.bulletNum;
            }

            if (cancel) return false;
            timer += dt;
            if (timer > reloadTime) {
                if (target == null) {
                    attacker.target = null;
                    u.CancelActions();
                    attacker.isAttacking = false;
                    return false;
                }

                if (tools.IsInAttackRange(attacker, target)) {
                    var t = target.GetTargetPos(attacker.GetTilePos());
                    targetPos.x = t.x;
                    targetPos.y = t.y;
                    Vector2Int tp = new Vector2Int();
                    tp.x = tools.RoundPosX(t.x);
                    tp.y = tools.RoundPosY(t.y);
                    var a = attacker.GetTilePos();
                    var dir = tools.GetDirection(tp - a);
                    if (dir != direction) {
                        gameManager.GetInstance().GetUnitManager().Attack(attacker, target, force);
                    }
                    else Fire();
                }
                else {
                    attacker.target = null;
                    attacker.isAttacking = false;
                    if (force) {
                        gameManager.GetInstance().GetUnitManager().MoveToTargetAndAttack(attacker, target);
                    }
                }

                return false;
            }

            return true;
        }


        private void Fire() {
            for (int i = 0; i < bulletNum; i++) {
                GameObject g = scene.Instantiate(Resources.Load("bullet", typeof(GameObject))) as GameObject;
                var b = g.GetComponent<bullet>();
                timer = 0.0f;
                speed = attacker.attackSpeed;
                reloadTime = attacker.reloadTime;
                var t = target.GetTargetPos(attacker.GetTilePos());
                attackerPos.x = attacker.transform.position.x;
                attackerPos.y = attacker.transform.position.y;
                b.target = target;
                b.inRange = true;
                b.speed = speed;
                b.damage = damage;
                b.transform.position = attackerPos;
                b.Init(attacker, target, i * 0.1f);
            }

            gameManager.GetInstance().GetUnitManager().Attack(attacker, target, force);
            //bullets.Add(b);
        }

        public override eActionType GetActionType() {
            return eActionType.kAttackAction;
        }
    }
}