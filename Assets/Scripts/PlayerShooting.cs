using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerShooting : MonoBehaviour
{

    [Header("Projectile Data")]
    [SerializeField]
    private GameObject laserPrefab;
    
    [SerializeField]
    private GameObject laserContainer;

    [SerializeField]
    private Vector3 laserOffset = new Vector3(0f, 0.8f, 0f);
    
    private float _cooldownTimer;
    
    private float _fireRate;

    void Update()
    {
        ShootInput();
    }

    private void ShootInput()
    {
        if (Input.GetKeyDown(KeyCode.Space) && Time.time > _cooldownTimer)
        {
            _cooldownTimer = Time.time + _fireRate;

            if (laserContainer != null)
            {
                Instantiate(laserPrefab, transform.position + laserOffset, Quaternion.identity, laserContainer.transform);
            }
            else
            {
                Instantiate(laserPrefab, transform.position + laserOffset, Quaternion.identity);
            }
        }   
    }
    
    public void SetFireRate(float rate)
    {
        this._fireRate = rate;
    }
}
