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
        
        TurnGunBarrelWithButtons();

        if (Input.GetMouseButton(1) && !_isReloading && !_isChecking)
        {
            Debug.Log("aiming button held down");
            _isAiming = true;
            
            weaponDirectionalAiming();
            
            if (Input.GetMouseButtonDown(0))
            {
                _isFiring = true;
                if (Time.time > _nextFire)
                {
                    pistolShoot();
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
        UIController.instance.CheckPistolMag(ammo.currentPistolMagCount);
        yield return new WaitForSeconds(checkTime);
        _isChecking = false;
        weaponAudio.PlayOneShot(pistolReload);
        UIController.instance.PutPistolMagAway(ammo.currentPistolMagCount);
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

            //DrawLine(gunBarrel.position, r2d.GetPoint(15f), Color.black);
            
            --ammo.currentPistolMagCount;
            
            --ammo.pistolMags[ammo.pistolMagID];
            
            if (hit.transform.CompareTag("Enemy"))
            {
                Debug.Log("you hit an enemy");

                EnemyController enemy = hit.transform.GetComponent<EnemyController>();

                enemy.DamageEnemy(damage);
            }

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

    void pistolShoot()
    {
        if (ammo.currentPistolMagCount > 0)
        {
            weaponAudio.PlayOneShot(pistolFire);
            CameraShake.instance.ShakeCamera(CameraShakeIntensity, CameraShakeTimer);
            Debug.DrawRay(gunBarrel.position, transform.TransformDirection(Vector3.right) * pistolRange, Color.yellow, 1f);
            //Debug.Log("shot the pistol");

            //r2d = new Ray(gunBarrel.position, gunBarrel.TransformDirection(Vector3.right));

            hit = Physics2D.Raycast(gunBarrel.position, transform.TransformDirection(Vector2.right), pistolRange, targetLayer);

            Instantiate(BulletTracerPrefab, gunBarrel.position, gunBarrel.rotation);

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
