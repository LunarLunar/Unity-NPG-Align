using UnityEngine;
using UnityEditor;
using System.Linq;

public class NPG_AlignWindow : EditorWindow
{
    private Texture2D npgLogo;

    [MenuItem("NPG/Open Align Window", false, 0)] // Menu item to open the window
    public static void ShowWindow()
    {
        GetWindow<NPG_AlignWindow>("NPG Align Tools"); // Create or get the window
    }

    void OnEnable()
    {
        // Get the path of the current script
        string scriptPath = AssetDatabase.GetAssetPath(MonoScript.FromScriptableObject(this));
        // Get the directory of the script
        string scriptDirectory = System.IO.Path.GetDirectoryName(scriptPath);
        // Construct the logo path relative to the script's directory
        string dynamicLogoPath = System.IO.Path.Combine(scriptDirectory, "NPG_Logo.png").Replace("\\", "/"); // Ensure forward slashes for Unity paths
        
        // Load the logo using the dynamic path
        npgLogo = AssetDatabase.LoadAssetAtPath<Texture2D>(dynamicLogoPath);
    }

    void OnGUI()
    {
        // Display the logo if loaded
        if (npgLogo != null)
        {
            // Calculate aspect ratio to maintain image proportions
            EditorGUILayout.Space(); // Add space above the logo
            float aspectRatio = (float)npgLogo.width / npgLogo.height;
            float targetWidth = (EditorGUIUtility.currentViewWidth - 20) * 1.2f; // Window width minus padding, then extended by 1.2
            float targetHeight = targetWidth / aspectRatio;

            // Ensure logo doesn't get too big or too small
            targetHeight = Mathf.Min(targetHeight, 100); // Max height 100 pixels
            targetWidth = targetHeight * aspectRatio;

            Rect logoRect = GUILayoutUtility.GetRect(targetWidth , targetHeight - 20, GUILayout.ExpandWidth(true));
            GUI.DrawTexture(logoRect, npgLogo, ScaleMode.ScaleToFit);
            EditorGUILayout.Space(); // Add space below the logo (original space)
        }
        else
        {
            EditorGUILayout.HelpBox("NPG Logo not found. Please ensure 'NPG_Logo.png' is in the same folder as NPG_AlignWindow.cs.", MessageType.Warning);
        }

        // Get selected objects
        GameObject[] selectedObjects = Selection.GetFiltered(typeof(GameObject), SelectionMode.Editable | SelectionMode.TopLevel).Select(o => o as GameObject).ToArray();
        int selectedCount = selectedObjects.Length;

        EditorGUILayout.LabelField("Selected Objects: " + selectedCount, EditorStyles.boldLabel);

        // --- Distribution Section ---
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Distribute", EditorStyles.boldLabel);
        
        // Disable buttons if less than 2 objects are selected for distribution
        EditorGUI.BeginDisabledGroup(selectedCount < 2);
        if (GUILayout.Button(new GUIContent("Distribute X", "Evenly distribute selected objects along the X-axis. (Hotkey: Ctrl+Q)")))
        {
            NPG_Align.DistributeX();
        }
        if (GUILayout.Button(new GUIContent("Distribute Y", "Evenly distribute selected objects along the Y-axis. (Hotkey: Ctrl+E)")))
        {
            NPG_Align.DistributeY();
        }
        if (GUILayout.Button(new GUIContent("Distribute Z", "Evenly distribute selected objects along the Z-axis. (Hotkey: Ctrl+W)")))
        {
            NPG_Align.DistributeZ();
        }
        EditorGUI.EndDisabledGroup();

        // --- Alignment Section ---
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Align X", EditorStyles.boldLabel);
        GUILayout.BeginHorizontal();
        EditorGUI.BeginDisabledGroup(selectedCount < 2);
        if (GUILayout.Button(new GUIContent("Min", "Align selected objects to the minimum X-coordinate.")))
        {
            NPG_Align.AlignXMin();
        }
        if (GUILayout.Button(new GUIContent("Center", "Align selected objects to the center X-coordinate.")))
        {
            NPG_Align.AlignXCenter();
        }
        if (GUILayout.Button(new GUIContent("Max", "Align selected objects to the maximum X-coordinate.")))
        {
            NPG_Align.AlignXMax();
        }
        EditorGUI.EndDisabledGroup();
        GUILayout.EndHorizontal();

        EditorGUILayout.LabelField("Align Y", EditorStyles.boldLabel);
        GUILayout.BeginHorizontal();
        EditorGUI.BeginDisabledGroup(selectedCount < 2);
        if (GUILayout.Button(new GUIContent("Min", "Align selected objects to the minimum Y-coordinate.")))
        {
            NPG_Align.AlignYMin();
        }
        if (GUILayout.Button(new GUIContent("Center", "Align selected objects to the center Y-coordinate.")))
        {
            NPG_Align.AlignYCenter();
        }
        if (GUILayout.Button(new GUIContent("Max", "Align selected objects to the maximum Y-coordinate.")))
        {
            NPG_Align.AlignYMax();
        }
        EditorGUI.EndDisabledGroup();
        GUILayout.EndHorizontal();

        EditorGUILayout.LabelField("Align Z", EditorStyles.boldLabel);
        GUILayout.BeginHorizontal();
        EditorGUI.BeginDisabledGroup(selectedCount < 2);
        if (GUILayout.Button(new GUIContent("Min", "Align selected objects to the minimum Z-coordinate.")))
        {
            NPG_Align.AlignZMin();
        }
        if (GUILayout.Button(new GUIContent("Center", "Align selected objects to the center Z-coordinate.")))
        {
            NPG_Align.AlignZCenter();
        }
        if (GUILayout.Button(new GUIContent("Max", "Align selected objects to the maximum Z-coordinate.")))
        {
            NPG_Align.AlignZMax();
        }
        EditorGUI.EndDisabledGroup();
        GUILayout.EndHorizontal();

        // --- Reset Section ---
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Reset", EditorStyles.boldLabel);
        // Only disable if no objects are selected for reset
        EditorGUI.BeginDisabledGroup(selectedCount == 0);
        if (GUILayout.Button(new GUIContent("Reset Transforms", "Reset position, rotation, and scale of selected objects to default. (Hotkey: Shift+Ctrl+Q)")))
        {
            NPG_Align.ResetAll();
        }
        EditorGUI.EndDisabledGroup();

        // Repaint the window every frame to update selected object count
        if (Event.current.type == EventType.Layout)
        {
            Repaint();
        }
    }
}