using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;
using UnityEngine.Events;
using CurveCollection;

[CustomEditor(typeof(CurveCreator))]
public class CurveEditor : Editor
{
    CurveCreator curveCreator;
    SelectionInfo selectionInfo;
    bool curveChangedSinceLastRepaint;
    float randomRange = 5f;

    // ---- Testing Variables ----

    public override void OnInspectorGUI() {
        base.OnInspectorGUI();

        if (GUILayout.Button("Clear Data")) {
            ClearAll();
        }

        for (int curveIdx = 0; curveIdx < curveCreator.curves.Count; curveIdx++) {
            string name = $"Rail - {curveIdx}";
            
            if (!string.IsNullOrEmpty(curveCreator.railNames[curveIdx])) {
                // Default name if it doesn't exist
                name = curveCreator.railNames[curveIdx];
            }  
            
            // Foldout for each animation rail
            curveCreator.showAnimationRail[curveIdx] = EditorGUILayout.Foldout(curveCreator.showAnimationRail[curveIdx], name);
            if (curveCreator.showAnimationRail[curveIdx]) {
                SerializedProperty railNames = serializedObject.FindProperty("railNames");
                SerializedProperty animationCurves = serializedObject.FindProperty("animationCurves");

                SerializedProperty endDelays = serializedObject.FindProperty("endDelays");
                SerializedProperty startDelays = serializedObject.FindProperty("startDelays");

                SerializedProperty startTriggerObjs = serializedObject.FindProperty("startTriggerObjs");
                SerializedProperty endTriggerObjs = serializedObject.FindProperty("endTriggerObjs");


                EditorGUILayout.PropertyField(railNames.GetArrayElementAtIndex(curveIdx), new GUIContent("Name"), false);
                EditorGUILayout.PropertyField(animationCurves.GetArrayElementAtIndex(curveIdx), new GUIContent("Curve"), false);

                EditorGUILayout.PropertyField(startDelays.GetArrayElementAtIndex(curveIdx), new GUIContent("Start Delay"), false);
                EditorGUILayout.PropertyField(endDelays.GetArrayElementAtIndex(curveIdx), new GUIContent("End Delay"), false);

                EditorGUILayout.PropertyField(startTriggerObjs.GetArrayElementAtIndex(curveIdx), new GUIContent("First Station"), false);
                EditorGUILayout.PropertyField(endTriggerObjs.GetArrayElementAtIndex(curveIdx), new GUIContent("Second Station"), false);
                
                if (GUILayout.Button("Remove Curve")) {
                    DeleteCurve(curveIdx);
                }
            }
        }

        if (GUI.changed)
        {
            serializedObject.ApplyModifiedProperties();
            curveChangedSinceLastRepaint = true;
            SceneView.RepaintAll();
        }
    }

    private void OnSceneGUI()
    {
        Event guiEvent = Event.current;

        Draw();

        if (guiEvent.type == EventType.Layout)
        {
            // This event happens before anything else
            // Useful in our case so that this triggers before the object is deselected when clicking in the scene
            // If we don't click on anything then trigger this
            // This will no deselect the gameobject when clicking on the draw plane
            HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));
        }
        else
        {
            HandleInput(guiEvent);
            if (curveChangedSinceLastRepaint)
            {
                HandleUtility.Repaint();
            }
        }
    }

    void HandleInput(Event guiEvent)
    {
        // Convert mouse position to world position via a ray
        Ray mouseRay = HandleUtility.GUIPointToWorldRay(guiEvent.mousePosition);

        if(guiEvent.type == EventType.MouseDown)
        {
            // Check if pressing mouse down and that we are not using alt/ctrl/fnc/etc...
            if (Event.current.button == 0)
            {
                if (guiEvent.modifiers == EventModifiers.None)
                {
                    // Select point
                    HandleLeftMouseDown();
                }
                else if (guiEvent.modifiers == EventModifiers.Shift)
                {
                    // Create point
                    HandleShiftLeftMouseDown(mouseRay);
                }
                else if (guiEvent.modifiers == EventModifiers.Control)
                {
                    // Delete point
                    HandleControlLeftMouseDown();
                }
            }
            else if (Event.current.button == 1)
            {
                if (guiEvent.modifiers == EventModifiers.Shift)
                {
                    // Create curve
                    HandleShiftRightMouseDown(mouseRay);
                }
            }
        }

        // Check if mouse is hovering over a point
        UpdateMouseOverInfo(mouseRay);
    }
    
    void HandleLeftMouseDown()
    {
        // Select the point and curve that is hovered over
        if(selectionInfo.pointHoverOver == -1)
        {
            // Click away from selected point if applicable
            ResetVariables();
        }

        // Select the point and curve that was hovered over
        selectionInfo.pointSelected = selectionInfo.pointHoverOver;
        selectionInfo.curveSelected = selectionInfo.curveHoverOver;
    }

    void HandleShiftLeftMouseDown(Ray mouseRay)
    {
        // Create new point in the selected curve

        Vector3 mousePosition = mouseRay.GetPoint(mouseRay.origin.magnitude);

        // If no curve is present, create one before creating a point
        if(selectionInfo.curveSelected == -1)
        {
            CreateNewCurve();
            
        }

        // Create point in the selected point
        CreateNewPoint(mousePosition);
        // New curve is the selected curve
        selectionInfo.pointSelected = curveCreator.curves[selectionInfo.curveSelected].Points.Count() - 1;
    }

    void HandleControlLeftMouseDown()
    {
        if(selectionInfo.pointHoverOver != -1)
        {
            if (selectionInfo.curveSelected != selectionInfo.curveHoverOver)
            {
                // Delete the entire curve
                DeleteCurve();
                return;
            }

            DeletePoint();
            if (!curveCreator.curves[selectionInfo.curveSelected].Points.Any())
            {
                // Remove the curve if there are no Points
                curveCreator.curves.RemoveAt(selectionInfo.curveSelected);
                selectionInfo.curveSelected = -1;
            }
            selectionInfo.pointSelected = -1;
        }
    }

    void HandleShiftRightMouseDown(Ray mouseRay) 
    {
        Vector3 mousePosition = mouseRay.GetPoint(mouseRay.origin.magnitude);

        CreateNewCurve();

        // Create point in the selected point
        CreateNewPoint(mousePosition);
        // New curve is the selected curve
        selectionInfo.pointSelected = curveCreator.curves[selectionInfo.curveSelected].Points.Count() - 1;
    }


    void UpdateMouseOverInfo(Ray mouseRay)
    {
        // Get direction of the mouse
        selectionInfo.pointHoverOver = -1;
        selectionInfo.curveHoverOver = -1;
        Vector3 mouseDir = mouseRay.direction.normalized;

        for (int curveIdx = 0; curveIdx < curveCreator.curves.Count; curveIdx++)
        {
            for (int pointIdx = 0; pointIdx < curveCreator.curves[curveIdx].Points.Count; pointIdx++)
            {
                Vector3 point = curveCreator.curves[curveIdx].Points[pointIdx];

                float x = point.x - mouseRay.origin.x;
                float y = point.y - mouseRay.origin.y;
                float z = point.z - mouseRay.origin.z;

                float t = (x * mouseDir.x + y * mouseDir.y + z * mouseDir.z) / 
                            (mouseDir.x * mouseDir.x + mouseDir.y * mouseDir.y + mouseDir.z * mouseDir.z);

                float D1 = (mouseDir.x * mouseDir.x + mouseDir.y * mouseDir.y + mouseDir.z * mouseDir.z) * (t * t);
                float D2 = (x * mouseDir.x + y * mouseDir.y + z * mouseDir.z) * 2 * t;
                float D3 = (x * x + y * y + z * z);

                float D = D1 - D2 + D3;
                
                if (D < 1)
                {
                    selectionInfo.pointHoverOver = pointIdx;
                    selectionInfo.curveHoverOver = curveIdx;
                }
            }
        }
    }

    void Draw()
    {
        EditorGUI.BeginChangeCheck();

        for (int curveIdx = 0; curveIdx < curveCreator.curves.Count; curveIdx++) {
            for (int pointIdx = 0; pointIdx < curveCreator.curves[curveIdx].Points.Count; pointIdx++) {
                bool hoverCurve = selectionInfo.curveHoverOver == curveIdx;
                bool hoverPoint = selectionInfo.pointHoverOver == pointIdx;

                bool selectedCurve = selectionInfo.curveSelected == curveIdx;
                bool selectedPoint = selectionInfo.pointSelected == pointIdx;

                Vector3 currentPointPos = curveCreator.curves[curveIdx].Points[pointIdx];
                int pointsCount = curveCreator.curves[curveIdx].Points.Count;

                string pointTypeLabel = "";
                if(pointIdx == curveCreator.curves[curveIdx].FirstStationIdx) {
                    pointTypeLabel += " First ";
                }
                if(pointIdx == curveCreator.curves[curveIdx].IntermediatePointIdx) {
                    pointTypeLabel += " Intermediate ";
                }
                if(pointIdx == curveCreator.curves[curveIdx].SecondStationIdx) {
                    pointTypeLabel += " Second ";
                }

                // Display the point type about the point
                DrawLabel(currentPointPos + new Vector3(0, 1f, 0), pointTypeLabel, Color.red);

                if (selectedCurve) {
                    if (selectedPoint) {
                        Vector3 newPointPosition = Handles.PositionHandle(curveCreator.curves[curveIdx].Points[pointIdx], Quaternion.identity);
                        if (EditorGUI.EndChangeCheck()) {
                            curveCreator.curves[curveIdx].Points[pointIdx] = newPointPosition;
                        }
                        // Show buttons that selects the point type
                        DrawPointOptionsButton("First Station", currentPointPos + new Vector3(-1f, -1f, 0), pointIdx, Color.green);
                        DrawPointOptionsButton("Intermediate", currentPointPos + new Vector3(0f, -1f, 0), pointIdx, Color.yellow);
                        DrawPointOptionsButton("Second Station", currentPointPos + new Vector3(1f, -1f, 0), pointIdx, Color.red);

                        if (curveCreator.curves[selectionInfo.curveSelected].FirstStationIdx != -1 &&
                            curveCreator.curves[selectionInfo.curveSelected].SecondStationIdx != -1) {
                                // Calculate the directions once we know where the first and second station is
                                CalculateDirections();
                        }
                    }

                    // Ensures that the point being hovered over is contained within the curve
                    Handles.color = hoverPoint && hoverCurve == selectedCurve ? Color.magenta : Color.black;

                    if (pointIdx == curveCreator.curves[curveIdx].IntermediatePointIdx) {
                        Handles.color = Color.yellow;
                    }

                    Handles.SphereHandleCap(0, currentPointPos, Quaternion.LookRotation(Vector3.up), 0.5f, EventType.Repaint);
                    Handles.color = Color.white;
                    if (pointIdx < pointsCount - 1) {
                        Handles.DrawDottedLine(currentPointPos, curveCreator.curves[curveIdx].Points[pointIdx + 1], 4);
                    }
                }
                else {
                    Handles.color = hoverCurve ? Color.green : Color.gray;

                    if (pointIdx == curveCreator.curves[curveIdx].IntermediatePointIdx) {
                        Handles.color = Color.yellow;
                    }

                    Handles.SphereHandleCap(0, currentPointPos, Quaternion.LookRotation(Vector3.up), 0.5f, EventType.Repaint);
                    Handles.color = hoverCurve ? Color.green : Color.gray;
                    if (pointIdx < pointsCount - 1) {
                        Handles.DrawDottedLine(currentPointPos, curveCreator.curves[curveIdx].Points[pointIdx + 1], 4);
                    }
                }
            }
        }
        curveChangedSinceLastRepaint = false;
    }

    void CalculateDirections() {
        // Calculate the movement point increments based on where the first and second index lies
        if (curveCreator.curves[selectionInfo.curveSelected].FirstStationIdx < curveCreator.curves[selectionInfo.curveSelected].IntermediatePointIdx) {
            curveCreator.curves[selectionInfo.curveSelected].ForwardDirectionIncrement = -1;
            curveCreator.curves[selectionInfo.curveSelected].BackwardDirectionIncrement = 1;
        }
        else {
            curveCreator.curves[selectionInfo.curveSelected].ForwardDirectionIncrement = 1;
            curveCreator.curves[selectionInfo.curveSelected].BackwardDirectionIncrement = -1;
        }
    }

    void DrawLabel(Vector3 position, string text, Color colour) {
        //Select the text colour
        GUIStyle style = new GUIStyle();
        style.normal.textColor = colour;

        // Draw out the label
        Handles.BeginGUI();
        Vector2 position2D = HandleUtility.WorldToGUIPoint(position);
        GUI.Label(new Rect(position2D.x, position2D.y, 100, 100), text, style);
        Handles.EndGUI();
    }

    void DrawPointOptionsButton(string pointType, Vector3 position, int pointIdx, Color colour) {
        float size = 0.5f;
        Handles.color = colour;
        if (Handles.Button(position, Quaternion.identity, size, size, Handles.CubeHandleCap)) {
            switch(pointType) {
                case "First Station":
                    curveCreator.curves[selectionInfo.curveSelected].FirstStationIdx = pointIdx;
                    break;
                case "Intermediate":
                    curveCreator.curves[selectionInfo.curveSelected].IntermediatePointIdx = pointIdx;
                    break;
                case "Second Station":
                    curveCreator.curves[selectionInfo.curveSelected].SecondStationIdx = pointIdx;
                    break;
            }

        }
    }

    private void OnEnable()
    {
        curveCreator = target as CurveCreator;
        selectionInfo = new SelectionInfo();
        Tools.hidden = true;
    }

    private void OnDisable()
    {
        Tools.hidden = false;
    }

    void CreateNewCurve()
    {
        // Create new curve
        curveCreator.curves.Add(new Curve());

        curveCreator.railNames.Add("");
        curveCreator.animationCurves.Add(new AnimationCurve());
        curveCreator.showAnimationRail.Add(false);

        curveCreator.startDelays.Add(0f);
        curveCreator.startTriggerObjs.Add(new UnityEvent());

        curveCreator.endDelays.Add(0f);
        curveCreator.endTriggerObjs.Add(new UnityEvent());

        // New curve is the selected curve
        selectionInfo.curveSelected = curveCreator.curves.Count() - 1;

        // Default it to -1 so the first point is not associated
        curveCreator.curves[selectionInfo.curveSelected].FirstStationIdx = -1;
        curveCreator.curves[selectionInfo.curveSelected].SecondStationIdx = -1;
    }

    void DeleteCurve()
    {
        curveCreator.curves.RemoveAt(selectionInfo.curveHoverOver);

        curveCreator.railNames.RemoveAt(selectionInfo.curveHoverOver);
        curveCreator.animationCurves.RemoveAt(selectionInfo.curveHoverOver);
        curveCreator.showAnimationRail.RemoveAt(selectionInfo.curveHoverOver);

        curveCreator.startDelays.RemoveAt(selectionInfo.curveHoverOver);
        curveCreator.startTriggerObjs.RemoveAt(selectionInfo.curveHoverOver);

        curveCreator.endDelays.RemoveAt(selectionInfo.curveHoverOver);
        curveCreator.endTriggerObjs.RemoveAt(selectionInfo.curveHoverOver);

        selectionInfo.curveSelected = -1;
    }

    void DeleteCurve(int curveIdx) {
        curveCreator.curves.RemoveAt(curveIdx);

        curveCreator.railNames.RemoveAt(curveIdx);
        curveCreator.animationCurves.RemoveAt(curveIdx);
        curveCreator.showAnimationRail.RemoveAt(curveIdx);

        curveCreator.startDelays.RemoveAt(curveIdx);
        curveCreator.startTriggerObjs.RemoveAt(curveIdx);

        curveCreator.endDelays.RemoveAt(curveIdx);
        curveCreator.endTriggerObjs.RemoveAt(curveIdx);
    }

    void CreateNewPoint(Vector3 position)
    {
        // Check which curve it goes into 
        Undo.RecordObject(curveCreator, "Create shape");
        // Add the point to the active curve
        curveCreator.curves[selectionInfo.curveSelected].Points.Add(position);
    }

    void DeletePoint()
    {
        curveCreator.curves[selectionInfo.curveSelected].Points.RemoveAt(selectionInfo.pointHoverOver);
    }

    void ClearAll()
    {
        curveCreator.curves.Clear();
    }

    void ResetVariables()
    {
        selectionInfo.pointHoverOver = -1;
        selectionInfo.pointSelected = -1;

        selectionInfo.curveHoverOver = -1;
        selectionInfo.curveSelected = -1;
    }

    public class SelectionInfo
    {
        // Points
        public int pointHoverOver = -1;
        public int pointSelected = -1;
    
        // Curve
        public int curveHoverOver = -1;
        public int curveSelected = -1;
    }
}
