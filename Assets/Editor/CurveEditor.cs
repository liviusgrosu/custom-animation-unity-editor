﻿using System.Collections;
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

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("Clear Data"))
        {
            ClearAll();
        }

        for (int curveIdx = 0; curveIdx < curveCreator.curves.Count; curveIdx++)
        {
            string name = $"Rail - {curveIdx}";
            
            if (!string.IsNullOrEmpty(curveCreator.railNames[curveIdx]))
            {
                // Default name if it doesn't exist
                name = curveCreator.railNames[curveIdx];
            }  
            
            // Foldout for each animation rail
            curveCreator.showAnimationRail[curveIdx] = EditorGUILayout.Foldout(curveCreator.showAnimationRail[curveIdx], name);
            if (curveCreator.showAnimationRail[curveIdx])
            {
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

                EditorGUILayout.PropertyField(startTriggerObjs.GetArrayElementAtIndex(curveIdx), new GUIContent("Start Trigger"), false);
                EditorGUILayout.PropertyField(endTriggerObjs.GetArrayElementAtIndex(curveIdx), new GUIContent("End Trigger"), false);
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
            else if (Event.current.button == 2)
            {
                if (guiEvent.modifiers == EventModifiers.None)
                {
                    // Select point
                    HandleMiddleMouseDown();
                }
            }
        }

        // Check if mouse is hovering over a point
        UpdateMouseOverInfo(mouseRay);
    }

    void HandleMiddleMouseDown()
    {
        if(selectionInfo.pointHoverOver == -1)
        {
            return;
        }
        // Select the curve
        selectionInfo.curveSelected = selectionInfo.curveHoverOver;
        // intermediate point is selected by the hovered point over
        curveCreator.curves[selectionInfo.curveSelected].intermediatePointIdx = selectionInfo.pointHoverOver;
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
            // New curve is the selected curve
            selectionInfo.curveSelected = curveCreator.curves.Count() - 1;
        }

        // Create point in the selected point
        CreateNewPoint(mousePosition);
        // New curve is the selected curve
        selectionInfo.pointSelected = curveCreator.curves[selectionInfo.curveSelected].points.Count() - 1;
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
            if (!curveCreator.curves[selectionInfo.curveSelected].points.Any())
            {
                // Remove the curve if there are no points
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
        // New curve is the selected curve
        selectionInfo.curveSelected = curveCreator.curves.Count() - 1;

        // Create point in the selected point
        CreateNewPoint(mousePosition);
        // New curve is the selected curve
        selectionInfo.pointSelected = curveCreator.curves[selectionInfo.curveSelected].points.Count() - 1;
    }


    void UpdateMouseOverInfo(Ray mouseRay)
    {
        // Get direction of the mouse
        selectionInfo.pointHoverOver = -1;
        selectionInfo.curveHoverOver = -1;
        Vector3 mouseDir = mouseRay.direction.normalized;

        for (int curveIdx = 0; curveIdx < curveCreator.curves.Count; curveIdx++)
        {
            for (int pointIdx = 0; pointIdx < curveCreator.curves[curveIdx].points.Count; pointIdx++)
            {
                Vector3 point = curveCreator.curves[curveIdx].points[pointIdx];

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

        for (int curveIdx = 0; curveIdx < curveCreator.curves.Count; curveIdx++)
        {
            for (int pointIdx = 0; pointIdx < curveCreator.curves[curveIdx].points.Count; pointIdx++)
            {
                bool hoverCurve = selectionInfo.curveHoverOver == curveIdx;
                bool hoverPoint = selectionInfo.pointHoverOver == pointIdx;

                bool selectedCurve = selectionInfo.curveSelected == curveIdx;
                bool selectedPoint = selectionInfo.pointSelected == pointIdx;

                Vector3 currentPointPos = curveCreator.curves[curveIdx].points[pointIdx];
                int pointsCount = curveCreator.curves[curveIdx].points.Count;

                if (selectedCurve)
                {
                    // Ensures that the point being hovered over is contained within the curve
                    Handles.color = hoverPoint && hoverCurve == selectedCurve ? Color.magenta : Color.black;

                    if (pointIdx == curveCreator.curves[curveIdx].intermediatePointIdx) 
                    {
                        Handles.color = Color.yellow;
                    }

                    if (selectedPoint)
                    {
                        Handles.color = Color.red;
                        Vector3 newPointPosition = Handles.PositionHandle(curveCreator.curves[curveIdx].points[pointIdx], Quaternion.identity);
                        if (EditorGUI.EndChangeCheck())
                        {
                            curveCreator.curves[curveIdx].points[pointIdx] = newPointPosition;
                        }
                    }

                    Handles.SphereHandleCap(0, currentPointPos, Quaternion.LookRotation(Vector3.up), 0.5f, EventType.Repaint);
                    Handles.color = Color.white;
                    if (pointIdx < pointsCount - 1)
                    {
                        Handles.DrawDottedLine(currentPointPos, curveCreator.curves[curveIdx].points[pointIdx + 1], 4);
                    }
                }
                else
                {
                    Handles.color = hoverCurve ? Color.green : Color.gray;

                    if (pointIdx == curveCreator.curves[curveIdx].intermediatePointIdx) 
                    {
                        Handles.color = Color.yellow;
                    }

                    Handles.SphereHandleCap(0, currentPointPos, Quaternion.LookRotation(Vector3.up), 0.5f, EventType.Repaint);
                    Handles.color = hoverCurve ? Color.green : Color.gray;
                    if (pointIdx < pointsCount - 1)
                    {
                        Handles.DrawDottedLine(currentPointPos, curveCreator.curves[curveIdx].points[pointIdx + 1], 4);
                    }
                }
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

    void CreateNewPoint(Vector3 position)
    {
        // Check which curve it goes into 
        Undo.RecordObject(curveCreator, "Create shape");
        // Add the point to the active curve
        curveCreator.curves[selectionInfo.curveSelected].points.Add(position);
    }

    void DeletePoint()
    {
        curveCreator.curves[selectionInfo.curveSelected].points.RemoveAt(selectionInfo.pointHoverOver);
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
