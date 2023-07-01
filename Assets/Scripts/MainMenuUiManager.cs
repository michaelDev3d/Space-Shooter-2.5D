using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuUiManager : MonoBehaviour
{
    [SerializeField] 
    private GameObject[] _blinkingTexts;

    [SerializeField] 
    private bool _LoadMainStageBool;
    
    void Start()
    {
        for (int i = 0; i < _blinkingTexts.Length; i++)
        {
            StartCoroutine(BlinkRoutine( _blinkingTexts[i],1.5f, 0.5f));
        }
    }

    private void Update()
    {
        if (_LoadMainStageBool)
        {
            SceneManager.LoadScene("Game");
        }
    }

    IEnumerator BlinkRoutine(GameObject gameObjectToBlink, float showTime,float hideTime)
    {
        while (true)
        {
            gameObjectToBlink.gameObject.SetActive(true);
            yield return new WaitForSeconds( showTime);
            gameObjectToBlink.gameObject.SetActive(false);
            yield return new WaitForSeconds(hideTime);
        }
    }

    public void LoadMainStage(bool loadStage)
    {
        _LoadMainStageBool = loadStage;
    }
}
