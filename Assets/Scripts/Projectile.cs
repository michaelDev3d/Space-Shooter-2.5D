using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    //Rate of which our projectile will move
    [SerializeField]
    private float projectileSpeed = 8f;
    
    //A Y Value that appears off screen above the player
    [SerializeField]
    private float screenHeight = 8f;

    private void Start()
    {
        //Ensuring our variable's data will be set on start 
        screenHeight = 8f;
    }
    
    void Update()
    {
        //The laser is always moving so it must be updated every frame
        MoveProjectile();
    }
    
    /*Move projectile works by constantly moving the object at a real time rate of 8f (or whatever you set it to.
      if the objects position goes above the screen
      DELETE the object.
    */
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
