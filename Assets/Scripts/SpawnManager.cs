using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UIElements;

public class SpawnManager : MonoBehaviour
{
    [Header("Enemy Prefab's")]
    [SerializeField]
    private GameObject[] _enemyMeteorPrefabs;
    
    [SerializeField]
    private GameObject[] _enemyShipPrefabs;

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
    private GameObject[] powerUps;
    
   
    public void StartSpawning()
    {
        StartCoroutine(SpawnMeteorRoutine(_spawnRateInSeconds));
        StartCoroutine(SpawnPowerUpRoutine());
    }

    // ReSharper disable Unity.PerformanceAnalysis
    IEnumerator SpawnMeteorRoutine(float seconds)
    {
        while (!_stopSpawning)
        {
            //Select a random enemy from our enemy prefab array.
            int randomEnemy = Random.Range(0, _enemyMeteorPrefabs.Length);
            Vector3 spawnPosition = new Vector3(Random.Range(_spawnXRangeMin,_spawnXRangeMax), _spawnHeight, 0);
            
            GameObject newEnemy = Instantiate(_enemyMeteorPrefabs[randomEnemy],spawnPosition, Quaternion.identity, _enemyContainer.transform);
            newEnemy.GetComponent<EnemyMovement>().SetSpeed(Random.Range(1,6));
            newEnemy.transform.parent = _enemyContainer.transform;

            if (_startSpawningShips)
            {
                int randomShipEnemy = Random.Range(0, _enemyShipPrefabs.Length);
                Vector3 spawnShipPosition = new Vector3(Random.Range(_spawnXRangeMin,_spawnXRangeMax), _spawnHeight, 0);
                
                GameObject newShipEnemy = Instantiate(_enemyShipPrefabs[randomShipEnemy],spawnShipPosition, Quaternion.identity, _enemyContainer.transform);
                newShipEnemy.GetComponent<EnemyMovement>().SetSpeed(Random.Range(1,5));
                newShipEnemy.transform.parent = _enemyContainer.transform;
            }

            yield return new WaitForSeconds(seconds); 
        }
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
}
