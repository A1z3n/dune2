using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class miniMapFx : MonoBehaviour
{

    private Texture2D noiseTexture;
    private SpriteRenderer spriteRenderer;

    // Start is called before the first frame update
    void Start()
    {
        /*spriteRenderer = GetComponent<SpriteRenderer>();
        int width = 256, height = 256;
        noiseTexture = new Texture2D(width, height);
        float scale = 20.0f;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                float xCoord = (float)x / width * scale;
                float yCoord = (float)y / height * scale;
                float sample = Mathf.PerlinNoise(xCoord, yCoord);
                Color color = new Color(sample, sample, sample);
                noiseTexture.SetPixel(x, y, color);
            }
        }

        noiseTexture.Apply();
        spriteRenderer.sprite = Sprite.Create(noiseTexture, new Rect(0, 0, width, height), new Vector2(0.5f, 0.5f));
        */
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Activate() {

    }

    public void Deactivate() {

    }
}
