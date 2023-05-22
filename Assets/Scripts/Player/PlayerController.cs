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

    public GameObject flashlight;
    public Animator flashlightAnimator;
    private bool _isFlashlightOn = false;

    private Animator _playerAnimator;

    private AudioSource _playerAudio;
    public float stepRate = 0.5f;
    public float sprintStepRate;
    public float stepCoolDown;
    public AudioClip[] footstepSounds;

    public AudioClip jumpSound, damagedSound, deathSound;

    public bool CanMove;

    public HealthManager healthMan;
    
    //private bool

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
        healthMan = GetComponent<HealthManager>();
        CanMove = true;
    }

    // Update is called once per frame
    void Update()
    {
        UIController.instance.PlayerHealth(healthMan.playerHealth);
        
        onGround = Physics2D.OverlapCircle(groundPoint.position, .1f, whatIsGround);
        
        _playerAnimator.SetBool("IsOnGround", onGround);

        stepCoolDown -= Time.deltaTime;

        //TurnWithMouse();
        
        TurnWithButtons();

        if(!CanMove)
        {
            return;
        }

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
        
        OperateFlashlight();

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
                PlayFootsteps(sprintStepRate);
            }
            else
            {
                _theRb.velocity = new Vector2(Input.GetAxis("Horizontal") * moveSpeed, _theRb.velocity.y);
                _playerAnimator.SetBool("IsSprinting", false);
                PlayFootsteps(stepRate);
            }
        }
        
        Interact();

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

    void OperateFlashlight()
    {
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

    void PlayFootsteps(float footstepRate)
    {
        if ((Input.GetAxis("Horizontal") != 0f) && stepCoolDown < 0f)
        {
            _playerAudio.pitch = 1f + Random.Range(-0.2f, 0.2f);
            _playerAudio.PlayOneShot(footstepSounds[Random.Range(0, footstepSounds.Length - 1)]);
            stepCoolDown = footstepRate;
        }
    }

    // This is the most garbage way to do it, but it'll work for now
    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("BarredEntrance"))
        {
            Debug.Log("near entrance");
            _nearBarredEntrance = true;
        }

        if (col.CompareTag("RealEntrance"))
        {
            Debug.Log("near the real entrance");
            _nearRealEntrance = true;
        }

        if (col.CompareTag("EnemyHitbox"))
        {
            Debug.Log("Player was hit");
            PlayerDamage(col.GetComponent<HitboxAttack>()._attackPower);
        }

        if (col.CompareTag("Shotgun"))
        {
            Debug.Log("near Shotgun");
            _nearShotgun = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("BarredEntrance"))
        {
            Debug.Log("left entrance");
            _nearBarredEntrance = false;
        }

        if (other.CompareTag("RealEntrance"))
        {
            Debug.Log("left the real entrance");
            _nearRealEntrance = false;
        }

        if (other.CompareTag("Shotgun"))
        {
            Debug.Log("left shotgun");
            _nearShotgun = false;
        }
    }

    private void Interact()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (_nearBarredEntrance)
            {
                _ = Time.unscaledTime;
                Debug.Log("Door's busted. Can't get in.");
                DialogueController.instance.StartDialogue();
                //OLD CODE: GameObject.FindWithTag("BarredEntrance").GetComponent<BarredEntranceInteract>().EnterBarredEntrance();
                //DialogueController.instance.dialogue.text = "The door is busted. No way I can get in there.";
                //StartCoroutine(DialogueController.instance.DisplayStrings(new string[]{"This door is busted.", "I can't get in."}));
            }

            if (_nearRealEntrance)
            {
                _ = Time.unscaledTime;
                Debug.Log("Looks like the basement window can be broken."); // Fade out from this scene and than go into the maintenance area, maybe do that from the game manager (another singleton)?
                //OLD CODE: GameObject.FindWithTag("RealEntrance").GetComponent<RealEntranceInteract>().EnterRealEntrance();
                DialogueController.instance.StartDialogue();
                StartCoroutine(GameManager.instance.NewScene(3));
            }

            if (_nearShotgun)
            {
                _ = Time.unscaledTime;
                Debug.Log("This is Troy's Shotgun...");
                // OLD CODE: GameObject.FindWithTag("Shotgun").GetComponent<ShotgunPickup>().FoundShotgun();
                if (WeaponManager.instance.shotgunFound == false)
                {
                    TextBoxManager.instance.EnableTextBox();
                    WeaponManager.instance.shotgunFound = true;
                }
                // Probably write something to the ui here
                else
                {
                    GameObject.FindWithTag("Shotgun").SetActive(false);
                    // GameObject.FindWithTag("Shotgun").IsDestroyed();
                   // Destroy(gameObject); // Probably do a little fade out animation before this
                }
            }
        }
    }

    void PlayerDamage(float amountOfDamage)
    {
        healthMan.playerHealth -= amountOfDamage;
        
        _playerAudio.PlayOneShot(damagedSound, 10);

        if (healthMan.playerHealth <= 0)
        {
            PlayerDeath();
        }
    }

    void PlayerDeath()
    {
        _playerAudio.PlayOneShot(deathSound);
    }
    
}
