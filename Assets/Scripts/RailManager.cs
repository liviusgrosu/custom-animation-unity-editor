using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RailManager : MonoBehaviour
{
    private MiningClaw MiningClaw;
    private GameObject _selectedStation;

    private void Start() {
        MiningClaw = GameObject.Find("Claw Bucket").GetComponent<MiningClaw>();
    } 

    public void SelectStation(GameObject station, int direction) {
        if (_selectedStation != station) {
            // Deselect the other buildings
            DeselectStations();
            _selectedStation = station;
            // Change mining claw to new station
            MiningClaw.SwitchRails(station.GetComponent<Station>().RailAssociated, direction);
        }
    }

    private void DeselectStations() {
        // Reset each stations outline back to default
        GameObject[] stations = GameObject.FindGameObjectsWithTag("Station");
        foreach(GameObject station in stations) {
            station.GetComponent<MouseHoverOver>().ResetOutline();
        }
    }
}
