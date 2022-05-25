using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class progressBar : MonoBehaviour {
    private RectTransform rect;

    private float startWidth;
    // Start is called before the first frame update
    void Start() {
        rect = GetComponent<RectTransform>();
        startWidth = rect.rect.width;
        rect.transform.position.Scale(new Vector3(0.0f,1.0f,1.0f));
        //rect.localScale.Set(0,1.0f,1.0f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetProgress(float p) {
        if (p > 1)
            p = 1;
        if (p < 0)
            p = 0;
        rect.transform.position.Scale(new Vector3(p, 1.0f, 1.0f));
        //rect.localScale.Set(startWidth * p,1.0f,1.0f);
    }
}
