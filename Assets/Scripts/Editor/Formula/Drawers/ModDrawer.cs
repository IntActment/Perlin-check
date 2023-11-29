using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

using UnityEditor;

using UnityEngine;

public class PopupMessage : PopupWindowContent
{
    public static void Show(string message, float zoom)
    {
        Matrix4x4 previousMatrix = GUI.matrix;
        GUI.matrix = GUI.matrix * Matrix4x4.Scale(new Vector3(1 / zoom, 1 / zoom, 1));

        PopupWindow.Show(new Rect(Event.current.mousePosition, Vector2.zero), new PopupMessage(message));

        GUI.matrix = previousMatrix;
    }

    public static void ShowCode(string code, float zoom)
    {
        Matrix4x4 previousMatrix = GUI.matrix;
        GUI.matrix = GUI.matrix * Matrix4x4.Scale(new Vector3(1 / zoom, 1 / zoom, 1));

        PopupWindow.Show(new Rect(Event.current.mousePosition, Vector2.zero), new PopupMessage(code, true));

        GUI.matrix = previousMatrix;
    }

    private string m_message;
    private Vector2 m_size;
    private GUIStyle m_style;
    private bool m_isCode;

    public override void OnGUI(Rect rect)
    {
        if (m_isCode)
        {
            EditorGUI.SelectableLabel(rect.Enlarge(-4), m_message, m_style);
        }
        else
        {
            EditorGUI.LabelField(rect.Enlarge(-4), m_message, m_style);
        }
    }

    private Texture2D MakeTex(int width, int height, Color col)
    {
        Color[] pix = new Color[width * height];
        for (int i = 0; i < pix.Length; ++i)
        {
            pix[i] = col;
        }
        Texture2D result = new Texture2D(width, height);
        result.SetPixels(pix);
        result.Apply();

        return result;
    }

    private PopupMessage(string message, bool isCode = false)
    {
        m_isCode = isCode;

        m_message = message;
        m_style = new GUIStyle(EditorStyles.label);

        if (isCode)
        {
            m_style = new GUIStyle(EditorStyles.label);
            m_style.normal.background = MakeTex(1, 1, Color.white);
            m_style.normal.textColor = Color.black;
            m_style.font = Font.CreateDynamicFontFromOSFont("Consolas", 10);
        }

        m_style.wordWrap = true;
        m_style.richText = true;
        m_size = m_style.CalcSize(new GUIContent(m_message));

        if (m_size.x > 600)
        {
            m_size.x = 600;
            m_size.y = m_style.CalcHeight(new GUIContent(m_message), 600);
        }

        m_size += Vector2.one * 8;
    }

    public override Vector2 GetWindowSize()
    {
        return m_size;
    }
}

public abstract class ModDrawer<T> where T : FormulaMod
{
    public readonly T mod;

    public EditorWindow Window { get; set; }

    public abstract string HelpMessage {get;}

    public static FormulaMod DraggingMod { get; private set; }

    public static Vector2? SelectionStart { get; private set; }

    public static Dictionary<FormulaMod, Vector2> Selected { get; } = new Dictionary<FormulaMod, Vector2>();

    private static Vector2 m_draggingModOffset;
    private static bool m_isConnecting = false;
    private static (FormulaMod, FormulaSocketType, int) m_connectingWith;

    public virtual bool HasInput { get; } = true;
    public virtual bool HasOutput { get; } = true;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected ModDrawer(T mod)
    {
        this.mod = mod;
    }

    public static readonly Vector2 SocketSize = Vector2.one * 14;
    public static readonly float TitleHeight = 23;
    public static readonly float SocketRadius = 6;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected Vector2 GetSocketCenter(FormulaSocketType socketType, int index)
    {
        Rect area = socketType == FormulaSocketType.In
            ? InputArea
            : OutputArea;

        if (-1 == index)
        {
            index = mod.Outputs.Count;
        }

        return new Vector2(area.x + area.width / 2f, area.y + index * SocketSize.y + SocketSize.y / 2f);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private Rect GetSocketArea(FormulaSocketType socketType, int index)
    {
        return new Rect(GetSocketCenter(socketType, index) - Vector2.one * SocketRadius, Vector2.one * SocketRadius * 2);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected virtual Vector2 GetTitleMinSize()
    {
        return GUI.skin.label.CalcSize(new GUIContent(GetTitleText()));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private string GetTitleText()
    {
        return mod.VarPrefix == null
            ? mod.name
            : $"{mod.name} [{mod.VarIndex}]";
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected static Vector2 GetLabelSize(string text)
    {
        return GUI.skin.label.CalcSize(new GUIContent(text));
    }

    public Rect WindowArea => new Rect(mod.Position, WindowSize);

    public Vector2 WindowSize { get; private set; } = default;
    public Vector2 ClientSize { get; private set; } = default;
    public Rect InputArea { get; private set; } = default;
    public Rect OutputArea { get; private set; } = default;
    public Rect HeaderArea { get; private set; } = default;
    public Rect BodyArea { get; private set; } = default;
    public Rect HeaderLabelPlace { get; private set; } = default;
    public Rect HelpBtnArea { get; private set; } = default;
    public RectOffset Border { get; private set; } = default;

    // <image src="$(ProjectDir)Docs/ModDrawer scheme.png" />

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void RecalculateSize()
    {
        static float RoundX(float val)
        {
            return Mathf.Ceil((val) / SocketSize.x) * SocketSize.x;
        }

        // window style's border
        Border = m_windowStyle.border;

        var inputSize = Vector2.zero;
        var outputSize = Vector2.zero;
        var headerTextSize = GetTitleMinSize();
        var textSize = headerTextSize;
        var titleMargin = new RectOffset(5, 5, 4, 4);
        float helpLeftMargin = 2;
        float titleHeight = Mathf.Max(Border.top, titleMargin.top + headerTextSize.y + titleMargin.bottom);
        var helpSize = Vector2.one * (titleHeight - titleMargin.top - titleMargin.bottom);
        var headerSize = new Vector2(titleMargin.left + headerTextSize.x + helpLeftMargin + helpSize.x + titleMargin.right, titleHeight);

        // fix snapping
        float newHeaderWidth = RoundX(headerSize.x);
        headerTextSize.x += newHeaderWidth - headerSize.x;
        headerSize.x = newHeaderWidth;

        if (true == HasInput)
        {
            inputSize = new Vector2(SocketSize.x, SocketSize.y * mod.Inputs.Count);
        }

        if (true == HasOutput)
        {
            outputSize = new Vector2(SocketSize.x, SocketSize.y * (mod.Outputs.Count + 1));
        }

        var bodySize = mod.IsInitialized
            ? GetBodySize()
            : Vector2.zero;
        float windowWidth = Border.left + inputSize.x + bodySize.x + outputSize.x + Border.right;
        float clientHeight = Mathf.Max(inputSize.y, bodySize.y, outputSize.y);

        WindowSize = new Vector2(Mathf.Max(windowWidth, headerSize.x), headerSize.y + clientHeight + Border.bottom);
        WindowSize = Vector2Int.CeilToInt((WindowSize) / SocketSize) * SocketSize;
        headerSize.x = Mathf.Max(headerSize.x, WindowSize.x);
        HeaderArea = new Rect(Vector2.zero, headerSize);
        HelpBtnArea = new Rect(HeaderArea.xMax - titleMargin.right - helpSize.x, titleMargin.top, helpSize.x, helpSize.y);
        HeaderLabelPlace = new Rect(HeaderArea.min.x + titleMargin.left, titleMargin.top, headerTextSize.x, headerSize.y - titleMargin.vertical)
            .center.MakeRectFromCenter(textSize);

        ClientSize = new Vector2(WindowSize.x - Border.left - Border.right, WindowSize.y - Border.bottom + headerSize.y);

        InputArea = new Rect(Border.left, headerSize.y, inputSize.x, ClientSize.y);
        OutputArea = new Rect(WindowSize.x - Border.right - outputSize.x, headerSize.y, outputSize.x, ClientSize.y);
        BodyArea = new Rect(InputArea.xMax, headerSize.y, OutputArea.xMin - InputArea.xMax, ClientSize.y);
    }

    private GUIStyle m_windowStyle;
    private GUIStyle m_buttonStyle;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void CalculateLayout(GUISkin skin)
    {
        string sel = Selected.ContainsKey(mod)
            ? " sel"
            : string.Empty;

        switch (mod)
        {
            case FormulaModInputX:
            case FormulaModInputY:
                m_windowStyle = skin.FindStyle($"win in{sel}");
                break;
            case FormulaModOutput:
                m_windowStyle = skin.FindStyle($"win out{sel}");
                break;
            default:
                m_windowStyle = skin.FindStyle($"win default{sel}");
                break;
        }

        m_buttonStyle = skin.button;

        RecalculateSize();
    }

    public bool HasActiveInput { get; set; } = false;

    public void DrawGUI(KeyHolder keys, ref bool down, ref bool up, Vector2 position, float zoom)
    {
        var ev = Event.current;

        {
            switch (ev.rawType)
            {
                case EventType.MouseUp:

                    if (ev.button == 0)
                    {
                        if (mod == DraggingMod)
                        {
                            DraggingMod.Save();
                            DraggingMod = null;
                            ev.Use();
                        }
                    }

                    break;

                case EventType.MouseDrag:

                    if (mod == DraggingMod)
                    {
                        foreach (var kv in Selected)
                        {
                            Vector2 pos = Vector2Int.FloorToInt((ev.mousePosition - m_draggingModOffset + kv.Value - position) / SocketSize) * SocketSize;
                            kv.Key.Position = pos;
                        }
                        ev.Use();
                    }
                    break;
            }
        }

        GUI.BeginGroup(new Rect(mod.Position + position, WindowSize), m_windowStyle);
        {
            // title
            GUI.Label(HeaderLabelPlace, GetTitleText());
            //EditorGUIUtility.AddCursorRect(HeaderArea, MouseCursor.Pan);

            // help
            if (GUI.Button(HelpBtnArea, "?", m_buttonStyle))
            {
                PopupMessage.Show(HelpMessage, zoom);
                //Window.ShowNotification(new GUIContent(HelpMessage));
            }

            //HeaderArea.DrawFrame(Color.red);
            //HelpBtnArea.DrawFrame(Color.blue);
            //HeaderLabelPlace.DrawFrame(Color.yellow);

            if (true == HasInput)
            {
                for (int i = 0; i < mod.Inputs.Count; i++)
                {
                    var area = GetSocketArea(FormulaSocketType.In, i);

                    GUI.Box(area, new GUIContent("", mod.Inputs[i].Title), GUI.skin.customStyles[97]);
                    //EditorGUIUtility.AddCursorRect(area, MouseCursor.Pan);

                    if ((ev.button == 0) && area.Contains(ev.mousePosition))
                    {
                        if (false == down)
                        {
                            if (ev.rawType == EventType.MouseDown)
                            {
                                down = true;
                                m_isConnecting = true;

                                if (null == mod.Inputs[i].Link)
                                {
                                    m_connectingWith = (mod, FormulaSocketType.In, i);
                                    //Debug.Log("Link start: in");
                                }
                                else
                                {
                                    m_connectingWith = (mod.Inputs[i].Link.Owner, FormulaSocketType.Out, mod.Inputs[i].Link.Owner.GetSocketIndex(mod.Inputs[i].Link));
                                    mod.ClearInput(mod.Inputs[i]);
                                    //Debug.Log("Link start: unbind in");
                                }
                            }
                        }

                        if (false == up)
                        {
                            if (ev.rawType == EventType.MouseUp)
                            {
                                up = true;

                                if (true == m_isConnecting)
                                {
                                    if (m_connectingWith.Item2 == FormulaSocketType.Out)
                                    {
                                        if ((m_connectingWith.Item3 == -1) && (m_connectingWith.Item1.AddOutput(mod.Inputs[i])))
                                        {
                                            m_isConnecting = false;
                                            //Debug.Log("Link complete: in (output+)");
                                        }
                                        else if ((m_connectingWith.Item3 > -1) && (m_connectingWith.Item1.ReplaceOutput(m_connectingWith.Item3, mod.Inputs[i])))
                                        {
                                            m_isConnecting = false;
                                            //Debug.Log("Link complete: in (output=)");
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            GUI.BeginClip(BodyArea);
            {
                Event old = null;

                if (false == HasActiveInput && ev.rawType == EventType.MouseDown && new Rect(Vector2.zero, BodyArea.size).Contains(ev.mousePosition))
                {
                    // block mouse events for inactive controls to prevent events be passed through the topmost window
                    old = ev;
                    
                    ev = new Event(ev);
                    ev.Use();
                    Event.current = ev;
                }

                DrawBodyGUI(ref down, ref up);

                if (old != null)
                {
                    // restore event
                    Event.current = old;
                    ev = old;
                }
            }
            GUI.EndClip();

            if (true == HasOutput)
            {
                for (int i = 0; i <= mod.Outputs.Count; i++)
                {
                    var area = GetSocketArea(FormulaSocketType.Out, i);

                    GUI.Box(area, "", GUI.skin.customStyles[97]);
                    //EditorGUIUtility.AddCursorRect(area, MouseCursor.Pan);

                    if (i == mod.Outputs.Count)
                    {
                        var style2 = GUI.skin.customStyles[489];
                        Rect place = new Rect(area.x + style2.fixedWidth / 2, area.y + style2.fixedWidth / 2, style2.fixedWidth, style2.fixedHeight);
                        GUI.Box(place, "", style2);
                    }

                    if ((ev.button == 0) && area.Contains(ev.mousePosition))
                    {
                        if (false == down)
                        {
                            if (ev.rawType == EventType.MouseDown)
                            {
                                down = true;
                                m_isConnecting = true;

                                if (i < mod.Outputs.Count)
                                {
                                    m_connectingWith = (mod.Outputs[i].Link.Owner, FormulaSocketType.In, mod.Outputs[i].Link.Owner.GetSocketIndex(mod.Outputs[i].Link));
                                    mod.RemoveOutput(mod.Outputs[i]);
                                    //Debug.Log("Link start: removed out");
                                }
                                else
                                {
                                    m_connectingWith = (mod, FormulaSocketType.Out, -1);
                                    //Debug.Log("Link start: add out");
                                }
                            }
                        }

                        if (false == up)
                        {
                            if (ev.rawType == EventType.MouseUp)
                            {
                                up = true;

                                if (true == m_isConnecting)
                                {
                                    if (m_connectingWith.Item2 == FormulaSocketType.In)
                                    {
                                        if ((i == mod.Outputs.Count) && (mod.AddOutput(m_connectingWith.Item1.Inputs[m_connectingWith.Item3])))
                                        {
                                            m_isConnecting = false;
                                            //Debug.Log("Link complete: (output+)");
                                        }
                                        else if ((i < mod.Outputs.Count) && (mod.ReplaceOutput(i, m_connectingWith.Item1.Inputs[m_connectingWith.Item3])))
                                        {
                                            m_isConnecting = false;
                                            //Debug.Log("Link complete: (output=)");
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            if ((false == down) && HasActiveInput)
            {
                if ((ev.rawType == EventType.MouseDown) && (ev.button == 0))
                {
                    // title click
                    if (HeaderArea.Contains(ev.mousePosition))
                    {
                        down = true;
                        DraggingMod = mod;
                        m_draggingModOffset = ev.mousePosition;

                        if (keys.IsCtrlPressed)
                        {
                            Selected.Remove(mod);
                        }
                        else
                        {
                            if (false == keys.IsShiftPressed)
                            {
                                if (false == Selected.ContainsKey(mod))
                                {
                                    Selected.Clear();
                                }
                            }

                            foreach (var kv in Selected.ToList())
                            {
                                Selected[kv.Key] = kv.Key.Position - mod.Position;
                            }

                            Selected[mod] = Vector2.zero;
                        }

                        ev.Use();
                    }
                    // window click
                    else if (new Rect(Vector2.zero, WindowSize).Contains(ev.mousePosition))
                    {
                        down = true;

                        if (keys.IsCtrlPressed)
                        {
                            Selected.Remove(mod);
                        }
                        else
                        {
                            if (false == keys.IsShiftPressed)
                            {
                                if (false == Selected.ContainsKey(mod))
                                {
                                    Selected.Clear();
                                }
                            }

                            Selected[mod] = Vector2.zero;
                        }

                        ev.Use();
                    }
                }

                if ((ev.rawType == EventType.MouseDown) && (ev.button == 1))
                {
                    if (new Rect(Vector2.zero, WindowSize).Contains(ev.mousePosition))
                    {
                        down = true;
                        ev.Use();

                        GenericMenu menu = new GenericMenu();

                        if (mod.IsRemovable)
                        {
                            menu.AddItem(new GUIContent("Remove"), false, () =>
                            {
                                if (Selected.ContainsKey(mod))
                                {
                                    foreach (var m in Selected)
                                    {
                                        mod.Formula.Delete(m.Key);
                                    }

                                    Selected.Clear();
                                }

                                mod.Formula.Delete(mod);

                            });
                        }
                        else
                        {
                            menu.AddDisabledItem(new GUIContent("Remove"), false);
                        }

                        {
                            // ShowAsContext does not respect out zoom, so we have to hack its position
                            Matrix4x4 previousMatrix = GUI.matrix;
                            GUI.matrix = GUI.matrix * Matrix4x4.Scale(new Vector3(1 / zoom, 1 / zoom, 1));

                            menu.ShowAsContext();

                            GUI.matrix = previousMatrix;
                        }
                    }
                }
            }
        }
        GUI.EndGroup();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void InvalidateConnection(Rect selectionArea, IDictionary<FormulaMod, BaseDrawer> dic, KeyHolder keys, bool down, Vector2 offset, float zoom)
    {
        var ev = Event.current;
        if ((ev.rawType == EventType.MouseUp) && (ev.button == 0))
        {
            m_isConnecting = false;
            //Debug.Log("Link canceled (all)");

            if (SelectionStart != null)
            {
                Rect selRectAbsolute = new Rect(SelectionStart.Value, ev.mousePosition - SelectionStart.Value);

                if (keys.IsCtrlPressed)
                {
                    foreach (var v in dic.Values)
                    {
                        Rect winRect = new Rect(v.mod.Position + offset, v.WindowSize);

                        if (selRectAbsolute.Overlaps(winRect, true))
                        {
                            Selected.Remove(v.mod);
                        }
                    }
                }
                else
                {
                    if (false == keys.IsShiftPressed)
                    {
                        Selected.Clear();
                    }

                    foreach (var v in dic.Values)
                    {
                        Rect winRect = new Rect(v.mod.Position + offset, v.WindowSize);

                        if (selRectAbsolute.Overlaps(winRect, true))
                        {
                            Selected[v.mod] = Vector2.zero;
                        }
                    }
                }

                //Debug.Log("Selection end");
                SelectionStart = null;
            }
        }

        if ((ev.rawType == EventType.MouseDown) && (ev.button == 0))
        {
            if ((false == keys.IsShiftPressed) && (false == keys.IsCtrlPressed))
            {
                Selected.Clear();
            }

            if ((false == down) && selectionArea.Contains(ev.mousePosition, true))
            {
                //Debug.Log("Selection start");
                SelectionStart = ev.mousePosition;
                ev.Use();
            }
        }

        if (Event.current.rawType == EventType.Repaint)
        {
            if (SelectionStart != null)
            {
                {
                    Rect selRectAbsolute = new Rect(SelectionStart.Value, ev.mousePosition - SelectionStart.Value);

                    foreach (var v in dic.Values)
                    {
                        Rect winRect = new Rect(v.mod.Position + offset, v.WindowSize);

                        if (selRectAbsolute.Overlaps(winRect, true))
                        {
                            if (keys.IsCtrlPressed)
                            {
                                if (Selected.ContainsKey(v.mod))
                                {
                                    // will be removed from selection
                                    winRect.Enlarge(1).DrawSolidRect(new Color(0.8f, 0.1f, 0, 0.25f));
                                }
                            }
                            else if (keys.IsShiftPressed)
                            {
                                if (Selected.ContainsKey(v.mod))
                                {
                                    // already selected
                                    winRect.Enlarge(1).DrawSolidRect(new Color(0.8f, 0.6f, 0, 0.25f));
                                }
                                else
                                {
                                    // will be added to selection
                                    winRect.Enlarge(1).DrawSolidRect(new Color(0.1f, 0.8f, 0, 0.25f));
                                }
                            }
                            else
                            {
                                // will be added to selection
                                winRect.Enlarge(1).DrawSolidRect(new Color(0.1f, 0.8f, 0, 0.25f));
                            }
                        }
                    }
                }

                Vector2 p1 = SelectionStart.Value;
                Vector2 p2 = ev.mousePosition;

                p2 = Vector2.Min(selectionArea.max - Vector2.one, Vector2.Max(p2, selectionArea.min));

                Handles.color = new Color(0.25f, 0.25f, 0, 0.15f);
                Handles.DrawAAConvexPolygon(
                    new Vector2(p1.x, p1.y),
                    new Vector2(p1.x, p2.y),
                    new Vector2(p2.x, p2.y),
                    new Vector2(p2.x, p1.y),
                    new Vector2(p1.x, p1.y));

                Handles.color = new Color(0.55f, 0.55f, 0, 0.35f);
                Handles.DrawPolyLine(
                    new Vector2(p1.x, p1.y),
                    new Vector2(p1.x, p2.y),
                    new Vector2(p2.x, p2.y),
                    new Vector2(p2.x, p1.y),
                    new Vector2(p1.x, p1.y));
            }

            if (true == m_isConnecting)
            {
                var drawer = dic[m_connectingWith.Item1];

                if (m_connectingWith.Item2 == FormulaSocketType.In)
                {

                    DrawLink(
                        ev.mousePosition - offset,
                        m_connectingWith.Item1.Position + drawer.GetSocketCenter(FormulaSocketType.In, m_connectingWith.Item3),
                        offset);
                }
                else
                {
                    DrawLink(
                        m_connectingWith.Item1.Position + drawer.GetSocketCenter(FormulaSocketType.Out, m_connectingWith.Item3),
                        ev.mousePosition - offset,
                        offset);
                }
            }
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void DrawLinks(IDictionary<FormulaMod, BaseDrawer> dic, Vector2 offset)
    {
        if (Event.current.rawType != EventType.Repaint)
        {
            return;
        }

        for (int j = 0; j < mod.Outputs.Count; j++)
        {
            DrawLink(
                mod.Position + GetSocketCenter(FormulaSocketType.Out, j),
                mod.Outputs[j].Link.Owner.Position + dic[mod.Outputs[j].Link.Owner].GetSocketCenter(FormulaSocketType.In, mod.Outputs[j].Link.Owner.GetSocketIndex(mod.Outputs[j].Link)),
                offset);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void DrawLink(Vector2 pointOut, Vector2 pointIn, Vector2 offset)
    {
        Handles.DrawSolidRectangleWithOutline((offset + pointOut).MakeRectFromCenter(SocketRadius / 2f), new Color(0, 0.5f, 0, 1), new Color(0.1f, 0.05f, 0.05f, 0.5f));
        Handles.DrawSolidRectangleWithOutline((offset + pointIn).MakeRectFromCenter(SocketRadius / 2f), new Color(0, 0.5f, 0, 1), new Color(0.1f, 0.05f, 0.05f, 0.5f));

        //Handles.color = new Color(0, 0.5f, 0, 1);

        //Handles.DrawSolidDisc(offset + pointIn, Vector3.forward, 1.5f);
        //Handles.DrawSolidDisc(offset + pointOut, Vector3.forward, 1.5f);


        Handles.DrawBezier(
            offset + pointOut,
            offset + pointIn,
            offset + pointOut + Vector2.right * 40,
            offset + pointIn + Vector2.left * 40,
            new Color(0, 0.5f, 0, 1),
            null,
            3);

        Handles.DrawBezier(
            offset + pointOut,
            offset + pointIn,
            offset + pointOut + Vector2.right * 40,
            offset + pointIn + Vector2.left * 40,
            Color.green,
            null,
            1);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected virtual void DrawBodyGUI(ref bool down, ref bool up)
    {

    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected abstract Vector2 GetBodySize();
}
