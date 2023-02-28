using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoodComponent : MonoBehaviour
{
    public float slowMoveSpeed, fastMoveSpeed;
    public bool fastHood, slowHood;
    public Transform target;
    private float _step;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (slowHood)
        {
            _step = slowMoveSpeed * Time.deltaTime;
        }
        else if (fastHood)
        {
            _step = fastMoveSpeed * Time.deltaTime;
        }
        

        transform.position = Vector2.MoveTowards(transform.position, target.position, _step);
    }
}
