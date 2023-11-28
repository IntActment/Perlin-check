using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public enum LayoutType
{
    Vertical,
    Horizontal,
}

#region Layout

public class Layout : IDisposable
{
    public LayoutType LayoutType { get; }

    private bool m_isDisposed = false;

    public Layout(LayoutType layoutType, params GUILayoutOption[] options)
    {
        LayoutType = layoutType;

        if (LayoutType == LayoutType.Vertical)
        {
            GUILayout.BeginVertical(options);
        }
        else
        {
            GUILayout.BeginHorizontal(options);
        }
    }

    public Layout(LayoutType layoutType, GUIStyle style, params GUILayoutOption[] options)
    {
        LayoutType = layoutType;
        if (LayoutType == LayoutType.Vertical)
        {
            GUILayout.BeginVertical(style, options);
        }
        else
        {
            GUILayout.BeginHorizontal(style, options);
        }
    }

    public Layout(LayoutType layoutType, string text, GUIStyle style, params GUILayoutOption[] options)
    {
        LayoutType = layoutType;
        if (LayoutType == LayoutType.Vertical)
        {
            GUILayout.BeginVertical(text, style, options);
        }
        else
        {
            GUILayout.BeginHorizontal(text, style, options);
        }
    }

    public Layout(LayoutType layoutType, GUIContent content, GUIStyle style, params GUILayoutOption[] options)
    {
        LayoutType = layoutType;
        if (LayoutType == LayoutType.Vertical)
        {
            GUILayout.BeginVertical(content, style, options);
        }
        else
        {
            GUILayout.BeginHorizontal(content, style, options);
        }
    }

    public Layout(LayoutType layoutType, Texture texture, GUIStyle style, params GUILayoutOption[] options)
    {
        LayoutType = layoutType;
        if (LayoutType == LayoutType.Vertical)
        {
            GUILayout.BeginVertical(texture, style, options);
        }
        else
        {
            GUILayout.BeginHorizontal(texture, style, options);
        }
    }

    public void Dispose()
    {
        if (m_isDisposed)
        {
            return;
        }

        m_isDisposed = true;

        if (LayoutType == LayoutType.Vertical)
        {
            GUILayout.EndVertical();
        }
        else
        {
            GUILayout.EndHorizontal();
        }
    }
}

#endregion

