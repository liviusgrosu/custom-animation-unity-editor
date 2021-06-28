using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Furnace : MonoBehaviour
{
    public OreManager OreManager;

    private List<IToggleMachine> _toggableMachine;

    private bool _state;


    private void Awake() {

    }

    private void Start() {

        _toggableMachine= new List<IToggleMachine>();
        GameObject[] rootGameObjects = SceneManager.GetActiveScene().GetRootGameObjects();
        foreach( var rootGameObject in rootGameObjects )
        {
            IToggleMachine[] childrenInterfaces = rootGameObject.GetComponentsInChildren<IToggleMachine>();
            foreach( var childInterface in childrenInterfaces )
            {
                _toggableMachine.Add(childInterface);
            }
        }

        _state = OreManager.CoalAmount > 0;
    }

    private void Update() {
        if (_state && OreManager.CoalAmount <= 0) {
            // Turn off
            _state = false;
            foreach (IToggleMachine machine in _toggableMachine) {
                machine.SetPowerState(_state);
            }
        }
        else if(!_state && OreManager.CoalAmount > 0) {
            // Turn on
            _state = false;
            foreach (IToggleMachine machine in _toggableMachine) {
                machine.SetPowerState(_state);
            }
        }
    }    
}
