using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuPlayerManager : MonoBehaviour
{
    [SerializeField]
    private GameObject spacePlayerPrefab;
    [SerializeField]
    private GameObject spacePlayerGameObject;
    [SerializeField]
    private SpacePlayer spacePlayer;
    
    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 1;
        spacePlayerGameObject = Instantiate(spacePlayerPrefab);
        spacePlayer = spacePlayerGameObject.GetComponent<SpacePlayer>();
        spacePlayer.mainMenuPlayer = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
