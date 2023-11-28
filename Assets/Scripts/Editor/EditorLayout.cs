using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

#region EditorLayout

public class EditorLayout : IDisposable
{
    public LayoutType LayoutType { get; }

    public Rect Rect { get; }

    private bool m_isDisposed = false;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public EditorLayout(LayoutType layoutType, params GUILayoutOption[] options)
    {
        LayoutType = layoutType;

        if (LayoutType == LayoutType.Vertical)
        {
            Rect = EditorGUILayout.BeginVertical(options);
        }
        else
        {
            Rect = EditorGUILayout.BeginHorizontal(options);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public EditorLayout(LayoutType layoutType, GUIStyle style, params GUILayoutOption[] options)
    {
        LayoutType = layoutType;
        if (LayoutType == LayoutType.Vertical)
        {
            Rect = EditorGUILayout.BeginVertical(style, options);
        }
        else
        {
            Rect = EditorGUILayout.BeginHorizontal(style, options);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Dispose()
    {
        if (m_isDisposed)
        {
            return;
        }

        m_isDisposed = true;

        if (LayoutType == LayoutType.Vertical)
        {
            EditorGUILayout.EndVertical();
        }
        else
        {
            EditorGUILayout.EndHorizontal();
        }
    }
}

#endregion
