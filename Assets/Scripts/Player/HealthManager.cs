using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthManager : MonoBehaviour
{
    public float playerHealth = 100.0f;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    // Important Note: when armor is found, the player gets a bonus to health AND receives less damage
    // Maybe split this into two upgrades? Let's see how it works

    // Damages the player (enemy bullets, melee attacks, etc.)
    public void DamagePlayer(float damageAmount)
    {
        playerHealth -= damageAmount;
    }

    // Heal the player (Medkits)
    public void HealPlayer(float healAmount)
    {
        playerHealth += healAmount;
    }
}
