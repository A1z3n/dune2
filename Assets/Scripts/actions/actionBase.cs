using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SuperTiled2Unity;
using UnityEngine;

namespace Dune2 {
    public class actionBase : MonoBehaviour {
        // Start is called before the first frame update
        private List<action> actions;
        private List<action> actionsToAdd;
        private List<action> actionsDelayed;
        private List<action> delItems;

        public actionBase() {
            actions = new List<action>();
            delItems = new List<action>();
            actionsToAdd = new List<action>();
            actionsDelayed = new List<action>();
        }


        // Update is called once per frame
        protected void Update() {
            float dt = Time.deltaTime;
            

            foreach (var currentAction in actions)
            {
                if (!currentAction.IsRemoving() && !currentAction.Update(this, dt))
                {
                    currentAction.OnEndCallback?.Invoke();
                    delItems.Add(currentAction);
                }
            }

            if (!delItems.IsEmpty()) {
                foreach (var delItem in delItems) {
                    actions.Remove(delItem);
                }

                delItems.Clear();
            }

            if (actions.IsEmpty()) {
                if (!actionsToAdd.IsEmpty()) {

                    AddActionInternal(actionsToAdd[0]);
                    actionsToAdd.RemoveAt(0);
                }
            }

            if (!actionsDelayed.IsEmpty()) {
                foreach (var a in actionsDelayed) {
                    AddActionInternal(a);
                }

                actionsDelayed.Clear();
            }

        }

        private void AddActionInternal(action a) {
            if (actions.Equals(null)) {
                actions = new List<action>();
            }
            actions.Add(a);

        }

        public void AddActionSeq(action a) {
            actionsToAdd.Add(a);
        }

        public void AddAction(action a) {
            actionsDelayed.Add(a);
        }

        private void RemoveActionInternal(action a) {
            actions.Remove(a);
        }
        public void RemoveAction(action a)
        {
            delItems.Add(a);
        }

        public void ClearActions() {
            actions.Clear();
            actionsToAdd.Clear();
        }
     

        public List<action> GetActions() {
            return actions;
        }

        public void ClearActionsType(eActionType a) {
            foreach(var currentAction in actions)
            {
                if (currentAction.GetActionType()==a) {
                    RemoveAction(currentAction);
                }
            }
        }

   

        public bool IsActions() {
            return !actions.IsEmpty() || !actionsToAdd.IsEmpty() || !actionsDelayed.IsEmpty();
        }

        public void CancelActions() {
            foreach (var currentAction in actions) {
                currentAction.Cancel();
            }

            actionsToAdd.Clear();
            actionsDelayed.Clear();
        }

        public void CancelNextActions() {
            int l = actions.Count();
            if (l > 1) {
                for (int i = 1; i < l; i++) {
                    actions[i].Cancel();
                }
            }
            actionsToAdd.Clear();
            actionsDelayed.Clear();
        }



    }
}