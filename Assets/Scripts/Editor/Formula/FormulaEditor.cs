using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Formula))]
public class FormulaEditor : Editor
{
    private Formula m_formula;

    private void OnEnable()
    {
        m_formula = (Formula)target;
    }

    private void OnDisable()
    {
        m_formula = null;
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        try
        {
            {
                var val = (ComplexFormula)EditorGUILayout.ObjectField("Formula", m_formula.ComplexFormula, typeof(ComplexFormula), true);
                if (val != m_formula.ComplexFormula)
                {
                    Undo.RecordObject(target, $"formula change");
                    m_formula.ComplexFormula = val;
                    EditorUtility.SetDirty(target);
                }
            }

            {
                var val = (TerrainTest)EditorGUILayout.ObjectField("Terrain test", m_formula.TerrainTest, typeof(TerrainTest), true);
                if (val != m_formula.TerrainTest)
                {
                    Undo.RecordObject(target, $"terrain selection change");
                    m_formula.TerrainTest = val;
                    EditorUtility.SetDirty(target);
                }
            }

            {
                var val = Vector2Int.Max(Vector2Int.one, Vector2Int.Min(Vector2Int.one * 256, EditorGUILayout.Vector2IntField("Map size", m_formula.size)));
                if (val != m_formula.size)
                {
                    Undo.RecordObject(target, $"formula size");
                    m_formula.size = val;
                    EditorUtility.SetDirty(target);
                }
            }

            {
                var val = EditorGUILayout.Toggle("Use cut", m_formula.UseCut);
                if (val != m_formula.UseCut)
                {
                    Undo.RecordObject(target, $"Use cut");
                    m_formula.UseCut = val;
                    EditorUtility.SetDirty(target);
                }
            }

            {
                EditorGUI.BeginDisabledGroup(false == m_formula.UseCut);

                EditorGUI.indentLevel++;
                {
                    var val = EditorGUILayout.Slider("Cut level", m_formula.CutLevel, 0, 1);
                    if (val != m_formula.CutLevel)
                    {
                        Undo.RecordObject(target, $"formula CutLevel");
                        m_formula.CutLevel = val;
                        EditorUtility.SetDirty(target);
                    }
                }

                {
                    var val = EditorGUILayout.ColorField("Cut color", m_formula.CutColor);
                    if (val != m_formula.CutColor)
                    {
                        Undo.RecordObject(target, $"formula CutColor");
                        m_formula.CutColor = val;
                        EditorUtility.SetDirty(target);
                    }
                }
                EditorGUI.indentLevel--;

                EditorGUI.EndDisabledGroup();
            }

            {
                var val = EditorGUILayout.FloatField("Height scale", m_formula.HeightScale);
                if (val != m_formula.HeightScale)
                {
                    Undo.RecordObject(target, $"formula HeightScale");
                    m_formula.HeightScale = val;
                    EditorUtility.SetDirty(target);
                }
            }

            if (false)
            {
                var val = EditorGUILayout.Toggle("Smooth normals", m_formula.SmoothNormals);
                if (val != m_formula.SmoothNormals)
                {
                    Undo.RecordObject(target, $"Smooth normals");
                    m_formula.SmoothNormals = val;
                    EditorUtility.SetDirty(target);
                }
            }
        }
        finally
        {
            serializedObject.ApplyModifiedProperties();
            AssetDatabase.SaveAssetIfDirty(target);

            try
            {
                if (GUI.changed)
                {
                    m_formula.Build();
                }
            }
            catch { }
        }
    }
}
