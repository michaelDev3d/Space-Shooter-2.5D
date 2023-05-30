using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField]
    private GameObject enemyPrefab;

    [SerializeField]
    private GameObject enemyContainer;
    
    [SerializeField] 
    private float spawnRateInSeconds;

    [SerializeField] 
    private bool stopSpawning = false;
    
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(SpawnRoutine(spawnRateInSeconds));
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator SpawnRoutine(float seconds)
    {
        while (!stopSpawning)
        {
            Vector3 spawnPosition = new Vector3(Random.Range(-9, 9), 8, 0);
            GameObject newEnemy = Instantiate(enemyPrefab,spawnPosition, Quaternion.identity);
            newEnemy.transform.parent = enemyContainer.transform; 
            yield return new WaitForSeconds(seconds); 
        }
    }

    public void OnPlayerDeath()
    {
        stopSpawning = true;
    }
}
