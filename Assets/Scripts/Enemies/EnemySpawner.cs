using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyToSpawn;
    public float spawnTimerUpper;
    public float spawnTimerLower;
    public float _nextSpawn;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time > _nextSpawn)
        {
            Instantiate(enemyToSpawn);
            _nextSpawn = Time.time + Random.Range(spawnTimerLower, spawnTimerUpper);
        }
    }
}
