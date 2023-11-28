using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

using UnityEditor;
using UnityEngine;

// Helper Rect extension methods
public static class RectExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Rect ScaleSizeBy(this Rect rect, float scale, Vector2 pivotPoint)
    {
        Rect result = rect;
        result.x -= pivotPoint.x;
        result.y -= pivotPoint.y;
        result.xMin *= scale;
        result.xMax *= scale;
        result.yMin *= scale;
        result.yMax *= scale;
        result.x += pivotPoint.x;
        result.y += pivotPoint.y;
        return result;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Rect ScaleSizeBy(this Rect rect, Vector2 scale, Vector2 pivotPoint)
    {
        Rect result = rect;
        result.x -= pivotPoint.x;
        result.y -= pivotPoint.y;
        result.xMin *= scale.x;
        result.xMax *= scale.x;
        result.yMin *= scale.y;
        result.yMax *= scale.y;
        result.x += pivotPoint.x;
        result.y += pivotPoint.y;
        return result;
    }
}

public class EditorZoomer
{
    private const float kEditorWindowTabHeight = 21.0f;

    public float zoom = 1f;

    public Rect zoomArea = new Rect();
    public Vector2 zoomOrigin = Vector2Int.zero;

    //Vector2 lastMouse = Vector2.zero;
    Matrix4x4 prevMatrix;

    public Rect Begin(Rect zoomArea, out bool isChanged)
    {
        if (Event.current.rawType != EventType.Layout)
        {
            this.zoomArea = zoomArea;
        }

        Rect clippedArea = zoomArea.ScaleSizeBy(1f / zoom, zoomArea.min);

        HandleEvents(zoomArea, out isChanged);

        //fill the available area
        //var possibleZoomArea = GUILayoutUtility.GetRect(0, 10000, 0, 10000, options);

        /*
        if (Event.current.type == EventType.Repaint) //the size is correct during repaint, during layout it's 1,1
        {
            zoomArea = possibleZoomArea;
        }
        */
        GUI.EndGroup(); // End the group Unity begins automatically for an EditorWindow to clip out the window tab. This allows us to draw outside of the size of the EditorWindow.

        clippedArea.y += kEditorWindowTabHeight;
        GUI.BeginGroup(clippedArea);

        prevMatrix = GUI.matrix;
        Matrix4x4 translation = Matrix4x4.TRS(clippedArea.min, Quaternion.identity, Vector3.one);
        Matrix4x4 scale = Matrix4x4.Scale(new Vector3(zoom, zoom, 1.0f));
        GUI.matrix = translation * scale * translation.inverse * GUI.matrix;

        return clippedArea;
    }

    public void End()
    {
        GUI.matrix = prevMatrix; //restore the original matrix
        GUI.EndGroup();
        GUI.BeginGroup(new Rect(0.0f, kEditorWindowTabHeight, Screen.width, Screen.height));
    }

    private bool m_dragging;

    public const float ZoomMin = 0.25f;
    public const float ZoomMax = 2f;

    public void HandleEvents(Rect clippedArea, out bool isChanged)
    {
        //clippedArea.Enlarge(-2).Draw(Color.red);
        isChanged = false;

        if (Event.current.isMouse)
        {
            if (m_dragging && Event.current.rawType == EventType.MouseDrag && Event.current.button == 2)
            {
                var mouseDelta = Event.current.delta;

                zoomOrigin += Vector2Int.RoundToInt(mouseDelta / zoom);
                Event.current.Use();
            }

            if (false == clippedArea.Contains(Event.current.mousePosition, true))
            {
                return;
            }

            if (Event.current.rawType == EventType.MouseDown && Event.current.button == 2)
            {
                m_dragging = true;
                Event.current.Use();
            }

            if (m_dragging && Event.current.rawType == EventType.MouseUp && Event.current.button == 2)
            {
                m_dragging = false;
                isChanged = true;
                Event.current.Use();
            }
        }

        if (Event.current.rawType == EventType.ScrollWheel)
        {
            if (false == clippedArea.Contains(Event.current.mousePosition, true))
            {
                return;
            }

            float oldZoom = zoom;

            float zoomChange = 1.10f;

            zoom *= Mathf.Pow(zoomChange, -Event.current.delta.y / 3f);
            zoom = Mathf.Clamp(zoom, ZoomMin, ZoomMax);

            //we want the same content that was under the mouse pre-zoom to be there post-zoom as well
            //in other words, the content's position *relative to the mouse* should not change

            Vector2 mousePos = Event.current.mousePosition - zoomArea.min;
            zoomOrigin = Vector2Int.RoundToInt((mousePos - (mousePos - (Vector2)zoomOrigin * oldZoom) * zoom / oldZoom) / zoom);
            isChanged = true;
        }
    }
}
