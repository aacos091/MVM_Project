using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoManager : MonoBehaviour
{
    [Header("Pistol Ammo")] 
    public int pistolBullets;
    public int currentPistolMagCount;
    public int maxPistolMagCount; 
    public int pistolMagID = 0;
    public List<int> pistolMags = new List<int>();

    [Header("Uzi Ammo")] public int uziBullets;
    public int currentUziMagCount;
    public int maxUziMagCount;
    public int uziMagID = 0;
    public List<int> uziMags = new List<int>();

    [Header("Shotgun Ammo")] 
    public int shotgunShells;
    public int currentShellCount;
    public int maxShellCount;
    
    public void findNewPistolMag(int amountOfBullets)
    {
        pistolMags.Add(amountOfBullets);
    }

    public void findNewUziMag(int amountOfBullets)
    {
        uziMags.Add(amountOfBullets);
    }

    public void addPistolBullets(int amountOfBullets)
    {
        pistolBullets += amountOfBullets;
    }

    public void addUziBullets(int amountOfBullets)
    {
        uziBullets += amountOfBullets;
    }

    public void addShotgunShells(int amountOfShells)
    {
        shotgunShells += amountOfShells;
    }
}
