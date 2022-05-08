using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {
    private bool moveLeft = false;
    private bool moveRight = false;
    private bool moveUp = false;
    private bool moveDown = false;
    private float moveTimer = 0.0f;
    private Vector3 speed;


    public float ortSize;
    public float ortSizeMin = 2f;
    public float ortSizeMax = 7f;


    public float mapX = 0;
    public float mapY = 0;

    private float minX;
    private float maxX;
    private float minY;
    private float maxY;


    public float StartSpeed = 10.0f;
    public float MaxSpeed = 15.0f;
    public float moveStepTime = 3.0f;

    
    // Start is called before the first frame update
    void Start() {
        
        float vertExtent = Camera.main.orthographicSize;
        float horzExtent = vertExtent * Screen.width / Screen.height;
        int mw = gameManager.GetInstance().GetMapSize().x;
        int mh = gameManager.GetInstance().GetMapSize().y;
        mapX = mw*gameManager.GetInstance().GetCellSize().x;
        mapY =  mh* gameManager.GetInstance().GetCellSize().y;
        // Calculations assume map is position at the origin
        minX = horzExtent;
        maxX = mapX  - horzExtent;
        minY = vertExtent - mapY;
        maxY = -vertExtent;
    }

    // Update is called once per frame
    void Update() {
        CheckInput();
        Vector3 pos = transform.position;
        bool isInput = false;
        if (moveLeft) {
            speed.x -= StartSpeed*Time.deltaTime;
            isInput = true;
        }
        else if (moveRight)
        {
            speed.x += StartSpeed * Time.deltaTime;
            isInput = true;
        }
        else {
            speed.x = 0.0f;
        }
        if (moveUp)
        {
            speed.y += StartSpeed * Time.deltaTime;
            isInput = true;
        }
        else if (moveDown)
        {
            speed.y -= StartSpeed * Time.deltaTime;
            isInput = true;
        }
        else {
            speed.y = 0.0f;
        }
        
        if (isInput) {
            moveTimer += Time.deltaTime;
            if (moveTimer > moveStepTime) {
                speed *= MaxSpeed / StartSpeed;
                if (speed.x > MaxSpeed) {
                    speed.x = MaxSpeed;
                }
                else if (speed.x < -MaxSpeed) {
                    speed.x = -MaxSpeed;
                }

                if (speed.y > MaxSpeed) {
                    speed.y = MaxSpeed;
                }
                else if (speed.y < -MaxSpeed) {
                    speed.y = -MaxSpeed;
                }
            }

        }
        else {
            moveTimer = 0.0f;
            speed.x = 0.0f;
            speed.y = 0.0f;
        }


        pos += Time.deltaTime * speed;
        pos.x = Mathf.Clamp(pos.x, minX, maxX);
        pos.y = Mathf.Clamp(pos.y, minY, maxY);

        transform.position = pos;
    }

    void CheckInput() {
        if (Input.GetKey(KeyCode.LeftArrow)) {
            moveLeft = true;
        }
        else
            moveLeft = false;

        if (Input.GetKey(KeyCode.RightArrow))
            moveRight = true;

        else
            moveRight = false;

        if (Input.GetKey(KeyCode.UpArrow)) {
            moveUp = true;
        }
        else moveUp = false;

        if (Input.GetKey(KeyCode.DownArrow)) {
            moveDown = true;
        }
        else moveDown = false;
    }
}
