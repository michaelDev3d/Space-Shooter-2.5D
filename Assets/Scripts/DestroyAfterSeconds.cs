using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAfterSeconds : MonoBehaviour
{
    //[SerializeField] 
    //private float _showInSeconds;
    [SerializeField] 
    private bool _destroy;
    [SerializeField] 
    private bool _repeatEffect;
    [SerializeField] 
    private float _destroyInSeconds;
    
    [SerializeField]
    private AudioClip _audioClip;
    
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(DestroyInSeconds(Random.Range(0,2),_destroyInSeconds));
    }

    IEnumerator DestroyInSeconds(float showAfterSeconds, float waitForSeconds)
    {
        while (_repeatEffect)
        {
            float scaleFactor = Random.Range(0.6f, 3.0f);
            gameObject.transform.localScale = new Vector3(scaleFactor,scaleFactor,1);
            Debug.Log(scaleFactor);
            this.gameObject.GetComponent<SpriteRenderer>().enabled = false;
            yield return new WaitForSeconds(showAfterSeconds);
            this.gameObject.GetComponent<SpriteRenderer>().enabled = true;
        
            if(_audioClip != null)
                AudioSource.PlayClipAtPoint(_audioClip, transform.position);
           
            yield return new WaitForSeconds(waitForSeconds);
            this.gameObject.GetComponent<SpriteRenderer>().enabled = false;

            if (_destroy)
            {
                yield return new WaitForSeconds(waitForSeconds);
                Destroy(this.gameObject);
            }
        }
    }
}
