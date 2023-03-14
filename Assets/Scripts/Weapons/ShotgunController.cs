using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShotgunController : MonoBehaviour
{
    [Header("Shotgun Fire Rate")]
    public float fireRate;
    private float _nextFire;
    private bool _isFiring;
    
    [Header("Shotgun Reload Rate")]
    public float reloadRate;
    private float _nextReload;
    private bool _isReloading;

    [Header("Shotgun Check Time")]
    public float checkTime;
    private bool _isChecking;

    [Header("Shotgun Aiming Angles")]
    public float upAimingAngle;
    public float downAimingAngle;
    private bool _isAiming;
    
    [Header("Shotgun Damage")] 
    public int damage;
    
    [Header("Camera Shake")] 
    public float CameraShakeIntensity;
    public float CameraShakeTimer;

    private const float MinimumHeldDuration = 0.25f;
    private float _reloadPressedTime = 0;
    private bool _reloadHeld = false;

    [Header("Other")]
    public Transform gunBarrel;
    public LayerMask targetLayer;
    public Image weaponImage;
    public float shotgunRange;
    public GameObject BulletTracerPrefab;
    //public float pellets, pelletSpread; // this is just for the bullet tracers, the shotgun works with a ray and that's it

    private bool _aimToTheLeft;
    private bool _aimToTheRight;
    
    public float AimUpAngle = 40f;
    public float AimDownAngle = 330f;
    private float _weaponAngle;

    private RaycastHit2D hit;
    private Animator _playerAnimator;
    private AmmoManager ammo;
    
    public Transform ShotgunBarrel, ShotgunBarrelUp, ShotgunBarrelDown;

    [Header("shotgun Sounds")]
    public AudioSource weaponAudio;
    public AudioClip shotgunEmpty, shotgunFire, shotgunInsertBullet, shotgunRemove, shotgunReload;

    // This is only used for drawing the lines in debugging
    [Header("Debug")]
    public Material lineMaterial;
    public Ray r2d;
    
    private void Start()
    {
        ammo = GetComponentInParent<AmmoManager>();
        
        Debug.Log("Amount of shells: " + ammo.shotgunShells);
        Debug.Log("Amount in barrel: " + ammo.currentShellCount);
        
        _playerAnimator = transform.parent.GetComponentInParent<Animator>();
        
        UIController.instance.UpdateTotalsShotgun(ammo.shotgunShells, ammo.currentShellCount);
        UIController.instance.UpdateStatus("Idle");
        
        Debug.Log("There are " + ammo.currentShellCount + " shells in the barrel");
    }

    private void Update()
    {
        //weaponRotationAiming();
        
        //TurnGunBarrelWithButtons();
        weaponDirectionalAiming();

        if (Input.GetMouseButton(1) && !_isReloading && !_isChecking)
        {
            Debug.Log("aiming button held down");
            _isAiming = true;
            _playerAnimator.SetBool("Aiming", true);
            
            
            if (Input.GetMouseButton(0))
            {
                //StartCoroutine(Shoot());
                _isFiring = true;
                if (Time.time > _nextFire)
                {
                    shotgunShoot();
                    _nextFire = Time.time + fireRate;
                }
            }
            else if (Input.GetMouseButtonUp(0))
            {
                _isFiring = false;
            }
        }
        else if (Input.GetMouseButtonUp(1))
        {
            transform.parent.localEulerAngles = new Vector3(0f, 0f, 0f);
            _isAiming = false;
            _playerAnimator.SetBool("Aiming", false);
            _playerAnimator.SetBool("AimUp", false);
            _playerAnimator.SetBool("AimDown", false);
        }




        if (Input.GetKeyDown(KeyCode.R) && !_isAiming)
        {
            //shotgunRemoveSound();
            _reloadPressedTime = Time.timeSinceLevelLoad;
            _reloadHeld = false;
        }
        else if (Input.GetKeyUp(KeyCode.R) && !_isAiming)
        {
            if (!_reloadHeld && !_isChecking)
            {
                StartCoroutine(Check());
            }
            else
            {
                weaponAudio.PlayOneShot(shotgunReload);
                UIController.instance.StopShotgunCheck(ammo.currentShellCount);
            }

            _reloadHeld = false;
        }
        
        if (Input.GetKey(KeyCode.R) && !_isAiming)
        {
            if (Time.timeSinceLevelLoad - _reloadPressedTime > MinimumHeldDuration)
            {
                _isReloading = true;
                Reload();
                _playerAnimator.SetBool("InsertBullet", true);
                _reloadHeld = true;
            }
        }
        else
        {
            _isReloading = false;
            _playerAnimator.SetBool("InsertBullet", false);
        }
        
        if (_isChecking || _isReloading)
        {
            PlayerController.instance.CanMove = false;
        }
        else
        {
            PlayerController.instance.CanMove = true;
        }

        if (_isFiring) UIController.instance.UpdateStatus("Firing");
        else if (_isAiming) UIController.instance.UpdateStatus("Aiming");
        else if (_isChecking) UIController.instance.UpdateStatus("Checking");
        else if (_isReloading) UIController.instance.UpdateStatus("Reloading");
        else UIController.instance.UpdateStatus("Idle");
    }

    IEnumerator Shoot()
    {
        if (ammo.currentShellCount > 0)
        {
            _isFiring = true;
            weaponAudio.PlayOneShot(shotgunFire);
            Debug.DrawRay(gunBarrel.position, gunBarrel.TransformDirection(Vector3.right) * 15f, Color.yellow, 1f);
            Debug.Log("shot the gun");

            r2d = new Ray(gunBarrel.position, gunBarrel.TransformDirection(Vector3.right));

            hit = Physics2D.Raycast(gunBarrel.position, gunBarrel.TransformDirection(Vector3.right), 15f, targetLayer);

            DrawLine(gunBarrel.position, r2d.GetPoint(15f), Color.black);
            
            --ammo.currentShellCount;
            
            if (hit)
            {
                hit.transform.gameObject.GetComponent<EnemyController>().DamageEnemy(damage);
            }

        }
        else if (ammo.currentShellCount == 0)
        {
            weaponAudio.PlayOneShot(shotgunEmpty);
            Debug.Log("No Ammo");
        }
        
        UIController.instance.UpdateTotalsShotgun(ammo.shotgunShells, ammo.currentShellCount);
        yield return new WaitForSeconds(fireRate);
        _isFiring = false;
    }
    
    void shotgunShoot()
    {
        if (ammo.currentShellCount > 0)
        {
            weaponAudio.PlayOneShot(shotgunFire);
            CameraShake.instance.ShakeCamera(CameraShakeIntensity, CameraShakeTimer);
            
            if (Input.GetAxisRaw("Vertical") > 0.2f)
            {
                _playerAnimator.SetTrigger("FireUp");
            }
            else if (Input.GetAxisRaw("Vertical") < -0.2f)
            {
                _playerAnimator.SetTrigger("FireDown");
            }
            else
            {
                _playerAnimator.SetTrigger("Fire");
            }
            
            Debug.DrawRay(gunBarrel.position, transform.TransformDirection(Vector3.left) * shotgunRange, Color.yellow, 1f);
            Debug.Log("shot the shotgun");

            //r2d = new Ray(gunBarrel.position, gunBarrel.TransformDirection(Vector3.right));

            hit = Physics2D.Raycast(gunBarrel.position, transform.TransformDirection(Vector3.left), shotgunRange, targetLayer);
            
            Instantiate(BulletTracerPrefab, gunBarrel.position, Quaternion.Euler(gunBarrel.localRotation.x, _aimToTheLeft ? -180f : 0f, _weaponAngle));

            //for (int i = 0; i <= pellets; i++)
            //{
            //    
            //}

            //DrawLine(gunBarrel.position, r2d.GetPoint(15f), Color.black);

            --ammo.currentShellCount;

            if (hit)
            {
                hit.transform.gameObject.GetComponent<EnemyController>().DamageEnemy(damage);
            }

        }
        else if (ammo.currentShellCount == 0)
        {
            weaponAudio.PlayOneShot(shotgunEmpty);
            Debug.Log("No Ammo");
        }
        
        UIController.instance.UpdateTotalsShotgun(ammo.shotgunShells, ammo.currentShellCount);
    }
    
    IEnumerator Check()
    {
        weaponAudio.PlayOneShot(shotgunRemove);
        Debug.Log("You have: " + ammo.currentShellCount + " in the barrel");
        _isChecking = true;
        _playerAnimator.SetTrigger("Check");
        UIController.instance.UpdateShotgunCount(ammo.currentShellCount);
        yield return new WaitForSeconds(checkTime);
        _isChecking = false;
        weaponAudio.PlayOneShot(shotgunReload);
        UIController.instance.StopShotgunCheck(ammo.currentShellCount);
        UIController.instance.UpdateTotalsShotgun(ammo.shotgunShells, ammo.currentShellCount);
    }
    
    void Reload()
    {
        if (ammo.currentShellCount == ammo.maxShellCount)
        {
            UIController.instance.UpdateShotgunCount(ammo.currentShellCount);
            _playerAnimator.SetBool("ShotgunFull", true);
            Debug.Log("Full mag, release the reload key");
        }
        else
        {
            if (ammo.shotgunShells > 0)
            {
                if (ammo.currentShellCount < ammo.maxShellCount && ammo.shotgunShells != 0)
                {
                    _playerAnimator.SetBool("ShotgunFull", false);
                    if (Time.time > _nextReload)
                    {
                        _nextReload = Time.time + reloadRate;
                        
                        Debug.Log("Shell added");
                        weaponAudio.PlayOneShot(shotgunInsertBullet);
                        ammo.shotgunShells--;
                        ammo.currentShellCount++;
                        UIController.instance.UpdateShotgunCount(ammo.currentShellCount);
                    }
                }
            }
            else
            {
                _playerAnimator.SetBool("ShotgunFull", true);
                UIController.instance.StopShotgunCheck(ammo.currentShellCount);
                Debug.Log("no more ammo");
            }
        }
        
        UIController.instance.UpdateTotalsShotgun(ammo.shotgunShells, ammo.currentShellCount);
    }

    private void OnEnable()
    {
        _playerAnimator.ResetTrigger("Check");
        StartCoroutine(ActivateThisWeapon(weaponImage));
        UIController.instance.EnableShotgunBarrel(true);
        UIController.instance.UpdateTotalsShotgun(ammo.shotgunShells, ammo.currentShellCount);
    }

    private void OnDisable()
    {
        if (weaponImage != null)
        {
            weaponImage.gameObject.SetActive(false);
            UIController.instance.EnableShotgunBarrel(false);
        }
    }
    
    void shotgunRemoveSound()
    {
        bool soundPlayed = false;
        while (!soundPlayed)
        {
            weaponAudio.PlayOneShot(shotgunRemove);
            soundPlayed = true;
        }
    }

    void shotgunReloadSound()
    {
        bool soundPlayed = false;
        while (!soundPlayed)
        {
            weaponAudio.PlayOneShot(shotgunReload);
            soundPlayed = true;
        }
    }
    
    // See https://youtu.be/FgI8cgYAewM for reference
    // This is for aiming with the mouse and/or thumbstick
    void weaponRotationAiming()
    {
        Vector3 mousePos = Input.mousePosition;
        Vector3 gunPosition = Camera.main.WorldToScreenPoint(transform.position);
        
        mousePos.x = mousePos.x - gunPosition.x;
        mousePos.y = mousePos.y - gunPosition.y;

        float gunAngle = Mathf.Atan2(mousePos.y, mousePos.x) * Mathf.Rad2Deg;

        if (Camera.main.ScreenToWorldPoint(Input.mousePosition).x < transform.position.x)
        {
            transform.rotation = Quaternion.Euler(new Vector3(180f, 0f, -gunAngle));
        }
        else
        {
            transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, gunAngle));
        }
    }

    // Use this to aim in pre-defined directions
    void weaponDirectionalAiming()
    {
        if (Input.GetAxis("Horizontal") < -0.2f)
        {
            //gunBarrel.localScale = new Vector3(-1f, 1f, 1f);
            //gunBarrel.localRotation = Quaternion.Euler(gunBarrel.localRotation.x, -180f, gunBarrel.localRotation.z);
            _aimToTheLeft = true;
            _aimToTheRight = false;
        }
        else if (Input.GetAxis("Horizontal") > 0.2f)
        {
            //gunBarrel.localScale = new Vector3(1f, 1f, 1f);
            //gunBarrel.localRotation = Quaternion.Euler(gunBarrel.localRotation.x, 0f, gunBarrel.localRotation.z);
            _aimToTheLeft = false;
            _aimToTheRight = true;
        }
        
        if (Input.GetAxisRaw("Vertical") > 0.2f)
        {
            //transform.parent.localEulerAngles = new Vector3(0f, 0f, upAimingAngle);
            gunBarrel = ShotgunBarrelUp;
            _weaponAngle = AimUpAngle;
            _playerAnimator.SetBool("AimUp", true);
        }
        else if (Input.GetAxisRaw("Vertical") < -0.2f)
        {
            //transform.parent.localEulerAngles = new Vector3(0f, 0f, downAimingAngle);
            gunBarrel = ShotgunBarrelDown;
            _weaponAngle = AimDownAngle;
            _playerAnimator.SetBool("AimDown", true);
        }
        else
        {
            //transform.parent.localEulerAngles = new Vector3(0f, 0f, 0f);
            gunBarrel = ShotgunBarrel;
            _weaponAngle = 0f;
            _playerAnimator.SetBool("AimUp", false);
            _playerAnimator.SetBool("AimDown", false);
        }
        
        if (_aimToTheLeft)
        {
            //PistolBarrel.rotation = Quaternion.Euler(PistolBarrel.rotation.x, -180f, PistolBarrel.rotation.z);
            //PistolBarrelUp.rotation = Quaternion.Euler(PistolBarrelUp.rotation.x, -180f, PistolBarrelUp.rotation.z); 
            //PistolBarrelDown.rotation = Quaternion.Euler(PistolBarrelDown.rotation.x, -180f, PistolBarrelDown.rotation.z);
            ShotgunBarrel.localScale = new Vector3(-1f, 1f, 1f);
            ShotgunBarrel.GetComponentInChildren<SpriteRenderer>().flipX = false;
            ShotgunBarrelUp.localScale = new Vector3(-1f, 1f, 1f);
            ShotgunBarrelUp.GetChild(0).localScale = new Vector3(-2f, 2f, 2f);
            ShotgunBarrelUp.GetComponentInChildren<SpriteRenderer>().flipY = true;
            ShotgunBarrelDown.localScale = new Vector3(-1f, 1f, 1f);
            ShotgunBarrelDown.GetChild(0).localScale = new Vector3(-2f, 2f, 2f);
            ShotgunBarrelDown.GetComponentInChildren<SpriteRenderer>().flipY = true;
        }
        else if (_aimToTheRight)
        {
            //PistolBarrel.rotation = Quaternion.Euler(PistolBarrel.rotation.x, 0f, PistolBarrel.rotation.z); 
            //PistolBarrelUp.rotation = Quaternion.Euler(PistolBarrelUp.rotation.x, 0f, PistolBarrelUp.rotation.z); 
            //PistolBarrelDown.rotation = Quaternion.Euler(PistolBarrelDown.rotation.x, 0f, PistolBarrelDown.rotation.z); 
            ShotgunBarrel.localScale = new Vector3(1f, 1f, 1f);
            ShotgunBarrel.GetComponentInChildren<SpriteRenderer>().flipX = true;
            ShotgunBarrelUp.localScale = new Vector3(1f, 1f, 1f);
            ShotgunBarrelUp.GetChild(0).localScale = new Vector3(2f, 2f, 2f);
            ShotgunBarrelUp.GetComponentInChildren<SpriteRenderer>().flipY = false;
            ShotgunBarrelDown.localScale = new Vector3(1f, 1f, 1f);
            ShotgunBarrelDown.GetChild(0).localScale = new Vector3(2f, 2f, 2f);
            ShotgunBarrelDown.GetComponentInChildren<SpriteRenderer>().flipY = false;
        }
    }
    
    void TurnGunBarrelWithButtons()
    {
        if (Input.GetAxis("Horizontal") < -0.2f)
        {
            //gunBarrel.localScale = new Vector3(-1f, 1f, 1f);
            gunBarrel.localRotation = Quaternion.Euler(0f, -180f, 0f);
        }
        else if (Input.GetAxis("Horizontal") > 0.2f)
        {
            //gunBarrel.localScale = new Vector3(1f, 1f, 1f);
            gunBarrel.localRotation = Quaternion.Euler(0f, 0f, 0f);
        }
    }
    
    IEnumerator ActivateThisWeapon(Image img)
    {
        img.gameObject.SetActive(true);

        for (float i = 0; i <= 1f; i += Time.deltaTime)
        {
            img.color = new Color(1, 1, 1, i);
            yield return null;
        }

        yield return new WaitForSeconds(0.5f);

        for (float i = 1f; i >= 0; i -= Time.deltaTime)
        {
            img.color = new Color(1, 1, 1, i);
            yield return null;
        }
    }
    
    IEnumerator playSoundWithDelay(AudioClip clip, float delay)
    {
        yield return new WaitForSeconds(delay);
        weaponAudio.PlayOneShot(clip);
    }
    
    // See https://answers.unity.com/questions/8338/how-to-draw-a-line-using-script.html for reference
    // Only used for debugging
    void DrawLine(Vector2 start, Vector2 end, Color color, float duration = 3f)
    {
        GameObject myLine = new GameObject();
        myLine.transform.position = start;
        myLine.AddComponent<LineRenderer>();
        LineRenderer lr = myLine.GetComponent<LineRenderer>();
        //lr.useWorldSpace = false;
        lr.material = lineMaterial;
        lr.startColor = color;
        lr.endColor = color;
        lr.startWidth = 0.1f;
        lr.endWidth = 0.1f;
        lr.SetPosition(0, start);
        lr.SetPosition(1, end);
        GameObject.Destroy(myLine, duration);
    }
}
