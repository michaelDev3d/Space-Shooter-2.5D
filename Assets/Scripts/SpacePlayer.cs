using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class SpacePlayer : MonoBehaviour
{
    [Header("Player Stats")]
    [SerializeField]
    private int _playerLives = 3;
    [SerializeField]
    private float _fireRate = 0.5f;
    [SerializeField]
    private float _movementSpeed = 5f;
    //[SerializeField]
    //private float _thrusterMultiplier = 1.2f;
    [SerializeField]
    private SpawnManager _spawnManager;

    [Header("Player Movement Data")]
    [SerializeField]
    private float _horizontal;
    [SerializeField]
    private float _vertical;

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
        
    [Header("Player Shooting Data")]
    [SerializeField]
    private GameObject _laserPrefab;
    [SerializeField]
    private GameObject _tripleLaserPrefab;
    [SerializeField]
    private GameObject _OrbitalLaserPrefab;
    [SerializeField]
    private Vector3 _laserOffset = new Vector3(0f, 0.8f, 0f);
    private float _cooldownTimer;
    [SerializeField] 
    private bool _isTripleShotPowerUpActive;
    [SerializeField] 
    private float _speedBoostMultiplier = 2f;
    [SerializeField] 
    private bool _isShieldPowerUpActive;
    
    [SerializeField] 
    private bool _isOrbitalShotPowerUpActive;
    
    private Renderer _renderer;
    private Material _material;
    
    [SerializeField] 
    public int _ammoCount = 15;
    
    
    [Header("Effects")]
    [SerializeField] 
    private bool _flickerEffectOn;
    [SerializeField] 
    private bool _flickerShieldEffectOn;
    
    [Header("Audio")]
    [SerializeField] 
    private AudioSource _audioSource;
    [SerializeField] 
    private AudioClip _laserSFX;
    [SerializeField] 
    private AudioClip _tripleShotSFX;
    [SerializeField] 
    private AudioClip _explosionSFX;
        
    [Header("Level Data")]
    [SerializeField] 
    private int _score;
    [SerializeField] 
    private UIManager _uiManager;
    [SerializeField] 
    private bool _mainMenuPlayer;

    [SerializeField] 
    private GameManager _gameManager;

    private static readonly int _playerMaterialOutlineColor = Shader.PropertyToID("_OutlineColor");
    private static readonly int _playerMaterialTurnOnOutline = Shader.PropertyToID("TurnOnOutline");
    private static readonly int _playerMaterialTurnOnColorMask = Shader.PropertyToID("_TurnOnMask");
    private static readonly int _playerMaterialColorMaskColor = Shader.PropertyToID("_ColorMaskColor");

    private CameraEffects _cameraEffects;

    [SerializeField]
    private bool _playerThrusterActive;
    
    
    // Start is called before the first frame update
    void Start()
    {
        CreatePlayer();
        
        transform.position = new Vector3(0,-0.45f, 0);

        //Starting position of main menu character
        if (_mainMenuPlayer)
        {
            transform.position = new Vector3(0,-2.15f, 0);
        }
    }
    
    //Referencing components and null checking for them
    private void CreatePlayer()
    {
        if (TryGetComponent(out Renderer spriteRenderer))
        {
            _renderer = spriteRenderer;
        }
        else
            Debug.LogError("Renderer Component not found on player");

        _material = _renderer.material;
        _material.SetInt(_playerMaterialTurnOnOutline, 0);
        
        if (TryGetComponent(out AudioSource audioSource))
            _audioSource = audioSource;
        else
            Debug.LogError("Audio Source Component not found on player");

        if (TryGetComponent(out Animator playerAnimator))
            _playerAnimator = playerAnimator;
        else
            Debug.LogError("Animator Component not found on player");


        switch (_mainMenuPlayer)
        {
            case true:
                return;
            case false:
            {
                GameObject uiManagerGameObject = GameObject.Find("UI_Manager");

                if (uiManagerGameObject != null)
                {
                    if (uiManagerGameObject.TryGetComponent(out UIManager uiManager))
                        _uiManager = uiManager;
                    else
                        Debug.LogError("UI Manager component is null");
                }

                break;
            }
        }

        GameObject spawnManagerGameObject = GameObject.Find("Spawn_Manager");

        if (spawnManagerGameObject != null)
        {
            if (spawnManagerGameObject.TryGetComponent(out SpawnManager spawnManager))
                _spawnManager = spawnManager;
            else
                Debug.LogError("Spawn Manager component is null");
        }

        GameObject gameManager = GameObject.Find("Game_Manager");
            
        if (gameManager != null)
        {
            if (gameManager.TryGetComponent(out GameManager gameManagerIsPresent))
                _gameManager = gameManagerIsPresent;
            else
                Debug.LogError("Game Manager component is null");
        }

        _cameraEffects = GameObject.Find("Effect_Manager").GetComponent<CameraEffects>();
    }
    
    void Update()
    {
        HandleMovement();

        if (_mainMenuPlayer)
        {
            ShootInput();
        }

        if (_gameManager != null)
        {
            if (!_gameManager.GetGameIsPaused())
            {
                ShootInput();
                
            }
        }
         
        HandleThruster();
        CheckOrbitalShotDeactivation();
    }

    #region Movement
        private void HandleThruster()
        {
            if (Input.GetKeyDown(KeyCode.LeftShift))
                _movementSpeed = _movementSpeed + 6; 
            if(Input.GetKeyUp(KeyCode.LeftShift))
                _movementSpeed = _movementSpeed - 6;
        }

        private void HandleMovement()
        {
            
            //Getting input data.
            _horizontal = Input.GetAxis("Horizontal");
            _vertical = Input.GetAxis("Vertical");

            if (_playerAnimator != null)
            {
                //Set animator value to input
                _playerAnimator.SetFloat(_animHorizontal, _horizontal);
            
                //If inputs are not pressed/transition from left to right
                //Set the animation to idle.
                if (_animHorizontal == 0)
                {
                    _playerAnimator.SetBool(_animVertical, true);
                }
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
        
    #endregion

    #region Dealing/Recieving Damage

        private void ShootInput()
        {
            if (_ammoCount > 0)
            {
                //Shooting Input and logic.
                if (Input.GetKeyDown(KeyCode.Space) && Time.time > _cooldownTimer)
                {
                    #region Shooting Logic
                    _cooldownTimer = Time.time + _fireRate;

                    if (_isTripleShotPowerUpActive)
                    {
                        Instantiate(_tripleLaserPrefab, transform.position + _laserOffset, Quaternion.identity);

                        if (_tripleShotSFX != null)
                        {
                            _audioSource.clip = _tripleShotSFX;
                            _audioSource.Play();
                        }
                    }
                    else
                    {
                        Instantiate(_laserPrefab, transform.position + _laserOffset, Quaternion.identity);

                        if (_laserSFX != null)
                        {
                            _audioSource.clip = _laserSFX;
                            _audioSource.Play();
                        }
                    }
                    #endregion
                    
                    _ammoCount--;
                    
                    if (_ammoCount == 0)
                    {
                        _uiManager.BlinkAmmoCountText();
                    }
                }
                
                _uiManager.UpdateAmmoCountUI(_ammoCount);
            }
        }

        public void ReceiveDamage()
        {
            _cameraEffects.ShakeCamera();
            
            if (_flickerShieldEffectOn || _flickerEffectOn)
            {
                return;
            }

            if (_isShieldPowerUpActive)
            {
                if (CheckIfPlayerHasShieldBoostColorMask())
                {
                    if (_material.GetColor(_playerMaterialOutlineColor) == Color.clear)
                    {
                        _isShieldPowerUpActive = false;
                        _material.SetInt(_playerMaterialTurnOnColorMask, 0);
                        FlickerShieldColorMaskEffect(0.15f, 2f);
                        return;
                    }
                }

                //Adjusting color of outline shader.
                if (_material.GetColor(_playerMaterialOutlineColor) == Color.cyan || _material.GetColor(_playerMaterialOutlineColor) == Color.clear)
                {
                    FlickerShieldOutlineEffect(0.15f, 2f);
                }

                if (_material.GetColor(_playerMaterialOutlineColor) == Color.green)
                {
                    FlickerShieldOutlineEffectWithAdditionalPowerUp(0.15f, 1f, Color.green);
                }
                
                if (_material.GetColor(_playerMaterialOutlineColor) == Color.red)
                {
                    FlickerShieldOutlineEffectWithAdditionalPowerUp(0.15f, 1f, Color.red);
                }

                return;
            }
            
            _uiManager.RemoveHealthFromBar(_playerLives,1f, true);
            _playerLives--;
            _material.SetInt(_playerMaterialTurnOnColorMask, 1);
            
            FlickerPlayerEffect(0.15f, 2f);
            if (_playerLives < 1)
            {
                if(_spawnManager != null)
                    _spawnManager.OnPlayerDeath();
                
                if(_spawnManager != null)
                    _uiManager.UpdateHealthOnDeath();
                
                Destroy(this.gameObject);
            }
        }

    #endregion

    #region Power Ups

        public void ActivateTripleShot()
        {
            _isTripleShotPowerUpActive = true;

            AdjustMaterialAppearance(_material, _playerMaterialOutlineColor, Color.green, _playerMaterialTurnOnOutline, 1);
            
            StartCoroutine(TripleShotDeactivateRoutine(5));
        }

        IEnumerator TripleShotDeactivateRoutine(float seconds)
        {
            yield return new WaitForSeconds(seconds);
            
            AdjustMaterialAppearance(_material, _playerMaterialOutlineColor, Color.clear, _playerMaterialTurnOnOutline, 0);
            
            _isTripleShotPowerUpActive = false;
        }
        
        public void ActivateSpeedBoost()
        {
            _movementSpeed *= _speedBoostMultiplier;
            AdjustMaterialAppearance(_material, _playerMaterialOutlineColor, Color.red, _playerMaterialTurnOnOutline, 1);
            
            StartCoroutine(SpeedBoostDeactivateRoutine(5f));
        }
        
        IEnumerator SpeedBoostDeactivateRoutine(float seconds)
        {
            yield return new WaitForSeconds(seconds);
            
            AdjustMaterialAppearance(_material, _playerMaterialOutlineColor, Color.clear, _playerMaterialTurnOnOutline, 0);
            
            _movementSpeed /= _speedBoostMultiplier;
        }
        
        public void ActivateShield()
        {
            _isShieldPowerUpActive= true;
            AdjustMaterialAppearance(_material, _playerMaterialOutlineColor, Color.cyan, _playerMaterialTurnOnOutline, 1);
            AdjustMaterialAppearance(_material, _playerMaterialColorMaskColor, Color.blue, _playerMaterialTurnOnColorMask, 1);
           
        }
        
        public void AmmoPickUp()
        {
            if (_ammoCount < 15)
                _ammoCount++;
        }
        
        public void HealthPickup()
        {
            if(_playerLives < 3)
                _uiManager.AddHealthToBar(_playerLives,1f, true);
        }

        public void ActivateOrbitalProjectiles()
        {
            Instantiate(_OrbitalLaserPrefab, transform.position, Quaternion.identity, gameObject.transform);
            _isOrbitalShotPowerUpActive = true;
        }

        private void CheckOrbitalShotDeactivation()
        {
            if (_isOrbitalShotPowerUpActive)
            {
                if (!gameObject.GetComponentInChildren<Projectile>())
                {
                    Destroy(gameObject.transform.GetChild(0).gameObject);
                    
                    _isOrbitalShotPowerUpActive = false;
                }
                 
            }
        }
    
    #endregion
    
    #region Player Hit Effects

        #region Player Hit Flicker (Lose health)
            
            private void FlickerPlayerEffect(float flickerDelay, float seconds)
            {
                StartCoroutine(PlayerHitFlicker(flickerDelay));
                StartCoroutine(DeactivatePlayerFlickerEffect(seconds));
            }

            IEnumerator PlayerHitFlicker(float flickerDelay)
            {
                _flickerEffectOn = true;
                
                while (_flickerEffectOn)
                {
                    _material.SetColor(_playerMaterialColorMaskColor, Color.white);
                    yield return new WaitForSeconds(flickerDelay);
                    _material.SetColor(_playerMaterialColorMaskColor, Color.clear);
                    yield return new WaitForSeconds(flickerDelay);
                }
            }

            IEnumerator DeactivatePlayerFlickerEffect(float seconds)
            {
                yield return new WaitForSeconds(seconds);
                _flickerEffectOn = false;
            }
            
        #endregion

        #region Player Hit Flicker (Lose Shield)

            private bool CheckIfPlayerHasShieldBoostColorMask()
            {
                if (_material.GetColor(_playerMaterialColorMaskColor) == Color.blue)
                {
                    Debug.Log("Player still has shield");
                    return true;
                }
                
                Debug.Log("Player does not have shield");
                return false;
            }

            private void FlickerShieldColorMaskEffect(float flickerDelay, float seconds)
            {
                _material.SetInt(_playerMaterialTurnOnColorMask, 1);
                StartCoroutine(PlayerColorMaskHitFlicker(flickerDelay));
                StartCoroutine(DeactivatePlayerShieldColorMaskFlickerEffect(seconds));
            }
            
            IEnumerator PlayerColorMaskHitFlicker(float flickerDelay)
            {
                _flickerEffectOn = true;
                
                while (_flickerEffectOn)
                {
                    _material.SetColor(_playerMaterialColorMaskColor, Color.cyan);
                    yield return new WaitForSeconds(flickerDelay);
                    _material.SetColor(_playerMaterialColorMaskColor, Color.clear);
                    yield return new WaitForSeconds(flickerDelay);
                }
            }
            
            IEnumerator DeactivatePlayerShieldColorMaskFlickerEffect(float seconds)
            {
                yield return new WaitForSeconds(seconds);
                _flickerEffectOn = false;
            }

        #endregion

        #region Player Hit Flicker (First hit of shield, Lose Outline)

            private void FlickerShieldOutlineEffect(float flickerDelay, float seconds)
            {
                StartCoroutine(PlayerShieldOutlineFlicker(flickerDelay));
                StartCoroutine(DeactivateShieldOutlineFlickerEffect(seconds));
            }

            IEnumerator PlayerShieldOutlineFlicker(float flickerDelay)
            {
                _flickerShieldEffectOn = true;

                while (_flickerShieldEffectOn)
                {
                    _material.SetColor(_playerMaterialOutlineColor, Color.cyan );
                    yield return new WaitForSeconds(flickerDelay);
                    _material.SetColor(_playerMaterialOutlineColor, Color.clear );
                    yield return new WaitForSeconds(flickerDelay);
                }
            }
            
            IEnumerator DeactivateShieldOutlineFlickerEffect(float seconds)
            {
                yield return new WaitForSeconds(seconds);
                _flickerShieldEffectOn = false;
            }

        #endregion

        #region Player Hit Flicker (With Shield and additional power up)

            private void FlickerShieldOutlineEffectWithAdditionalPowerUp(float flickerDelay, float seconds, Color powerUpColor)
            {
                StartCoroutine(PlayerShieldOutlineFlickerWithAdditionalPowerUp(flickerDelay, powerUpColor));
                StartCoroutine(DeactivateShieldOutlineFlickerEffectWithAdditionalPowerUp(seconds));
            }

            IEnumerator PlayerShieldOutlineFlickerWithAdditionalPowerUp(float flickerDelay, Color powerUpColor)
            {
                _flickerShieldEffectOn = true;

                while (_flickerShieldEffectOn)
                {
                    _material.SetColor(_playerMaterialOutlineColor, Color.cyan );
                    yield return new WaitForSeconds(flickerDelay);
                    _material.SetColor(_playerMaterialOutlineColor, Color.clear );
                    yield return new WaitForSeconds(flickerDelay);
                }
                
                _material.SetColor(_playerMaterialOutlineColor, powerUpColor );
            }
                    
            IEnumerator DeactivateShieldOutlineFlickerEffectWithAdditionalPowerUp(float seconds)
            {
                yield return new WaitForSeconds(seconds);
                _flickerShieldEffectOn = false;
            }

        #endregion

        //Method to turn on/off or change the color of our outline or color mask shader.
        private void AdjustMaterialAppearance(Material gameObjectMaterial, int materialColorID, Color color,int materialVisibleID, int active)
        {
            if (gameObjectMaterial != null)
            {
                gameObjectMaterial.SetColor(materialColorID, color);
                gameObjectMaterial.SetInt(materialVisibleID, active);
            }
        }

    #endregion

    #region Scene Specific Methods

        public bool GetMainMenuPlayer()
        {
            return _mainMenuPlayer;
        }
        
        public int GetScore()
        {
            return _score;
        }
        
        public void AddScore(int score)
        {
            _score += score;
            
            _uiManager.UpdateScore(_score);
        }

    #endregion
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Projectile"))
        {
            Projectile projectile = other.GetComponent<Projectile>();

            if (projectile != null)
            {
                if (projectile.IsEnemyLaser())
                {
                    _audioSource.clip = _explosionSFX;
                    _audioSource.Play();
                    
                    ReceiveDamage();
                    Destroy(other.gameObject);
                }
            }
        }
    }
}
