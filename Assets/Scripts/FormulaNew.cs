using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

using UnityEngine;

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

    private TileBatchBuilder<VertexData> m_batcher;
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

        Action<int, VertexData> setVertex;

        if ((m_meshData.GetIndexCount(0) == 0) || (size.x * size.y != m_meshData.GetVertexCount()))
        {
            indicesChanged = true;
            m_meshData.Clear();

            setVertex = (p, v) => m_meshData.AddVertex(v);
        }
        else
        {
            setVertex = (p, v) => m_meshData.SetVertex(p, v);
        }

        float d;

        for (int y = 0; y < size.y; y++)
        {
            for (int x = 0; x < size.x; x++)
            {
                d = Mathf.Clamp(data[x, y], 0, CutLevel);

                setVertex(y * size.x + x, new VertexData(new Vector3(x, d * HeightScale, y), Vector3.up, Color.LerpUnclamped(Color.black, Color.white, d)));

                if (indicesChanged && (x > 0) && (y > 0))
                {
                    m_meshData.AddIndex(0, (ushort)((y - 1) * size.x + x - 1));
                    m_meshData.AddIndex(0, (ushort)((y - 0) * size.x + x - 1));
                    m_meshData.AddIndex(0, (ushort)((y - 0) * size.x + x - 0));
                    m_meshData.AddIndex(0, (ushort)((y - 1) * size.x + x - 0));
                }
            }
        }

        MeshRenderer.sharedMaterial.SetColor("_CutColor", CutColor);
        MeshRenderer.sharedMaterial.SetFloat("_Cutout", CutLevel);

        return true;
    }

    private void Awake()
    {
        Debug.Log("Awake");
        
    }

    private void OnDestroy()
    {

    }

    private void OnEnable()
    {
        //Debug.Log("OnEnable");

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

            m_meshData = new MeshData<VertexData>(1, 256 * 256, false);
            m_batcher = new TileBatchBuilder<VertexData>(m_meshData, GenerateMesh, RebuildNormals);

            m_batcher.Initialize(MeshFilter.sharedMesh, 1);
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
        m_batcher = null;
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
        if (m_needRebuild)
        {
            //m_batcher.Generate();
        }
    }

    private bool m_needRebuild = false;

    public void MarkChanged()
    {
        if (null == ComplexFormula)
        {
            return;
        }

        m_needRebuild = true;
        m_batcher.Generate();
    }

    private bool GenerateMesh(MeshData<VertexData> meshData, out bool indicesChanged)
    {
        if (false == m_needRebuild)
        {
            indicesChanged = false;
            return false;
        }

        m_needRebuild = false;

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

        if (false == Rebuild(out indicesChanged))
        {
            return false;
        }

        meshData.ApplyVertexData(MeshFilter.sharedMesh);

        return true;
    }
}
