using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Mime;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIController : MonoBehaviour
{
    public static UIController instance;

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

    public TMP_Text totalBullets;
    public TMP_Text magAmount;
    public TMP_Text weaponStatus;

    public Image pistolMag;
    public Image uziMag;
    public GameObject shotgunBarrel;
    
    public Image[] pistolBullets;
    public Image[] uziBullets;
    public Image[] shotgunShells;

    public void UpdateTotals(int total, int mag)
    {
        totalBullets.text = "Total Bullets: " + total;
        magAmount.text = "Bullets in Mag: " + mag;
    }

    public void UpdateTotalsShotgun(int total, int mag)
    {
        totalBullets.text = "Total Shells: " + total;
        magAmount.text = "Shells in Barrel: " + mag;
    }

    public void meleeWeaponEquipped(string weapon)
    {
        totalBullets.text = weapon + " equipped";
        magAmount.text = "";
        weaponStatus.text = "";
    }

    public void UpdateStatus(string status)
    {
        weaponStatus.text = status;
    }

    public void CheckPistolMag(int bulletsInMag)
    {
        pistolMag.color = new Color(255, 255, 255, 255);

        for (int i = 0; i < bulletsInMag; i++)
        {
            pistolBullets[i].gameObject.SetActive(true);
        }
    }
    
    public void CheckUziMag(int uziBulletsInMag)
    {
        uziMag.color = new Color(255, 255, 255, 255);

        for (int i = 0; i < uziBulletsInMag; i++)
        {
            uziBullets[i].gameObject.SetActive(true);
        }
    }

    public void PutPistolMagAway(int pistolBulletsInMag)
    {
        pistolMag.color = new Color(255, 255, 255, 0);
        
        for (int i = 0; i < pistolBulletsInMag; i++)
        {
            pistolBullets[i].gameObject.SetActive(false);
        }
    }
    
    public void PutUziMagAway(int uziBulletsInMag)
    {
        uziMag.color = new Color(255, 255, 255, 0);
        
        for (int i = 0; i < uziBulletsInMag; i++)
        {
            uziBullets[i].gameObject.SetActive(false);
        }
    }

    public void EnablePistolMag(bool onOrOff)
    {
        pistolMag.gameObject.SetActive(onOrOff);
    }

    public void EnableUziMag(bool onOrOff)
    {
        uziMag.gameObject.SetActive(onOrOff);
    }

    public void EnableShotgunBarrel(bool onOrOff)
    {
        shotgunBarrel.SetActive(onOrOff);
    }

    public void UpdateShotgunCount(int shellsInBarrel)
    {
        for (int i = 0; i < shellsInBarrel; i++)
        {
            shotgunShells[i].gameObject.SetActive(true);
        }
    }
    
    public void StopShotgunCheck(int shellsInBarrel)
    {
        for (int i = 0; i < shellsInBarrel; i++)
        {
            shotgunShells[i].gameObject.SetActive(false);
        }
    }
}
