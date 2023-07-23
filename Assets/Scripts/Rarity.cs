using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rarity : MonoBehaviour
{
    private enum RarityMeasure
    {
        NoRarity,
        Common,
        Uncommon,
        Rare,
    }
    
    [SerializeField] 
    private RarityMeasure _rarity;
}

