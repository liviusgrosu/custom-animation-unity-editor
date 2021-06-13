using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OreManager : MonoBehaviour
{
    public Text GoldAmountText, SilverAmountText, CoalAmountText;
    public int MaxCoalAmount = 10;

    private int _goldAmount = 0, _silverAmount = 0; 
    private int __coalAmount = 4;
    public int CoalAmount
    {
        get { return __coalAmount; } 
        set { __coalAmount = Mathf.Clamp(value, 0, MaxCoalAmount); } 
    }
    private bool _isLoadingCoal;

    private void Start() {
        InvokeRepeating("DrainCoalAmount", 1.5f, 1.5f);
    }

    private void Update() {

        if (CoalAmount <= 0 && !_isLoadingCoal || CoalAmount >= 10 && _isLoadingCoal) {
            _isLoadingCoal = !_isLoadingCoal;
        } 

        GoldAmountText.text = $"Gold: {_goldAmount.ToString()}";
        SilverAmountText.text = $"Silver: {_silverAmount.ToString()}";
        CoalAmountText.text = $"Coal: {CoalAmount.ToString()}/{MaxCoalAmount}";
    }

    public void IncrementOre(string oreName) {
        switch(oreName) {
            case "Gold":
                _goldAmount++;
                break;
            case "Silver":
                _silverAmount++;
                break;
            case "Coal":
                CoalAmount++;
                break;
        }
    }

    private void DrainCoalAmount() {
        if (!_isLoadingCoal) {
            // Drain the furnace over time
            CoalAmount--;
        }
    }
}
