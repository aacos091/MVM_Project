using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// See https://www.youtube.com/watch?v=1QfxdUpVh5I for reference
public class MeleeWeapon : MonoBehaviour
{
    private float _timeBetweenAttack;
    public float startTimeBetweenAttack;

    public Transform attackPos;
    public float attackRange;
    public LayerMask targets;

    public int damage;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(1))
        {
            // Use this to charge up hit
            if (Input.GetMouseButtonDown(0))
            {
                // Than you can attack
                if (_timeBetweenAttack <= 0)
                {
                    Collider2D[] enemiesToDamage = Physics2D.OverlapCircleAll(attackPos.position, attackRange, targets);
                    for (int i = 0; i < enemiesToDamage.Length; i++)
                    {
                        Destroy(enemiesToDamage[i].gameObject);
                        Debug.Log("attacked with melee weapon");
                    }
                }
                _timeBetweenAttack = startTimeBetweenAttack;
            }
            else
            {
                _timeBetweenAttack -= Time.deltaTime;
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPos.position, attackRange);
    }
}
