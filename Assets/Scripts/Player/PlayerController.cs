using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

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

    public LightSelectEvent LightSelectEvents;
    
    public GameObject flashlight;
    private bool _isFlashlightOn = false;

    private Animator _playerAnimator;

    private AudioSource _playerAudio;
    public float stepRate = 0.5f;
    public float sprintStepRate;
    public float stepCoolDown;
    public AudioClip[] footstepSounds;

    public bool CanMove;
    

    // Just to make sure that melee weapons can at least change properly
    [NotNull] private WeaponManager _weaponMan;
    

    // Start is called before the first frame update
    void Start()
    {
        _theRb = GetComponent<Rigidbody2D>();
        _weaponMan = GetComponentInChildren<WeaponManager>();
        _playerAnimator = GetComponent<Animator>();
        _playerAudio = GetComponent<AudioSource>();
        CanMove = true;
        // lightToggle = GetComponent<LightSwitcher>();
    }

    // Update is called once per frame
    void Update()
    {
        
        onGround = Physics2D.OverlapCircle(groundPoint.position, .1f, whatIsGround);

        stepCoolDown -= Time.deltaTime;

        //TurnWithMouse();
        
        TurnWithButtons();

        if (Input.GetKeyDown(KeyCode.F) && AbilitiesManager.instance.flashlightFound)
        {
            if (!_isFlashlightOn)
            {
                flashlight.SetActive(true);
                _isFlashlightOn = true;
            }
            else
            {
                flashlight.SetActive(false);
                _isFlashlightOn = false;
            }
        }

        //
        // if (Input.GetKeyDown(KeyCode.V))
        // {
        //     lightToggle.SelectUVLight();
        // }

        if (Input.GetMouseButton(1))
        {
            _weaponReady = true;
        }
        else
        {
            _weaponReady = false;
        }
        
        if (onGround && !_weaponReady && CanMove)
        {
            if (Input.GetKey(KeyCode.LeftShift))
            {
                _theRb.velocity = new Vector2(Input.GetAxis("Horizontal") * sprintSpeed, _theRb.velocity.y);
                //_playerAnimator.SetBool("IsMoving", true);
                _playerAnimator.SetBool("IsSprinting", true);
                PlayFootsteps(sprintStepRate);
            }
            else
            {
                _theRb.velocity = new Vector2(Input.GetAxis("Horizontal") * moveSpeed, _theRb.velocity.y);
                //_playerAnimator.SetBool("IsMoving", true);
                _playerAnimator.SetBool("IsSprinting", false);
                PlayFootsteps(stepRate);
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

    void PlayFootsteps(float footstepRate)
    {
        if ((Input.GetAxis("Horizontal") != 0f || Input.GetAxis("Vertical") != 0f) && stepCoolDown < 0f)
        {
            _playerAudio.pitch = 1f + Random.Range(-0.2f, 0.2f);
            _playerAudio.PlayOneShot(footstepSounds[Random.Range(0, footstepSounds.Length - 1)]);
            stepCoolDown = footstepRate;
        }
    }

    public void OnLightPick()
    {
        LightSelectEvents.InvokeSelectNormalLight();
    }

    // This is the most garbage way to do it, but it'll work for now
    private void OnTriggerEnter2D(Collider2D col)
    {
        Debug.Log("Enter collider");
        
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (col.tag == "BarredEntrance")
            {
                Debug.Log("This door is jammed.");
            }
        }
    }
}
