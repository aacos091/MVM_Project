using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Serialization;

public class WeaponController : MonoBehaviour
{   
    // Weapon Type & Ammo
    public int pistolBullets;
    public int currentPistolMagCount;
    public int maxPistolMagCount;
    public int numOfMags;
    
    public float fireRate;
    private float _nextFire;
    private bool _isFiring;
    
    public float reloadRate;
    private float _nextReload;
    private bool _isReloading;

    public float checkTime;
    private bool _isChecking;

    private const float MinimumHeldDuration = 0.25f;
    private float _reloadPressedTime = 0;
    private bool _reloadHeld = false;
    
    public Transform gunBarrel;
    public LayerMask targetLayer;

    private RaycastHit2D hit;

    public AudioSource weaponAudio;
    public AudioClip pistolEmpty, pistolFire, pistolInsertBullet;
    [FormerlySerializedAs("pistolCheck")] public AudioClip pistolRemove;
    public AudioClip pistolReload;

    private void Start()
    {
        Debug.Log("Amount of bullets: " + pistolBullets);
        Debug.Log("Amount in mag: " + currentPistolMagCount);
        
        UIController.instance.UpdateTotals(pistolBullets, currentPistolMagCount);
        UIController.instance.UpdateStatus("Idle");
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && !_isReloading && !_isChecking && !_isFiring)
        {
            StartCoroutine(Shoot());
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            pistolRemoveSound();
            _reloadPressedTime = Time.timeSinceLevelLoad;
            _reloadHeld = false;
        }
        else if (Input.GetKeyUp(KeyCode.R))
        {
            if (!_reloadHeld && !_isChecking)
            {
                StartCoroutine(Check());
            }
            else
            {
                weaponAudio.PlayOneShot(pistolReload);
                UIController.instance.putMagAway(currentPistolMagCount);
            }

            _reloadHeld = false;
        }
        
        if (Input.GetKey(KeyCode.R))
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
        else if (_isChecking) UIController.instance.UpdateStatus("Checking");
        else if (_isReloading) UIController.instance.UpdateStatus("Reloading");
        else UIController.instance.UpdateStatus("Idle");;
    }

    void Reload()
    {
        if (currentPistolMagCount == maxPistolMagCount)
        {
            Debug.Log("Full mag, release the reload key");
        }
        else
        {
            if (pistolBullets > 0)
            {
                if (currentPistolMagCount < maxPistolMagCount && pistolBullets != 0)
                {
                    if (Time.time > _nextReload)
                    {
                        _nextReload = Time.time + reloadRate;
                        
                        Debug.Log("Bullet added");
                        weaponAudio.PlayOneShot(pistolInsertBullet);
                        pistolBullets--;
                        currentPistolMagCount++;
                        UIController.instance.checkMag(currentPistolMagCount);
                    }
                }
            }
            else
            {
                Debug.Log("no more ammo");
            }
        }
        
        UIController.instance.UpdateTotals(pistolBullets, currentPistolMagCount);
    }

    IEnumerator Check()
    {
        Debug.Log("You have: " + currentPistolMagCount + " in the mag");
        _isChecking = true;
        UIController.instance.checkMag(currentPistolMagCount);
        yield return new WaitForSeconds(checkTime);
        _isChecking = false;
        weaponAudio.PlayOneShot(pistolReload);
        UIController.instance.putMagAway(currentPistolMagCount);
        UIController.instance.UpdateTotals(pistolBullets, currentPistolMagCount);
    }

    IEnumerator Shoot()
    {
        if (currentPistolMagCount > 0)
        {
            _isFiring = true;
            weaponAudio.PlayOneShot(pistolFire);
            Debug.DrawRay(gunBarrel.position, gunBarrel.TransformDirection(Vector3.right) * 15f, Color.yellow, 1f);
            Debug.Log("shot the gun");

            hit = Physics2D.Raycast(gunBarrel.position, gunBarrel.TransformDirection(Vector3.right), 15f, targetLayer);
            
            if (hit)
            {
                Debug.Log("you hit " + hit.transform.name);
            }

            --currentPistolMagCount;
        }
        else if (currentPistolMagCount == 0)
        {
            weaponAudio.PlayOneShot(pistolEmpty);
            Debug.Log("No Ammo");
        }
        
        UIController.instance.UpdateTotals(pistolBullets, currentPistolMagCount);
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
    
}
