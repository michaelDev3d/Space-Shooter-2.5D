using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField]
    private float _projectileSpeed = 8f;
    
    [SerializeField]
    private float _screenHeight = 8f;

    [SerializeField] 
    private bool _isEnemyLaser;
    
    [SerializeField] 
    private bool _isSwarmEnemyLaser;

    [SerializeField] 
    private float _enemyProjectileSpeed = 6f;
    
    [SerializeField] 
    private bool _isOrbitalLaser;
    
    [SerializeField] 
    private bool _isHomingLaser;
    
    [SerializeField] 
    private bool _isEventLaser;

    [SerializeField] 
    private Transform _homingLaserTarget;

    [SerializeField] 
    private bool _reverseDirection;
    

    private static readonly int _enemyLaserAnimBool = Animator.StringToHash("EnemyLaser");
    private static readonly int _enemySwarmLaserAnimBool = Animator.StringToHash("SwarmLaser");

    void Update()
    {
        MoveProjectile();
    }
    
    private void MoveProjectile()
    {
        if (!_isEnemyLaser && !_isOrbitalLaser)
        {
            transform.Translate(Vector3.up * (Time.deltaTime * _projectileSpeed));

            if (transform.position.y > _screenHeight )
            {
                if (transform.parent != null)
                {
                    Destroy(transform.parent.gameObject);
                }

                Destroy(this.gameObject);
            }
        }

        if (_isEnemyLaser && !_isOrbitalLaser && !_reverseDirection && !_isEventLaser)
        {
            Animator animator = GetComponent<Animator>();
            
            if (_isSwarmEnemyLaser)
            {
                if(animator != null)
                    animator.SetBool(_enemySwarmLaserAnimBool, true);
            }

            if(animator != null)
                animator.SetBool(_enemyLaserAnimBool, true);

            transform.Translate(Vector3.down * (Time.deltaTime * _enemyProjectileSpeed));
            
            if (transform.position.y < -_screenHeight )
            {
                Destroy(this.gameObject);
            }
        }

        if (_isEnemyLaser && !_isOrbitalLaser && _reverseDirection)
        {
            Animator animator = GetComponent<Animator>();
            
            if (_isSwarmEnemyLaser)
            {
                if(animator != null)
                    animator.SetBool(_enemySwarmLaserAnimBool, true);
            }

            if(animator != null)
                animator.SetBool(_enemyLaserAnimBool, true);

            transform.Translate(Vector3.up * (Time.deltaTime * _enemyProjectileSpeed));
            
            if (transform.position.y < -_screenHeight )
            {
                Destroy(this.gameObject);
            }
        }

        if (_isOrbitalLaser)
        {
            Transform orbitalContainer = gameObject.transform.parent;
            
            orbitalContainer.transform.Rotate(Vector3.forward, 1, Space.Self);
        }

        if (_isHomingLaser)
        {
            EnemyMovement[] onScreenEnemies = FindObjectsOfType<EnemyMovement>();

            if (onScreenEnemies.Length > 0)
            {
                Debug.Log("Enemies are found");
                if (_homingLaserTarget == null)
                {
                    foreach (EnemyMovement enemy in onScreenEnemies)
                    {
                        Debug.Log("Checking for enemies on screen");
                        if (CheckIfEnemyIsOnScreen(enemy))
                        {
                            Debug.Log("Enemy on screen");
                            _homingLaserTarget = enemy.gameObject.transform;
                        }
                    }
                }

                if (_homingLaserTarget != null)
                {
                    transform.position = Vector2.MoveTowards(transform.position, 
                        _homingLaserTarget.transform.position,0.1f);
                }
            }
        }
        
    }
    
    private bool CheckIfEnemyIsOnScreen(EnemyMovement enemy)
    {
        if (enemy.gameObject.transform.position.y < 3.1)
        {
            return true;
        }
            
        return false;
    }

    public bool CheckIfIsEnemyLaser()
    {
        return _isEnemyLaser;
    }
    
    public bool CheckIfIsEventLaser()
    {
        return _isEventLaser;
    }

    public void SetEnemyLaser(bool isEnemyLaser)
    {
        _isEnemyLaser = isEnemyLaser;
    }
    
    public void SetSwarmEnemyLaser(bool isSwarmEnemyLaser)
    {
        _isSwarmEnemyLaser =  isSwarmEnemyLaser;
    }

    public bool IsHomingLaser
    {
        get => _isHomingLaser;
        set => _isHomingLaser = value;
    }

    public bool ReverseDirection
    {
        get => _reverseDirection;
        set => _reverseDirection = value;
    }

    public float ProjectileSpeed
    {
        get => _projectileSpeed;
        set => _projectileSpeed = value;
    }
}
