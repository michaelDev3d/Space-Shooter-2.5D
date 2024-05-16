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
    private bool _startSpawningRareEnemies= false;
    
    [SerializeField]
    private GameObject[] powerUps;

    [SerializeField] 
    private Vector3 _swarmSpawnPosition = new Vector3(5.92f, 0.5f,0);

    [SerializeField] private int _enemyRaritySelector;
    [SerializeField] private int _powerRaritySelector;

    [Header("Wave/Level stat's")]
    [SerializeField] private bool _newWave = false;
    [SerializeField] private int _currentWave = 1;
    
    [SerializeField] private int _enemiesToCompleteWave = 10;
    [SerializeField] private int _enemiesSpawned;
    [SerializeField] private int _enemiesDefeated;
    
    [SerializeField] private bool _stopSpawningEnemies = false;
    [FormerlySerializedAs("_bossStage")] [SerializeField] private bool _startBossStage = false;
    [SerializeField] private GameObject _bossPrefab; 
    [SerializeField]
    private UIManager _uiManager;
    [SerializeField]
    private bool _bossHasBeenDefeated;

    [SerializeField] private GameObject _boss;
    public void StartSpawning()
    {
        StartCoroutine(SpawnEnemyRoutine(_spawnRateInSeconds));
        StartCoroutine(SpawnPowerUpRoutine(_commonPowerUpPrefabs,_uncommonPowerUpPrefabs, _rarePowerUpPrefabs));
    }

    // ReSharper disable Unity.PerformanceAnalysis
    IEnumerator SpawnEnemyRoutine(float seconds)
    {
        while (!_stopSpawning)
        {
            
            if (_enemiesDefeated > _enemiesToCompleteWave)
            {
                EnemyMovement[] inSceneEnemies = GameObject.FindObjectsOfType<EnemyMovement>();
                foreach (EnemyMovement enemy in inSceneEnemies)
                {
                    //Play Fleeing enemy ship
                    //Disable enemy movement Destroy(enemy);
                    Destroy(enemy.gameObject);
                }
                
                _currentWave++;
                _enemiesDefeated = 0;
                _enemiesSpawned = 0;
                _enemiesToCompleteWave = (_enemiesToCompleteWave + 5);
            }

            if (_currentWave == 1)
            {
                yield return new WaitForSeconds(4); 
            }
            
            if (_currentWave > 1 && _enemiesSpawned == 0 && _enemiesDefeated == 0)
            {
                _newWave = true;
                _enemiesSpawned++;
                Debug.Log("New Wave Spawning: "+_currentWave);
                if (_currentWave == 5)
                {
                    _startBossStage = true;
                    _stopSpawningEnemies = true;
                    _stopSpawning = true;
                }
                yield return new WaitForSeconds(8);
                _newWave = false;
            }

            switch (_currentWave)
            {
                case 1:
                    _enemyRaritySelector = Random.Range(0, 50);
                    _spawnRateInSeconds = 0.25f;
                    break;
                case 2:
                    _spawnRateInSeconds = 1.5f;
                    _startSpawningShips = true;
                    _enemyRaritySelector = Random.Range(0, 90);
                    break;
                case 3:
                    _spawnRateInSeconds = 1.5f;
                    _startSpawningShips = true;
                    _startSpawningRareEnemies = true;
                    _enemyRaritySelector = Random.Range(0, 100);
                    break;
                case 4:
                    _enemyRaritySelector = Random.Range(0, 100);
                    break;
            }
            
            if(_enemyRaritySelector < 50) 
                Debug.Log("Meteor Spawn "+_enemyRaritySelector);
            if(_enemyRaritySelector > 50 && _enemyRaritySelector < 80)
                Debug.Log("Ship Spawn "+_enemyRaritySelector);
            if(_enemyRaritySelector > 80)
                Debug.Log("Rare Ship Spawn: "+_enemyRaritySelector);
            
            //Spawn Uncommon Enemy
            if (_enemyRaritySelector < 50 && !_stopSpawningEnemies)
            {                 
                //Debug.Log(_enemyRaritySelector);
                SpawnEnemyRoutine(_commonEnemyPrefabs);
            }

            //Spawn common Enemy
            if (_startSpawningShips && _enemyRaritySelector > 50 && _enemyRaritySelector < 80)
            {
                //Debug.Log(_enemyRaritySelector);
                SpawnEnemyRoutine(_uncommonEnemyPrefabs);
            }

            //Spawn rare Enemy
            if (_startSpawningRareEnemies && _enemyRaritySelector > 90)
            {
                SpawnEnemyRoutine(_rareEnemyPrefabs);
            }

            if (_startBossStage || _stopSpawningEnemies)
            {
                _boss = Instantiate(_bossPrefab);
            }
            
            yield return new WaitForSeconds(_spawnRateInSeconds); 
        }
    }
    
    private void SpawnEnemyRoutine(GameObject[] enemyPrefabs)
    {
        //Select a random enemy from our enemy prefab array.
        int randomEnemy = Random.Range(0, enemyPrefabs.Length);
        Vector3 spawnPosition = new Vector3(Random.Range(_spawnXRangeMin,_spawnXRangeMax), _spawnHeight, 0);

        if (randomEnemy == 0)
        {
            spawnPosition = new Vector3(Random.Range(_spawnXRangeMin,_spawnXRangeMax), _spawnHeight-1.7f, 0);
        }
            
        GameObject newEnemy = Instantiate(enemyPrefabs[randomEnemy],spawnPosition, Quaternion.identity, _enemyContainer.transform);
        if(newEnemy.GetComponent<EnemyMovement>() != null)
            newEnemy.GetComponent<EnemyMovement>().SetSpeed(Random.Range(1,6));
        
       
        newEnemy.transform.parent = _enemyContainer.transform;
        _enemiesSpawned++;
    }
    
    IEnumerator SpawnPowerUpRoutine(GameObject[] commonPowerupPrefabs,GameObject[] uncommonPowerupPrefabs,GameObject[] rarePowerupPrefabs)
    {
        yield return new WaitForSeconds(8);

        while (!_stopSpawning)
        {
            if (_currentWave <= 5)
            {
                Vector3 spawnPosition = new Vector3(Random.Range(_spawnXRangeMin, _spawnXRangeMax), _spawnHeight, 0);

                Instantiate(commonPowerupPrefabs[Random.Range(0, commonPowerupPrefabs.Length)], spawnPosition, Quaternion.identity);
            }
            if (_currentWave>2 && _currentWave<3)
            {
                Vector3 spawnPosition = new Vector3(Random.Range(_spawnXRangeMin, _spawnXRangeMax), _spawnHeight, 0);

                Instantiate(uncommonPowerupPrefabs[Random.Range(0, uncommonPowerupPrefabs.Length)], spawnPosition, Quaternion.identity);
            }
            if (_currentWave>2 && _currentWave<=5)
            {
                Vector3 spawnPosition = new Vector3(Random.Range(_spawnXRangeMin, _spawnXRangeMax), _spawnHeight, 0);

                Instantiate(rarePowerupPrefabs[Random.Range(0, rarePowerupPrefabs.Length)], spawnPosition, Quaternion.identity);
            }
            yield return new WaitForSeconds(Random.Range(3f, 9f));
        }
        
        while(_stopSpawningEnemies && !_bossHasBeenDefeated)
        {
            if (_currentWave <= 5)
            {
                Vector3 spawnPosition = new Vector3(Random.Range(_spawnXRangeMin, _spawnXRangeMax), _spawnHeight, 0);

                Instantiate(commonPowerupPrefabs[Random.Range(0, commonPowerupPrefabs.Length)], spawnPosition, Quaternion.identity);
            }
            if (_currentWave>2 && _currentWave<=5)
            {
                Vector3 spawnPosition = new Vector3(Random.Range(_spawnXRangeMin, _spawnXRangeMax), _spawnHeight, 0);

                Instantiate(uncommonPowerupPrefabs[Random.Range(0, uncommonPowerupPrefabs.Length)], spawnPosition, Quaternion.identity);
            }
            if (_currentWave>2 && _currentWave<=5)
            {
                Vector3 spawnPosition = new Vector3(Random.Range(_spawnXRangeMin, _spawnXRangeMax), _spawnHeight, 0);

                Instantiate(rarePowerupPrefabs[Random.Range(0, rarePowerupPrefabs.Length)], spawnPosition, Quaternion.identity);
            }
            yield return new WaitForSeconds(Random.Range(3f, 9f));
        }
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
    
    public void SetStartSpawningRareEnemy(bool spawnShipsBool)
    {
        _startSpawningRareEnemies = spawnShipsBool;
    }

    public int EnemiesToCompleteWave
    {
        get => _enemiesToCompleteWave;
        set => _enemiesToCompleteWave = value;
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

    public bool NewWave
    {
        get => _newWave;
        set => _newWave = value;
    }

    public bool StartBossStageBool
    {
        get => _startBossStage;
        set => _startBossStage = value;
    }

    public void BossDefeated()
    {
        _bossHasBeenDefeated = true;
    }
}
