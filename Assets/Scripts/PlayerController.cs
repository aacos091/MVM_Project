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
        
        if (onGround)
        {
            if (Input.GetAxis("Horizontal") < 0f )
            {
                transform.localScale = new Vector3(-1f, 1f, 1f);
            }
            else
            {
                transform.localScale = new Vector3(1f, 1f, 1f);
            }
        
            if (Input.GetKey(KeyCode.LeftShift))
            {
                theRB.velocity = new Vector2(Input.GetAxis("Horizontal") * sprintSpeed, theRB.velocity.y);
            }
            else
            {
                theRB.velocity = new Vector2(Input.GetAxis("Horizontal") * moveSpeed, theRB.velocity.y);
            }

            if (Input.GetButtonDown("Jump"))
            {
                theRB.velocity = new Vector2(theRB.velocity.x, jumpForce);
            }
        }
    }
}
