using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollingBackground : MonoBehaviour
{
    [SerializeField]
    private float _scrollingSpeed;
    
    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.down * (_scrollingSpeed * Time.deltaTime));
        if (transform.position.y <= -6f)
        {
            transform.position = new Vector3(transform.position.x, 6.8f, transform.position.z);
        }
    }
}
