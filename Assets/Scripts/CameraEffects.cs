using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public class CameraEffects : MonoBehaviour
{
    private Animator _cameraAnimator;
    private static readonly int _shakeEffect = Animator.StringToHash("ShakeEffect");

    private void Start()
    {
        _cameraAnimator = GameObject.FindWithTag("MainCamera").GetComponent<Animator>();  
    } 
    public void ShakeCamera()
    {
        _cameraAnimator.SetTrigger(_shakeEffect);
    }
}
  
