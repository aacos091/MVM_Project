using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DogComponent : MonoBehaviour
{
    public float moveSpeed;
    public float detectRange, stoppingRange;
    private float _step;
    public LayerMask whatIsPlayer;
    private bool _playerFound;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        _step = moveSpeed * Time.deltaTime;

        _playerFound = Physics2D.OverlapCircle(transform.position, detectRange, whatIsPlayer);

        if (_playerFound)
        {
            Collider2D hit = Physics2D.OverlapCircle(transform.position, detectRange, whatIsPlayer);
            if (Vector2.Distance(transform.position, hit.transform.position) < stoppingRange)
            {
                transform.position = this.transform.position;
                //AttackPlayer();
            }
            else
            {
                ChasePlayer(hit.transform.position);
            }
        }
    }
    
    // Patrol between certain points
    void Patrol(Vector2[] patrolPoints)
    {
        //transform.position = Vector2.MoveTowards(transform.position, target.position, _step);
    }

    // Chase the player if they are detected
    void ChasePlayer(Vector2 player)
    {
        transform.position = Vector2.MoveTowards(transform.position, player, _step);
    }

    // Attack the player if they get close
    void AttackPlayer(){}
    
    // Get staggered when player hits enemy
    void Stagger(){}
    
    // If they die, check to see if they drop anything
    void Defeated(){}
}
