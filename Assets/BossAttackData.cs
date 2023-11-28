using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAttackData : MonoBehaviour
{
   [SerializeField] private string _bossAttackName = "";
   [SerializeField] private GameObject _bossAttackContainer;
   [SerializeField] private GameObject _damageSource;
   [SerializeField] private GameObject _warningIcon;
   [SerializeField] private GameObject _attackLocationWarning;

   private void Start()
   {
      _bossAttackContainer = gameObject.transform.GetChild(0).gameObject;
      _damageSource = GetComponentInChildren<Projectile>().gameObject;
      _attackLocationWarning = GetComponentInChildren<LineRenderer>().gameObject;
   }
}
