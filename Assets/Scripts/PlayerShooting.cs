using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerShooting : MonoBehaviour
{

    [SerializeField]
    private GameObject laserPrefab;

    [SerializeField]
    private Vector3 laserOffset = new Vector3(0f, 0.8f, 0f);
    
    private float _cooldownTimer;

    [SerializeField]
    private float fireRate = 0.5f;


    void Update()
    {
        ShootInput();
    }

    private void ShootInput()
    {
        if (Input.GetKeyDown(KeyCode.Space) && Time.time > _cooldownTimer)
        {
            _cooldownTimer = Time.time + fireRate;
            Instantiate(laserPrefab, transform.position + laserOffset, Quaternion.identity);
        }   
    }
}
