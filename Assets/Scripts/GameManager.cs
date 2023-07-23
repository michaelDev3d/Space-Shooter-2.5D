using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] 
    private bool _isGameOver;

    [SerializeField] 
    private SpacePlayer _player;

    [SerializeField] 
    private SpawnManager _spawnManager;

    [SerializeField] 
    private UIManager _uiManager;
    
    [SerializeField]
    private bool _gameIsPaused;
    
    private void Start()
    {
        
        //Referencing components and null checking for them
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

        GameObject spawnManagerObject = GameObject.Find("Spawn_Manager");

        if (spawnManagerObject != null)
        {
            if (spawnManagerObject.TryGetComponent(out SpawnManager spawnManager))
                _spawnManager = spawnManager;
            else
                Debug.LogError("Spawn Manager component is NULL on Spawn Manager gameObject");
        }
        else
            Debug.LogError("Spawn Manager gameObject is NULL");
        
        GameObject uiManagerGameObject = GameObject.Find("UI_Manager");

        if (uiManagerGameObject != null)
        {
            if (uiManagerGameObject.TryGetComponent(out UIManager uiManager))
                _uiManager = uiManager;
            else
                Debug.LogError("Spawn Manager component is NULL on Spawn Manager gameObject");
        }
        else
            Debug.LogError("Spawn Manager gameObject is NULL");
        
    }

    void Update()
    {
        StartSpawningShips();
        ResetLevel();
        PauseGame();
    }

    public void GameOver()
    {
        _isGameOver = true;
    }

    public bool GetGameOver()
    {
        return _isGameOver;
    }

    private void ResetLevel()
    {
        if(Input.GetKeyDown(KeyCode.R) && _isGameOver)
        {
            SceneManager.LoadScene(1);
        }
    }

    private void PauseGame()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !_gameIsPaused)
        {
            _gameIsPaused = _uiManager.DisplayUI(false);
        }
        else if (Input.GetKeyDown(KeyCode.Escape) && _gameIsPaused)
        {
            _gameIsPaused = _uiManager.DisplayUI(true);
        }

        if (_gameIsPaused)
        {
            Time.timeScale = 0;
        }
        else if (!_gameIsPaused)
        {
            Time.timeScale = 1;
        }
        
    }
    
    //Method to start spawning enemy ships after player score is over 100
    private void StartSpawningShips()
    {
        if (_player != null)
        {
            if (_player.GetScore() > 100)
            {
                if (_spawnManager != null)
                {
                    _spawnManager.SetStartSpawningShips(true);
                }
            }
            
            if (_player.GetScore() == 200)
            {
                if (_spawnManager != null)
                {
                    _spawnManager.SetStartSpawningSwarmEnemy(true);
                    _player.AddScore(10);
                }
            }
        }
    }

    public bool GetGameIsPaused()
    {
        return _gameIsPaused;
    }
}
