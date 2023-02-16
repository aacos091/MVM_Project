using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D theRB;

    public float moveSpeed;
    public float sprintSpeed;
    public float jumpForce;

    public Transform groundPoint;
    private bool onGround;
    public LayerMask whatIsGround;
    

    // Start is called before the first frame update
    void Start()
    {
        theRB = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        onGround = Physics2D.OverlapCircle(groundPoint.position, .2f, whatIsGround);
        
        if (Input.GetKey(KeyCode.LeftShift) && onGround)
        {
            theRB.velocity = new Vector2(Input.GetAxis("Horizontal") * sprintSpeed, theRB.velocity.y);
        }
        else if (onGround)
        {
            theRB.velocity = new Vector2(Input.GetAxis("Horizontal") * moveSpeed, theRB.velocity.y);
        }

        if (Input.GetButtonDown("Jump") && onGround)
        {
            theRB.velocity = new Vector2(theRB.velocity.x, jumpForce);
        }
        
        
    }
}
