using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class EnemyMovement : MonoBehaviour
{
    [FormerlySerializedAs("_movementSpeed")] [SerializeField]
    private float movementSpeed = 5f;
    
    
    // Start is called before the first frame update
    void Start()
    {
        movementSpeed = 5f;
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.down * (movementSpeed * Time.deltaTime));

        if (transform.position.y <= -4)
        {
            Debug.Log("Spawn above");
            
            float randomX = Random.Range(-9, 9);
            transform.position = new Vector3(randomX, 8, 0);
        }
    }
    
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Hit "+other.transform.name);
        if (other.CompareTag("Player"))
        {
            Destroy(this.gameObject);
            
        }
        
        if (other.CompareTag("Projectile"))
        {
            Destroy(this.gameObject);
            Destroy(other.gameObject);
        }
    }
}
