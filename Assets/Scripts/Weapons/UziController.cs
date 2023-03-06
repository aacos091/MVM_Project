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

    private RaycastHit2D hit;
    
    private AmmoManager ammo;

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
        
        
        UIController.instance.UpdateTotals(ammo.uziBullets, ammo.currentUziMagCount);
        UIController.instance.UpdateStatus("Idle");
        
        Debug.Log("There are " + ammo.uziMags[0] + " bullets in the current mag");
    }

    private void Update()
    {
        //weaponRotationAiming();
        
        TurnGunBarrelWithButtons();

        if (Input.GetMouseButton(1) && !_isReloading && !_isChecking)
        {
            Debug.Log("aiming button held down");
            _isAiming = true;
            
            weaponDirectionalAiming();
            
            if (Input.GetMouseButton(0))
            {
                //StartCoroutine(Shoot());
                _isFiring = true;
                if (Time.time > _nextFire)
                {
                    uziShoot();
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
                _reloadHeld = true;
            }
        }
        else
        {
            _isReloading = false;
        }

        if (Input.GetKeyDown(KeyCode.Q) && !_isReloading && !_isChecking && !_isAiming && !_isFiring)
        {
            changeMagazines();
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
        UIController.instance.EnableUziMag(true);
        UIController.instance.UpdateTotals(ammo.uziBullets, ammo.currentUziMagCount);
    }

    private void OnDisable()
    {
        weaponImage.gameObject.SetActive(false);
        UIController.instance.EnableUziMag(false);
    }

    void Reload()
    {
        if (ammo.currentUziMagCount == ammo.maxUziMagCount)
        {
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
        UIController.instance.CheckUziMag(ammo.currentUziMagCount);
        yield return new WaitForSeconds(checkTime);
        _isChecking = false;
        weaponAudio.PlayOneShot(uziReload);
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
            Debug.DrawRay(gunBarrel.position, gunBarrel.TransformDirection(Vector3.right) * uziRange, Color.yellow, 1f);
            Debug.Log("shot the uzi");

            //r2d = new Ray(gunBarrel.position, gunBarrel.TransformDirection(Vector3.right));

            hit = Physics2D.Raycast(gunBarrel.position, gunBarrel.TransformDirection(Vector3.right), uziRange, targetLayer);
            
            Instantiate(BulletTracerPrefab, gunBarrel.position, gunBarrel.rotation);

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
        if (Input.GetAxisRaw("Vertical") > 0.2f)
        {
            transform.parent.localEulerAngles = new Vector3(0f, 0f, upAimingAngle);
        }
        else if (Input.GetAxisRaw("Vertical") < -0.2f)
        {
            transform.parent.localEulerAngles = new Vector3(0f, 0f, downAimingAngle);
        }
        else
        {
            transform.parent.localEulerAngles = new Vector3(0f, 0f, 0f);
        }
        
    }

    void changeMagazines()
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
