using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotgunPickup : MonoBehaviour
{
    private void OnDestroy()
    {
        WeaponManager.instance.shotgunFound = true;
    }
}

