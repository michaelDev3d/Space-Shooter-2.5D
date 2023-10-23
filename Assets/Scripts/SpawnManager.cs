using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;

public class SpawnManager : MonoBehaviour
{
    [Header("Enemy Prefab's")]
    [SerializeField]
    private GameObject[] _commonEnemyPrefabs;
    [SerializeField]
    private GameObject[] _uncommonEnemyPrefabs;
    [SerializeField] 
    private GameObject[] _rareEnemyPrefabs;
    
    [Header("PowerUp Prefab's")]
    [SerializeField]
    private GameObject[] _commonPowerUpPrefabs;
    [SerializeField] 
    private GameObject[] _uncommonPowerUpPrefabs;
    [SerializeField] 
    private GameObject[] _rarePowerUpPrefabs;

    [Header("Enemy Spawn Setting's")]
    [SerializeField]
    private GameObject _enemyContainer;
    
    [SerializeField] 
    private float _spawnRateInSeconds;

    [SerializeField] 
    private float _spawnXRangeMax;
    
    [SerializeField] 
    private float _spawnXRangeMin;
    
    [SerializeField] 
    private float _spawnHeight = 3f;
    
    [SerializeField] 
    private bool _stopSpawning = false;

    [SerializeField] 
    private bool _startSpawningShips = false;
    
    [SerializeField] 
    private bool _startSpawningSwarmEnemy= false;
    
    [SerializeField]
    private GameObject[] powerUps;

    [SerializeField] 
    private Vector3 _swarmSpawnPosition = new Vector3(5.92f, 0.5f,0);

    [SerializeField] private int _enemyRaritySelector;
    [SerializeField] private int _powerRaritySelector;

    [SerializeField] private int _currentWave = 1;
    [SerializeField] private int _maxEnemiesPerWave = 10;
    [SerializeField] private int _enemiesSpawned;
    [SerializeField] private int _enemiesDefeated;

    private void Start()
    {
        
    }

    public void StartSpawning()
    {
        StartCoroutine(SpawnEnemyRoutine(_spawnRateInSeconds,4f));
        StartCoroutine(SpawnPowerUpRoutine());
    }

    // ReSharper disable Unity.PerformanceAnalysis
    IEnumerator SpawnEnemyRoutine(float seconds, float newWaveDelay)
    {
        while (!_stopSpawning)
        {
            if (_enemiesSpawned < _maxEnemiesPerWave)
            {
                _enemyRaritySelector = Random.Range(0, 100);
                //Debug.Log(_enemyRaritySelector);

                //Spawn Uncommon Enemy
                if (_enemyRaritySelector < 50)
                {
                    _spawnRateInSeconds = 1f;
                    SpawnEnemyRoutine(_commonEnemyPrefabs);
                }

                if (_startSpawningShips && _enemyRaritySelector > 50 && _enemyRaritySelector < 90)
                {
                    _spawnRateInSeconds = 0.5f;
                    SpawnEnemyRoutine(_uncommonEnemyPrefabs);
                }

                if (_startSpawningSwarmEnemy && _enemyRaritySelector > 90)
                {
                    _spawnRateInSeconds = 0.5f;
                    SpawnRareEnemyRoutine();
                    _startSpawningSwarmEnemy = true;
                }
            }
            else if (_enemiesDefeated == _maxEnemiesPerWave)
            {
                yield return new WaitForSeconds(seconds); 
                _currentWave++;
                _maxEnemiesPerWave = (_maxEnemiesPerWave * 2);
                _enemiesDefeated = 0;
                _enemiesSpawned = 0;
            }
            yield return new WaitForSeconds(seconds); 
        }
    }


    private void SpawnEnemyRoutine(GameObject[] enemyPrefabs)
    {
        //Select a random enemy from our enemy prefab array.
        int randomEnemy = Random.Range(0, enemyPrefabs.Length);
        Vector3 spawnPosition = new Vector3(Random.Range(_spawnXRangeMin,_spawnXRangeMax), _spawnHeight, 0);
            
        GameObject newEnemy = Instantiate(enemyPrefabs[randomEnemy],spawnPosition, Quaternion.identity, _enemyContainer.transform);
        newEnemy.GetComponent<EnemyMovement>().SetSpeed(Random.Range(1,6));
        newEnemy.transform.parent = _enemyContainer.transform;
        _enemiesSpawned++;
    }

    
    private void SpawnRareEnemyRoutine()
    {
        _startSpawningSwarmEnemy = false;
        
        Vector3 spawnShipPosition =_swarmSpawnPosition;
        Instantiate(_rareEnemyPrefabs[0],spawnShipPosition, Quaternion.identity, _enemyContainer.transform);

        float nextSpawn = Random.Range(30, 60);
            
        Debug.Log(nextSpawn);
        _enemiesSpawned++;
    }

    IEnumerator SpawnPowerUpRoutine()
    {
        while (!_stopSpawning )
        {
            Vector3 spawnPosition = new Vector3(Random.Range(_spawnXRangeMin,_spawnXRangeMax), _spawnHeight, 0);
            Instantiate(powerUps[Random.Range(0,powerUps.Length)], spawnPosition, Quaternion.identity);
            
            
            yield return new WaitForSeconds(Random.Range(3f,9f)); 
        }
    }
    
    private void SpawnCommonPowerUpRoutine(GameObject[] powerUpPrefabs)
    {
        //Select a random enemy from our enemy prefab array.
        int randomPowerUp = Random.Range(0, powerUpPrefabs.Length);
        Vector3 spawnPosition = new Vector3(Random.Range(_spawnXRangeMin,_spawnXRangeMax), _spawnHeight, 0);
            
        GameObject newPowerUp = Instantiate(powerUpPrefabs[randomPowerUp],spawnPosition, Quaternion.identity);
        newPowerUp.GetComponent<EnemyMovement>().SetSpeed(Random.Range(1,6));
    }

    
    public void OnPlayerDeath()
    {
        _stopSpawning = true;
    }

    public void SetStopSpawning(bool spawnBool)
    {
        _stopSpawning = spawnBool;
    }

    public void SetStartSpawningShips(bool spawnShipsBool)
    {
        _startSpawningShips = spawnShipsBool;
    }
    
    public void SetStartSpawningSwarmEnemy(bool spawnShipsBool)
    {
        _startSpawningSwarmEnemy = spawnShipsBool;
    }

    public int MaxEnemiesPerWave
    {
        get => _maxEnemiesPerWave;
        set => _maxEnemiesPerWave = value;
    }

    public int EnemiesDefeated
    {
        get => _enemiesDefeated;
        set => _enemiesDefeated = value;
    }

    public int CurrentWave
    {
        get => _currentWave;
        set => _currentWave = value;
    }
}
