using UnityEngine;
using UnityEditor;
using System.Linq;

// NPG_Align v2.0
// Changelog:
// - Added full Undo/Redo support for all actions.
// - Added full alignment options: Min, Max, and Center for X, Y, Z axes.
// - Refactored code to reduce redundancy.
// - Re-organized menu items into sub-menus for better clarity.

class NPG_Align : ScriptableObject
{
    const float VERSION = 2.0f;

#if UNITY_EDITOR

    #region Helper Methods & Enums

    private enum Axis { X, Y, Z }

    private static GameObject[] GetSelectedObjects()
    {
        return Selection.GetFiltered(
                typeof(GameObject),
                SelectionMode.Editable | SelectionMode.TopLevel)
            .Select(o => o as GameObject)
            .ToArray();
    }

    #endregion

    #region Distribution

    [MenuItem("NPG/Distribute/Distribute on X %q", false, 10)]
    public static void DistributeX() => DistributeAlongAxis(Axis.X);

    [MenuItem("NPG/Distribute/Distribute on Y %e", false, 11)]
    public static void DistributeY() => DistributeAlongAxis(Axis.Y);

    [MenuItem("NPG/Distribute/Distribute on Z %w", false, 12)]
    public static void DistributeZ() => DistributeAlongAxis(Axis.Z);

    private static void DistributeAlongAxis(Axis axis)
    {
        GameObject[] selectedObjects = GetSelectedObjects();
        if (selectedObjects.Length < 2) return;

        Undo.RecordObjects(selectedObjects.Select(go => go.transform).ToArray(), $"Distribute on {axis}");

        float min = 0, max = 0;

        // Get axis value using a delegate
        System.Func<Vector3, float> getAxisValue = pos => pos.x;
        if (axis == Axis.Y) getAxisValue = pos => pos.y;
        if (axis == Axis.Z) getAxisValue = pos => pos.z;

        for (int i = 0; i < selectedObjects.Length; i++)
        {
            float val = getAxisValue(selectedObjects[i].transform.position);
            if (i == 0)
            {
                min = max = val;
            }
            else
            {
                if (val < min) min = val;
                if (val > max) max = val;
            }
        }

        var sortedObjects = selectedObjects.OrderBy(go => getAxisValue(go.transform.position)).ToArray();

        float gap = (max - min) / (selectedObjects.Length - 1);
        Vector3 basePosition = sortedObjects[0].transform.position;

        for (int i = 0; i < sortedObjects.Length; i++)
        {
            Vector3 currentPos = sortedObjects[i].transform.position;
            float newCoord = min + gap * i;

            if (axis == Axis.X)
                sortedObjects[i].transform.position = new Vector3(newCoord, basePosition.y, basePosition.z);
            else if (axis == Axis.Y)
                sortedObjects[i].transform.position = new Vector3(basePosition.x, newCoord, basePosition.z);
            else if (axis == Axis.Z)
                sortedObjects[i].transform.position = new Vector3(basePosition.x, basePosition.y, newCoord);
        }
    }

    #endregion

    #region Alignment

    // --- ALIGN X ---
    [MenuItem("NPG/Align/X/Align Min", false, 30)]
    public static void AlignXMin() => AlignObjects(Axis.X, AlignMode.Min);
    [MenuItem("NPG/Align/X/Align Center", false, 31)]
    public static void AlignXCenter() => AlignObjects(Axis.X, AlignMode.Center);
    [MenuItem("NPG/Align/X/Align Max", false, 32)]
    public static void AlignXMax() => AlignObjects(Axis.X, AlignMode.Max);

    // --- ALIGN Y ---
    [MenuItem("NPG/Align/Y/Align Min", false, 40)]
    public static void AlignYMin() => AlignObjects(Axis.Y, AlignMode.Min);
    [MenuItem("NPG/Align/Y/Align Center", false, 41)]
    public static void AlignYCenter() => AlignObjects(Axis.Y, AlignMode.Center);
    [MenuItem("NPG/Align/Y/Align Max", false, 42)]
    public static void AlignYMax() => AlignObjects(Axis.Y, AlignMode.Max);

    // --- ALIGN Z ---
    [MenuItem("NPG/Align/Z/Align Min", false, 50)]
    public static void AlignZMin() => AlignObjects(Axis.Z, AlignMode.Min);
    [MenuItem("NPG/Align/Z/Align Center", false, 51)]
    public static void AlignZCenter() => AlignObjects(Axis.Z, AlignMode.Center);
    [MenuItem("NPG/Align/Z/Align Max", false, 52)]
    public static void AlignZMax() => AlignObjects(Axis.Z, AlignMode.Max);

    private enum AlignMode { Min, Max, Center }

    private static void AlignObjects(Axis axis, AlignMode mode)
    {
        GameObject[] selectedObjects = GetSelectedObjects();
        if (selectedObjects.Length < 2) return;

        Undo.RecordObjects(selectedObjects.Select(go => go.transform).ToArray(), $"Align {axis} {mode}");

        float min = 0, max = 0;
        
        System.Func<Vector3, float> getAxisValue = pos => pos.x;
        if (axis == Axis.Y) getAxisValue = pos => pos.y;
        if (axis == Axis.Z) getAxisValue = pos => pos.z;

        for (int i = 0; i < selectedObjects.Length; i++)
        {
            float val = getAxisValue(selectedObjects[i].transform.position);
            if (i == 0)
            {
                min = max = val;
            }
            else
            {
                if (val < min) min = val;
                if (val > max) max = val;
            }
        }

        float targetValue = 0;
        switch (mode)
        {
            case AlignMode.Min:
                targetValue = min;
                break;
            case AlignMode.Max:
                targetValue = max;
                break;
            case AlignMode.Center:
                targetValue = (min + max) / 2f;
                break;
        }

        foreach (var go in selectedObjects)
        {
            Vector3 pos = go.transform.position;
            if (axis == Axis.X) go.transform.position = new Vector3(targetValue, pos.y, pos.z);
            if (axis == Axis.Y) go.transform.position = new Vector3(pos.x, targetValue, pos.z);
            if (axis == Axis.Z) go.transform.position = new Vector3(pos.x, pos.y, targetValue);
        }
    }

    #endregion

    #region Reset

    [MenuItem("NPG/Reset/Reset Transforms #%q", false, 100)]
    public static void ResetAll()
    {
        GameObject[] selectedObjects = GetSelectedObjects();
        if (selectedObjects.Length == 0) return;

        Undo.RecordObjects(selectedObjects.Select(go => go.transform).ToArray(), "Reset Transforms");

        foreach (var go in selectedObjects)
        {
            go.transform.position = Vector3.zero;
            go.transform.rotation = Quaternion.identity;
            go.transform.localScale = Vector3.one;
        }
    }

    #endregion

#endif
}