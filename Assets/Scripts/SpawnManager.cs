using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UIElements;

public class SpawnManager : MonoBehaviour
{
    [Header("Enemy Prefab's")]
    [SerializeField]
    private GameObject[] _commonEnemyPrefabs;
    
    [SerializeField]
    private GameObject[] _uncommonEnemyPrefabs;

    [SerializeField] 
    private GameObject[] _rareEnemyPrefabs;

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

    [SerializeField] private bool _spawnCommonEnemy;
    [SerializeField] private bool _spawnUncommonEnemy;
    [SerializeField] private bool _spawnRareEnemy;

    [SerializeField] private int _raritySelector;
    public void StartSpawning()
    {
        StartCoroutine(SpawnEnemyRoutine(_spawnRateInSeconds));
        StartCoroutine(SpawnPowerUpRoutine());
    }

    // ReSharper disable Unity.PerformanceAnalysis
    IEnumerator SpawnEnemyRoutine(float seconds)
    {
        while (!_stopSpawning)
        {
            _raritySelector = Random.Range(0, 100);

            //Spawn Uncommon Enemy
            if (_raritySelector < 50)
            {
                _spawnRateInSeconds = 1f;
               SpawnCommonEnemyRoutine() ;
            }

            if (_startSpawningShips && _raritySelector > 50 && _raritySelector < 90)
            {
                _spawnRateInSeconds = 0.5f;
               SpawnUncommonEnemyRoutine();
            }
            
            if (_startSpawningSwarmEnemy && _raritySelector > 90)
            {
                _spawnRateInSeconds = 0.5f;
                SpawnRareEnemyRoutine();
                _startSpawningSwarmEnemy = true;
            }

            yield return new WaitForSeconds(seconds); 
        }
    }


    private void SpawnCommonEnemyRoutine()
    {
        //Select a random enemy from our enemy prefab array.
        int randomEnemy = Random.Range(0, _commonEnemyPrefabs.Length);
        Vector3 spawnPosition = new Vector3(Random.Range(_spawnXRangeMin,_spawnXRangeMax), _spawnHeight, 0);
            
        GameObject newEnemy = Instantiate(_commonEnemyPrefabs[randomEnemy],spawnPosition, Quaternion.identity, _enemyContainer.transform);
        newEnemy.GetComponent<EnemyMovement>().SetSpeed(Random.Range(1,6));
        newEnemy.transform.parent = _enemyContainer.transform;
    }
    
    private void SpawnUncommonEnemyRoutine()
    {
        int randomShipEnemy = Random.Range(0, _uncommonEnemyPrefabs.Length);
        Vector3 spawnShipPosition = new Vector3(Random.Range(_spawnXRangeMin,_spawnXRangeMax), _spawnHeight, 0);
                
        GameObject newShipEnemy = Instantiate(_uncommonEnemyPrefabs[randomShipEnemy],spawnShipPosition, Quaternion.identity, _enemyContainer.transform);
        newShipEnemy.GetComponent<EnemyMovement>().SetSpeed(Random.Range(1,5));
        newShipEnemy.transform.parent = _enemyContainer.transform;
    }
    
    private void SpawnRareEnemyRoutine()
    {
        _startSpawningSwarmEnemy = false;
        
        Vector3 spawnShipPosition =_swarmSpawnPosition;
        Instantiate(_rareEnemyPrefabs[0],spawnShipPosition, Quaternion.identity, _enemyContainer.transform);

        float nextSpawn = Random.Range(30, 60);
            
        Debug.Log(nextSpawn);
        
        
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
    
    
    
}
