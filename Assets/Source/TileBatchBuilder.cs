using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Rendering;

public class TileBatchBuilder<T> where T : struct, IVertexData
{
    public delegate bool GenerateDelegate(MeshData<T> meshData, out bool indicesChanged);
    public delegate void PostBuildDelegate();

    private readonly GenerateDelegate m_generateFunc;
    private readonly PostBuildDelegate m_postBuildFunc;
    private readonly MeshData<T> m_meshData;

    private int m_submeshCountMax;
    private Mesh m_mesh;

    private int m_maxVertCountUsed;

    public Mesh Mesh
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => m_mesh;
    }

    public int MaxVertCountUsed
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => m_maxVertCountUsed;
    }

    public string Name
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => m_mesh.name;
    }

    private bool m_isInitialized;

    public TileBatchBuilder(MeshData<T> meshData, GenerateDelegate generateFunc, PostBuildDelegate onPostBuild)
    {
        m_meshData = meshData;
        m_generateFunc = generateFunc;
        m_postBuildFunc = onPostBuild;
    }

    public void Generate()
    {
        if (false == m_isInitialized)
        {
            return;
        }

        Prepare();
        GenerateVertices(out bool indicesChanged);
        GenerateIndices(indicesChanged);

        m_postBuildFunc?.Invoke();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Initialize(Mesh mesh, int submeshCount)
    {
        Assert.IsTrue(mesh != null);

        if (true == m_isInitialized)
        {
            return;
        }

        m_mesh = mesh;
        m_submeshCountMax = submeshCount;

        // prepare mesh
        m_mesh.SetVertexBufferParams(m_meshData.VertexBufferLength, new T().Layout);
        m_mesh.SetIndexBufferParams(m_meshData.IndexBufferLength, IndexFormat.UInt16);
        m_mesh.indexFormat = IndexFormat.UInt16;
        m_mesh.subMeshCount = submeshCount;

        for (int i = 0; i < m_mesh.subMeshCount; i++)
        {
            m_mesh.SetSubMesh(i, new SubMeshDescriptor(0, 0, MeshTopology.Quads)
            {
                bounds = default,
            }, MeshUpdateFlags.DontRecalculateBounds | MeshUpdateFlags.DontValidateIndices | MeshUpdateFlags.DontNotifyMeshUsers | MeshUpdateFlags.DontResetBoneBounds);
        }

        m_mesh.bounds = default;

        m_isInitialized = true;
    }

    private bool m_tileChanged;
    private int[] m_indexBaseOffset;
    private int[] m_indexCount;
    private Bounds?[] m_tileMeshBounds;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void Prepare()
    {
        m_tileChanged = false;
        m_indexBaseOffset = new int[m_submeshCountMax];
        m_indexCount = new int[m_submeshCountMax];
        m_tileMeshBounds = new Bounds?[m_submeshCountMax];
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private bool GenerateVertices(out bool indicesChanged)
    {
        bool rebuild = m_generateFunc(m_meshData, out indicesChanged);
        m_tileChanged = m_tileChanged || rebuild;

        if (null != m_meshData)
        {
            if (rebuild)
            {
                m_maxVertCountUsed = Mathf.Max(m_maxVertCountUsed, m_meshData.MaxVertCountUsed);
            }

            for (int submesh = 0; submesh < m_submeshCountMax; submesh++)
            {
                m_indexCount[submesh] += m_meshData.GetIndexCount(submesh);
                m_meshData.EncapsulateBounds(submesh, ref m_tileMeshBounds[submesh]);
            }
        }

        return rebuild;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void GenerateIndices(bool indicesChanged)
    {
        if (false == m_tileChanged)
        {
            return;
        }

        for (int submesh = 1; submesh < m_submeshCountMax; submesh++)
        {
            m_indexBaseOffset[submesh] = m_indexCount[submesh - 1] + m_indexBaseOffset[submesh - 1];
        }

        int[] indexCounter = m_indexBaseOffset.ToArray();

        if (null != m_meshData)
        {
            for (int submesh = 0; submesh < m_submeshCountMax; submesh++)
            {
                indexCounter[submesh] += m_meshData.ApplyIndexData(m_mesh, submesh, indexCounter[submesh]);
            }
        }

        {
            Bounds? totalBounds = null;

            for (int submesh = 0; submesh < m_submeshCountMax; submesh++)
            {
                if (true == indicesChanged)
                {
                    m_mesh.SetSubMesh(submesh, new SubMeshDescriptor(m_indexBaseOffset[submesh], m_indexCount[submesh], MeshTopology.Quads)
                    {
                        bounds = m_tileMeshBounds[submesh] ?? default,
                    }, MeshUpdateFlags.DontRecalculateBounds | MeshUpdateFlags.DontValidateIndices | MeshUpdateFlags.DontNotifyMeshUsers | MeshUpdateFlags.DontResetBoneBounds);
                }

                if (m_tileMeshBounds[submesh].HasValue)
                {
                    if (totalBounds.HasValue)
                    {
                        totalBounds = totalBounds.Value.CombineWith(m_tileMeshBounds[submesh].Value);
                    }
                    else
                    {
                        totalBounds = m_tileMeshBounds[submesh];
                    }
                }
            }

            m_mesh.bounds = totalBounds ?? default;
        }

        m_mesh.UploadMeshData(false);
    }
}
