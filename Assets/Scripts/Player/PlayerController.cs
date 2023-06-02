using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class PlayerController : MonoBehaviour
{
    // This is to ensure the player persists between levels, so the inventory and abilites remain
    public static PlayerController instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    private Rigidbody2D _theRb;

    public float moveSpeed;
    public float sprintSpeed;
    public float jumpForce;

    private bool _weaponReady;
    public bool canUseWeapons = true;

    public Transform groundPoint;
    private bool onGround;
    public LayerMask whatIsGround;

    public GameObject flashlight;
    public Animator flashlightAnimator;
    private bool _isFlashlightOn = false;

    private Animator _playerAnimator;

    private AudioSource _playerAudio;
    public float stepRate = 0.5f;
    public float sprintStepRate;
    public float stepCoolDown;
    public AudioClip[] footstepSounds;

    public AudioClip jumpSound;

    public bool CanMove;

    private bool _nearBarredEntrance = false;
    private bool _nearRealEntrance = false;

    private bool _nearShotgun = false;


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
    }

    // Update is called once per frame
    void Update()
    {
        onGround = Physics2D.OverlapCircle(groundPoint.position, .1f, whatIsGround);
        
        _playerAnimator.SetBool("IsOnGround", onGround);

        stepCoolDown -= Time.deltaTime;
        
        if(!CanMove)
        {
            return;
        }

        OperateFlashlight();

        PlayerMovement();
        
        PlayerWeapons();
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

    // Moving the player, along with the appropriate animations and sounds
    void PlayerMovement()
    {
        //TurnWithMouse();
        
        TurnWithButtons();
        
        if (onGround && !_weaponReady && CanMove)
        {
            if (Input.GetButtonDown("Jump"))
            {
                _theRb.velocity = new Vector2(_theRb.velocity.x, jumpForce);
                _playerAudio.PlayOneShot(jumpSound, 2);
                _playerAnimator.SetTrigger("Jumped");
            }
            
            if (Input.GetAxis("Horizontal") != 0f)
            {
                _playerAnimator.SetBool("IsMoving", true);
            }
            else
            {
                _playerAnimator.SetBool("IsMoving", false);
            }
            
            if (Input.GetKey(KeyCode.LeftShift))
            {
                _theRb.velocity = new Vector2(Input.GetAxis("Horizontal") * sprintSpeed, _theRb.velocity.y);
                _playerAnimator.SetBool("IsSprinting", true);
                PlayFootstepSounds(sprintStepRate);
            }
            else
            {
                _theRb.velocity = new Vector2(Input.GetAxis("Horizontal") * moveSpeed, _theRb.velocity.y);
                _playerAnimator.SetBool("IsSprinting", false);
                PlayFootstepSounds(stepRate);
            }
        }
    }

    // Operate the flashlight, if the player found one
    void OperateFlashlight()
    {
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
        
        if (_isFlashlightOn)
        {
            if (Input.GetAxis("Vertical") > 0.2f)
            {
                flashlightAnimator.SetBool("FlashlightUp", true);
            }
            else if (Input.GetAxis("Vertical") < -0.2f)
            {
                flashlightAnimator.SetBool("FlashlightDown", true);
            }
            else
            {
                flashlightAnimator.SetBool("FlashlightUp", false);
                flashlightAnimator.SetBool("FlashlightDown", false);
            }
        }
    }

    void PlayFootstepSounds(float footstepRate)
    {
        if ((Input.GetAxis("Horizontal") != 0f) && stepCoolDown < 0f)
        {
            _playerAudio.pitch = 1f + Random.Range(-0.2f, 0.2f);
            _playerAudio.PlayOneShot(footstepSounds[Random.Range(0, footstepSounds.Length - 1)]);
            stepCoolDown = footstepRate;
        }
    }

    void PlayerWeapons()
    {
        if (Input.GetMouseButton(1) && canUseWeapons)
        {
            _weaponReady = true;
        }
        else
        {
            _weaponReady = false;
        }
        
        // Change weapons
        // if (Input.GetKeyDown(KeyCode.Alpha1))
        // {
        //     _weaponMan.ChangeWeapon(WeaponManager.Weapons.Melee);
        // }
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            _weaponMan.ChangeWeapon(WeaponManager.Weapons.Pistol);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            _weaponMan.ChangeWeapon(WeaponManager.Weapons.Uzi);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            _weaponMan.ChangeWeapon(WeaponManager.Weapons.Shotgun);
        }
    }
}
