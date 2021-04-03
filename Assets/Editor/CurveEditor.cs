using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(CurveCreator))]
public class CurveEditor : Editor
{
    CurveCreator curveCreator;
    SelectionInfo selectionInfo;
    bool curveChangedSinceLastRepaint;
    float randomRange = 5f;

    // ---- Testing Variables ----

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        // ---- Buttons ----
        if (GUILayout.Button("Create New Random set"))
        {
            CreateRandomSet(5);
        }

        if (GUILayout.Button("Create 1 point"))
        {
            CreateRandomSet(1);
        }


        // ---- Gameobjects ----

        SerializedProperty curve = serializedObject.FindProperty("curve");
        SerializedProperty startDelay = serializedObject.FindProperty("startDelay");
        SerializedProperty endDelay = serializedObject.FindProperty("endDelay");
        SerializedProperty startTriggerObj = serializedObject.FindProperty("startTriggerObj");
        SerializedProperty endTriggerObj = serializedObject.FindProperty("endTriggerObj");
        EditorGUILayout.PropertyField(curve);
        EditorGUILayout.PropertyField(startDelay);
        EditorGUILayout.PropertyField(endDelay);
        EditorGUILayout.PropertyField(startTriggerObj);
        EditorGUILayout.PropertyField(endTriggerObj);

        // ---- Repaint ----

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

        // Check if pressing mouse down and that we are not using alt/ctrl/fnc/etc...
        if (guiEvent.type == EventType.MouseDown && guiEvent.button == 0)
        {
            if (guiEvent.modifiers == EventModifiers.None)
            {
                HandleLeftMouseDown();
            }
            else if (guiEvent.modifiers == EventModifiers.Shift)
            {
                HandleShiftLeftMouseDown(mouseRay);
            }
            else if (guiEvent.modifiers == EventModifiers.Control)
            {
                HandleControlLeftMouseDown();
            }
        }


        if (!selectionInfo.pointIsSelected)
        {
            // Don't select any other point when dragging
            UpdateMouseOverInfo(mouseRay);
        }
    }
    
    void HandleLeftMouseDown()
    {
        if(!selectionInfo.mouseIsOverPoint)
        {
            // Click away from selected point if applicable
            selectionInfo.pointSelected = -1;
        }
        selectionInfo.pointSelected = selectionInfo.pointHoverOver;
    }

    void HandleShiftLeftMouseDown(Ray mouseRay)
    {
        Vector3 mousePosition = mouseRay.GetPoint(mouseRay.origin.magnitude);
        CreateNewPoint(mousePosition);
    }

    void HandleControlLeftMouseDown()
    {
        if(selectionInfo.mouseIsOverPoint)
        {
            DeletePoint();
        }
    }

    void UpdateMouseOverInfo(Ray mouseRay)
    {
        // Get direction of the mouse
        selectionInfo.pointHoverOver = -1;
        Vector3 mouseDir = mouseRay.direction.normalized;

        for (int pointIdx = 0; pointIdx < curveCreator.points.Count; pointIdx++)
        {
            float x = curveCreator.points[pointIdx].x - mouseRay.origin.x;
            float y = curveCreator.points[pointIdx].y - mouseRay.origin.y;
            float z = curveCreator.points[pointIdx].z - mouseRay.origin.z;

            float t = (x * mouseDir.x + y * mouseDir.y + z * mouseDir.z) / 
                        (mouseDir.x * mouseDir.x + mouseDir.y * mouseDir.y + mouseDir.z * mouseDir.z);

            float D1 = (mouseDir.x * mouseDir.x + mouseDir.y * mouseDir.y + mouseDir.z * mouseDir.z) * (t * t);
            float D2 = (x * mouseDir.x + y * mouseDir.y + z * mouseDir.z) * 2 * t;
            float D3 = (x * x + y * y + z * z);

            float D = D1 - D2 + D3;
            
            if (D < 1)
            {
                selectionInfo.pointHoverOver = pointIdx;
            }
        }
        selectionInfo.mouseIsOverPoint = selectionInfo.pointHoverOver != -1;
    }

    void Draw()
    {
        EditorGUI.BeginChangeCheck();
        for (int pointIdx = 0; pointIdx < curveCreator.points.Count; pointIdx++)
        {

            Handles.color = selectionInfo.pointHoverOver == pointIdx ? Color.red : Color.white;
            
            if (selectionInfo.pointSelected == pointIdx)
            {
                Handles.color = Color.green;
                Vector3 newPointPosition = Handles.PositionHandle(curveCreator.points[pointIdx], Quaternion.identity);
                if (EditorGUI.EndChangeCheck())
                {
                    curveCreator.points[pointIdx] = newPointPosition;
                }
            }

            Handles.SphereHandleCap(0, curveCreator.points[pointIdx], Quaternion.LookRotation(Vector3.up), 0.5f, EventType.Repaint);
            Handles.color = Color.black;
            if (pointIdx < curveCreator.points.Count - 1)
            {
                Handles.DrawDottedLine(curveCreator.points[pointIdx], curveCreator.points[pointIdx + 1], 4);
            }
        }
        curveChangedSinceLastRepaint = false;
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

    void CreateNewPoint(Vector3 position)
    {
        Undo.RecordObject(curveCreator, "Create shape");
        curveCreator.points.Add(position);
    }

    void DeletePoint()
    {
        curveCreator.points.RemoveAt(selectionInfo.pointHoverOver);
    }

    void CreateRandomSet(int amount)
    {
        curveCreator.points.Clear();
        for (int pointIdx = 0; pointIdx < amount; pointIdx++)
        {
            Vector3 newPos = new Vector3(
                UnityEngine.Random.Range(-randomRange, randomRange),
                UnityEngine.Random.Range(-randomRange, randomRange),
                UnityEngine.Random.Range(-randomRange, randomRange)
            );
            curveCreator.points.Add(newPos);
        }
    }

    void ResetVariables()
    {
        selectionInfo.pointHoverOver = -1;
        selectionInfo.pointSelected = -1;
        selectionInfo.pointIsSelected = false;
    }

    public class SelectionInfo
    {
        public int pointHoverOver = -1;
        public int pointSelected = -1;
        public bool mouseIsOverPoint;
        public bool pointIsSelected;
    }
}
