using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Rendering;

public class MeshData<T> : IDisposable where T : struct, IVertexData
{
    [ThreadStatic]
    private static T[] m_vertexData;

    private readonly ushort[] m_indexData;

    private int m_vertexDataCount;
    private int m_indexDataCount;
    private Vector3 m_boundsMin;
    private Vector3 m_boundsMax;

    #region VertexBufferLength
    private readonly int m_vertexBufferLength;

    public int VertexBufferLength
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => m_vertexBufferLength;
    }
    #endregion

    #region IndexBufferLength
    private readonly int m_indexBufferLength;

    public int IndexBufferLength
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => m_indexBufferLength;
    }
    #endregion

    #region Lifetime

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="vertexMaxCount">Max vertex buffer length</param>
    public MeshData(int vertexMaxCount)
    {
        m_addVertex = AddVertexInit;

        m_vertexBufferLength = vertexMaxCount;
        m_indexBufferLength = m_vertexBufferLength * 4;

        if ((null != m_vertexData) && (m_vertexData.Length < m_vertexBufferLength))
        {
            m_vertexData = null;
        }

        if (null == m_vertexData)
        {
            m_vertexData = new T[m_vertexBufferLength];
        }

        m_indexData = new ushort[m_indexBufferLength];
        m_indexDataCount = 0;

        m_vertexDataCount = 0;
    }

    public void InitializeMesh(Mesh mesh)
    {
        // prepare mesh
        mesh.SetVertexBufferParams(m_vertexBufferLength, new T().Layout);
        mesh.SetIndexBufferParams(m_indexBufferLength, IndexFormat.UInt16);
        mesh.indexFormat = IndexFormat.UInt16;
        mesh.subMeshCount = 1;

        mesh.SetSubMesh(0, new SubMeshDescriptor(0, 0, MeshTopology.Quads)
        {
            bounds = default,
        }, MeshUpdateFlags.DontRecalculateBounds | MeshUpdateFlags.DontValidateIndices | MeshUpdateFlags.DontNotifyMeshUsers | MeshUpdateFlags.DontResetBoneBounds);

        mesh.bounds = default;
    }

    private bool m_isDisposed;

    public void Dispose()
    {
        if (m_isDisposed)
        {
            return;
        }

        m_isDisposed = true;
    }

    #endregion

    #region Vertices

    private delegate void AddVertexDelegate(T vertexData);

    private AddVertexDelegate m_addVertex = null;


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void AddVertexInit(T vertexData)
    {
        //Assert.IsTrue(m_vertexDataCount + 1 < m_vertexData.Length, $"MeshData<{typeof(T)}> vertex buffer too small.");

        m_boundsMin = vertexData.Position;
        m_boundsMax = m_boundsMin;

        m_vertexData[m_vertexDataCount] = vertexData;

        m_vertexDataCount++;

        m_addVertex = AddVertexCommon;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void AddVertexCommon(T vertexData)
    {
        //Assert.IsTrue(m_vertexDataCount + 1 < m_vertexData.Length, $"MeshData<{typeof(T)}> vertex buffer too small.");

        m_boundsMin = Vector3.Min(m_boundsMin, vertexData.Position);
        m_boundsMax = Vector3.Max(m_boundsMin, vertexData.Position);

        m_vertexData[m_vertexDataCount] = vertexData;

        m_vertexDataCount++;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AddVertex(T vertexData)
    {
        m_addVertex(vertexData);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int GetVertexCount()
    {
        return m_vertexDataCount;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void ApplyVertexData(Mesh mesh)
    {
        ref int cnt = ref m_vertexDataCount;
        if (cnt > 0)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();

            mesh.SetVertexBufferData(m_vertexData, 0, 0, cnt, 0, MeshUpdateFlags.DontRecalculateBounds | MeshUpdateFlags.DontValidateIndices | MeshUpdateFlags.DontNotifyMeshUsers | MeshUpdateFlags.DontResetBoneBounds);
            sw.Stop();

            UnityEngine.Debug.Log($"SetVertexBufferData: {sw.ElapsedTicks} ticks");

            MaxVertCountUsed = Mathf.Max(MaxVertCountUsed, cnt);
        }
    }

    public int MaxVertCountUsed { get; private set; }

    #endregion

    #region Indices

    /// <summary>
    /// Adds index
    /// </summary>
    /// <param name="value">Mesh index value. Index base is BatchIndex * VertexCountMax</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AddIndex(ushort value)
    {
        //Assert.IsTrue(cnt + 1 < m_indexBufferLength, $"MeshData<{typeof(T)}> index buffer too small.");

        m_indexData[m_indexDataCount++] = value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int GetIndex(int pos)
    {
        //Assert.IsTrue(pos >= 0);
        //Assert.IsTrue(pos < m_indexDataCount[submesh]);

        return m_indexData[pos];
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int GetIndexCount()
    {
        return m_indexDataCount;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int ApplyIndexData(Mesh mesh)
    {
        if (m_indexDataCount > 0)
        {
            mesh.SetIndexBufferData(
                m_indexData,
                0,
                0,
                m_indexDataCount,
                MeshUpdateFlags.DontRecalculateBounds | MeshUpdateFlags.DontValidateIndices | MeshUpdateFlags.DontNotifyMeshUsers | MeshUpdateFlags.DontResetBoneBounds);

            mesh.SetSubMesh(0, new SubMeshDescriptor(0, m_indexDataCount, MeshTopology.Quads)
            {
                bounds = Bounds,
            }, MeshUpdateFlags.DontValidateIndices | MeshUpdateFlags.DontNotifyMeshUsers | MeshUpdateFlags.DontResetBoneBounds);

            return m_indexDataCount;
        }

        return 0;
    }

    private Bounds Bounds
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => new Bounds(m_boundsMin + (m_boundsMax + m_boundsMin) * 0.5f, m_boundsMax - m_boundsMin);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void UpdateMesh(Mesh mesh)
    {
        mesh.bounds = Bounds;

        mesh.UploadMeshData(false);
    }

    #endregion

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Clear()
    {
        m_indexDataCount = 0;

        ClearVertexData();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void ClearVertexData()
    {
        m_vertexDataCount = 0;
        m_addVertex = AddVertexInit;
    }
}
