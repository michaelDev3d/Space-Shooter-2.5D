using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthUi : MonoBehaviour
{
    [SerializeField] 
    private float _health;
    [SerializeField] 
    private float _lerpTimer;
    [SerializeField] 
    private float _maxHealth = 100f;
    [SerializeField] 
    private float _chipSpeed = 2f;
    [SerializeField]
    private Image frontHealthBar;
    [SerializeField]
    private Image backHealthBar;
    
    // Start is called before the first frame update
    void Start()
    {
        _health = _maxHealth;

    }

    // Update is called once per frame
    void Update()
    {
        _health = Mathf.Clamp(_health, 0, _maxHealth);
        UpdateHealthUi();
        
        if (Input.GetKeyDown(KeyCode.Q))
        {
            RemoveHealthFromBar(Random.Range(10,15));
        }
    }

    private void UpdateHealthUi()
    {
        Debug.Log(_health);
        //float fillF = frontHealthBar.fillAmount;
        float fillB = backHealthBar.fillAmount;
        float hFraction = _health / _maxHealth;

        if (fillB > hFraction)
        {
            frontHealthBar.fillAmount = hFraction;
            Color loseHealthColor = new Vector4(0.882353f,0.7529413f,0.6039216f,1);
            backHealthBar.color = loseHealthColor;
           
            _lerpTimer += Time.deltaTime;
            float percentComplete = _lerpTimer / _chipSpeed;
            percentComplete = percentComplete * percentComplete;
            backHealthBar.fillAmount = Mathf.Lerp(fillB, hFraction, percentComplete);
        }
    }

    public void RemoveHealthFromBar(float damage)
    {
        _health -= damage;
        _lerpTimer = 0f;
    }

}
