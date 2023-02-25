using System;
using System.Collections;
using System.Collections.Generic;
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

    public Image mag;
    public Image[] bullets;

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

    public void UpdateStatus(string status)
    {
        weaponStatus.text = status;
    }

    public void checkMag(int bulletsInMag)
    {
        mag.color = new Color(255, 255, 255, 255);

        for (int i = 0; i < bulletsInMag; i++)
        {
            bullets[i].gameObject.SetActive(true);
        }
    }

    public void putMagAway(int bulletsInMag)
    {
        mag.color = new Color(255, 255, 255, 0);
        
        for (int i = 0; i < bulletsInMag; i++)
        {
            bullets[i].gameObject.SetActive(false);
        }
    }
}
