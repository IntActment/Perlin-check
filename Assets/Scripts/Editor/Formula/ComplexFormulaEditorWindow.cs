using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

using UnityEditor;

using UnityEngine;

public class ComplexFormulaEditorWindow : EditorWindow
{
    public GUISkin skin;
    private static ComplexFormula m_formula;
    private KeyHolder m_keys = new KeyHolder();
    private Texture2D m_backgroundGrid;
    private GUIStyle m_backgroundGridStyle;

    private EditorZoomer m_zoomer;

    // Add menu named "My Window" to the Window menu
    [MenuItem("Window/ComplexFormula editor")]
    public static async void ShowEditor()
    {
        ComplexFormulaEditorWindow window = GetWindow<ComplexFormulaEditorWindow>();

        await window.Enable();

        if (null == m_formula)
        {
            return;
        }

        window.Show();
    }

    private async Task Enable()
    {
        titleContent = new GUIContent("Formula graph editor");

        m_formula = Selection.activeObject as ComplexFormula;
        if (null == m_formula)
        {
            return;
        }

        bool showingProgress = false;
        if (false == m_formula.IsInitialized)
        {
            showingProgress = true;
            EditorUtility.DisplayProgressBar("Initialization in progress", "Please wait...", 80);
        }

        await m_formula.WaitInit();

        wantsMouseMove = true;
        autoRepaintOnSceneChange = true;
        m_zoomer = new EditorZoomer();
        m_backgroundGrid = new Texture2D((int)BaseDrawer.SocketSize.x, (int)BaseDrawer.SocketSize.y, TextureFormat.RGBA32, false, true);

        Color[] colors = new Color[(int)BaseDrawer.SocketSize.x * (int)BaseDrawer.SocketSize.y];

        for (int i = 0; i < (int)BaseDrawer.SocketSize.x; i++)
        {
            colors[i] = new Color(1, 1, 1, 0.02f);
            colors[(int)BaseDrawer.SocketSize.x * i] = new Color(1, 1, 1, 0.02f);
        }

        m_backgroundGrid.SetPixels(colors);
        m_backgroundGrid.alphaIsTransparency = true;
        m_backgroundGrid.filterMode = FilterMode.Bilinear;
        m_backgroundGrid.wrapMode = TextureWrapMode.Repeat;
        m_backgroundGrid.Apply();

        m_backgroundGridStyle = new GUIStyle
        {
            stretchHeight = true,
            stretchWidth = true,
            
        };
        m_backgroundGridStyle.normal.background = m_backgroundGrid;

        if (true == showingProgress)
        {
            EditorUtility.ClearProgressBar();
        }
    }

    private void OnDisable()
    {
        BaseDrawer.Selected.Clear();
        DestroyImmediate(m_backgroundGrid);
        m_backgroundGrid = null;
    }

    private Vector2 GetCenter()
    {
        return (m_zoomer.zoomArea.center - m_zoomer.zoomArea.min) / m_formula.Zoom - m_formula.ScreenOffset;
    }

    async void OnGUI()
    {
        if (null == m_formula)
        {
            return;
        }

        if (false == m_formula.IsInitialized)
        {
            return;
        }

        bool needRepaint = m_keys.UpdateState();
        var ev = Event.current;

        try
        {
            if ((null != ev) && (ev.isMouse))
            {
                needRepaint = true;
            }

            m_zoomer.zoomOrigin = m_formula.ScreenOffset;
            m_zoomer.zoom = m_formula.Zoom;

            using (_ = new EditorLayout(LayoutType.Horizontal, GUI.skin.customStyles[62], GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true)))
            {
                using (_ = new EditorLayout(LayoutType.Vertical, GUI.skin.customStyles[48], GUILayout.Width(180), GUILayout.ExpandHeight(true)))
                {
                    GUILayout.Box("Operators", GUI.skin.customStyles[202], GUILayout.ExpandWidth(true));

                    using (_ = new EditorLayout(LayoutType.Vertical, GUI.skin.customStyles[116]))
                    {
                        using (_ = new EditorLayout(LayoutType.Horizontal))
                        {
                            if (GUILayout.Button("Simplex01"))
                            {
                                m_formula.CreateMod<FormulaModSimplex01>(GetCenter());
                            }

                            if (GUILayout.Button("Number"))
                            {
                                m_formula.CreateMod<FormulaModNumber>(GetCenter());
                            }

                            if (GUILayout.Button("Clamp"))
                            {
                                m_formula.CreateMod<FormulaModClamp>(GetCenter());
                            }
                        }

                        using (_ = new EditorLayout(LayoutType.Horizontal))
                        {
                            if (GUILayout.Button("Lerp"))
                            {
                                m_formula.CreateMod<FormulaModLerp>(GetCenter());
                            }

                            if (GUILayout.Button("Norm01"))
                            {
                                m_formula.CreateMod<FormulaModNorm01>(GetCenter());
                            }

                            if (GUILayout.Button("1 - a"))
                            {
                                m_formula.CreateMod<FormulaModOneMinus>(GetCenter());
                            }

                            if (GUILayout.Button("1 / a"))
                            {
                                m_formula.CreateMod<FormulaModInvert>(GetCenter());
                            }

                            if (GUILayout.Button("-a"))
                            {
                                m_formula.CreateMod<FormulaModNegate>(GetCenter());
                            }
                        }

                        using (_ = new EditorLayout(LayoutType.Horizontal))
                        {
                            if (GUILayout.Button("+"))
                            {
                                m_formula.CreateMod<FormulaModSum>(GetCenter());
                            }

                            if (GUILayout.Button("-"))
                            {
                                m_formula.CreateMod<FormulaModSubtract>(GetCenter());
                            }

                            if (GUILayout.Button("×"))
                            {
                                m_formula.CreateMod<FormulaModMultiply>(GetCenter());
                            }

                            if (GUILayout.Button("÷"))
                            {
                                m_formula.CreateMod<FormulaModDivide>(GetCenter());
                            }

                            if (GUILayout.Button("√a"))
                            {
                                m_formula.CreateMod<FormulaModSqrt>(GetCenter());
                            }

                            if (GUILayout.Button("a²"))
                            {
                                m_formula.CreateMod<FormulaModPow>(GetCenter());
                            }

                            if (GUILayout.Button("|a|"))
                            {
                                m_formula.CreateMod<FormulaModAbs>(GetCenter());
                            }
                        }
                    }

                    GUILayout.FlexibleSpace();

                    if (GUILayout.Button("Show code"))
                    {
                        var code = m_formula.GetCode();
                        PopupMessage.ShowCode(code, 1);
                    }

                    EditorGUILayout.HelpBox(new GUIContent("Hold shift to add to selection\nHold ctrl to remove from selection\nPress RMB to remove\n    Note: basic inputs and output\n    cannot be removed\nAll input sockets those has\n  no connection has default\n  input value = 0.0f"));
                }

                Rect m_lastActive = default;

                using (_ = new EditorLayout(LayoutType.Vertical, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true)))
                {
                    InputState state = new InputState();

                    Rect elRect;
                    using (var el = new EditorLayout(LayoutType.Vertical, GUI.skin.customStyles[114], GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true)))
                    {
                        elRect = el.Rect;

                        Dictionary<FormulaMod, BaseDrawer> drawersDic = new Dictionary<FormulaMod, BaseDrawer>();
                        List<BaseDrawer> drawersList = new List<BaseDrawer>(m_formula.Modifiers.Count);

                        m_zoomer.Begin(elRect, out bool isChanged);
                        {
                            if (ev.rawType == EventType.Repaint)
                            {
                                Vector2 cellsPerView = (elRect.size / BaseDrawer.SocketSize / m_zoomer.zoom);

                                Rect rep = new Rect(
                                    -m_formula.ScreenOffset / BaseDrawer.SocketSize,
                                    cellsPerView)
                                    .FlipY();

                                GUI.DrawTextureWithTexCoords(new Rect(Vector2.zero, elRect.size / m_zoomer.zoom), m_backgroundGrid, rep);
                            }

                            {
                                for (int i = 0; i < m_formula.Modifiers.Count; i++)
                                {
                                    var drawer = BaseDrawer.GetInstance(m_formula.Modifiers[i]);
                                    drawer.Window = this;
                                    drawersDic.Add(m_formula.Modifiers[i], drawer);
                                    drawersList.Add(drawer);
                                    drawer.CalculateLayout(skin);
                                }

                                if (ev.isMouse && ev.rawType == EventType.MouseDown)
                                {
                                    int oldIndex = -1;

                                    // decide which window has exclusive input
                                    for (int i = drawersList.Count - 1; i >= 0 ; i--)
                                    {
                                        m_lastActive = new Rect(drawersList[i].mod.Position + m_zoomer.zoomOrigin, drawersList[i].WindowSize);

                                        if (m_lastActive.Contains(ev.mousePosition))
                                        {
                                            drawersList[i].HasActiveInput = true;
                                            oldIndex = i;

                                            break;
                                        }
                                    }

                                    // pull to front active window
                                    if ((oldIndex != -1) && (oldIndex != drawersList.Count - 1))
                                    {
                                        var drawer = drawersList[oldIndex];
                                        drawersList.RemoveAt(oldIndex);
                                        drawersList.Add(drawer);

                                        m_formula.BringToFront(oldIndex);
                                    }
                                }

                                for (int i = 0; i < drawersList.Count; i++)
                                {
                                    drawersList[i].DrawGUI(m_keys, state, m_zoomer.zoomOrigin, m_zoomer.zoom);
                                }
                            }

                            foreach (var v in drawersDic.Values)
                            {
                                v.DrawLinks(drawersDic, m_zoomer.zoomOrigin);
                            }

                            BaseDrawer.InvalidateConnection(new Rect(Vector2.zero, elRect.size / m_zoomer.zoom), drawersDic, m_keys, state.down, m_zoomer.zoomOrigin);
                        }
                        m_zoomer.End();

                        m_formula.ScreenOffset = m_zoomer.zoomOrigin;
                        m_formula.Zoom = m_zoomer.zoom;

                        if (isChanged)
                        {
                            m_formula.Save();
                        }
                    }

                    using (_ = new Layout(LayoutType.Horizontal, GUI.skin.customStyles[361], GUILayout.ExpandWidth(true), GUILayout.Height(38)))
                    {
                        if (GUILayout.Button("Reset view", GUILayout.ExpandHeight(true)))
                        {
                            m_formula.ScreenOffset = Vector2Int.zero;
                            m_formula.Zoom = 1;
                            m_formula.Save();
                        }

                        if (GUILayout.Button("Reset zoom", GUILayout.ExpandHeight(true)))
                        {
                            var scaleOld = 1f / m_formula.Zoom * 100;
                            var newZoom = 1f;
                            if (newZoom != m_formula.Zoom)
                            {
                                Vector2 mousePos = m_zoomer.zoomArea.center - m_zoomer.zoomArea.min;
                                m_formula.ScreenOffset = Vector2Int.RoundToInt((mousePos - (mousePos - m_formula.ScreenOffset * m_formula.Zoom) * newZoom / m_formula.Zoom) / newZoom);
                                m_formula.Zoom = newZoom;
                                m_formula.Save();
                            }
                        }

                        using (_ = new Layout(LayoutType.Vertical, GUILayout.ExpandHeight(true)))
                        {
                            GUILayout.FlexibleSpace();

                            {
                                var scaleOld = 1f / m_formula.Zoom * 100;
                                var newZoom = 1f / (EditorGUILayout.Slider(scaleOld, 1f / EditorZoomer.ZoomMax * 100f, 1f / EditorZoomer.ZoomMin * 100f, GUILayout.Width(340)) / 100f);
                                if (newZoom != m_formula.Zoom)
                                {
                                    Vector2 mousePos = m_zoomer.zoomArea.center - m_zoomer.zoomArea.min;
                                    m_formula.ScreenOffset = Vector2Int.RoundToInt((mousePos - (mousePos - m_formula.ScreenOffset * m_formula.Zoom) * newZoom / m_formula.Zoom) / newZoom);
                                    m_formula.Zoom = newZoom;
                                    m_formula.Save();
                                }
                            }

                            GUILayout.FlexibleSpace();
                        }

                        GUILayout.FlexibleSpace();
                    }
                }
            }
        }
        finally
        {
            if ((ev.rawType != EventType.Repaint) && needRepaint)
            {
                Repaint();
            }
        }
    }
}
