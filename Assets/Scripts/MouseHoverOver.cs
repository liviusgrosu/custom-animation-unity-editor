using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseHoverOver : MonoBehaviour
{
    public RailManager RailManager;

    private void OnMouseOver() {
        if (Input.GetMouseButtonDown(0)) {
            // Change to that station
            RailManager.SelectStation(this.gameObject);
        }
    }
}
