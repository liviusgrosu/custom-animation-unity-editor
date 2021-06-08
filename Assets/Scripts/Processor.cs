using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Processor : MonoBehaviour
{
    // TODO: add queue here of processed ore
    public Queue<string> _queuedOres;
    
    // Happens when power is on and claw is present
    private bool _moveConveyorBelt;

    public Transform SpawnPoint;
    public GameObject ProcessedCoal, ProcessedGold, ProcessedSilver;

    public List<ConveyorBelt> ConveyorBelts;
    private bool _stationState;

    public GameObject TempBlockingBarrier;

    private void Awake() {
        _queuedOres = new Queue<string>();
        InvokeRepeating("SpawnProcessedOre", 0.5f, 1f);
    } 
    private void OnTriggerEnter(Collider obj) {
        if (obj.tag == "Ore") {
            if(obj.name.Contains("Coal")) {
                _queuedOres.Enqueue("Coal");
            }
            else if(obj.name.Contains("Silver")) {
                _queuedOres.Enqueue("Silver");
            }
            else if(obj.name.Contains("Gold")) {
                _queuedOres.Enqueue("Gold");                
            }
        }
        Destroy(obj.gameObject);
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Space)) {
            // Toggle the station and blocking barrier
            _stationState = !_stationState;
            TempBlockingBarrier.SetActive(!TempBlockingBarrier.activeSelf);
        }
    }

    private void SpawnProcessedOre() {
        if (_queuedOres.Count == 0 && !_stationState) {
            return;
        }

        string nextOreName = _queuedOres.Dequeue();
        GameObject nextOreObj;
        switch(nextOreName) {
            case "Coal":
                nextOreObj = ProcessedCoal;
                break;
            case "Silver":
                nextOreObj = ProcessedSilver;
                break;
            case "Gold":
                nextOreObj = ProcessedGold;
                break;
            default:
                return;
        }

        Instantiate(nextOreObj, SpawnPoint.position, nextOreObj.transform.rotation);
    }

    public void MoveProcessedOre(float secondsToWait) {
        StartCoroutine(MoveConveyorBelts(secondsToWait));
    }

    IEnumerator MoveConveyorBelts(float secondsToWait) {
        // Start moving the belts
        SetBeltsState(false);
        yield return new WaitForSeconds(secondsToWait);
        // Stop moving the belts
        SetBeltsState(true);
    } 

    private void SetBeltsState(bool isStopped) {
        foreach (ConveyorBelt belt in ConveyorBelts) {
            belt.SetState(isStopped);
        }
    }

    // TODO: Update some sort of text 
}
