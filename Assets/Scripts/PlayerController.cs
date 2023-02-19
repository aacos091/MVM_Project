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

        Vector3 gunPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        
        if (gunPos.x < transform.position.x)
        {
            transform.eulerAngles = new Vector3(transform.rotation.x, 180f, transform.rotation.z);
        }
        else
        {
            transform.eulerAngles = new Vector3(transform.rotation.x, 0f, transform.rotation.z);
        }
        
        if (onGround)
        {
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
