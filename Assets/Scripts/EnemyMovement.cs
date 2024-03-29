﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class EnemyMovement :  Rarity
{
    [SerializeField] private int _enemyTypeID;
    [SerializeField] private bool _isEvent;
    [SerializeField] private bool _isBossEnemy;
    
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
    
    //[Header("Swarm Enemy Data")]
    //[SerializeField] 
    private float _swarmRotationSpeed = 0.15f;
    //[SerializeField] 
    private int _enemyCount;
    //[SerializeField] 
    private GameObject _swarmEnemyPivotGameObject;
    //[SerializeField] 
    private GameObject _swarmEnemyContainer;

    [Header(("Special Attributes"))] 
    [SerializeField]
    private bool _hasShield;
    [SerializeField] 
    private bool _flickerShieldEffectOn;
    
    private Renderer _renderer;
    private Material _material;
    private static readonly int _enemyMaterialOutlineColor = Shader.PropertyToID("_OutlineColor");
    private static readonly int _enemyMaterialTurnOnOutline = Shader.PropertyToID("TurnOnOutline");
    private bool _boolSwapMovementDirection;
    
    
    [Header(("Boss Stats"))] 
    [SerializeField] 
    private GameObject _specialAttackLeftWeapon;
    [SerializeField] 
    private GameObject _specialAttackRightWeapon;
    [SerializeField] 
    private GameObject _regularAttackLeftWeapon;
    [SerializeField] 
    private GameObject _regularAttackRightWeapon;
    [SerializeField] 
    private GameObject _bossAttackPrefab;

    
    void Start()
    {
        CreateEnemy(_enemyTypeID);
    }

    void Update()
    {
        CalculateMovement();
    }
    
    //Referencing components and null checking for them
    private void CreateEnemy(int enemyID)
    {
        if (!_isEvent)
        {
            if (gameObject.transform.GetChild(0).gameObject.TryGetComponent(out Renderer spriteRenderer))
            {
                _renderer = spriteRenderer;
            }
            else
                Debug.LogError("Renderer Component not found on enemy");

            if (_renderer != null)
            {
                _material = _renderer.material;
                _material.SetInt(_enemyMaterialTurnOnOutline, 0);
            }


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
                //Rendering Gameobject
                _spriteGameObject = transform.GetChild(0).gameObject;

                //Animator
                if (_spriteGameObject.TryGetComponent(out Animator animator))
                    _animator = animator;
                else
                    Debug.LogError("Animator component for " + gameObject.name + " is null");
            }
            else
                Debug.LogError(
                    "Enemy does not have child gameObject, therefore enemy does not have sprite or animator");


            //Getting BoxCollider2D and NULL checking
            if (TryGetComponent(out BoxCollider2D boxCollider))
                _collider2D = boxCollider;
            else
                Debug.LogError("Collider component for " + gameObject.name + " is null");
            
            //Getting AudioSource and NULL checking
            if (TryGetComponent(out AudioSource audioSource))
                _audioSource= audioSource;
            else
                Debug.LogError("Audio component for "+ gameObject.name +" is null");
        }

        if (_isEvent)
        {
            //Finding and Null Checking main Player.
            GameObject EventEnemy = transform.GetChild(0).gameObject;

            if (EventEnemy != null)
            {
               
                if (EventEnemy.TryGetComponent(out AudioSource audioSource))
                    _audioSource= audioSource;
                else
                    Debug.LogError("Audio component for "+ gameObject.name +" is null");
                
                if (EventEnemy.TryGetComponent(out BoxCollider2D boxCollider))
                    _collider2D = boxCollider;
                else
                    Debug.LogError("Collider component for " + gameObject.name + " is null");
                
                if (EventEnemy.TryGetComponent(out Animator animator))
                    _animator = animator;
                else
                    Debug.LogError("Animator component for " + gameObject.name + " is null");
            }
            else
                Debug.LogError("LaserEnemy is NULL");
                
        }

        if (!_isBossEnemy)
        {
            switch (enemyID)
            {
                case 0:
                    _spinSpeed = Random.Range(0.1f, 1f);
                    _spriteGameObject.transform.Rotate(Vector3.forward * _spinSpeed);
                    break;
                case 1:
                    SetSpeed(Random.Range(1, 3));
                    StartCoroutine(EnemyShooting());
                    break;
                case 2:
                    _enemyCount = SwarmEnemyCount();
                    StartCoroutine(EnemyShooting());
                    break;
                case 3:
                    SetSpeed(Random.Range(0.5f, 0.7f));
                    StartCoroutine(EnemyShooting());
                    ActivateShield();
                    break;
                case 4:
                    transform.position = new Vector3(5.35f, -2.25f, 0);
                    SetSpeed(Random.Range(1.1f, 1.5f));
                    StartCoroutine(EnemyShooting());
                    StartCoroutine(FleeSequence(20));
                    break;
                case 10:
                    StartCoroutine(LaserEventSequence());
                    // Debug.Log("Boss Laser Event");
                    break;
            }
        }

        if (_isBossEnemy)
        {
            switch (enemyID)
            {
                case 0:
                    StartCoroutine(BossAttackSequence());
                    break;
            }
        }
    }

    private void CalculateMovement()
    {
        //if the enemy is not a mainMenuEnemy or StartGameEnemy enable movement
        if (!_mainMenuEnemy && !_startGameEnemy && _enemyTypeID != 10 && _enemyTypeID == 0 || _enemyTypeID == 1 || _enemyTypeID == 3)
        {
            transform.Translate(Vector3.down * (_movementSpeed * Time.deltaTime));

            if (transform.position.y <= _screenBoundsY)
            {
                float randomX = Random.Range(-_screenBoundsX, _screenBoundsX);
                transform.position = new Vector3(randomX, _offScreenSpawnHeight, 0);
            }
        }

        //Adding spinning to a meteor enemy
        if (_enemyTypeID == 0)
        {
            _spriteGameObject.transform.Rotate(Vector3.forward * _spinSpeed);
        }

        if (_enemyTypeID == 2)
        {
            _enemyCount = SwarmEnemyCount();

            _swarmEnemyPivotGameObject.transform.Rotate(Vector3.forward,
                _swarmRotationSpeed / _enemyCount, Space.Self);

            transform.Rotate(Vector3.forward, -_swarmRotationSpeed, Space.World);

            GameObject _spawnEnemyContainer = _swarmEnemyPivotGameObject.transform.parent.gameObject;
            _spawnEnemyContainer.transform.Translate(Vector3.right * (-_movementSpeed * Time.deltaTime));


            if (_spawnEnemyContainer.transform.position.x < -_screenBoundsX)
            {
                Destroy(_spawnEnemyContainer);
            }
        }

        if (_enemyTypeID == 4)
        {
            if (!_boolSwapMovementDirection)
            {
                transform.Translate(Vector3.right * (_movementSpeed * Time.deltaTime));

                if (transform.position.x < -5.5)
                {
                    _movementSpeed = _movementSpeed * -1;
                }

                if (transform.position.x > 5.5)
                {
                    _movementSpeed = _movementSpeed * -1;
                }
            }
            else
            {
                transform.Translate(Vector3.down * (_movementSpeed * Time.deltaTime));
            }
        }

        if (_enemyTypeID == 10)
        {
            GameObject Laser = transform.GetChild(0).gameObject;
            Laser.transform.Translate(Vector3.down * (_movementSpeed * Time.deltaTime));
            
            if (Laser.transform.position.y < -12)
                SetSpeed(0);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Hit: "+other.transform.name);
        
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

            if (!projectile.CheckIfIsEnemyLaser())
            {
                if (_hasShield)
                {
                    Destroy(projectile.gameObject);
                    if (_material.GetColor(_enemyMaterialOutlineColor) == Color.cyan)
                    {
                        FlickerShieldOutlineEffect(0.15f, 2f);
                    }
                }

                if (!_hasShield && !_flickerShieldEffectOn)
                {
                    _animator.SetBool(_explosionAnimBool, true);

                    if (_player != null && !_player.GetMainMenuPlayer() && !projectile.CheckIfIsEnemyLaser())
                        _player.AddScore(_scorePerKill);



                    StartCoroutine(DestroySequence(1));
                    Destroy(other.gameObject);
                }
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
        GameObject spawnManagerGameObject= GameObject.Find("Spawn_Manager");
        SpawnManager spawnManagerComponent = null;

        if (spawnManagerGameObject != null)
        {
            if (spawnManagerGameObject.TryGetComponent(out SpawnManager spawnManager))
                spawnManagerComponent = spawnManager;
            else
                Debug.Log("Spawn Manager is Null");
        }
        
        if(!_mainMenuEnemy)
            spawnManagerComponent.EnemiesDefeated++;
        
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

    private IEnumerator FleeSequence(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        _movementSpeed = 0;
        yield return new WaitForSeconds(1);
        _stopShooting = true;
        _movementSpeed = -3;
        _boolSwapMovementDirection = true;
        yield return new WaitForSeconds(5);
        Destroy(this.gameObject);

    }

    private void InitiateGame()
    {
        //Initiating the game once this enemy is defeated.
        if (_startGameEnemy)
        {
            AudioSource backgroundAudioSource = GameObject.Find("Audio_Manager").GetComponentInChildren<AudioSource>();
            
            if (backgroundAudioSource != null)
                backgroundAudioSource.Play();
            else
                Debug.LogError("Background Audio Source component is NULL in "+gameObject.name);
            
            SpawnManager spawnManager = GameObject.Find("Spawn_Manager").GetComponent<SpawnManager>();
            
            if (spawnManager != null)
                spawnManager.StartSpawning();
            else
                Debug.LogError("Spawn Manager component is n NULL in "+gameObject.name);

            UIManager uiManager = GameObject.Find("UI_Manager").GetComponent<UIManager>();

            if (uiManager != null)
            {
                uiManager.TurnOffStartGameText();
                uiManager.BlinkWaveText(0.5f, 3f);
            }
            else
                Debug.LogError("UI Manager component is NULL in "+gameObject.name);
        }
    }

    private IEnumerator EnemyShooting()
    {
        GameManager gameManager = GameObject.Find("Game_Manager").GetComponent<GameManager>();
        
        while (!_stopShooting)
        {
            _audioSource.clip = _laserSFX;
            _audioSource.Play();
            
            if (_enemyTypeID != 4)
            {
                Debug.Log("Testing Top screen enemy");
                Projectile laser = Instantiate(_laserPrefab, transform.position, Quaternion.Euler(Vector3.down))
                    .GetComponent<Projectile>();
                laser.SetEnemyLaser(true);
                
                if (_enemyTypeID == 2)
                    laser.SetSwarmEnemyLaser(true);
            }

            if (_enemyTypeID == 4)
            {
                Debug.Log("Testing Bottom screen enemy");
                if (transform.position.x >= _player.transform.position.x-0.3 && transform.position.x <= _player.transform.position.x+0.3)
                {
                    Projectile laser = Instantiate(_laserPrefab, transform.position, Quaternion.Euler(Vector3.down))
                        .GetComponent<Projectile>();
                    
                    laser.SetEnemyLaser(true);
                    laser.ReverseDirection = true;
                }
              
            }

            if(_enemyTypeID == 4) 
                yield return new WaitForSeconds(Random.Range(0.3f, 0.5f));
           
            if(_enemyTypeID != 4) 
                yield return new WaitForSeconds(Random.Range(2, 4));
            
            if (gameManager != null)
            {
                if (gameManager.GetGameOver())
                {
                    _stopShooting = true;
                }
            }
            else
                Debug.LogError("Game manager component is NULL in "+gameObject.name);
            
        }
    }

    private void ActivateShield()
    {
        _hasShield= true;
        AdjustMaterialAppearance(_material, _enemyMaterialOutlineColor, Color.cyan, _enemyMaterialTurnOnOutline, 1);
           
    }
    
    private void AdjustMaterialAppearance(Material gameObjectMaterial, int materialColorID, Color color,int materialVisibleID, int active)
    {
        if (gameObjectMaterial != null)
        {
            gameObjectMaterial.SetColor(materialColorID, color);
            gameObjectMaterial.SetInt(materialVisibleID, active);
        }
    }
    
    private void FlickerShieldOutlineEffect(float flickerDelay, float seconds)
    {
        SetSpeed(0);
        StartCoroutine(EnemyShieldOutlineFlicker(flickerDelay));
        StartCoroutine(DeactivateShieldOutlineFlickerEffect(seconds));
    }

    IEnumerator EnemyShieldOutlineFlicker(float flickerDelay)
    {
        _flickerShieldEffectOn = true;

        while (_flickerShieldEffectOn)
        {
            _material.SetColor(_enemyMaterialOutlineColor, Color.cyan );
            yield return new WaitForSeconds(flickerDelay);
            _material.SetColor(_enemyMaterialOutlineColor, Color.clear );
            yield return new WaitForSeconds(flickerDelay);
        }
    }
            
    IEnumerator DeactivateShieldOutlineFlickerEffect(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        _flickerShieldEffectOn = false;
        _hasShield = false;
        SetSpeed(Random.Range(0.5f,0.7f));
    }
    
    private int SwarmEnemyCount()
    {
        _swarmEnemyPivotGameObject = gameObject.transform.parent.gameObject;
        _swarmEnemyContainer =   _swarmEnemyPivotGameObject.gameObject.transform.parent.gameObject;

        if (_swarmEnemyPivotGameObject != null && _swarmEnemyContainer != null)
        {
            return _enemyCount = _swarmEnemyPivotGameObject.GetComponentsInChildren<EnemyMovement>().Length;
        }
        
        Debug.LogError("Swarm container not found");
        return 0;
    }

    private IEnumerator LaserEventSequence()
    {
        GameObject warningIcon = transform.GetChild(1).gameObject;
        //Debug.Log("Warning Icon: ", warningIcon.gameObject);
        GameObject dangerLine = transform.GetChild(2).gameObject;
        //Debug.Log("Danger Line: ", dangerLine.gameObject);
        
        dangerLine.SetActive(false);

        yield return new WaitForSeconds(warningIcon.GetComponent<Blink>().DestroyInSeconds-2);
        dangerLine.SetActive(true);
        dangerLine.GetComponent<LineRenderer>().enabled = true;
        dangerLine.GetComponent<Blink>().enabled = true;
        dangerLine.GetComponent<Blink>().ToggleDestroy = true;
        dangerLine.GetComponent<Blink>().ToggleBlinking = true;
        yield return new WaitForSeconds(1);
        SetSpeed(50);
        Destroy(gameObject,1);
    }
    
    private IEnumerator BossAttackSequence()
    {
        yield return new WaitForSeconds(1);
        Instantiate(_bossAttackPrefab, _specialAttackRightWeapon.transform);
        Instantiate(_bossAttackPrefab, _specialAttackLeftWeapon.transform);
    }

    public int EnemyTypeID
    {
        get => _enemyTypeID;
        set => _enemyTypeID = value;
    }
}
