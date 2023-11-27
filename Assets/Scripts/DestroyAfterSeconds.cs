using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAfterSeconds : MonoBehaviour
{
    [SerializeField] 
    private float _destroyInSeconds;
    
    [SerializeField]
    private AudioClip _audioClip;
    
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(DestroyInSeconds(_destroyInSeconds));
    }

    IEnumerator DestroyInSeconds(float waitForSeconds)
    {
        if(_audioClip != null)
            AudioSource.PlayClipAtPoint(_audioClip, transform.position);
        yield return new WaitForSeconds(waitForSeconds);
        
        Destroy(this.gameObject);
    }
}
