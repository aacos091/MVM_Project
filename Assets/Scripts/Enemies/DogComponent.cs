using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DogComponent : MonoBehaviour
{
    public float moveSpeed;
    public Transform target;
    private float _step;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        _step = moveSpeed * Time.deltaTime;

        transform.position = Vector2.MoveTowards(transform.position, target.position, _step);
    }
}
