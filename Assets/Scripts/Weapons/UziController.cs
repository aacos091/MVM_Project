using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using UnityEngine.Serialization;

public class UziController : MonoBehaviour
{
    [Header("Uzi Fire Rate")]
    public float fireRate;
    private float _nextFire;
    private bool _isFiring;
    
    [Header("Uzi Reload Rate")]
    public float reloadRate;
    private float _nextReload;
    private bool _isReloading;

    [Header("Uzi Check Time")]
    public float checkTime;
    private bool _isChecking;
    
    [Header("Pistol Change Magazine Time")]
    public float changeTime;
    public float changeTimeWhileAiming;
    private bool _changingMags;

    [Header("Uzi Aiming Angles")]
    public float upAimingAngle;
    public float downAimingAngle;
    private bool _isAiming;
    
    [Header("Uzi Damage")] 
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
    public Image weaponMag;
    public float uziRange;
    public GameObject BulletTracerPrefab;
    
    private bool _aimToTheLeft;
    private bool _aimToTheRight;
    
    public float AimUpAngle = 40f;
    public float AimDownAngle = 330f;
    private float _weaponAngle;

    private bool _canChangeMags;

    private RaycastHit2D hit;
    private Animator _playerAnimator;
    private AmmoManager ammo;
    
    public Transform UziBarrel, UziBarrelUp, UziBarrelDown;

    [Header("Uzi Sounds")]
    public AudioSource weaponAudio;
    public AudioClip uziEmpty, uziFire, uziInsertBullet;
    [FormerlySerializedAs("pistolCheck")] public AudioClip uziRemove;
    public AudioClip uziReload;

    // This is only used for drawing the lines in debugging
    [Header("Debug")]
    public Material lineMaterial;
    public Ray r2d;

    private void Start()
    {
        ammo = GetComponentInParent<AmmoManager>();
        
        Debug.Log("Amount of bullets: " + ammo.uziBullets);
        Debug.Log("Amount in mag: " + ammo.currentUziMagCount);

        ammo.uziMags.Add(ammo.currentUziMagCount);
        
        _playerAnimator = transform.parent.GetComponentInParent<Animator>();
        
        UIController.instance.UpdateTotals(ammo.uziBullets, ammo.currentUziMagCount);
        UIController.instance.UpdateStatus("Idle");
        
        Debug.Log("There are " + ammo.uziMags[0] + " bullets in the current mag");
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
                    _canChangeMags = false;
                    uziShoot();
                    _nextFire = Time.time + fireRate;
                }
            }
            else if (Input.GetMouseButtonUp(0))
            {
                _canChangeMags = true;
                _isFiring = false;
            }
        }
        else if (Input.GetMouseButtonUp(1))
        {
            transform.parent.localEulerAngles = new Vector3(0f, 0f, 0f);
            _isAiming = false;
            _isAiming = false;
            _playerAnimator.SetBool("Aiming", false);
            _playerAnimator.SetBool("UziAimUp", false);
            _playerAnimator.SetBool("UziAimDown", false);
        }




        if (Input.GetKeyDown(KeyCode.R) && !_isAiming)
        {
            uziRemoveSound();
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
                weaponAudio.PlayOneShot(uziReload);
                UIController.instance.PutUziMagAway(ammo.currentUziMagCount);
            }

            _reloadHeld = false;
        }
        
        if (Input.GetKey(KeyCode.R) && !_isAiming)
        {
            if (Time.timeSinceLevelLoad - _reloadPressedTime > MinimumHeldDuration)
            {
                _isReloading = true;
                Reload();
                _playerAnimator.SetBool("UziInsertBullet", true);
                _reloadHeld = true;
            }
        }
        else
        {
            _isReloading = false;
            _playerAnimator.SetBool("UziInsertBullet", false);
        }
        
        if (_isChecking || _isReloading)
        {
            PlayerController.instance.CanMove = false;
        }
        else
        {
            PlayerController.instance.CanMove = true;
        }

        if (Input.GetKeyDown(KeyCode.Q) && !_isReloading && !_isChecking && !_isAiming && !_isFiring)
        {
            //changeMagazines();
            StartCoroutine(changeMagazines());
            _playerAnimator.SetTrigger("UziChangeMagazines");
        }

        if (_isFiring) UIController.instance.UpdateStatus("Firing");
        else if (_isAiming) UIController.instance.UpdateStatus("Aiming");
        else if (_isChecking) UIController.instance.UpdateStatus("Checking");
        else if (_isReloading) UIController.instance.UpdateStatus("Reloading");
        else UIController.instance.UpdateStatus("Idle");
    }

    private void OnEnable()
    {
        _playerAnimator.ResetTrigger("Check");
        StartCoroutine(ActivateThisWeapon(weaponImage));
        UIController.instance.EnableUziMag(true);
        UIController.instance.UpdateTotals(ammo.uziBullets, ammo.currentUziMagCount);
    }

    private void OnDisable()
    {
        if (weaponImage != null)
        {
            weaponImage.gameObject.SetActive(false);
            UIController.instance.EnableUziMag(false);
        }
    }

    void Reload()
    {
        if (ammo.currentUziMagCount == ammo.maxUziMagCount)
        {
            UIController.instance.CheckUziMag(ammo.currentUziMagCount);
            Debug.Log("Full mag, release the reload key");
        }
        else
        {
            if (ammo.uziBullets > 0)
            {
                if (ammo.currentUziMagCount < ammo.maxUziMagCount && ammo.uziBullets != 0)
                {
                    if (Time.time > _nextReload)
                    {
                        _nextReload = Time.time + reloadRate;
                        
                        Debug.Log("Bullet added");
                        weaponAudio.PlayOneShot(uziInsertBullet);
                        ammo.uziBullets--;
                        ammo.currentUziMagCount++;
                        UIController.instance.CheckUziMag(ammo.currentUziMagCount);
                        ammo.uziMags[ammo.uziMagID]++;
                    }
                }
            }
            else
            {
                UIController.instance.CheckUziMag(ammo.currentUziMagCount);
                Debug.Log("no more ammo");
            }
        }
        
        UIController.instance.UpdateTotals(ammo.uziBullets, ammo.currentUziMagCount);
    }

    IEnumerator Check()
    {
        Debug.Log("You have: " + ammo.currentUziMagCount + " in the mag");
        _isChecking = true;
        _playerAnimator.SetTrigger("Check");
        UIController.instance.CheckUziMag(ammo.currentUziMagCount);
        yield return new WaitForSeconds(checkTime);
        _isChecking = false;
        //weaponAudio.PlayOneShot(uziReload);
        UIController.instance.PutUziMagAway(ammo.currentUziMagCount);
        UIController.instance.UpdateTotals(ammo.uziBullets, ammo.currentUziMagCount);
    }

    IEnumerator Shoot()
    {
        if (ammo.currentUziMagCount > 0)
        {
            _isFiring = true;
            weaponAudio.PlayOneShot(uziFire);
            Debug.DrawRay(gunBarrel.position, gunBarrel.TransformDirection(Vector3.right) * 15f, Color.yellow, 1f);
            Debug.Log("shot the gun");

            r2d = new Ray(gunBarrel.position, gunBarrel.TransformDirection(Vector3.right));

            hit = Physics2D.Raycast(gunBarrel.position, gunBarrel.TransformDirection(Vector3.right), 15f, targetLayer);

            DrawLine(gunBarrel.position, r2d.GetPoint(15f), Color.black);
            
            --ammo.currentUziMagCount;
            
            --ammo.uziMags[ammo.uziMagID];
            
            if (hit)
            {
                hit.transform.gameObject.GetComponent<EnemyController>().DamageEnemy(damage);
            }
        }
        else if (ammo.currentUziMagCount == 0)
        {
            weaponAudio.PlayOneShot(uziEmpty);
            Debug.Log("No Ammo");
        }
        
        UIController.instance.UpdateTotals(ammo.uziBullets, ammo.currentUziMagCount);
        yield return new WaitForSeconds(fireRate);
        _isFiring = false;
    }

    void uziShoot()
    {
        if (ammo.currentUziMagCount > 0)
        {
            weaponAudio.PlayOneShot(uziFire);
            CameraShake.instance.ShakeCamera(CameraShakeIntensity, CameraShakeTimer);
            
            if (Input.GetAxisRaw("Vertical") > 0.2f)
            {
                _playerAnimator.SetTrigger("UziFireUp");
            }
            else if (Input.GetAxisRaw("Vertical") < -0.2f)
            {
                _playerAnimator.SetTrigger("UziFireDown");
            }
            else
            {
                _playerAnimator.SetTrigger("UziFire");
            }
            
            Debug.DrawRay(gunBarrel.position, gunBarrel.TransformDirection(Vector3.left) * uziRange, Color.yellow, 1f);
            Debug.Log("shot the uzi");

            //r2d = new Ray(gunBarrel.position, gunBarrel.TransformDirection(Vector3.right));

            hit = Physics2D.Raycast(gunBarrel.position, gunBarrel.TransformDirection(Vector2.left), uziRange, targetLayer);
            
            Instantiate(BulletTracerPrefab, gunBarrel.position, Quaternion.Euler(gunBarrel.localRotation.x, _aimToTheLeft ? -180f : 0f, _weaponAngle));

            //DrawLine(gunBarrel.position, r2d.GetPoint(15f), Color.black);
            
            --ammo.currentUziMagCount;
            
            --ammo.uziMags[ammo.uziMagID];
            
            if (hit)
            {
                hit.transform.gameObject.GetComponent<EnemyController>().DamageEnemy(damage);
            }

        }
        else if (ammo.currentUziMagCount == 0)
        {
            weaponAudio.PlayOneShot(uziEmpty);
            Debug.Log("No Ammo");
        }
        
        UIController.instance.UpdateTotals(ammo.uziBullets, ammo.currentUziMagCount);
    }

    void uziRemoveSound()
    {
        bool soundPlayed = false;
        while (!soundPlayed)
        {
            weaponAudio.PlayOneShot(uziRemove);
            soundPlayed = true;
        }
    }

    void uziReloadSound()
    {
        bool soundPlayed = false;
        while (!soundPlayed)
        {
            weaponAudio.PlayOneShot(uziReload);
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
            gunBarrel = UziBarrelUp;
            _weaponAngle = AimUpAngle;
            _playerAnimator.SetBool("PistolAimUp", true);
        }
        else if (Input.GetAxisRaw("Vertical") < -0.2f)
        {
            //transform.parent.localEulerAngles = new Vector3(0f, 0f, downAimingAngle);
            gunBarrel = UziBarrelDown;
            _weaponAngle = AimDownAngle;
            _playerAnimator.SetBool("PistolAimDown", true);
        }
        else
        {
            //transform.parent.localEulerAngles = new Vector3(0f, 0f, 0f);
            gunBarrel = UziBarrel;
            _weaponAngle = 0f;
            _playerAnimator.SetBool("PistolAimUp", false);
            _playerAnimator.SetBool("PistolAimDown", false);
        }
        
        if (_aimToTheLeft)
        {
            //PistolBarrel.rotation = Quaternion.Euler(PistolBarrel.rotation.x, -180f, PistolBarrel.rotation.z);
            //PistolBarrelUp.rotation = Quaternion.Euler(PistolBarrelUp.rotation.x, -180f, PistolBarrelUp.rotation.z); 
            //PistolBarrelDown.rotation = Quaternion.Euler(PistolBarrelDown.rotation.x, -180f, PistolBarrelDown.rotation.z);
            UziBarrel.localScale = new Vector3(-1f, 1f, 1f);
            UziBarrelUp.localScale = new Vector3(-1f, 1f, 1f);
            UziBarrelDown.localScale = new Vector3(-1f, 1f, 1f);
        }
        else if (_aimToTheRight)
        {
            //PistolBarrel.rotation = Quaternion.Euler(PistolBarrel.rotation.x, 0f, PistolBarrel.rotation.z); 
            //PistolBarrelUp.rotation = Quaternion.Euler(PistolBarrelUp.rotation.x, 0f, PistolBarrelUp.rotation.z); 
            //PistolBarrelDown.rotation = Quaternion.Euler(PistolBarrelDown.rotation.x, 0f, PistolBarrelDown.rotation.z); 
            UziBarrel.localScale = new Vector3(1f, 1f, 1f);
            UziBarrelUp.localScale = new Vector3(1f, 1f, 1f);
            UziBarrelDown.localScale = new Vector3(1f, 1f, 1f);
        }
    }

    IEnumerator changeMagazines()
    {
        if (ammo.uziMags.Count == 1)
        {
            Debug.Log("You only have one mag");
        }
        else if (ammo.uziMagID < ammo.uziMags.Count - 1)
        {
            ammo.uziMagID++;
            ammo.currentUziMagCount = ammo.uziMags[ammo.uziMagID];
            UIController.instance.UpdateTotals(ammo.uziBullets, ammo.currentUziMagCount);
        }
        else
        {
            ammo.uziMagID = 0;
            ammo.currentUziMagCount = ammo.uziMags[ammo.uziMagID];
            UIController.instance.UpdateTotals(ammo.uziBullets, ammo.currentUziMagCount);
        }
        
        yield return new WaitForSeconds(changeTime);
        _changingMags = false;
        uziReloadSound();
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
