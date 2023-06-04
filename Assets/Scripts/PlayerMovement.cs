using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerMovement : MonoBehaviour
{
    [Header("Input Data")]
    //Variables that will be used for out inputs
    //horizontal = left and right
    //vertical = up and down
    [SerializeField]
    private float horizontal;
    
    [SerializeField]
    private float vertical;
    
    //Variable to adjust how fast the player move
    private float _movementSpeed = 5f;
    
    [Header("Screen Bounds")]
    [SerializeField]
    private float playerBoundsXMax = 11.27f;
    
    [SerializeField]
    private float playerBoundsXMin = -11.27f;

    [SerializeField]
    private float playerBoundsYMax = 0f;
    
    [SerializeField]
    private float playerBoundsYMin = -4f;

    [SerializeField] 
    private Animator playerAnimator;

    //Used for animator parameters
    private static readonly int Horizontal = Animator.StringToHash("Horizontal");
    private static readonly int Idle = Animator.StringToHash("Idle");
    
    void Start()
    {
        playerAnimator = GetComponent<Animator>();
        
        transform.position = new Vector3(0,-0.45f, 0);
    }

    void Update()
    {
        HandleMovement();
    }

    private void HandleMovement()
    {
        //Getting input data.
        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");

        //Set animator value to input
        playerAnimator.SetFloat(Horizontal, horizontal);
        
        //If inputs are not pressed/transition from left to right
        //Set the animation to idle.
        if (Horizontal == 0)
        {
            playerAnimator.SetBool(Idle, true);
        }
        
        //Combine inputs into vector3 for clean code
        Vector3 direction = new Vector3(horizontal, vertical, 0);
       
        //Moves the player based on inputs pressed
        transform.Translate(direction * (_movementSpeed * Time.deltaTime));
        
        //Locking Player position inbetween min and max height using a clamp.
        transform.position = new Vector3(transform.position.x,
            Mathf.Clamp(transform.position.y, playerBoundsYMin, playerBoundsYMax), 0);
        
        //Screen wrapping on left
        if (transform.position.x < playerBoundsXMin)
        {
            transform.position = new Vector3(playerBoundsXMax,transform.position.y, 0);
        }
        
        //Screen wrapping on Right
        if (transform.position.x > playerBoundsXMax)
        {
            transform.position = new Vector3(playerBoundsXMin,transform.position.y, 0);
        }
    }

    public void SetMovementSpeed(float speed)
    {
        this._movementSpeed = speed;
    }
}
