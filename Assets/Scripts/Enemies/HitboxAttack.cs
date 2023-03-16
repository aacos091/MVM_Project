using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitboxAttack : MonoBehaviour
{
    public float _attackPower = 0.5f;

    private bool _canAttack;

    private void OnEnable()
    {
        _canAttack = true;
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
        {
            Debug.Log("Player has been damaged");
            _canAttack = false;
        }
    }
}
