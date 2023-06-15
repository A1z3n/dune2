using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dune2 {
    public class progressBar : MonoBehaviour {
        private RectTransform rect;

        private Vector3 scale;

        // Start is called before the first frame update
        void Start() {
            rect = GetComponent<RectTransform>();
            scale = rect.transform.localScale;
            scale.x = 0.0f;
            rect.transform.localScale = scale;
        }

        // Update is called once per frame
        void Update() {

        }

        public void SetProgress(float p) {
            if (p > 1)
                p = 1;
            if (p < 0)
                p = 0;
            scale.x = p;
            if (rect == null)
                Start();
            rect.transform.localScale = scale;
        }
    }
}