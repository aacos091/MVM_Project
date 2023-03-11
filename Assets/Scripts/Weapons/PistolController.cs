using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class PistolController : MonoBehaviour
{
    [Header("Pistol Fire Rate")]
    public float fireRate;
    private float _nextFire;
    private bool _isFiring;
    
    [Header("Pistol Reload Rate")]
    public float reloadRate;
    private float _nextReload;
    private bool _isReloading;

    [Header("Pistol Check Time")]
    public float checkTime;
    private bool _isChecking;

    [Header("Pistol Change Magazine Time")]
    public float changeTime;
    public float changeTimeWhileAiming;
    private bool _changingMags;

    [Header("Pistol Aiming Angles")]
    public float upAimingAngle;
    public float downAimingAngle;
    private bool _isAiming;

    [Header("Pistol Damage")] 
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
    public float pistolRange;
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

    private bool _canMove;

    public Transform PistolBarrel, PistolBarrelUp, PistolBarrelDown;

    [Header("Pistol Sounds")]
    public AudioSource weaponAudio;
    public AudioClip pistolEmpty, pistolFire, pistolInsertBullet;
    [FormerlySerializedAs("pistolCheck")] public AudioClip pistolRemove;
    public AudioClip pistolReload;

    // This is only used for drawing the lines in debugging
    [Header("Debug")]
    public Material lineMaterial;
    public Ray r2d;

    private void Start()
    {
        ammo = GetComponentInParent<AmmoManager>();
        
        Debug.Log("Amount of bullets: " + ammo.pistolBullets);
        Debug.Log("Amount in mag: " + ammo.currentPistolMagCount);

        //ammo.pistolMags.Add(ammo.currentPistolMagCount);

        _playerAnimator = transform.parent.GetComponentInParent<Animator>();
        
        
        UIController.instance.UpdateTotals(ammo.pistolBullets, ammo.currentPistolMagCount);
        UIController.instance.UpdateStatus("Idle");
        
        Debug.Log("There are " + ammo.pistolMags[0] + " bullets in the current mag");
        
        _canMove = true;
    }

    private void Update()
    {
        //weaponRotationAiming();
        
        //TurnGunBarrelWithButtons();
        weaponDirectionalAiming();

        if (Input.GetMouseButton(1) && !_isReloading && !_isChecking && !_changingMags)
        {
            Debug.Log("aiming button held down");
            _isAiming = true;
            _playerAnimator.SetBool("Aiming", true);


            if (Input.GetMouseButtonDown(0))
            {
                _isFiring = true;
                if (Time.time > _nextFire)
                {
                    _canChangeMags = false;
                    pistolShoot();
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
            _playerAnimator.SetBool("Aiming", false);
            _playerAnimator.SetBool("PistolAimUp", false);
            _playerAnimator.SetBool("PistolAimDown", false);
        }




        if (Input.GetKeyDown(KeyCode.R) && !_isAiming)
        {
            pistolRemoveSound();
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
                weaponAudio.PlayOneShot(pistolReload);
                UIController.instance.PutPistolMagAway(ammo.currentPistolMagCount);
            }

            _reloadHeld = false;
        }
        
        if (Input.GetKey(KeyCode.R) && !_isAiming)
        {
            if (Time.timeSinceLevelLoad - _reloadPressedTime > MinimumHeldDuration)
            {
                _isReloading = true;
                Reload();
                _playerAnimator.SetBool("PistolInsertBullet", true);
                _reloadHeld = true;
            }
        }
        else
        {
            _isReloading = false;
            _playerAnimator.SetBool("PistolInsertBullet", false);
        }

        if (_isChecking || _isReloading)
        {
            PlayerController.instance.CanMove = false;
        }
        else
        {
            PlayerController.instance.CanMove = true;
        }

        if (Input.GetKeyDown(KeyCode.Q) && _isAiming && !_isChecking && !_isFiring && !_isReloading)
        {
            StartCoroutine(changeMagazines());
            _playerAnimator.SetTrigger("PistolChangeMagazines");
        }

        if (_isFiring) UIController.instance.UpdateStatus("Firing");
        else if (_isAiming) UIController.instance.UpdateStatus("Aiming");
        else if (_isChecking) UIController.instance.UpdateStatus("Checking");
        else if (_isReloading) UIController.instance.UpdateStatus("Reloading");
        else UIController.instance.UpdateStatus("Idle");
    }
    
    private void OnEnable()
    {
        weaponImage.gameObject.SetActive(true);
        UIController.instance.EnablePistolMag(true);
        UIController.instance.UpdateTotals(ammo.pistolBullets, ammo.currentPistolMagCount);
    }

    private void OnDisable()
    {
        if (weaponImage != null)
        {
            weaponImage.gameObject.SetActive(false);
            UIController.instance.EnablePistolMag(false);
        }
    }

    void Reload()
    {
        if (ammo.currentPistolMagCount == ammo.maxPistolMagCount)
        {
            UIController.instance.CheckPistolMag(ammo.currentPistolMagCount);
            Debug.Log("Full mag, release the reload key");
        }
        else
        {
            if (ammo.pistolBullets > 0)
            {
                if (ammo.currentPistolMagCount < ammo.maxPistolMagCount && ammo.pistolBullets != 0)
                {
                    if (Time.time > _nextReload)
                    {
                        _nextReload = Time.time + reloadRate;
                        
                        Debug.Log("Bullet added");
                        weaponAudio.PlayOneShot(pistolInsertBullet);
                        ammo.pistolBullets--;
                        ammo.currentPistolMagCount++;
                        UIController.instance.CheckPistolMag(ammo.currentPistolMagCount);
                        ammo.pistolMags[ammo.pistolMagID]++;
                    }
                }
            }
            else
            {
                UIController.instance.CheckPistolMag(ammo.currentPistolMagCount);
                Debug.Log("no more ammo");
            }
        }
        
        UIController.instance.UpdateTotals(ammo.pistolBullets, ammo.currentPistolMagCount);
    }

    IEnumerator Check()
    {
        Debug.Log("You have: " + ammo.currentPistolMagCount + " in the mag");
        _isChecking = true;
        _playerAnimator.SetTrigger("Check");
        UIController.instance.CheckPistolMag(ammo.currentPistolMagCount);
        yield return new WaitForSeconds(checkTime);
        _isChecking = false;
        weaponAudio.PlayOneShot(pistolReload);
        UIController.instance.PutPistolMagAway(ammo.currentPistolMagCount);
        UIController.instance.UpdateTotals(ammo.pistolBullets, ammo.currentPistolMagCount);
    }

    void pistolShoot()
    {
        if (ammo.currentPistolMagCount > 0)
        {
            weaponAudio.PlayOneShot(pistolFire);
            CameraShake.instance.ShakeCamera(CameraShakeIntensity, CameraShakeTimer);


            if (Input.GetAxisRaw("Vertical") > 0.2f)
            {
                _playerAnimator.SetTrigger("PistolFireUp");
            }
            else if (Input.GetAxisRaw("Vertical") < -0.2f)
            {
                _playerAnimator.SetTrigger("PistolFireDown");
            }
            else
            {
                _playerAnimator.SetTrigger("PistolFire");
            }
            //Debug.Log("shot the pistol");

            //r2d = new Ray(gunBarrel.position, gunBarrel.TransformDirection(Vector3.right));
            
            Debug.DrawRay(gunBarrel.position, gunBarrel.TransformDirection(Vector3.left) * pistolRange, Color.yellow, 1f);

            hit = Physics2D.Raycast(gunBarrel.position, gunBarrel.TransformDirection(Vector2.left), pistolRange, targetLayer);
            
            //Debug.Log(gunBarrel.rotation + ", " + gunBarrel.localRotation);

            Instantiate(BulletTracerPrefab, gunBarrel.position, Quaternion.Euler(gunBarrel.localRotation.x, _aimToTheLeft ? -180f : 0f, _weaponAngle));

            //DrawLine(gunBarrel.position, r2d.GetPoint(pistolRange), Color.red);
            
            --ammo.currentPistolMagCount;
            
            --ammo.pistolMags[ammo.pistolMagID];

            if (hit)
            {
                hit.transform.gameObject.GetComponent<EnemyController>().DamageEnemy(damage);
            }

        }
        else if (ammo.currentPistolMagCount == 0)
        {
            weaponAudio.PlayOneShot(pistolEmpty);
            Debug.Log("No Ammo");
        }
        
        UIController.instance.UpdateTotals(ammo.pistolBullets, ammo.currentPistolMagCount);
    }

    void pistolRemoveSound()
    {
        bool soundPlayed = false;
        while (!soundPlayed)
        {
            weaponAudio.PlayOneShot(pistolRemove);
            soundPlayed = true;
        }
    }

    void pistolReloadSound()
    {
        bool soundPlayed = false;
        while (!soundPlayed)
        {
            weaponAudio.PlayOneShot(pistolReload);
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
            gunBarrel = PistolBarrelUp;
            _weaponAngle = AimUpAngle;
            _playerAnimator.SetBool("PistolAimUp", true);
        }
        else if (Input.GetAxisRaw("Vertical") < -0.2f)
        {
            //transform.parent.localEulerAngles = new Vector3(0f, 0f, downAimingAngle);
            gunBarrel = PistolBarrelDown;
            _weaponAngle = AimDownAngle;
            _playerAnimator.SetBool("PistolAimDown", true);
        }
        else
        {
            //transform.parent.localEulerAngles = new Vector3(0f, 0f, 0f);
            gunBarrel = PistolBarrel;
            _weaponAngle = 0f;
            _playerAnimator.SetBool("PistolAimUp", false);
            _playerAnimator.SetBool("PistolAimDown", false);
        }
        
        if (_aimToTheLeft)
        {
            //PistolBarrel.rotation = Quaternion.Euler(PistolBarrel.rotation.x, -180f, PistolBarrel.rotation.z);
            //PistolBarrelUp.rotation = Quaternion.Euler(PistolBarrelUp.rotation.x, -180f, PistolBarrelUp.rotation.z); 
            //PistolBarrelDown.rotation = Quaternion.Euler(PistolBarrelDown.rotation.x, -180f, PistolBarrelDown.rotation.z);
            PistolBarrel.localScale = new Vector3(-1f, 1f, 1f);
            PistolBarrelUp.localScale = new Vector3(-1f, 1f, 1f);
            PistolBarrelDown.localScale = new Vector3(-1f, 1f, 1f);
        }
        else if (_aimToTheRight)
        {
            //PistolBarrel.rotation = Quaternion.Euler(PistolBarrel.rotation.x, 0f, PistolBarrel.rotation.z); 
            //PistolBarrelUp.rotation = Quaternion.Euler(PistolBarrelUp.rotation.x, 0f, PistolBarrelUp.rotation.z); 
            //PistolBarrelDown.rotation = Quaternion.Euler(PistolBarrelDown.rotation.x, 0f, PistolBarrelDown.rotation.z); 
            PistolBarrel.localScale = new Vector3(1f, 1f, 1f);
            PistolBarrelUp.localScale = new Vector3(1f, 1f, 1f);
            PistolBarrelDown.localScale = new Vector3(1f, 1f, 1f);
        }
    }

    IEnumerator changeMagazines()
    {
        _changingMags = true;

        if (ammo.pistolMags.Count == 1)
        {
            Debug.Log("You only have one mag");
        }
        else if (ammo.pistolMagID < ammo.pistolMags.Count - 1)
        {
            pistolRemoveSound();
            ammo.pistolMagID++;
            ammo.currentPistolMagCount = ammo.pistolMags[ammo.pistolMagID];
            UIController.instance.UpdateTotals(ammo.pistolBullets, ammo.currentPistolMagCount);
        }
        else
        {
            pistolRemoveSound();
            ammo.pistolMagID = 0;
            ammo.currentPistolMagCount = ammo.pistolMags[ammo.pistolMagID];
            UIController.instance.UpdateTotals(ammo.pistolBullets, ammo.currentPistolMagCount);
        }
        
        yield return new WaitForSeconds(changeTime);
        _changingMags = false;
        pistolReloadSound();
    }

    void TurnGunBarrelWithButtons()
    {
        if (Input.GetAxis("Horizontal") < -0.2f)
        {
            //gunBarrel.localScale = new Vector3(-1f, 1f, 1f);
            gunBarrel.localRotation = Quaternion.Euler(gunBarrel.localRotation.x, -180f, gunBarrel.localRotation.z);
        }
        else if (Input.GetAxis("Horizontal") > 0.2f)
        {
            //gunBarrel.localScale = new Vector3(1f, 1f, 1f);
            gunBarrel.localRotation = Quaternion.Euler(gunBarrel.localRotation.x, 0f, gunBarrel.localRotation.z);
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
