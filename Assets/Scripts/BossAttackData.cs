using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAttackData : MonoBehaviour
{
   [SerializeField] private string _bossAttackName = "";
   [SerializeField] private int _bossAttackID;
   [SerializeField] private GameObject _bossAttackContainer;
   [SerializeField] private GameObject _damageSource;
   [SerializeField] private GameObject _warningIcon;
   [SerializeField] private GameObject _attackLocationWarning;
   [SerializeField] private AudioClip _laserSFX;
   [SerializeField] private AudioSource _audioSource;

   private void Start()
   {
      if (TryGetComponent(out AudioSource audioSource))
         _audioSource= audioSource;
      else
         Debug.LogError("Audio component for "+ gameObject.name +" is null");
      _bossAttackContainer = gameObject.transform.GetChild(0).gameObject;
      _damageSource = GetComponentInChildren<Projectile>().gameObject;
      _attackLocationWarning = GetComponentInChildren<LineRenderer>().gameObject;

      StartCoroutine(LaserAttackRoutine(_bossAttackID,3));
   }

   IEnumerator LaserAttackRoutine(int attackId, float seconds)
   {
      
      //Debug.Log("Start Laser Sequence");
      yield return new WaitForSeconds(seconds);
      Destroy(_attackLocationWarning);
      //Debug.Log("Shoot Laser");

      Projectile attackProjectile = _damageSource.GetComponent<Projectile>();
      attackProjectile.SetEnemyLaser(true);
      attackProjectile.IsEventLaser = false;
      attackProjectile.EnemyProjectileSpeed=20;
      _audioSource.clip = _laserSFX;
      _audioSource.Play();
      
   }

   public string bossAttackName => _bossAttackName;

   public GameObject bossAttackContainer => _bossAttackContainer;

   public GameObject damageSource => _damageSource;

   public GameObject warningIcon => _warningIcon;

   public GameObject attackLocationWarning => _attackLocationWarning;
}
