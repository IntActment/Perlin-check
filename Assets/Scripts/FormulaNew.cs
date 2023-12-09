using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

using UnityEngine;
using UnityEngine.Rendering;

[ExecuteInEditMode]
public class FormulaNew : MonoBehaviour
{
    public float CutLevel = 1;
    public Color CutColor = Color.red;
    public float HeightScale = 10;
    public ComplexFormula ComplexFormula;
    public MeshFilter MeshFilter;
    public MeshRenderer MeshRenderer;

    public Vector2Int size = new Vector2Int(128, 128);
    private float[,] data = new float[128, 128];

    private MeshData<VertexData> m_meshData;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void RebuildNormals()
    {
        MeshFilter?.sharedMesh?.RecalculateNormals();
        //MeshFilter?.sharedMesh?.RecalculateTangents();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Rebuild(out bool indicesChanged)
    {
        indicesChanged = false;

        if (null == data)
        {
            return false;
        }

        Vector2Int size = new Vector2Int(data.GetLength(0), data.GetLength(1));

        if (size.x < 2)
        {
            return false;
        }

        if (size.y < 2)
        {
            return false;
        }

        if ((m_meshData.GetIndexCount() == 0) || (size.x * size.y != m_meshData.GetVertexCount()))
        {
            indicesChanged = true;
            m_meshData.Clear();
        }
        else
        {
            m_meshData.ClearVertexData();
        }

        float d;
        VertexData vd = default;

        for (int y = 0; y < size.y; y++)
        {
            for (int x = 0; x < size.x; x++)
            {
                d = Mathf.Clamp(data[x, y], 0, CutLevel);

                vd.position = new Vector3(x, d * HeightScale, y);
                vd.normal = Vector3.up;
                vd.color = Color.LerpUnclamped(Color.black, Color.white, d);
                m_meshData.AddVertex(vd);

                if (indicesChanged && (x > 0) && (y > 0))
                {
                    m_meshData.AddIndex((ushort)((y - 1) * size.x + x - 1));
                    m_meshData.AddIndex((ushort)((y - 0) * size.x + x - 1));
                    m_meshData.AddIndex((ushort)((y - 0) * size.x + x - 0));
                    m_meshData.AddIndex((ushort)((y - 1) * size.x + x - 0));
                }
            }
        }

        MeshRenderer.sharedMaterial.SetColor("_CutColor", CutColor);
        MeshRenderer.sharedMaterial.SetFloat("_Cutout", CutLevel);

        return true;
    }

    private void Awake()
    {

    }

    private void OnDestroy()
    {

    }

    private void OnEnable()
    {
#if UNITY_EDITOR
        ComplexFormula.OnFormulaChanged += ComplexFormula_OnFormulaChanged;
#endif

        if (null == MeshFilter)
        {
            MeshFilter = gameObject.GetComponent<MeshFilter>();
        }

        if (null == MeshRenderer)
        {
            MeshRenderer = gameObject.GetComponent<MeshRenderer>();
        }

        if (null != MeshFilter)
        {
            if (null == MeshFilter.sharedMesh)
            {
                MeshFilter.sharedMesh = new Mesh();
            }

            m_meshData = new MeshData<VertexData>(256 * 256);
            m_meshData.InitializeMesh(MeshFilter.sharedMesh);
        }

        MarkChanged();
    }

    private void OnDisable()
    {
#if UNITY_EDITOR
        ComplexFormula.OnFormulaChanged -= ComplexFormula_OnFormulaChanged;

        UnityEditor.AssetDatabase.SaveAssetIfDirty(this);
#endif
        //Debug.Log("OnDisable");

        m_meshData?.Dispose();
        m_meshData = null;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void ComplexFormula_OnFormulaChanged(ComplexFormula formula)
    {
        if (ComplexFormula != formula)
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
            GenerateMesh();

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

    private bool GenerateMesh()
    {
        if ((data.GetLength(0) != size.x) || (data.GetLength(1) != size.y))
        {
            data = new float[size.x, size.y];
        }

        for (int y = 0; y < size.y; y++)
        {
            for (int x = 0; x < size.x; x++)
            {
                data[x, y] = ComplexFormula.Calculate(x, y);
            }
        }

        bool indicesChanged = false;
        Stopwatch sw = new Stopwatch();
        sw.Start();
        {
            Stopwatch sw1 = new Stopwatch();
            sw1.Start();
            {
                if (false == Rebuild(out indicesChanged))
                {
                    return false;
                }
            }
            sw1.Stop();
            //UnityEngine.Debug.Log($"FormulaNew Rebuild: {sw1.ElapsedMilliseconds} ms");

            m_meshData.ApplyVertexData(MeshFilter.sharedMesh);

            if (true == indicesChanged)
            {
                m_meshData.ApplyIndexData(MeshFilter.sharedMesh);
            }

            m_meshData.UpdateMesh(MeshFilter.sharedMesh);
            MeshFilter.sharedMesh.RecalculateNormals();
        }
        sw.Stop();
        //UnityEngine.Debug.Log($"FormulaNew GenerateMesh: {sw.ElapsedMilliseconds} ms (indices: {indicesChanged})");

        return true;
    }
}
