using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public float health;

    public void DamageEnemy(float damageDealt)
    {
        health -= damageDealt;
        if (health <= 0f)
        {
            // Run a check to see if the enemy drops something
            Destroy(gameObject);
        }
    }
}
