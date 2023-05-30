using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField]
    private float projectileSpeed = 8f;
    
    [SerializeField]
    private float screenHeight = 8f;

    private void Start()
    {
        screenHeight = 8f;
    }
    
    void Update()
    {
        MoveProjectile();
    }
    
    private void MoveProjectile()
    {
        transform.Translate(Vector3.up * (Time.deltaTime * projectileSpeed));

        if (transform.position.y > screenHeight)
        { 
            Destroy(this.gameObject); 
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Hit "+other.transform.name);
        if (other.CompareTag("Enemy"))
        {
            Destroy(other.gameObject);
        }
    }
}
