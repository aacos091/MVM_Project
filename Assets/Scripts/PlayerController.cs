using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D theRB;

    public float moveSpeed;
    public float sprintSpeed;
    public float jumpForce;

    private bool _isAiming;

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

        //TurnWithMouse();
        
        TurnWithButtons();

        if (Input.GetMouseButton(1))
        {
            _isAiming = true;
        }
        else
        {
            _isAiming = false;
        }
        
        if (onGround && !_isAiming)
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

    // Use this if you're planning to aim with the mouse and/or thumbstick
    void TurnWithMouse()
    {
        Vector3 gunPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        
        if (gunPos.x < transform.position.x)
        {
            transform.eulerAngles = new Vector3(transform.rotation.x, 180f, transform.rotation.z);
        }
        else
        {
            transform.eulerAngles = new Vector3(transform.rotation.x, 0f, transform.rotation.z);
        }
    }

    // Use this if you want to aim in pre-defined directions
    void TurnWithButtons()
    {
        if (Input.GetAxis("Horizontal") < -0.2f)
        {
            transform.localScale = new Vector3(-1f, 1f, 1f);
        }
        else if (Input.GetAxis("Horizontal") > 0.2f)
        {
            transform.localScale = new Vector3(1f, 1f, 1f);
        }
    }
}
