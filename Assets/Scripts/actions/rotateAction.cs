using System;
using System.Collections;
using System.Collections.Generic;
using Dune2;
using UnityEngine;

namespace Dune2 {
    public class rotateAction : action {
        private int startAng;
        private int destAng;
        private float timer = 0.0f;
        private float duration;
        private bool inited = false;
        private unit un;
        private int total_turns = 0;
        private int turns_count = 0;

        public void Init(int dest) {
            duration = 0.3f;
            destAng = dest;
        }

        public override bool Update(actionBase u, float dt) {
            if (!inited) {
                inited = true;
                un = u as unit;
                if (un == null) {
                    return false;
                }

                //maxDirection = un.GetMaxDirection();
                startAng = un.GetDirection();
                if (destAng == startAng) {
                    return false;
                }

                total_turns = (destAng - startAng);
                if (total_turns > 8) {
                    total_turns = total_turns - 16;
                }
                else if (total_turns < -8) {
                    total_turns = total_turns + 16;
                }
            }

            timer += dt;
            if (total_turns > 0) {
                if (timer >= Math.Abs(turns_count + 1) * duration) {
                    turns_count++;
                    un.TurnRight();
                    if (turns_count >= total_turns || cancel) {
                        return false;
                    }
                }
            }
            else if (total_turns < 0) {
                if (timer >= Math.Abs(turns_count + 1) * duration) {
                    turns_count--;
                    un.TurnLeft();
                    if (turns_count <= total_turns || cancel) {
                        return false;
                    }
                }
            }
            else return false;

            float p = timer / duration;
            return true;
        }

        public override eActionType GetActionType() {
            return eActionType.kRotateAction;
        }
    }
}