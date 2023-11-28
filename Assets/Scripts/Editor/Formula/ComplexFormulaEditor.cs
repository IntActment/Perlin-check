using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ComplexFormula))]
public class ComplexFormulaEditor : Editor
{
    public override void OnInspectorGUI()
    {
        if (GUILayout.Button("Edit..."))
        {
            ComplexFormulaEditorWindow.ShowEditor();
        }
    }
}
