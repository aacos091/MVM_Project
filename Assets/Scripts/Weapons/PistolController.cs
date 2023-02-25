using System;
using System.Collections;
using System.Collections.Generic;
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

    [Header("Pistol Aiming Angles")]
    public float upAimingAngle;
    public float downAimingAngle;
    private bool _isAiming;

    private const float MinimumHeldDuration = 0.25f;
    private float _reloadPressedTime = 0;
    private bool _reloadHeld = false;

    [Header("Other")]
    public Transform gunBarrel;
    public LayerMask targetLayer;
    public Image weaponImage;

    private RaycastHit2D hit;

    private AmmoManager ammo;

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

        ammo.pistolMags.Add(ammo.currentPistolMagCount);
        
        
        UIController.instance.UpdateTotals(ammo.pistolBullets, ammo.currentPistolMagCount);
        UIController.instance.UpdateStatus("Idle");
        
        Debug.Log("There are " + ammo.pistolMags[0] + " bullets in the current mag");
    }

    private void Update()
    {
        //weaponRotationAiming();

        if (Input.GetMouseButton(1) && !_isReloading && !_isChecking)
        {
            Debug.Log("aiming button held down");
            _isAiming = true;
            
            weaponDirectionalAiming();
            
            if (Input.GetMouseButtonDown(0))
            {
                StartCoroutine(Shoot());
            }
        }
        else if (Input.GetMouseButtonUp(1))
        {
            transform.parent.localEulerAngles = new Vector3(0f, 0f, 0f);
            _isAiming = false;
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
                UIController.instance.putMagAway(ammo.currentPistolMagCount);
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
        UIController.instance.UpdateTotals(ammo.pistolBullets, ammo.currentPistolMagCount);
    }

    private void OnDisable()
    {
        weaponImage.gameObject.SetActive(false);
    }

    void Reload()
    {
        if (ammo.currentPistolMagCount == ammo.maxPistolMagCount)
        {
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
                        UIController.instance.checkMag(ammo.currentPistolMagCount);
                        ammo.pistolMags[ammo.pistolMagID]++;
                    }
                }
            }
            else
            {
                UIController.instance.checkMag(ammo.currentPistolMagCount);
                Debug.Log("no more ammo");
            }
        }
        
        UIController.instance.UpdateTotals(ammo.pistolBullets, ammo.currentPistolMagCount);
    }

    IEnumerator Check()
    {
        Debug.Log("You have: " + ammo.currentPistolMagCount + " in the mag");
        _isChecking = true;
        UIController.instance.checkMag(ammo.currentPistolMagCount);
        yield return new WaitForSeconds(checkTime);
        _isChecking = false;
        weaponAudio.PlayOneShot(pistolReload);
        UIController.instance.putMagAway(ammo.currentPistolMagCount);
        UIController.instance.UpdateTotals(ammo.pistolBullets, ammo.currentPistolMagCount);
    }

    IEnumerator Shoot()
    {
        if (ammo.currentPistolMagCount > 0)
        {
            _isFiring = true;
            weaponAudio.PlayOneShot(pistolFire);
            Debug.DrawRay(gunBarrel.position, gunBarrel.TransformDirection(Vector3.right) * 15f, Color.yellow, 1f);
            Debug.Log("shot the gun");

            r2d = new Ray(gunBarrel.position, gunBarrel.TransformDirection(Vector3.right));

            hit = Physics2D.Raycast(gunBarrel.position, gunBarrel.TransformDirection(Vector3.right), 15f, targetLayer);

            DrawLine(gunBarrel.position, r2d.GetPoint(15f), Color.black);
            
            if (hit)
            {
                Debug.Log("you hit " + hit.transform.name);
                Destroy(hit.transform.gameObject);
            }

            --ammo.currentPistolMagCount;
            
            --ammo.pistolMags[ammo.pistolMagID];
        }
        else if (ammo.currentPistolMagCount == 0)
        {
            weaponAudio.PlayOneShot(pistolEmpty);
            Debug.Log("No Ammo");
        }
        
        UIController.instance.UpdateTotals(ammo.pistolBullets, ammo.currentPistolMagCount);
        yield return new WaitForSeconds(fireRate);
        _isFiring = false;
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
        if (ammo.pistolMags.Count == 1)
        {
            Debug.Log("You only have one mag");
        }
        else if (ammo.pistolMagID < ammo.pistolMags.Count - 1)
        {
            ammo.pistolMagID++;
            ammo.currentPistolMagCount = ammo.pistolMags[ammo.pistolMagID];
            UIController.instance.UpdateTotals(ammo.pistolBullets, ammo.currentPistolMagCount);
        }
        else
        {
            ammo.pistolMagID = 0;
            ammo.currentPistolMagCount = ammo.pistolMags[ammo.pistolMagID];
            UIController.instance.UpdateTotals(ammo.pistolBullets, ammo.currentPistolMagCount);
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
