using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.Serialization;
using Debug = UnityEngine.Debug;
using Random = UnityEngine.Random;

public class EnemyMovement : MonoBehaviour
{
    [SerializeField] 
    private int _EnemyTypeID;
    
    [Header("Movement Data")]
    [SerializeField]
    private float _movementSpeed;
    [SerializeField] 
    private float _screenBoundsY = -6f;
    [SerializeField] 
    private float _screenBoundsX = 2.5f;
    [SerializeField] 
    private float _offScreenSpawnHeight = 8f;

    [Header("Shooting Data")] 
    [SerializeField]
    private bool _stopShooting = false;

    [Header("Sprite Location")]
    [SerializeField] 
    private GameObject _spriteGameObject;

    [Header("Enemy Components")]
    [SerializeField] 
    private SpriteRenderer _sprite;
    [SerializeField]
    private BoxCollider2D _collider2D;
    [SerializeField] 
    private Animator _animator;
    [SerializeField]
    private AudioSource _audioSource;
    
    private float _spinSpeed;
    private static readonly int _explosionAnimBool = Animator.StringToHash("Explosion");

    [Header("Level Data")]
    [SerializeField]
    private SpacePlayer _player;
    [SerializeField]
    private int _scorePerKill;
    [SerializeField] 
    private bool _mainMenuEnemy;
    [SerializeField] 
    private bool _startGameEnemy;

    [Header("Audio")]
    [SerializeField] 
    private AudioClip _explosionSFX;
    [SerializeField] 
    private AudioClip _laserSFX;
    [SerializeField]
    private GameObject _laserPrefab;
    
    void Start()
    {
        CreateEnemy(_EnemyTypeID);
    }

    void Update()
    {
        CalculateMovement();
    }
    
    //Referencing components and null checking for them
    private void CreateEnemy(int enemyID)
    {
        //Finding and Null Checking main Player.
        GameObject playerObject = GameObject.Find("Player");

        if (playerObject != null)
        {
            if (playerObject.TryGetComponent(out SpacePlayer spacePlayer))
                _player = spacePlayer;
            else
                Debug.LogError("Main player component for " + gameObject.name + " is null");
        }
        else
            Debug.LogError("Main Player is NULL");

        
        //Finding and Null Checking for Sprite GameObject, Renderer, and animator.
        if (transform.childCount > 0)
        {
            _spriteGameObject= transform.GetChild(0).gameObject;
            
            //Renderer
            if (_spriteGameObject.TryGetComponent(out SpriteRenderer spriteRenderer))
                _sprite = spriteRenderer;
            else
                Debug.LogError("Sprite component for "+ gameObject.name +" is null");
            
            //Animator
            if (_spriteGameObject.TryGetComponent(out Animator animator))
                _animator = animator;
            else
                Debug.LogError("Animator component for " + gameObject.name + " is null");
        }
        else
            Debug.LogError("Enemy does not have child gameObject, therefore enemy does not have sprite or animator");

    
        //Getting BoxCollider2D and NULL checking
        if (TryGetComponent(out BoxCollider2D boxCollider))
            _collider2D = boxCollider;
        else
            Debug.LogError("Collider component for "+ gameObject.name +" is null");
        
        
        //Getting AudioSource and NULL checking
        if (TryGetComponent(out AudioSource audioSource))
            _audioSource= audioSource;
        else
            Debug.LogError("Audio component for "+ gameObject.name +" is null");
        
        switch (enemyID)
        {
            case 0:
                _spinSpeed = Random.Range(0.1f, 1f);
                _spriteGameObject.transform.Rotate(Vector3.forward * _spinSpeed);
                break;
            case 1:
                SetSpeed(Random.Range(1,3));
                StartCoroutine(EnemyShooting());
                break;
        }
    }


    private void CalculateMovement()
    {
        //if the enemy is not a mainMenuEnemy or StartGameEnemy enable movement
        if (!_mainMenuEnemy && !_startGameEnemy)
        {
            transform.Translate(Vector3.down * (_movementSpeed * Time.deltaTime));

            if (transform.position.y <=  _screenBoundsY )
            {
                float randomX = Random.Range(-_screenBoundsX, _screenBoundsX);
                transform.position = new Vector3(randomX, _offScreenSpawnHeight, 0);
            }
        }
        
        //Adding spinning to a meteor enemy
        if (_EnemyTypeID == 0)
        {
            _spriteGameObject.transform.Rotate(Vector3.forward * _spinSpeed);
        }
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        //Debug.Log("Hit "+other.transform.name);
        
        if (other.CompareTag("Player"))
        {
            SpacePlayer playerData = other.transform.GetComponent<SpacePlayer>();

            if (playerData != null)
                playerData.ReceiveDamage();
            else
                Debug.LogError("player script not found on collision");

            _animator.SetBool(_explosionAnimBool, true);
            
            StartCoroutine(DestroySequence(1));
        }

        if (other.CompareTag("Projectile"))
        {
            Projectile projectile = other.GetComponent<Projectile>();

            if (!projectile.isEnemyLaser())
            {
                _animator.SetBool(_explosionAnimBool, true);
                
                if (_player != null && !_player.GetMainMenuPlayer() && !projectile.isEnemyLaser())
                    _player.AddScore(_scorePerKill);

                StartCoroutine(DestroySequence(1));
                Destroy(other.gameObject);
            }
        }
    }

    public void SetSpeed(float speed)
    {
        this._movementSpeed = speed;
    }

    //A coroutine used to delay a death.
    //Used to disable collisions and let animations play
    private IEnumerator DestroySequence(float seconds)
    {
        _audioSource.clip = _explosionSFX;
        _audioSource.Play();

        _spinSpeed = 0;
        _spriteGameObject.transform.rotation = Quaternion.Euler(Vector3.zero);
        _collider2D.enabled = false;
        
        yield return new WaitForSeconds(seconds);
        
        
        //Searching for Main Menu Specific components
        if (_mainMenuEnemy)
        {
            MainMenuUiManager mainMenuUiManager = GameObject.Find("UI_Manager").GetComponent<MainMenuUiManager>();

            if (mainMenuUiManager != null)
                mainMenuUiManager.LoadMainStage(true);
            else
                Debug.LogError("Main Menu UI component is null");
        }

        InitiateGame();
        
        Destroy(this.gameObject);
    }

    private void InitiateGame()
    {
        //Initiating the game once this enemy is defeated.
        if (_startGameEnemy)
        {
            AudioSource _backgroundAudioSource = GameObject.Find("Audio_Manager").GetComponentInChildren<AudioSource>();
            
            if (_backgroundAudioSource != null)
                _backgroundAudioSource.Play();
            else
                Debug.LogError("Background Audio Source component is null");
            
            SpawnManager _spawnManager = GameObject.Find("Spawn_Manager").GetComponent<SpawnManager>();
            
            if (_spawnManager != null)
                _spawnManager.StartSpawning();
            else
                Debug.LogError("Spawn Manager component is null");

            UIManager _uiManager = GameObject.Find("UI_Manager").GetComponent<UIManager>();
            if (_uiManager != null)
                _uiManager.TurnOffStartGameText();
            else
                Debug.LogError("UI Manager component is null");
        }
    }

    private IEnumerator EnemyShooting()
    {
        GameManager gameManager = GameObject.Find("Game_Manager").GetComponent<GameManager>();
        
        while (!_stopShooting)
        {
            _audioSource.clip = _laserSFX;
            _audioSource.Play();
            
            Projectile laser = Instantiate(_laserPrefab, transform.position, Quaternion.identity).GetComponent<Projectile>();
            laser.SetEnemyLaser(true);
            yield return new WaitForSeconds(Random.Range(3, 6));
            
            if (gameManager != null)
            {
                if (gameManager.GetGameOver())
                {
                    _stopShooting = true;
                }
            }
            else
                Debug.LogError("Game manager component is null");
        }
    }
}
