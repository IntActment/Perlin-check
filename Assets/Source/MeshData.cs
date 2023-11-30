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
    private readonly Vector3[] m_vertices;

    private int m_vertexDataCount;
    private readonly int[] m_indexDataCount;
    private readonly Bounds[] m_indexBounds;

    #region SubmeshCount
    private int m_submeshCount;

    public int SubmeshCount
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => m_submeshCount;
    }
    #endregion

    #region VertexBufferLength
    private readonly int m_vertexBufferLength;

    public int VertexBufferLength
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => m_vertexBufferLength;
    }
    #endregion

    public int VertexBaseOffset
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => 0;
    }

    #region IndexBufferLength
    private readonly int m_indexBufferLength;

    public int IndexBufferLength
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => m_indexBufferLength;
    }
    #endregion

    #region Lifetime

    private readonly bool m_holdVertices;

#if SUBMESH_RANGE_CHECK
        private readonly int[] m_indMin;
        private readonly int[] m_indMax;
#endif
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="kindCount">Max submesh count</param>
    /// <param name="vertexMaxCount">Max vertex buffer length</param>
    public MeshData(int kindCount, int vertexMaxCount, bool holdVertices)
    {
        m_submeshCount = kindCount;
        m_vertexBufferLength = vertexMaxCount;
        m_indexBufferLength = m_vertexBufferLength * 4;
        m_holdVertices = holdVertices;

        if (m_holdVertices)
        {
            m_vertices = new Vector3[m_submeshCount * m_vertexBufferLength];
        }

        if ((null != m_vertexData) && (m_vertexData.Length < m_submeshCount * m_vertexBufferLength))
        {
            m_vertexData = null;
        }

        if (null == m_vertexData)
        {
            m_vertexData = new T[m_submeshCount * m_vertexBufferLength];
        }

        m_indexData = new ushort[m_submeshCount * m_indexBufferLength];
        m_indexDataCount = new int[m_submeshCount];
        m_indexBounds = new Bounds[m_submeshCount];

#if SUBMESH_RANGE_CHECK
            m_indMin = new int[m_kindCount];
            m_indMax = new int[m_kindCount];

            for (int i = 0; i < m_kindCount; i++)
            {
                m_indMin[i] = -1;
                m_indMax[i] = -1;
            }
#endif

        m_vertexDataCount = 0;
    }

    public void InitializeMesh(Mesh mesh, int countMultiplier = 1)
    {
        Assert.IsTrue(countMultiplier > 0);

        // prepare mesh
        mesh.SetVertexBufferParams(countMultiplier * m_vertexBufferLength, new T().Layout);
        mesh.SetIndexBufferParams(countMultiplier * m_indexBufferLength, IndexFormat.UInt16);
        mesh.indexFormat = IndexFormat.UInt16;
        mesh.subMeshCount = m_submeshCount;

        for (int i = 0; i < mesh.subMeshCount; i++)
        {
            mesh.SetSubMesh(i, new SubMeshDescriptor(0, 0, MeshTopology.Quads)
            {
                bounds = default,
            }, MeshUpdateFlags.DontRecalculateBounds | MeshUpdateFlags.DontValidateIndices | MeshUpdateFlags.DontNotifyMeshUsers | MeshUpdateFlags.DontResetBoneBounds);
        }

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

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AddVertex(T vertexData)
    {
        Assert.IsTrue(m_vertexDataCount + 1 < m_vertexData.Length, $"MeshData<{typeof(T)}> vertex buffer too small.");

        m_vertexData[m_vertexDataCount] = vertexData;

        if (m_holdVertices)
        {
            m_vertices[m_vertexDataCount] = vertexData.Position;
        }

        m_vertexDataCount++;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AddVertex(int submesh, T vertexData)
    {
        Assert.IsTrue(m_vertexDataCount + 1 < m_vertexData.Length, $"MeshData<{typeof(T)}> vertex buffer too small.");

        if (m_vertexDataCount == 0)
        {
            m_indexBounds[submesh] = new Bounds() { center = vertexData.Position };
        }
        else
        {
            m_indexBounds[submesh].Encapsulate(vertexData.Position);
        }

        m_vertexData[m_vertexDataCount] = vertexData;

        if (m_holdVertices)
        {
            m_vertices[m_vertexDataCount] = vertexData.Position;
        }

        m_vertexDataCount++;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T GetVertexData(int pos)
    {
        Assert.IsTrue(pos >= 0);
        Assert.IsTrue(pos < m_vertexData.Length);

        return m_vertexData[pos];
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Vector3 GetVertex(int pos)
    {
        Assert.IsTrue(pos >= 0);
        Assert.IsTrue(pos < m_vertexData.Length);
        Assert.IsTrue(m_holdVertices);

        return m_vertices[pos];
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void SetVertex(int pos, T vertexData)
    {
        Assert.IsTrue(pos >= 0);
        Assert.IsTrue(pos < m_vertexData.Length);

        m_vertexData[pos] = vertexData;
    }

    public T Last
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => m_vertexData[m_vertexDataCount - 1];

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set
        {
            m_vertexData[m_vertexDataCount - 1] = value;
        }
    }

    public T First
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => m_vertexData[0];

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set
        {
            m_vertexData[0] = value;
        }
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
            mesh.SetVertexBufferData(m_vertexData, 0, VertexBaseOffset, cnt, 0, MeshUpdateFlags.DontRecalculateBounds | MeshUpdateFlags.DontValidateIndices | MeshUpdateFlags.DontNotifyMeshUsers | MeshUpdateFlags.DontResetBoneBounds);

            MaxVertCountUsed = Mathf.Max(MaxVertCountUsed, cnt);
        }
    }

    public int MaxVertCountUsed { get; private set; }

    #endregion

    #region Indices

    /// <summary>
    /// Adds index
    /// Note: vertex at value position must be added before this call
    /// </summary>
    /// <param name="submesh">Submesh</param>
    /// <param name="value">Mesh index value. Index base is BatchIndex * VertexCountMax</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AddIndex(int submesh, ushort value)
    {
        ref int cnt = ref m_indexDataCount[submesh];

        Assert.IsTrue(cnt + 1 < m_indexBufferLength, $"MeshData<{typeof(T)}> index buffer too small.");

        var v = m_vertexData[value].Position;

        if (cnt == 0)
        {
            m_indexBounds[submesh] = new Bounds() { center = v };
        }
        else
        {
            m_indexBounds[submesh].Encapsulate(v);
        }

        m_indexData[submesh * m_indexBufferLength + cnt++] = (ushort)(value + VertexBaseOffset);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int GetIndex(int pos, int submesh)
    {
        Assert.IsTrue(pos >= 0);
        Assert.IsTrue(pos < m_indexDataCount[submesh]);

        return m_indexData[submesh * m_indexBufferLength + pos] - VertexBaseOffset;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int GetIndexCount(int submesh)
    {
        return m_indexDataCount[submesh];
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int ApplyIndexData(Mesh mesh, int submesh, int startIndexDest)
    {
        if (m_indexDataCount[submesh] > 0)
        {
            mesh.SetIndexBufferData(
                m_indexData,
                submesh * m_indexBufferLength,
                startIndexDest,
                m_indexDataCount[submesh],
                MeshUpdateFlags.DontRecalculateBounds | MeshUpdateFlags.DontValidateIndices | MeshUpdateFlags.DontNotifyMeshUsers | MeshUpdateFlags.DontResetBoneBounds);

            return m_indexDataCount[submesh];
        }

        return 0;
    }

    #endregion

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool EncapsulateBounds(int submesh, ref Bounds? ret)
    {
        if (m_indexDataCount[submesh] == 0)
        {
            return false;
        }

        if (null == ret)
        {
            ret = m_indexBounds[submesh];
        }
        else
        {
            ret = ret.Value.CombineWith(m_indexBounds[submesh]);
        }

        return true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Clear()
    {
        m_vertexDataCount = 0;

        for (int i = 0; i < m_indexDataCount.Length; i++)
        {
            m_indexDataCount[i] = 0;
            m_indexBounds[i] = default;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void ClearVertexData()
    {
        m_vertexDataCount = 0;
    }
}
