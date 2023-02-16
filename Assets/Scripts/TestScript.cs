using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScript : MonoBehaviour
{
    public Rigidbody2D theRB;

    public float moveSpeed;
    
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Oh gee, uh, just putting this script here to test stuff out, uhm, making sure that the repo can update properly.");
    }

    // Update is called once per frame
    void Update()
    {
        theRB.velocity = new Vector2(Input.GetAxisRaw("Horizontal") * moveSpeed, theRB.velocity.y);
    }
}
