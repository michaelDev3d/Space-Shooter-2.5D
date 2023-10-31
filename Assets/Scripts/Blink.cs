using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blink : MonoBehaviour
{
    [SerializeField]
    private bool _isBlinking;
    [SerializeField]
    private SpriteRenderer _renderer;
    [SerializeField]
    private LineRenderer _lineRenderer;
    [SerializeField] 
    private float _blinkRate = 0.4f;
    [SerializeField] 
    private float _destroyInSeconds = 5f;
    [SerializeField]
    private bool destroy;
    
    // Start is called before the first frame update
    void Start()
    {
        _renderer = GetComponent<SpriteRenderer>();
        _lineRenderer = GetComponent<LineRenderer>();
        StartCoroutine(BlinkGameObject(_blinkRate));
    }

    private void Update()
    {
        if (destroy)
        {
            Destroy(gameObject,_destroyInSeconds);
        }
    }

    IEnumerator BlinkGameObject(float seconds)
    {
        while(_isBlinking)
        {
            if (_renderer != null)
            {
                _renderer.enabled = false;
                yield return new WaitForSeconds(seconds);
                _renderer.enabled = true;
                yield return new WaitForSeconds(seconds);
            }
            
            if (_lineRenderer != null)
            {
                _lineRenderer.enabled = false;
                yield return new WaitForSeconds(seconds);
                _lineRenderer.enabled = true;
                yield return new WaitForSeconds(seconds);
            }
        }
    }
}
