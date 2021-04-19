using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseHoverOver : MonoBehaviour
{
    public RailManager RailManager;
    private Material _outlineShader;
    private Color _hoverColor = Color.green, _selectedColour = Color.red, _defaultColour = Color.white;

    private float _outlineWidth = 0.1f;

    private void Awake() {
        _outlineShader = GetComponent<Renderer>().material;
    }

    private void OnMouseOver() {
        if (IsSelected()) {
            // If its already selected, ignore the next calculation
            return;
        }

        // Make outline visible
        ChangeOutline(_outlineWidth, _hoverColor);

        if (Input.GetMouseButtonDown(0) && RailManager.SelectStation(this.gameObject)) {
            // Change the object outline colour to selected
            ChangeOutline(_outlineWidth, _selectedColour);
        }
    }

    private void OnMouseExit() {
        if (!IsSelected()) {
            // Hide outline on exit
            ResetOutline();
        }
    }

    private bool IsSelected() {
        return _outlineShader.GetColor("_OutlineColor") == _selectedColour;
    }

    private void ChangeOutline(float width, Color colour) {
        _outlineShader.SetFloat("_Outline", width);
        _outlineShader.SetColor("_OutlineColor", colour);
    }

    public void ResetOutline() {
        // Only hide the outline if its not selected
        ChangeOutline(0f, default);
    }
}
