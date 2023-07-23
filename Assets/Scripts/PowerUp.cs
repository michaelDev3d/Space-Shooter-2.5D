using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp : Rarity
{
    [SerializeField]
    private int _powerUpID;
    [SerializeField]
    private float _movementSpeed = 1.5f;
    [SerializeField] 
    private AudioClip _audioClip;

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
                    default:
                        Debug.Log("Default Power Up");
                        break;
                }
            }

            Destroy(this.gameObject);
        }
    }
}
