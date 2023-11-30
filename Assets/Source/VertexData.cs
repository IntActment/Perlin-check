using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
using UnityEngine.Rendering;

public static partial class Extensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe ref float GetIndex(ref Vector4 vec, int index)
    {
        switch (index)
        {
            case 0: fixed (float* ptr = &vec.x) return ref *ptr;
            case 1: fixed (float* ptr = &vec.y) return ref *ptr;
            case 2: fixed (float* ptr = &vec.z) return ref *ptr;
            case 3: fixed (float* ptr = &vec.w) return ref *ptr;
            default: throw new NotSupportedException();
        }
    }
}

public interface IVertexData
{
    Vector3 Position { get; set; }

    VertexAttributeDescriptor[] Layout { get; }
}

[StructLayout(LayoutKind.Sequential)]
public struct VertexData : IVertexData
{
    public Vector3 position;
    public Vector3 normal;
    public Color color;
    public Vector4 uv0;
    public Vector4 uv3;
    public Vector4 uv4;
    public Vector4 uv5;
    public Vector4 uv6;
    public Vector4 uv7;

    Vector3 IVertexData.Position
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => position;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set => position = value;
    }

    VertexAttributeDescriptor[] IVertexData.Layout
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => Layout;
    }

    public static readonly VertexAttributeDescriptor[] Layout = new VertexAttributeDescriptor[]
    {
            new VertexAttributeDescriptor(VertexAttribute.Position, VertexAttributeFormat.Float32, 3),
            new VertexAttributeDescriptor(VertexAttribute.Normal, VertexAttributeFormat.Float32, 3),
            new VertexAttributeDescriptor(VertexAttribute.Color, VertexAttributeFormat.Float32, 4),
            new VertexAttributeDescriptor(VertexAttribute.TexCoord0, VertexAttributeFormat.Float32, 4),
            new VertexAttributeDescriptor(VertexAttribute.TexCoord3, VertexAttributeFormat.Float32, 4),
            new VertexAttributeDescriptor(VertexAttribute.TexCoord4, VertexAttributeFormat.Float32, 4),
            new VertexAttributeDescriptor(VertexAttribute.TexCoord5, VertexAttributeFormat.Float32, 4),
            new VertexAttributeDescriptor(VertexAttribute.TexCoord6, VertexAttributeFormat.Float32, 4),
            new VertexAttributeDescriptor(VertexAttribute.TexCoord7, VertexAttributeFormat.Float32, 4),
    };

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public VertexData(Vector3 position, Vector3 normal, Color color)
    {
        this.position = position;
        this.normal = normal;
        this.color = color;

        uv0 = default;
        uv3 = default;
        uv4 = default;
        uv5 = default;
        uv6 = default;
        uv7 = default;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public VertexData(Vector3 position, Color color)
    {
        this.position = position;
        this.normal = default;
        this.color = color;

        uv0 = default;
        uv3 = default;
        uv4 = default;
        uv5 = default;
        uv6 = default;
        uv7 = default;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public unsafe ref Vector4 GetUV(int index)
    {
        switch (index)
        {
            case 0: fixed (Vector4* ptr = &uv0) return ref *ptr;
            case 1: fixed (Vector4* ptr = &uv3) return ref *ptr;
            case 2: fixed (Vector4* ptr = &uv4) return ref *ptr;
            case 3: fixed (Vector4* ptr = &uv5) return ref *ptr;
            case 4: fixed (Vector4* ptr = &uv6) return ref *ptr;
            case 5: fixed (Vector4* ptr = &uv7) return ref *ptr;
            default: throw new NotSupportedException();
        }
    }
}
