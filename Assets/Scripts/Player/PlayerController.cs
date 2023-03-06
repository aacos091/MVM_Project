using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static PlayerController instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }
    
    private Rigidbody2D _theRb;

    public float moveSpeed;
    public float sprintSpeed;
    public float jumpForce;

    private bool _weaponReady;

    public Transform groundPoint;
    private bool onGround;
    public LayerMask whatIsGround;

    // Just to make sure that melee weapons can at least change properly
    [NotNull] private WeaponManager _weaponMan;
    

    // Start is called before the first frame update
    void Start()
    {
        _theRb = GetComponent<Rigidbody2D>();
        _weaponMan = GetComponentInChildren<WeaponManager>();
    }

    // Update is called once per frame
    void Update()
    {
        onGround = Physics2D.OverlapCircle(groundPoint.position, .2f, whatIsGround);

        //TurnWithMouse();
        
        TurnWithButtons();

        if (Input.GetMouseButton(1))
        {
            _weaponReady = true;
        }
        else
        {
            _weaponReady = false;
        }
        
        if (onGround && !_weaponReady)
        {
            if (Input.GetKey(KeyCode.LeftShift))
            {
                _theRb.velocity = new Vector2(Input.GetAxis("Horizontal") * sprintSpeed, _theRb.velocity.y);
            }
            else
            {
                _theRb.velocity = new Vector2(Input.GetAxis("Horizontal") * moveSpeed, _theRb.velocity.y);
            }

            if (Input.GetButtonDown("Jump"))
            {
                _theRb.velocity = new Vector2(_theRb.velocity.x, jumpForce);
            }
        }

        // Change weapons
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            _weaponMan.ChangeWeapon(WeaponManager.Weapons.Melee);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            _weaponMan.ChangeWeapon(WeaponManager.Weapons.Pistol);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            _weaponMan.ChangeWeapon(WeaponManager.Weapons.Uzi);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            _weaponMan.ChangeWeapon(WeaponManager.Weapons.Shotgun);
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
