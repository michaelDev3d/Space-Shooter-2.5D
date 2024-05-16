using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("Game UI Text Elements")]
    [SerializeField]
    private TextMeshProUGUI _scoreText;
    [SerializeField]
    private TextMeshProUGUI _gameOverText;
    [SerializeField]
    private TextMeshProUGUI _restartGameText;
    [SerializeField]
    private TextMeshProUGUI _startGameText;
    [SerializeField] 
    private TextMeshProUGUI _pauseGameText;
    [SerializeField] 
    private TextMeshProUGUI _ShieldCountText;
    [SerializeField]
    private TextMeshProUGUI _waveText;
    [SerializeField]
    private TextMeshProUGUI _waveCountText;
    [SerializeField]
    private TextMeshProUGUI _waveBossText;
    [SerializeField]
    private TextMeshProUGUI _waveCompleteText;
    [SerializeField]
    private TextMeshProUGUI _bossStageText;
    [SerializeField] 
    private TextMeshProUGUI _levelCompleteText;
    
    [SerializeField]
    private Button _QuitButton;
    [SerializeField]
    private Button _MainMenuButton;
    private GameManager _gameManager;
    [SerializeField]
    private Button _restartButton;
    
    [SerializeField] 
    private TextMeshProUGUI _ammoCountText;
    [SerializeField] 
    private TextMeshProUGUI _ammoCountTextREDEffect;

    
    [Header("Health Bar Management")] 
    [SerializeField] 
    private GameObject _healthBarGameObject;
    [SerializeField] 
    private float _CurrentPlayerLives;
    [SerializeField] 
    private float _timer;
    [SerializeField] 
    private float _maxPlayerLives = 3f;
    [SerializeField] 
    private float _drainHealthUiSpeed = 2f;
    [SerializeField]
    private Image frontHealthBar;
    [SerializeField]
    private Image backHealthBar;
    
    [Header("Boss Health Bar Management")] 
    [SerializeField] 
    private GameObject _bossHealthBarGameObject;
    [SerializeField] 
    private float _bossTimer;
    [SerializeField] 
    private float _CurrentBossHealth;
    [SerializeField] 
    private float _maxBossHealth = 3f;
    [SerializeField] 
    private float _drainBossHealthUiSpeed = 2f;
    [SerializeField]
    private Image frontBossHealthBar;
    [SerializeField]
    private Image backBossHealthBar;

    [SerializeField] 
    private bool _isUpdatingUI;
    [SerializeField] 
    private bool _isUpdatingBossUI;
    [SerializeField] 
    private bool _bossHasBeenDefeated;

    [Header("Temporary Blink Flags")]
    [SerializeField] 
    private bool _blinkAmmo = true;
    [SerializeField] 
    private bool _blinkWave = true;
    [SerializeField] 
    private bool _blinkCompleteWave = true;
    [SerializeField] 
    private bool _blinkBossStage = true;
    
    private bool _hideShieldAfterBlinking = false;

    private bool _showBossHealthBoss = false;
    
    // Start is called before the first frame update
    void Start()
    {
        if(_scoreText != null)
            _scoreText.text = "Score: " + 0;
        
        if(_gameOverText != null)
            _gameOverText.gameObject.SetActive(false);
        
        if(_restartGameText != null)
            _restartGameText.gameObject.SetActive(false);
        
        if(_ShieldCountText != null)
            _ShieldCountText.gameObject.SetActive(false);
        
        if(_waveText != null)
            _waveText.gameObject.SetActive(false);
        
        if(_levelCompleteText != null)
            _levelCompleteText.gameObject.SetActive(false);
        
        if(_waveCompleteText != null)
            _waveCompleteText.gameObject.SetActive(false);
        
        if(_bossStageText != null)
            _bossStageText.gameObject.SetActive(false);

        if(_waveCountText != null)
            _waveCountText.text = "Wave " + 0;
        
        if(_waveBossText != null)
            _waveBossText.gameObject.SetActive(false);
        
        if(_ammoCountTextREDEffect != null)
            _ammoCountTextREDEffect.gameObject.SetActive(false);

        GameObject _gameManagerGameObject = GameObject.Find("Game_Manager");

        if (_gameManagerGameObject != null)
        {
            if (_gameManagerGameObject.TryGetComponent(out GameManager gameManager))
                _gameManager = gameManager;
            else
                Debug.LogError("Game Manager component is NULL");
        }
        
        _healthBarGameObject =  GameObject.Find("Health_Bar");
        
        if(_healthBarGameObject == null)
            Debug.LogError("Health Bar is null");

        _CurrentPlayerLives = _maxPlayerLives;
        
         
        _bossHealthBarGameObject =  GameObject.Find("Boss Health Bar");

        if (_bossHealthBarGameObject == null)
        {
            Debug.LogError("Boss Health Bar is null");
            
        }

        _CurrentBossHealth = _maxBossHealth;
        _bossHealthBarGameObject.SetActive(false);
        _bossHealthBarGameObject.SetActive(true);
    }

    private void Update()
    {
        UpdateHealthUiOnlyOnChange();
        UpdateBossHealthUiOnlyOnChange();
        ShowBossHealthUI(_showBossHealthBoss);
        _gameManager.isBossStageComplete = _bossHasBeenDefeated;
        ShowLevelCompleteScreen(_bossHasBeenDefeated);
    }

    IEnumerator UpdateHealthAnim(bool turnOn, float seconds)
    {
        UpdateHealthAnimBool(turnOn);
        UpdateHealthUiOnlyOnChange();
        yield return new WaitForSeconds(seconds);
        _isUpdatingUI = false;
    }
    
    private void UpdateHealthUiOnlyOnChange()
    {
        if (_isUpdatingUI)
        {
            _CurrentPlayerLives = Mathf.Clamp(_CurrentPlayerLives, 0, _maxPlayerLives);
            float frontHealthBarFillAmount = frontHealthBar.fillAmount;
            float backHealthBarFillAmount = backHealthBar.fillAmount;
            float healthPercentage = _CurrentPlayerLives / _maxPlayerLives;

            if (backHealthBarFillAmount > healthPercentage)
            {
                frontHealthBar.fillAmount = healthPercentage;
                Color loseHealthColor = Color.gray;                backHealthBar.color = loseHealthColor;
                _timer += Time.deltaTime;

                Debug.Log("Player timer: "+_timer);
                float loseHealthEffectPercentageToComplete = _timer / _drainHealthUiSpeed;
                loseHealthEffectPercentageToComplete *= loseHealthEffectPercentageToComplete;
                backHealthBar.fillAmount = Mathf.Lerp(backHealthBarFillAmount, healthPercentage,
                    loseHealthEffectPercentageToComplete);
            }

            if (frontHealthBarFillAmount < healthPercentage)
            {
                backHealthBar.color = Color.green;
                backHealthBar.fillAmount = healthPercentage;
                _timer += Time.deltaTime;


                float loseHealthEffectPercentageToComplete = _timer / _drainHealthUiSpeed;
                loseHealthEffectPercentageToComplete *= loseHealthEffectPercentageToComplete;
                frontHealthBar.fillAmount = Mathf.Lerp(frontHealthBarFillAmount, backHealthBar.fillAmount,
                    loseHealthEffectPercentageToComplete);
            }
        }
    }

    private void UpdateHealthAnimBool(bool turnOn)
    {
        _isUpdatingUI = turnOn;
    }

    public void RemoveHealthFromBar(int playerLives, float damage, bool turnOn)
    {
        _CurrentPlayerLives = playerLives;
        _CurrentPlayerLives-= damage;
        _timer = 0f;
        
        
        StartCoroutine(UpdateHealthAnim(turnOn, _drainHealthUiSpeed));
    }

    public void AddHealthToBar(int playerLives, float healAmount, bool turnOn)
    {
        _CurrentPlayerLives = playerLives;
        _CurrentPlayerLives += healAmount;
        _timer = 0f;
        
        StartCoroutine(UpdateHealthAnim(turnOn, _drainHealthUiSpeed));
    }
    
     IEnumerator UpdateBossHealthAnim(bool turnOn, float seconds)
    {
        UpdateBossHealthAnimBool(turnOn);
        UpdateBossHealthUiOnlyOnChange();
        yield return new WaitForSeconds(seconds);
        _isUpdatingBossUI = false;
    }
    
    private void UpdateBossHealthUiOnlyOnChange()
    {
        if (_isUpdatingBossUI)
        {
           _CurrentBossHealth = Mathf.Clamp(_CurrentBossHealth, 0, _maxBossHealth);
            float frontBossHealthBarFillAmount = frontBossHealthBar.fillAmount;
            float backBossHealthBarFillAmount = backBossHealthBar.fillAmount;
            float healthPercentage = _CurrentBossHealth / _maxBossHealth;

            if (backBossHealthBarFillAmount > healthPercentage)
            {
                frontBossHealthBar.fillAmount = healthPercentage;
                Color loseHealthColor = Color.gray;
                backBossHealthBar.color = loseHealthColor;       
                _bossTimer += Time.deltaTime;
                
                float effectCatchUpPoint =  _bossTimer / _drainBossHealthUiSpeed;
                effectCatchUpPoint *= effectCatchUpPoint;
                backBossHealthBar.fillAmount = Mathf.Lerp(backBossHealthBarFillAmount, healthPercentage, effectCatchUpPoint);
                
            }

            if (frontBossHealthBarFillAmount < healthPercentage)
            {
                backBossHealthBar.color = Color.green;
                backBossHealthBar.fillAmount = healthPercentage;
                _bossTimer += Time.deltaTime;


                float loseHealthEffectPercentageToComplete =  _bossTimer / _drainBossHealthUiSpeed;
                loseHealthEffectPercentageToComplete *= loseHealthEffectPercentageToComplete;
                frontBossHealthBar.fillAmount = Mathf.Lerp(frontBossHealthBarFillAmount, backBossHealthBar.fillAmount,
                    loseHealthEffectPercentageToComplete);
            }
        }
    }

    private void UpdateBossHealthAnimBool(bool turnOn)
    {
        _isUpdatingBossUI = turnOn;
    }

    public void RemoveHealthFromBossBar(int playerLives, float damage, bool turnOn)
    {
        _CurrentBossHealth = playerLives;
        _CurrentBossHealth-= damage;
        //Debug.Log("Boss timer: "+_bossTimer);
        _bossTimer = 0f;
        
        
        StartCoroutine(UpdateBossHealthAnim(turnOn, _drainBossHealthUiSpeed));
    }

    public void AddHealthToBossBar(int bossHealth, float healAmount, bool turnOn)
    {
        _CurrentBossHealth = bossHealth;
        _CurrentBossHealth += healAmount;
        _bossTimer = 0f;
        
        StartCoroutine(UpdateBossHealthAnim(turnOn, _drainBossHealthUiSpeed));
    }
    
    public void UpdateScore(int playerScore)
    {
        _scoreText.text = "Score: " + playerScore.ToString();
    }
    
    public void UpdateWaveText(int wave)
    {
        _waveText.text = "Wave " + wave.ToString();
        _waveCountText.text = "Wave: " + wave.ToString();
        if (wave == 5)
        {
            _waveCountText.text = "Wave: ";
            if(!_QuitButton.IsActive())
                _waveBossText.gameObject.SetActive(true);
        }
    }

    private void GameOverSequence()
    {
        _gameManager.GameOver();
        _gameOverText.gameObject.SetActive(true);
        _restartGameText.gameObject.SetActive(true);
        _ammoCountText.gameObject.SetActive(false);
        _ammoCountTextREDEffect.gameObject.SetActive(false);
        StartCoroutine( BlinkText( true,_gameOverText,0.5f));
    }

    public void TurnOffStartGameText()
    {
        if(_startGameText != null) 
            _startGameText.gameObject.SetActive(false);
    }

    public void UpdateHealthOnDeath()
    {
        GameOverSequence();
    }

    public bool DisplayUI(bool displayUI)
    {
        if (_CurrentPlayerLives > 0)
        {
            if (_CurrentPlayerLives > 0)
            {
                _gameOverText.gameObject.SetActive(false);
                _restartGameText.gameObject.SetActive(false);
            }

            _pauseGameText.text = displayUI ? "Press 'ESC' to pause the game" : "Press 'ESC' to unpause the game";
            
            _QuitButton.gameObject.SetActive(false);
            _MainMenuButton.gameObject.SetActive(false);
            _restartButton.gameObject.SetActive(false);

            _scoreText.gameObject.SetActive(displayUI);
            _waveCountText.gameObject.SetActive(displayUI);
            _startGameText.gameObject.SetActive(displayUI); 
            _ammoCountText.gameObject.SetActive(displayUI);
            _ammoCountTextREDEffect.gameObject.SetActive(displayUI);
            frontHealthBar.transform.parent.gameObject.SetActive(displayUI);
            frontBossHealthBar.transform.parent.gameObject.SetActive(displayUI);
            
            _waveBossText.gameObject.SetActive(false);
            _waveCompleteText.gameObject.SetActive(false);
            _waveText.gameObject.SetActive(false);
            _bossStageText.gameObject.SetActive(false);
            

            if (!displayUI)
                ShowPauseUI();

            return !displayUI;

        }
        
        return displayUI;
    }

    public void ShowShieldCount()
    {
        _hideShieldAfterBlinking = false;
        _ShieldCountText.gameObject.SetActive(true);
    }

    public void SetShieldCount(int count)
    {
        _ShieldCountText.text = "Shield: " + count;
    }
    
    IEnumerator BlinkText(bool blinking, TextMeshProUGUI text, float seconds)
    {
        while (blinking)
        {
            text.gameObject.SetActive(true);
            yield return new WaitForSeconds(seconds);
            text.gameObject.SetActive(false);
            yield return new WaitForSeconds(seconds);
        }
    }
    
    public void BlinkShieldCountText()
    {
        //Start Blinking after ammo runs out
        StartCoroutine(BlinkShieldTextRoutine(0.5f, _hideShieldAfterBlinking));
        
        //Stop Blinking and hide text after X seconds.
        StartCoroutine(HideBlinkingShieldText(3));
    }
    
    IEnumerator BlinkShieldTextRoutine(float BlinkRateInSeconds, bool hideShieldUI)
    {
        while (!hideShieldUI)
        {
            //Blink while hideShieldUI is false
            _ShieldCountText.transform.gameObject.SetActive(true);
            yield return new WaitForSeconds(BlinkRateInSeconds);
            _ShieldCountText.gameObject.SetActive(false);
            yield return new WaitForSeconds(BlinkRateInSeconds);
            
            //Check if hideShieldUI is true
            hideShieldUI = GetHideBlinkingShieldBool();
        }
        
        //Hide shield after blinking ends.
        _ShieldCountText.gameObject.SetActive(false);
    }

    IEnumerator HideBlinkingShieldText(float seconds)
    {
        //Set hideShieldUI to true after X amount of seconds
        yield return new WaitForSeconds(seconds);
        _hideShieldAfterBlinking = true;
    }

    private bool GetHideBlinkingShieldBool()
    {
        return _hideShieldAfterBlinking;
    }

    public void UpdateAmmoCountUI(int AmmoCount, int maxAmmoCount)
    {
        _ammoCountText.text = "Ammo: " + AmmoCount + "/" + maxAmmoCount;
        if (AmmoCount <= maxAmmoCount * 0.25f)
        {
            _ammoCountText.text = "Ammo: ";
            _ammoCountTextREDEffect.gameObject.SetActive(true);
            _ammoCountTextREDEffect.text = AmmoCount + "/" + maxAmmoCount;
        }
        if (AmmoCount > maxAmmoCount * 0.25f)
        {
            
            _ammoCountTextREDEffect.text = "";
            _ammoCountTextREDEffect.gameObject.SetActive(false);
        }
    }

    public void BlinkAmmoCountText(float blinkRate)
    {
        StartCoroutine(BlinkAmmoRoutine(blinkRate));
    }
    
    IEnumerator BlinkAmmoRoutine(float seconds)
    {
        while (_blinkAmmo)
        {
            _ammoCountText.transform.gameObject.SetActive(true);
            _ammoCountTextREDEffect.transform.gameObject.SetActive(true);
            yield return new WaitForSeconds(seconds);
            _ammoCountText.transform.gameObject.SetActive(false);
            _ammoCountTextREDEffect.transform.gameObject.SetActive(false);
            yield return new WaitForSeconds(seconds);
        }
        _ammoCountText.transform.gameObject.SetActive(true);
        _ammoCountTextREDEffect.transform.gameObject.SetActive(true);
    }
    
    public void DisableAmmoBlink()
    {
        _blinkAmmo = false;
    }
    
    public void BlinkWaveText(float blinkRate, float blinkingDuration)
    {
        StartCoroutine(BlinkWaveRoutine(blinkRate));
        StartCoroutine(DisableWaveBlinkAfterSeconds(blinkingDuration));
    }
    
    IEnumerator BlinkWaveRoutine(float seconds)
    {
        while (_blinkWave)
        {
            _waveText.transform.gameObject.SetActive(true);
            yield return new WaitForSeconds(seconds);
            _waveText.transform.gameObject.SetActive(false);
            yield return new WaitForSeconds(seconds);
        }
        _waveText.transform.gameObject.SetActive(false);
    }

    IEnumerator DisableWaveBlinkAfterSeconds(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        _blinkWave = false;
    }
    
    public void BlinkWaveCompleteText(float blinkRate, float blinkingDuration)
    {
        _blinkCompleteWave = true;
        StartCoroutine(BlinkWaveCompleteRoutine(blinkRate));
        StartCoroutine(DisableWaveCompleteBlinkAfterSeconds(blinkingDuration));
        StartCoroutine(DisplayNewWaveOnWaveCompletion(3.5f));
    }
    
    public void BlinkBossWaveText(bool blink, float delay, float blinkRate, float blinkingDuration)
    {
        if (blink)
        {
            StartCoroutine(BlinkBossWaveRoutine(delay,blinkRate,blinkingDuration));
            blink = false;
        }
    }

    IEnumerator DisplayNewWaveOnWaveCompletion(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        _waveText.transform.gameObject.SetActive(true);
        _blinkWave = true;
        BlinkWaveText(0.5f, 3f);
    }
    IEnumerator BlinkWaveCompleteRoutine(float seconds)
    {
        while (_blinkCompleteWave)
        {
            _waveCompleteText.transform.gameObject.SetActive(true);
            yield return new WaitForSeconds(seconds);
            _waveCompleteText.transform.gameObject.SetActive(false);
            yield return new WaitForSeconds(seconds);
        }
        _waveCompleteText.transform.gameObject.SetActive(false);
    }
    
    IEnumerator BlinkBossWaveRoutine(float delay, float seconds, float blinkDuration)
    {
        StartCoroutine(DisableBossWaveBlinkAfterSeconds(delay, blinkDuration));
        yield return new WaitForSeconds(delay);
        while (_blinkBossStage)
        {
            _bossStageText.transform.gameObject.SetActive(true);
            yield return new WaitForSeconds(seconds);
            _bossStageText.transform.gameObject.SetActive(false);
            yield return new WaitForSeconds(seconds);
        }
        _bossStageText.transform.gameObject.SetActive(true);
        _bossStageText.text = "FIGHT!";
        yield return new WaitForSeconds(2);
        _bossStageText.transform.gameObject.SetActive(false);
    }

    IEnumerator DisableWaveCompleteBlinkAfterSeconds(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        _blinkCompleteWave = false;
    }
    
    IEnumerator DisableBossWaveBlinkAfterSeconds(float delay, float seconds)
    {
        yield return new WaitForSeconds(delay+seconds);
        _blinkBossStage = false;
        
    }

    private void ShowBossHealthUI(bool setActiveBool)
    {
        _bossHealthBarGameObject.SetActive(setActiveBool);
    }

    private void ShowPauseUI()
    {
        _QuitButton.gameObject.SetActive(true);
        _MainMenuButton.gameObject.SetActive(true);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
    
    public void GoToMainMenu()
    {
        SceneManager.LoadScene("Main Menu");
    }

    private void ShowLevelCompleteScreen(bool bossEnemyDefeat)
    {
        if (bossEnemyDefeat)
        {
            _restartButton.gameObject.SetActive(true);
            _levelCompleteText.gameObject.SetActive(true);
        }
    }
    
    public bool BlinkAmmo
    {
        get => _blinkAmmo;
        set => _blinkAmmo = value;
    }

    public bool ShowBossHealthBool
    {
        get => _showBossHealthBoss;
        set => _showBossHealthBoss = value;
    }

    public float currentBossHealth
    {
        get => _CurrentBossHealth;
        set => _CurrentBossHealth = value;
    }

    public float maxBossHealth
    {
        get => _maxBossHealth;
        set => _maxBossHealth = value;
    }

    public bool BossHasBeenDefeated
    {
        get => _bossHasBeenDefeated;
        set => _bossHasBeenDefeated = value;
    }
}
