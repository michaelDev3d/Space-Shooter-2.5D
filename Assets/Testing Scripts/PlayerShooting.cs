using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerShooting : MonoBehaviour
{
    [Header("Projectile Data")]
    [SerializeField]
    private GameObject _laserPrefab;
    
    [SerializeField]
    private GameObject _tripleLaserPrefab;
    
    [SerializeField]
    private GameObject _laserContainer;

    [SerializeField]
    private Vector3 _laserOffset = new Vector3(0f, 0.8f, 0f);

    [SerializeField] 
    private bool tripleShotPowerUpActive;
    
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
            
            if (tripleShotPowerUpActive)
            {
                Instantiate(_tripleLaserPrefab, transform.position + _laserOffset, Quaternion.identity);
            }
            else
            {
                Instantiate(_laserPrefab, transform.position + _laserOffset, Quaternion.identity);
            }
        }   
    }
    
    public void SetFireRate(float rate)
    {
        this._fireRate = rate;
    }

    public void ActivateTripleShot()
    {
        tripleShotPowerUpActive = true;
        StartCoroutine(TripleShotDeactivateRoutine(5));
    }

    IEnumerator TripleShotDeactivateRoutine(float seconds)
    {
        yield return new WaitForSeconds(seconds);

        tripleShotPowerUpActive = false;
    }
}
