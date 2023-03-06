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

    private RaycastHit2D hit;

    private AmmoManager ammo;

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
        
        UIController.instance.UpdateTotalsShotgun(ammo.shotgunShells, ammo.currentShellCount);
        UIController.instance.UpdateStatus("Idle");
        
        Debug.Log("There are " + ammo.currentShellCount + " shells in the barrel");
    }

    private void Update()
    {
        //weaponRotationAiming();

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
                _reloadHeld = true;
            }
        }
        else
        {
            _isReloading = false;
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
            Debug.DrawRay(gunBarrel.position, gunBarrel.TransformDirection(Vector3.right) * shotgunRange, Color.yellow, 1f);
            Debug.Log("shot the shotgun");

            //r2d = new Ray(gunBarrel.position, gunBarrel.TransformDirection(Vector3.right));

            hit = Physics2D.Raycast(gunBarrel.position, gunBarrel.TransformDirection(Vector3.right), shotgunRange, targetLayer);

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
        
        UIController.instance.UpdateTotals(ammo.shotgunShells, ammo.currentShellCount);
    }
    
    IEnumerator Check()
    {
        weaponAudio.PlayOneShot(shotgunRemove);
        Debug.Log("You have: " + ammo.currentShellCount + " in the barrel");
        _isChecking = true;
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
            Debug.Log("Full mag, release the reload key");
        }
        else
        {
            if (ammo.shotgunShells > 0)
            {
                if (ammo.currentShellCount < ammo.maxShellCount && ammo.shotgunShells != 0)
                {
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
                UIController.instance.StopShotgunCheck(ammo.currentShellCount);
                Debug.Log("no more ammo");
            }
        }
        
        UIController.instance.UpdateTotalsShotgun(ammo.shotgunShells, ammo.currentShellCount);
    }

    private void OnEnable()
    {
        weaponImage.gameObject.SetActive(true);
        UIController.instance.EnableShotgunBarrel(true);
        UIController.instance.UpdateTotalsShotgun(ammo.shotgunShells, ammo.currentShellCount);
    }

    private void OnDisable()
    {
        weaponImage.gameObject.SetActive(false);
        UIController.instance.EnableShotgunBarrel(false);
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
