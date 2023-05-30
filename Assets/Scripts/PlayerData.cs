using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerData : MonoBehaviour
{
    [SerializeField]
    private int playerLives = 3;


    [SerializeField] 
    private SpawnManager spawnManager;

    private void Start()
    {
        spawnManager = GameObject.Find("Spawn Manager").GetComponent<SpawnManager>();

        if (spawnManager == null)
        {
            Debug.LogError("Spawn Manager is NULL");
        }
    }

    public void ReceiveDamage()
    {
        playerLives--;
        if (playerLives < 1)
        {
            spawnManager.OnPlayerDeath();
            Destroy(this.gameObject);
        }
    }
}
