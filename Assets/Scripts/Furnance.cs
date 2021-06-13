using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Furnance : MonoBehaviour
{
    public OreManager OreManager;
    //public CrushingMachineCollider;

    private List<ConveyorBelt> ConveyorBelts;

    private void Awake() {
        ConveyorBelts = new List<ConveyorBelt>();
    }

    private void Start() {
        
    }

    private void Update() {
        if (OreManager.CoalAmount <= 0) {
            
        }
    }    
}
