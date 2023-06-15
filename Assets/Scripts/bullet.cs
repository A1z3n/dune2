using System;
using System.Collections;
using System.Collections.Generic;
using Dune2;
using UnityEngine;

namespace Dune2 {
    public class bullet : MonoBehaviour {
        public destructableObject target;
        private Vector2 targetPos;
        private Vector2 startPos;
        public bool inRange = false;
        public float args;
        public float speed;
        public int damage;
        private float progress = 0.0f;
        private Vector2Int targetTilePos;
        private float waitTime;

        // Start is called before the first frame update
        void Start() {
        }

        public void Init(destructableObject attacker, destructableObject pTarget, float pWaitTime) {
            target = pTarget;
            targetPos = target.GetTargetPos(attacker.GetTilePos());
            targetTilePos = pTarget.GetTilePos();
            startPos = attacker.transform.position;
            speed = attacker.attackSpeed;
            waitTime = pWaitTime;
        }

        // Update is called once per frame
        void Update() {
            /*if (inRange == true)
            {
                transform.position = Vector2.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);
                //if (V3Equal(transform.position, target.position)) Destroy(gameObject);
                if (V3Equal(transform.position, targetPos))
                {
                    //if (target != null) target.GetComponent<unitManager>().SendMessage("Damaged", args);
                    target.Damage(damage);
                    Destroy(gameObject);
                }
            }*/

            if (waitTime > 0) {
                waitTime -= Time.deltaTime;
                return;
            }

            progress += speed * Time.deltaTime;
            if (progress > 1.0f) {
                progress = 1.0f;
                if (target.GetTilePos() != targetTilePos) {
                    var actions = target.GetActions();
                    foreach (var a in actions) {
                        if (a.GetActionType() == eActionType.kMoveAction) {
                            float p = (a as moveAction).GetProgress();
                            if (p > 0.5f) {
                                Destroy(gameObject);
                                return;
                            }

                            break;
                        }
                    }
                }

                target.Damage(damage);
                Destroy(gameObject);
            }

            transform.position = startPos + (targetPos - startPos) * progress;
        }

        public bool V3Equal(Vector3 a, Vector3 b) {
            return Vector3.SqrMagnitude(a - b) < 0.00001;
        }
    }
}
