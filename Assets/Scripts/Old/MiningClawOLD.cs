using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiningClawOLD : MonoBehaviour
{
    [Header("Rail")]
    public CurveCreator RailEditor;
    public string OreRailName, CoalRailName;
    public bool LoopRail;
    [Space(5)]
    [Header("Movement")]
    public float MovementSpeed = 1f;
    [Header("Other")]
    public OreManager OreManager;

    private int _currentRailIdx = -1;
    private string _currentRailName;
    private int _currentTravelPoint;
    private int _railDirection = 1; // 1: forwards, -1: backwards
    private float _distanceTolerance;
    private bool _cartIsPausing;

    public delegate void RailDelegate();
    private RailDelegate _railDelegate;

    private void Awake() {
        _railDelegate = ToggleRailTracks;
    }

    private void Start() {
        // Pass rail delegate to the ore manager
        GameObject.Find("Ore Manager").GetComponent<OreManager>().ReceiveToggleRailCallback(_railDelegate);

        // Set the ore rail as the first rail 
        _currentRailName = OreRailName;
        _currentRailIdx = RailEditor.GetRailIdx(_currentRailName);

        if(_currentRailIdx < 0) {
            Debug.LogError("Can't find rail index given name");
        }

        // Start at the first point
        transform.position = RailEditor.curves[_currentRailIdx].points[0];
        _currentTravelPoint = 1;
    }

    private void Update()
    {
        if (_cartIsPausing) {
            return;
        }
        if (Vector3.Distance(transform.position, RailEditor.curves[_currentRailIdx].points[_currentTravelPoint]) > _distanceTolerance) { 
            // Go to the next point if not close
            float step =  MovementSpeed * Time.deltaTime; // calculate distance to move
            transform.position = Vector3.MoveTowards(transform.position, RailEditor.curves[_currentRailIdx].points[_currentTravelPoint], step);
        }
        else {
            if (_currentTravelPoint == 0) {
                // Start trigger and delay
                RailEditor.InvokeStartObject(_currentRailIdx);
                StartCoroutine(PauseCartForDelay(RailEditor.startDelays[_currentRailIdx]));
                // Toggle direction
                ToggleRailDirection();
            }
            else if (_currentTravelPoint == RailEditor.curves[_currentRailIdx].points.Count - 1) {
                // End trigger and delay
                RailEditor.InvokeEndObjects(_currentRailIdx);
                StartCoroutine(PauseCartForDelay(RailEditor.endDelays[_currentRailIdx]));
                // Toggle direction
                ToggleRailDirection();
            }
            // Step over to next point on rail
            _currentTravelPoint += _railDirection;
        }
    }

    private void ToggleRailDirection() {
        // Change the direction of the cart
        _railDirection *= -1;
    }

    private void ToggleRailTracks() {
        // Toggle the two rails 
        if (_currentRailName == OreRailName) {
            _currentRailName = CoalRailName;
        } 
        else if (_currentRailName == CoalRailName) {
            _currentRailName = OreRailName;
        }
        _currentRailIdx = RailEditor.GetRailIdx(_currentRailName);
    }

    IEnumerator PauseCartForDelay(float pauseTime) {
        // Pause the cart for a set amount of time
        _cartIsPausing = true;
        yield return new WaitForSeconds(pauseTime);
        _cartIsPausing = false;
    }
}
