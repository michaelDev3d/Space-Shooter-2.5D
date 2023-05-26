using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    //Variable to adjust how fast the player move
    public float movementSpeed = 5f;
    
    //Variables that will be used for out inputs
    //horizontal = left and right
    //vertical = up and down
    public float horizontal;
    public float vertical;

    private const float PlayerBoundsXMax = 11.27f;
    private const float PlayerBoundsXMin = -11.27f;

    private const float PlayerBoundsYMax = 0f;
    private const float PlayerBoundsYMin = -4f;

    // Start is called before the first frame update
    void Start()
    {
        
        transform.position = new Vector3(0,-2, 0);
    }

    // Update is called once per frame
    void Update()
    {
        HandleMovement();
        
        #region movement uncleaned
        /*
        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");
        
        transform.Translate(new Vector3(horizontal,vertical,0) * (movementSpeed * Time.deltaTime));

        #region movement uncleaned

        if (transform.position.y >= PlayerBoundsYMax)
        {
            transform.position = new Vector3(transform.position.x,PlayerBoundsYMax, 0);
        }
        
        if (transform.position.y <= PlayerBoundsYMin)
        {
            transform.position = new Vector3(transform.position.x,PlayerBoundsYMin, 0);
        }

        if (transform.position.x < PlayerBoundsXMin)
        {
            transform.position = new Vector3(PlayerBoundsXMax,transform.position.y, 0);
        }
        
        if (transform.position.x > PlayerBoundsXMax)
        {
            transform.position = new Vector3(PlayerBoundsXMin,transform.position.y, 0);
        }
        #endregion
        */
        #endregion
    }

    private void HandleMovement()
    {
        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");
        
        Vector3 direction = new Vector3(horizontal, vertical, 0);
       
        transform.Translate(direction * (movementSpeed * Time.deltaTime));
        
        transform.position = new Vector3(transform.position.x,
            Mathf.Clamp(transform.position.y, PlayerBoundsYMin, PlayerBoundsYMax), 0);
        
        if (transform.position.x < PlayerBoundsXMin)
        {
            transform.position = new Vector3(PlayerBoundsXMax,transform.position.y, 0);
        }
        
        if (transform.position.x > PlayerBoundsXMax)
        {
            transform.position = new Vector3(PlayerBoundsXMin,transform.position.y, 0);
        }
    }
}
