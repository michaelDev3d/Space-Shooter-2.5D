using System;
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
    private float _enemyProjectileSpeed = 6f;
    
    [SerializeField] 
    private bool _isOrbitalLaser;

    private static readonly int _EnemyLaserAnimBool = Animator.StringToHash("EnemyLaser");

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

        if (_isEnemyLaser  && !_isOrbitalLaser)
        {
            Animator animator = GetComponent<Animator>();
            
            if(animator != null)
                animator.SetBool(_EnemyLaserAnimBool, true);
            
            transform.Translate(Vector3.down * (Time.deltaTime * _enemyProjectileSpeed));
            
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
    }

    public bool IsEnemyLaser()
    {
        return _isEnemyLaser;
    }

    public void SetEnemyLaser(bool isEnemyLaser)
    {
        _isEnemyLaser = isEnemyLaser;
    }
}
