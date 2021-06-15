using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Furnance : MonoBehaviour
{
    public OreManager OreManager;

    private List<ConveyorBelt> _conveyorBelts;

    private bool _state;


    private void Awake() {
        _conveyorBelts = new List<ConveyorBelt>();
    }

    private void Start() {
        foreach(ConveyorBelt conveyorBelt in FindObjectsOfType(typeof(ConveyorBelt))) {
            _conveyorBelts.Add(conveyorBelt);
        }

        _state = OreManager.CoalAmount > 0;
    }

    private void Update() {
        if (_state && OreManager.CoalAmount <= 0) {
            // Turn off
            _state = false;
            foreach (ConveyorBelt conveyorBelt in _conveyorBelts) {
                conveyorBelt.SetPowerState(_state);
            }
        }
        else if(!_state && OreManager.CoalAmount > 0) {
            // Turn on
            _state = false;
            foreach (ConveyorBelt conveyorBelt in _conveyorBelts) {
                conveyorBelt.SetPowerState(_state);
            }
        }
    }    
}
