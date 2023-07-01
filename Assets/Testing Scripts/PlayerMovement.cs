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
    private float _horizontal;
    
    [SerializeField]
    private float _vertical;
    
    //Variable to adjust how fast the player move
    private float _movementSpeed = 5f;
    
    [Header("Screen Bounds")]
    [SerializeField]
    private float _playerBoundsXMax = 11.27f;
    
    [SerializeField]
    private float _playerBoundsXMin = -11.27f;

    [SerializeField]
    private float _playerBoundsYMax = 0f;
    
    [SerializeField]
    private float _playerBoundsYMin = -4f;

    [SerializeField] 
    private Animator _playerAnimator;

    //Used for animator parameters
    private static readonly int _animHorizontal = Animator.StringToHash("Horizontal");
    private static readonly int _animVertical = Animator.StringToHash("Idle");
    
    void Start()
    {
        _playerAnimator = GetComponent<Animator>();
        
        transform.position = new Vector3(0,-0.45f, 0);
    }

    void Update()
    {
        HandleMovement();
    }

    private void HandleMovement()
    {
        //Getting input data.
        _horizontal = Input.GetAxis("Horizontal");
        _vertical = Input.GetAxis("Vertical");

        //Set animator value to input
        _playerAnimator.SetFloat(_animHorizontal, _horizontal);
        
        //If inputs are not pressed/transition from left to right
        //Set the animation to idle.
        if (_animHorizontal == 0)
        {
            _playerAnimator.SetBool(_animVertical, true);
        }
        
        //Combine inputs into vector3 for clean code
        Vector3 direction = new Vector3(_horizontal, _vertical, 0);
       
        //Moves the player based on inputs pressed
        transform.Translate(direction * (_movementSpeed * Time.deltaTime));
        
        //Locking Player position in between min and max height using a clamp.
        transform.position = new Vector3(transform.position.x,
            Mathf.Clamp(transform.position.y, _playerBoundsYMin, _playerBoundsYMax), 0);
        
        //Screen wrapping on left
        if (transform.position.x < _playerBoundsXMin)
        {
            transform.position = new Vector3(_playerBoundsXMax,transform.position.y, 0);
        }
        
        //Screen wrapping on Right
        if (transform.position.x > _playerBoundsXMax)
        {
            transform.position = new Vector3(_playerBoundsXMin,transform.position.y, 0);
        }
    }

    public void SetMovementSpeed(float speed)
    {
        this._movementSpeed = speed;
    }
}
