using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoodComponent : MonoBehaviour
{
    public float slowMoveSpeed, fastMoveSpeed;
    public bool fastHood, slowHood;
    public float detectRange, stoppingRange;
    public float health;
    public float staggerTime;
    public float attackTimer;
    public float damage;
    private float _nextAttack;
    private float _step;
    private bool _isDead = false;
    public LayerMask whatIsPlayer;
    private bool _playerFound;
    private EnemyController _enemyController;
    private Animator _animator;
    public bool CanMove = true;
    private AudioSource _enemyAudio;
    public AudioClip attackSound, deathSound, stepSound;
    public float stepRate = 0.5f;
    public float stepCoolDown;
    
    // Start is called before the first frame update
    void Start()
    {
        _enemyController = GetComponent<EnemyController>();
        _animator = GetComponent<Animator>();
        _enemyAudio = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        stepCoolDown -= Time.deltaTime;
        
        if (!_isDead && CanMove)
        {
            if (slowHood)
            {
                _step = slowMoveSpeed * Time.deltaTime;
            }
            else if (fastHood)
            {
                _step = fastMoveSpeed * Time.deltaTime;
            }

            _playerFound = Physics2D.OverlapCircle(transform.position, detectRange, whatIsPlayer);

            if (_playerFound)
            {
                Collider2D hit = Physics2D.OverlapCircle(transform.position, detectRange, whatIsPlayer);
                if (Vector2.Distance(transform.position, hit.transform.position) < stoppingRange)
                {
                    transform.position = this.transform.position;
                    AttackPlayer();
                }
                else
                {
                    ChasePlayer(hit.transform.position);
                }
            }
        }
        
        _animator.SetBool("isMoving", CanMove);

    }

    // Patrol between certain points
    void Patrol(Vector2[] patrolPoints)
    {
        //transform.position = Vector2.MoveTowards(transform.position, target.position, _step);
    }

    // Chase the player if they are detected
    void ChasePlayer(Vector2 player)
    {
        if (player.x > transform.position.x)
        {
            transform.localScale = new Vector3(-1f, 1f, 1f);
        }
        else
        {
            transform.localScale = new Vector3(1f, 1f, 1f);
        }
        transform.position = Vector2.MoveTowards(transform.position, player, _step);
        PlayFootsteps(stepRate);
    }

    public void Damage(float damageDoneToHood)
    {
        health -= damageDoneToHood;

        if (health <= 0f)
        {
            StartCoroutine(Defeated());
            Destroy(gameObject, 4f);
        }
        else
        {
            StartCoroutine(Staggered());
        }
    }

    // Attack the player if they get close
    void AttackPlayer()
    {
        if (Time.time > _nextAttack)
        {
            _animator.SetTrigger("Attack");
            _enemyAudio.PlayOneShot(attackSound, 6);
            _nextAttack = Time.time + attackTimer;
        }
        Debug.Log("Hood attacks the player.");
    }
    
    // Get staggered when player hits enemy
    IEnumerator Staggered()
    {
        CanMove = false;
        transform.position = this.transform.position;
        _animator.SetTrigger("Damaged");
        yield return new WaitForSeconds(0.5f);
        CanMove = true;
    }

    // If they die, check to see if they drop anything
    IEnumerator Defeated()
    {
        _enemyAudio.PlayOneShot(deathSound, 6);
        _isDead = true;
        GetComponent<CapsuleCollider2D>().enabled = false;
        GetComponent<Rigidbody2D>().gravityScale = 0;
        for (float i = 1f; i >= 0; i -= Time.deltaTime)
        {
            GetComponent<SpriteRenderer>().color = new Color(255, 255, 255, i);
            yield return null;
        }
        Debug.Log("This hood is dead.");
    }
    
    void PlayFootsteps(float footstepRate)
    {
        if (stepCoolDown < 0f)
        {
            _enemyAudio.pitch = 1f + Random.Range(-0.2f, 0.2f);
            _enemyAudio.PlayOneShot(stepSound, 6);
            stepCoolDown = footstepRate;
        }
    }
}
