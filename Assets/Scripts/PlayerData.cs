using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerData : MonoBehaviour
{
    [SerializeField]
    private int playerLives = 3;
    
    [SerializeField]
    private float fireRate = 0.5f;
    
    [SerializeField]
    private float movementSpeed = 5f;

    private SpawnManager _spawnManager;
    
    private PlayerMovement _playerMovement;
    private PlayerShooting _playerShooting;

    private void Start()
    {
        _spawnManager = GameObject.Find("Spawn Manager").GetComponent<SpawnManager>();
        _playerMovement = GetComponent<PlayerMovement>();
        _playerShooting = GetComponent<PlayerShooting>();
        
        _playerMovement.SetMovementSpeed(movementSpeed);
        _playerShooting.SetFireRate(fireRate);

        if (_spawnManager == null)
        {
            Debug.LogError("Spawn Manager is NULL");
        }
    }

    public void ReceiveDamage()
    {
        playerLives--;
        if (playerLives < 1)
        {
            _spawnManager.OnPlayerDeath();
            Destroy(this.gameObject);
        }
    }
}
