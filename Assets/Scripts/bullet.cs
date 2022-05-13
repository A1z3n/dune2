using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bullet : MonoBehaviour
{
    public unit target;
    private Vector3 targetPos;
    public bool inRange = false;
    public float args;
    public float speed;
    public int damage;
    // Start is called before the first frame update
    void Start()
    {
        targetPos = target.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (inRange == true)
        {
            transform.position = Vector2.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);
            //if (V3Equal(transform.position, target.position)) Destroy(gameObject);
            if (V3Equal(transform.position, targetPos))
            {
                //if (target != null) target.GetComponent<unitManager>().SendMessage("Damaged", args);
                target.Damage(damage);
                Destroy(gameObject);
            }
        }
    }
    public bool V3Equal(Vector3 a, Vector3 b)
    {
        return Vector3.SqrMagnitude(a - b) < 0.00001;
    }
}
