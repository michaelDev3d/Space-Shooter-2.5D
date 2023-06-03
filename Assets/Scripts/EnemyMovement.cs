using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class EnemyMovement : MonoBehaviour
{
    [Header("Movement Data")]
    [System.NonSerialized]
    private float movementSpeed;

    [Header("Sprite Location")]
    [SerializeField] 
    private GameObject spriteGameObject;
    
    
    [Header("Enemy Components")]
    [SerializeField] 
    private SpriteRenderer sprite;
    
    [SerializeField] 
    private BoxCollider2D _collider2D;

    [SerializeField] 
    private Animator animator;
    
    private float _spinSpeed;
    
    private static readonly int Explosion = Animator.StringToHash("Explosion");

    // Start is called before the first frame update
    void Start()
    {
        sprite = GetComponentInChildren<SpriteRenderer>();
        spriteGameObject = sprite.gameObject;
        
        animator = spriteGameObject.GetComponent<Animator>();
        _collider2D = GetComponent<BoxCollider2D>();
        _spinSpeed = Random.Range(0.1f, 1f);
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.down * (movementSpeed * Time.deltaTime));

        if (transform.position.y <= -6 )
        {
            Debug.Log("Spawn above");
            
            float randomX = Random.Range(-9, 9);
            transform.position = new Vector3(randomX, 8, 0);
        }
        
        spriteGameObject.transform.Rotate(Vector3.forward * _spinSpeed);
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Hit "+other.transform.name);
        if (other.CompareTag("Player"))
        {
            PlayerData playerData = other.transform.GetComponent<PlayerData>();

            if (playerData != null)
            {
                playerData.ReceiveDamage();
            }
            
            animator.SetBool(Explosion, true);
            StartCoroutine(DelayDestroy(1));
            
        }
        
        
        
        if (other.CompareTag("Projectile"))
        {
            animator.SetBool(Explosion, true);
            StartCoroutine(DelayDestroy(1));
            
            Destroy(other.gameObject);
        }

        
    }

    public void SetSpeed(float speed)
    {
        this.movementSpeed = speed;
    }

    //A coroutine used to delay a death.
    //Used to disable collisions and let animations play
    IEnumerator DelayDestroy(float seconds)
    {
        _spinSpeed = 0;
        spriteGameObject.transform.rotation = Quaternion.Euler(Vector3.zero);
        _collider2D.enabled = false;
        
        yield return new WaitForSeconds(seconds);
        
        Destroy(this.gameObject);
    }
    
    
}
