using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
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
    private Button _QuitButton;
    private GameManager _gameManager;
    
    [SerializeField] 
    private TextMeshProUGUI _ammoCountText;

    
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
    
    
    
    
    // Start is called before the first frame update
    void Start()
    {
        if(_scoreText != null)
            _scoreText.text = "Score: " + 0;
        
        if(_gameOverText != null)
            _gameOverText.gameObject.SetActive(false);
        
        if(_restartGameText != null)
            _restartGameText.gameObject.SetActive(false);
        
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
    }

    private void Update()
    {
        UpdateHealthUi();
    }
    
    public void UpdateScore(int playerScore)
    {
        _scoreText.text = "Score: " + playerScore.ToString();
    }

    private void GameOverSequence()
    {
        _gameManager.GameOver();
        _gameOverText.gameObject.SetActive(true);
        _restartGameText.gameObject.SetActive(true);
        StartCoroutine(GameOverFlicker(0.5f));
    }

    IEnumerator GameOverFlicker(float seconds)
    {
        while (true)
        {
            _gameOverText.gameObject.SetActive(true);
            yield return new WaitForSeconds(seconds);
            _gameOverText.gameObject.SetActive(false);
            yield return new WaitForSeconds(seconds);
        }
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
    
    private void UpdateHealthUi()
    {
        
        _CurrentPlayerLives = Mathf.Clamp(_CurrentPlayerLives, 0, _maxPlayerLives);
        float backHealthBarFillAmount = backHealthBar.fillAmount;
        float healthPercentage = _CurrentPlayerLives / _maxPlayerLives;

        if (backHealthBarFillAmount > healthPercentage)
        {
            frontHealthBar.fillAmount = healthPercentage;
            Color loseHealthColor = new Vector4(0.882353f,0.7529413f,0.6039216f,1);
            backHealthBar.color = loseHealthColor;
           
            _timer += Time.deltaTime;
            float loseHealthEffectPercentageToComplete = _timer / _drainHealthUiSpeed;
            loseHealthEffectPercentageToComplete *= loseHealthEffectPercentageToComplete;
            backHealthBar.fillAmount = Mathf.Lerp(backHealthBarFillAmount, healthPercentage, loseHealthEffectPercentageToComplete);
        }
    }
    
    public void RemoveHealthFromBar(int playerLives, float damage)
    {
        _CurrentPlayerLives = playerLives;
        _CurrentPlayerLives-= damage;
        _timer = 0f;
        
        UpdateHealthUi();
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
            _scoreText.gameObject.SetActive(displayUI);
            _startGameText.gameObject.SetActive(displayUI);
            frontHealthBar.transform.parent.gameObject.SetActive(displayUI);

            if (!displayUI)
                ShowPauseUI();

            return !displayUI;

        }
        
        return displayUI;
    }

    public void UpdateAmmoCountUI(int AmmoCount)
    {
        _ammoCountText.text = "Ammo: " + AmmoCount;
    }

    public void BlinkAmmoCountText()
    {
        StartCoroutine(BlinkAmmoRoutine(0.5f));
    }

    IEnumerator BlinkAmmoRoutine(float seconds)
    {
        while (true)
        {
            _ammoCountText.transform.gameObject.SetActive(true);
            yield return new WaitForSeconds(seconds);
            _ammoCountText.transform.gameObject.SetActive(false);
            yield return new WaitForSeconds(seconds);
        }
    }

    private void ShowPauseUI()
    {
        _QuitButton.gameObject.SetActive(true);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
    
    
}
