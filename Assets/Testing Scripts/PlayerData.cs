using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerData : MonoBehaviour
{
    [SerializeField]
    private int _playerLives = 3;
    
    [SerializeField]
    private float _fireRate = 0.5f;
    
    [SerializeField]
    private float _movementSpeed = 5f;

    //Custom Script Components
    private SpawnManager _spawnManager;
    private PlayerMovement _playerMovement;
    private PlayerShooting _playerShooting;

    private void Start()
    {
        _spawnManager = GameObject.Find("Spawn Manager").GetComponent<SpawnManager>();
        _playerMovement = GetComponent<PlayerMovement>();
        _playerShooting = GetComponent<PlayerShooting>();
        
        _playerMovement.SetMovementSpeed(_movementSpeed);
        _playerShooting.SetFireRate(_fireRate);

        if (_spawnManager == null)
        {
            Debug.LogError("Spawn Manager is NULL");
        }
    }

    public void ReceiveDamage()
    {
        _playerLives--;
        if (_playerLives < 1)
        {
            _spawnManager.OnPlayerDeath();
            Destroy(this.gameObject);
        }
    }
}
