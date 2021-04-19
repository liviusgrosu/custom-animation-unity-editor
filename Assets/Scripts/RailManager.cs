using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RailManager : MonoBehaviour
{
    private GameObject _selectedStation;

    public bool SelectStation(GameObject building) {
        if (_selectedStation == null || _selectedStation != building) {
            // Deselect the other buildings
            DeselectStations();
            _selectedStation = building;
            return true;
        }
        return false;
    }

    private void DeselectStations() {
        // Reset each stations outline back to default
        GameObject[] stations = GameObject.FindGameObjectsWithTag("Station");
        foreach(GameObject station in stations) {
            station.GetComponent<MouseHoverOver>().ResetOutline();
        }
    }
}
