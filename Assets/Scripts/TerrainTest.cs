using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

using UnityEngine;

[ExecuteInEditMode]
public class TerrainTest : MonoBehaviour
{
    public MeshFilter MeshFilter;
    public MeshRenderer MeshRenderer;

    private Vector3[] m_vertices = new Vector3[0];
    private Color[] m_colors = new Color[0];
    private ushort[] m_indices = new ushort[0];
    private Vector2Int m_lastSize = new Vector2Int(0, 0);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Rebuild(float[,] data, float cutLevel, Color cutColor, float heightScale, bool forceRebuildIndices, bool smoothNormals, bool useCut)
    {
        if (null == data)
        {
            return;
        }

        Vector2Int size = new Vector2Int(data.GetLength(0), data.GetLength(1));

        if (size.x == 0)
        {
            return;
        }

        if (size.y == 0)
        {
            return;
        }

        if (null == MeshFilter.sharedMesh)
        {
            MeshFilter.sharedMesh = new Mesh();
        }

        bool setIndices = false;

        Stopwatch sw = new Stopwatch();
        sw.Start();
        {
            if ((true == forceRebuildIndices) || (m_lastSize != size))
            {
                MeshFilter.sharedMesh.Clear();

                m_lastSize = size;
                m_vertices = new Vector3[size.x * size.y];
                m_colors = new Color[m_vertices.Length];
                m_indices = new ushort[(size.x - 1) * (size.y - 1) * 4];
                setIndices = true;
            }

            Stopwatch sw2 = new Stopwatch();
            sw2.Start();
            {
                int vIndex;
                int iIndex = 0;
                float d;

                for (int x = 0; x < size.x; x++)
                {
                    for (int y = 0; y < size.y; y++)
                    {
                        d = useCut ? Mathf.Clamp(data[x, y], 0, cutLevel) : data[x, y];
                        vIndex = y * size.x + x;
                        m_vertices[vIndex] = new Vector3(x, d * heightScale, y);
                        m_colors[vIndex] = Color.LerpUnclamped(Color.black, Color.white, d);

                        if (setIndices && (x < size.x - 1) && (y < size.y - 1))
                        {
                            m_indices[iIndex++] = (ushort)((y + 0) * size.x + x + 0);
                            //m_indices[iIndex++] = (ushort)((y + 1) * size.x + x + 1);
                            m_indices[iIndex++] = (ushort)((y + 1) * size.x + x + 0);
                            //m_indices[iIndex++] = (ushort)((y + 0) * size.x + x + 0);
                            m_indices[iIndex++] = (ushort)((y + 1) * size.x + x + 1);
                            m_indices[iIndex++] = (ushort)((y + 0) * size.x + x + 1);
                        }
                    }
                }
            }
            sw2.Stop();
            //UnityEngine.Debug.Log($"TerrainTest Rebuild: {sw2.ElapsedMilliseconds} ms");

            MeshRenderer.sharedMaterial.SetColor("_CutColor", cutColor);
            MeshRenderer.sharedMaterial.SetFloat("_Cutout", useCut ? cutLevel : 100000);

            Stopwatch sw1 = new Stopwatch();
            sw1.Start();
            {
                MeshFilter.sharedMesh.SetVertices(m_vertices);
                MeshFilter.sharedMesh.SetColors(m_colors);
            }
            sw1.Stop();
            //UnityEngine.Debug.Log($"SetVertices + SetColors: {sw1.ElapsedTicks} ticks");

            if (setIndices)
            {
                MeshFilter.sharedMesh.SetIndices(m_indices, MeshTopology.Quads, 0, true);
            }

            if (smoothNormals)
            {
                MeshFilter.sharedMesh.RecalculateNormals();
                //MeshFilter.sharedMesh.RecalculateTangents();
            }
        }
        sw.Stop();

        //UnityEngine.Debug.Log($"TerrainTest Rebuild: {sw.ElapsedMilliseconds} ms (indices: {setIndices})");
    }
}
