using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthManager : MonoBehaviour
{
    public float playerHealth = 100.0f;
    
    private AudioSource _playerAudio;
    public AudioClip damagedSound, deathSound;
    
    // Start is called before the first frame update
    void Start()
    {
        _playerAudio = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        UIController.instance.PlayerHealth(playerHealth);
    }
    
    // Important Note: when armor is found, the player gets a bonus to health AND receives less damage
    // Maybe split this into two upgrades? Let's see how it works

    // Damages the player (enemy bullets, melee attacks, etc.)
    public void DamagePlayer(float damageAmount)
    {
        playerHealth -= damageAmount;

        _playerAudio.PlayOneShot(damagedSound, 10);

        if (playerHealth <= 0)
        {
            PlayerDeath();
        }
    }

    // Heal the player (Medkits)
    public void HealPlayer(float healAmount)
    {
        playerHealth += healAmount;
    }
    
    void PlayerDeath()
    {
        _playerAudio.PlayOneShot(deathSound);
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("EnemyHitbox"))
        {
            Debug.Log("Player was hit");
            DamagePlayer(col.GetComponent<HitboxAttack>()._attackPower);
        }
    }
}
