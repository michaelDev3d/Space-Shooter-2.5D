using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class PowerUp : Rarity
{
    [SerializeField]
    private int _powerUpID;
    private float _defaultSpeed;
    [SerializeField]
    private float _movementSpeed = 1.5f;
    [SerializeField]
    private float _movementSpeedBoost = 2.5f;
    [SerializeField] 
    private AudioClip _audioClip;

    [SerializeField] 
    private GameObject _explosionPrefab;
    
    private void Awake()
    {
        _defaultSpeed = _movementSpeed;
    }

    void Update()
    {
        transform.Translate(Vector3.down * (_movementSpeed * Time.deltaTime));

        if (transform.position.y <= -3.5f)
        {
            Destroy(this.gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            SpacePlayer player = other.GetComponent<SpacePlayer>();
            AudioSource.PlayClipAtPoint(_audioClip, transform.position);
            
            if (player != null)
            {
                switch (_powerUpID)
                {
                    case 0:
                        player.ActivateTripleShot();
                        break;
                    case 1:
                        player.ActivateSpeedBoost();
                        break;
                    case 2:
                        player.ActivateShield();
                        break;
                    case 3:
                        player.HealthPickup();
                        break;
                    case 4:
                        player.AmmoPickUp();
                        break;
                    case 5:
                        player.ActivateOrbitalProjectiles();
                        break;
                    case 6:
                        player.ActivateHomingShot();
                        break;
                    case 7:
                        player.ActivateSlowDebuff();
                        break;
                    default:
                        Debug.Log("Default Power Up");
                        break;
                }
            }

            Destroy(this.gameObject);
        }
        
        if (other.CompareTag("Projectile"))
        {
            if (other.GetComponent<Projectile>().CheckIfIsEnemyLaser())
            {
                Instantiate(_explosionPrefab, transform.position, quaternion.identity);
                Destroy(this.gameObject);
            }
               
        }

    }

    public void SpeedUpPowerUp()
    {
        _movementSpeed = _movementSpeedBoost;
    }
    
    public void SlowDownPowerUp()
    {
        _movementSpeed = _defaultSpeed;
    }

    public void ChangePowerUpSpeed(bool changeSpeed)
    {
        if (changeSpeed)
        {
            _movementSpeed = _movementSpeedBoost;
        }
        else
        {
            _movementSpeed = _defaultSpeed;
        }
    }
    
}
