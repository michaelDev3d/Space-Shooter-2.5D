using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class SpawnManager : MonoBehaviour
{
    [Header("Enemy Prefab's")]
    [SerializeField]
    private GameObject[] enemyPrefabs;

    [Header("Enemy Spawn Setting's")]
    [SerializeField]
    private GameObject enemyContainer;
    
    [SerializeField] 
    private float spawnRateInSeconds;

    [SerializeField] 
    private float spawnXRangeMax;
    
    [SerializeField] 
    private float spawnXRangeMin;
    
    [SerializeField] 
    private float spawnHeight = 3f;
    
    [SerializeField] 
    private bool stopSpawning = false;
    
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(SpawnRoutine(spawnRateInSeconds));
    }

    // ReSharper disable Unity.PerformanceAnalysis
    IEnumerator SpawnRoutine(float seconds)
    {
        while (!stopSpawning)
        {
            //Select a random enemy from our enemy prefab array.
            int randomEnemy = Random.Range(0, enemyPrefabs.Length);
            Vector3 spawnPosition = new Vector3(Random.Range(spawnXRangeMin,spawnXRangeMax), spawnHeight, 0);
            
            GameObject newEnemy = Instantiate(enemyPrefabs[randomEnemy],spawnPosition, Quaternion.identity, enemyContainer.transform);
            newEnemy.GetComponent<EnemyMovement>().SetSpeed(Random.Range(1,6));
            newEnemy.transform.parent = enemyContainer.transform; 
            
            yield return new WaitForSeconds(seconds); 
        }
    }

    public void OnPlayerDeath()
    {
        stopSpawning = true;
    }
}
