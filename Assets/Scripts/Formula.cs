using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

using UnityEngine;

[ExecuteInEditMode]
public class Formula : MonoBehaviour
{
    public TerrainTest TerrainTest;

    public float CutLevel = 1;
    public Color CutColor = Color.red;
    public float HeightScale = 10;
    public ComplexFormula ComplexFormula;

    public Vector2Int size = new Vector2Int(128, 128);
    public float[,] data = new float[128, 128];

    private bool m_forceRebuild = false;

    private void OnEnable()
    {
        m_forceRebuild = true;
#if UNITY_EDITOR
        ComplexFormula.OnFormulaChanged += ComplexFormula_OnFormulaChanged;
#endif
    }

    private void OnDisable()
    {
#if UNITY_EDITOR
        ComplexFormula.OnFormulaChanged -= ComplexFormula_OnFormulaChanged;
#endif
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void ComplexFormula_OnFormulaChanged(ComplexFormula formula)
    {
        if (this.ComplexFormula != formula)
        {
            return;
        }

        MarkChanged();
    }

    private void Update()
    {
        if (false == enabled)
        {
            return;
        }

        if (m_needRebuild)
        {
            Build();

            m_needRebuild = false;
        }
    }

    void OnDrawGizmos()
    {
        // Your gizmo drawing thing goes here if required...

#if UNITY_EDITOR
        // Ensure continuous Update calls.
        if (false == Application.isPlaying)
        {
            UnityEditor.EditorApplication.QueuePlayerLoopUpdate();
            UnityEditor.SceneView.RepaintAll();
        }
#endif
    }

    private bool m_needRebuild = false;

    public void MarkChanged()
    {
        if (null == ComplexFormula)
        {
            return;
        }

        m_needRebuild = true;
    }

    public void Build()
    {
        if (null == ComplexFormula)
        {
            return;
        }

        if ((data.GetLength(0) != size.x) || (data.GetLength(1) != size.y))
        {
            data = new float[size.x, size.y];
        }

        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
            {
                data[x, y] = ComplexFormula.Calculate(x, y);
            }
        }

        TerrainTest.Rebuild(data, CutLevel, CutColor, HeightScale, m_forceRebuild);

        m_forceRebuild = false;
    }
}
