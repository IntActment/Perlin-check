using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

public class StyleEditor : EditorWindow
{

    // Add menu named "My Window" to the Window menu
    [MenuItem("Window/Style editor")]
    public static void ShowEditor()
    {
        // Get existing open window or if none, make a new one:
        StyleEditor window = (StyleEditor)GetWindow(typeof(StyleEditor));
        window.Show();
    }

    public static int StyleIndex { get; set; } = 0;

    public static GUIStyle Style => GUI.skin.customStyles[StyleIndex];

    void OnGUI()
    {

        using (var _ = new Layout(LayoutType.Horizontal))
        {
            if (GUILayout.Button("<", GUILayout.Width(40)))
            {
                StyleIndex = (StyleIndex > 0) ? StyleIndex - 1 : GUI.skin.customStyles.Length - 1;
            }

            StyleIndex = EditorGUILayout.IntField(Mathf.Clamp(StyleIndex, 0, GUI.skin.customStyles.Length - 1), GUILayout.Width(80));

            if (GUILayout.Button(">", GUILayout.Width(40)))
            {
                StyleIndex = (StyleIndex == GUI.skin.customStyles.Length - 1) ? 0 : StyleIndex + 1;
            }
        }

        // style checker
        EditorGUILayout.SelectableLabel($"Name: {GUI.skin.customStyles[StyleIndex].name}");
    }
}
