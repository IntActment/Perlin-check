using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.CompilerServices;

using UnityEditor;

using UnityEngine;
using UnityEngine.Assertions;

public static class RectLayoutExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static RectLayout WithLayout(this Rect container, string columns = "*", string rows = "*")
    {
        return new RectLayout(container, columns, rows);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Rect Center(this Rect container, float newWidth, float newHeight)
    {
        return new Rect(0, 0, newWidth, newHeight)
        {
            center = container.center,
        };
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Rect FlipY(this Rect r)
    {
        return new Rect(r.x, r.yMax, r.width, -r.height);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Rect Enlarge(this Rect r, float by)
    {
        return new Rect(r.xMin - by, r.yMin - by, r.width + by * 2, r.height + by * 2);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Rect WithOffset(this Rect r, Vector2 offset)
    {
        return new Rect(r.min + offset, r.size);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Rect MakeRectFromCenter(this Vector2 center, float radius)
    {
        return new Rect(center.x - radius, center.y - radius, radius * 2, radius * 2);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Rect MakeRectFromCenter(this Vector2 center, Vector2 size)
    {
        return new Rect(center - size / 2f, size);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void DrawFrame(this Rect rect, Color color)
    {
        Handles.color = color;
        Handles.DrawPolyLine(
            new Vector2(rect.min.x, rect.min.y),
            new Vector2(rect.min.x, rect.max.y),
            new Vector2(rect.max.x, rect.max.y),
            new Vector2(rect.max.x, rect.min.y),
            new Vector2(rect.min.x, rect.min.y));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void DrawSolidRect(this Rect rect, Color color)
    {
        Handles.color = color;
        Handles.DrawAAConvexPolygon(
            new Vector2(rect.min.x, rect.min.y),
            new Vector2(rect.min.x, rect.max.y),
            new Vector2(rect.max.x, rect.max.y),
            new Vector2(rect.max.x, rect.min.y),
            new Vector2(rect.min.x, rect.min.y));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Rect FitWithAspectRatio(this Rect container, float ratio)
    {
        Rect res;

        if (ratio > 1f)
        {
            res = container.Center(container.width, container.width / ratio);
        }
        else
        {
            res = container.Center(container.height * ratio, container.height);
        }

        if (res.width > container.width)
        {
            float downScale = res.width / container.width;

            res = res.Center(container.width, res.height / downScale);
        }

        if (res.height > container.height)
        {
            float downScale = res.height / container.height;

            res = res.Center(res.width / downScale, container.height);
        }

        return res;
    }
}

public class RectLayout
{
    public Rect Container { get; }

    public uint RowCount { get; }

    public uint ColCount { get; }

    private float[] m_widths;
    private float[] m_heights;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Rect GetArea(uint col = 0, uint row = 0, uint colSpan = 1, uint rowSpan = 1)
    {
        if (row >= m_heights.Length)
        {
            throw new System.IndexOutOfRangeException(nameof(row));
        }

        if (col >= m_widths.Length)
        {
            throw new System.IndexOutOfRangeException(nameof(col));
        }

        if (row + rowSpan - 1 >= m_heights.Length)
        {
            throw new System.IndexOutOfRangeException(nameof(rowSpan));
        }

        if (col + colSpan - 1 >= m_widths.Length)
        {
            throw new System.IndexOutOfRangeException(nameof(colSpan));
        }

        float left = (col == 0) ? Container.xMin : m_widths[col - 1];
        float top = (row == 0) ? Container.yMin : m_heights[row - 1];
        float width = m_widths[col + colSpan - 1] - left;
        float height = m_heights[row + rowSpan - 1] - top;

        return new Rect(left, top, width, height);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public RectLayout(Rect container, string columns = "*", string rows = "*")
    {
        Container = container;

        if (string.IsNullOrWhiteSpace(columns))
        {
            throw new System.NullReferenceException(nameof(columns));
        }

        if (string.IsNullOrWhiteSpace(rows))
        {
            throw new System.NullReferenceException(nameof(rows));
        }

        m_widths = Parse(columns, Container.xMin, Container.xMax);
        m_heights = Parse(rows, Container.yMin, Container.yMax);

        ColCount = (uint)m_widths.Length;
        RowCount = (uint)m_heights.Length;

        Assert.IsTrue(Mathf.Approximately(m_widths[m_widths.Length - 1], Container.xMax));
        Assert.IsTrue(Mathf.Approximately(m_heights[m_heights.Length - 1], Container.yMax));
    }

    private static float[] Parse(string text, float min, float max)
    {
        var arr = text.Split(',');

        if (arr.Length == 0)
        {
            throw new System.FormatException("No data");
        }

        float[] res = new float[arr.Length];
        string val;
        int index;
        float floatVal;
        float usedSize = 0;
        float percentSum = 0;
        float size = max - min;

        if (size < 0)
        {
            throw new System.FormatException("Negative size");
        }

        // Parse values
        for (int i = 0; i < arr.Length; i++)
        {
            val = arr[i].Trim();

            if (true == float.TryParse(val, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out floatVal))
            {
                floatVal = Mathf.Min(floatVal, Mathf.Max(0, size - usedSize));
                res[i] = floatVal;
                usedSize += floatVal;
                continue;
            }
            else if (false == string.IsNullOrEmpty(val))
            {
                index = val.IndexOf('*');
                if ((-1 != index) && (index == val.Length - 1))
                {
                    if (0 == index)
                    {
                        res[i] = -1;
                        percentSum += 1;
                        continue;
                    }
                    else if ((true == float.TryParse(val.Substring(0, index), NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out floatVal)) && (floatVal > 0))
                    {
                        res[i] = -floatVal;
                        percentSum += floatVal;
                        continue;
                    }
                }
            }

            throw new System.FormatException("Invalid format");
        }

        float sizeLeft = Mathf.Max(0, size - usedSize);
        float left = min;

        // Rescale *
        for (int i = 0; i < res.Length; i++)
        {
            if (res[i] < 0)
            {
                res[i] = (-res[i] / percentSum) * sizeLeft;
            }

            left += res[i];
            res[i] = left;
        }

        return res;
    }
}

