using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Dune2 {
    public class creditsGUI : MonoBehaviour {
        public TextMeshProUGUI CreditsText;

        private int currentCredits = 0;
        private int destCredits = 0;
        private int startCredits = 0;
        private float speed;

        private float progress = 0.0f;

        // Start is called before the first frame update
        void Start() {
            CreditsText = GetComponent<TextMeshProUGUI>();
        }

        // Update is called once per frame
        void Update() {

            if (destCredits != currentCredits) {
                progress += Time.deltaTime * 0.3f;
                currentCredits =
                    (int)(startCredits + (destCredits - startCredits) * progress * progress * (3 - 2 * progress));
                if (progress >= 1.0f) {
                    currentCredits = destCredits;
                    progress = 0.0f;
                }

                CreditsText.text = $"{currentCredits}";
            }
        }

        public void AddCredits(int num) {
            progress = 0.0f;
            destCredits = destCredits + num;
            startCredits = currentCredits;
        }

        public void SetCredits(int num) {
            currentCredits = num;
            destCredits = num;
            CreditsText.text = $"{currentCredits}";
        }
    }
}