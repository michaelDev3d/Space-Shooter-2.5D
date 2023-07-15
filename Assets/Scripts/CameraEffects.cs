using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraEffects : MonoBehaviour
{
    private Animator _cameraAnimator;
    private static readonly int _shakeEffect = Animator.StringToHash("ShakeEffect");

    private void Start()
    {
        GameObject cameraGameObject = GameObject.FindWithTag("MainCamera");

        if (cameraGameObject != null)
        {
            if (cameraGameObject.TryGetComponent(out Animator animator))
                _cameraAnimator = animator;
            else
            {
                Debug.Log("Camera Animator is NULL in CameraEffects component");
            }
        }
    } 
    public void ShakeCamera()
    {
        _cameraAnimator.SetTrigger(_shakeEffect);
    }
}
  
