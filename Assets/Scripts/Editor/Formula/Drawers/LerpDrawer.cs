using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

using UnityEditor;

using UnityEngine;

public class LerpDrawer : BaseDrawer
{
    public override string HelpMessage => m_helpMessage;

    private const string m_helpMessage = "[<color=orange>Lerp</color>] linearly interpolates between the first and the second input slot\n" +
        "  values with delta value provided with the third input slot.\n" +
        "Underlying formula will be like\n" +
        "        res = a + delta * (b - a)\n" +
        "where 'a' is the first input slot parameter,\n" +
        "      'b' is the second input slot parameter and\n" +
        "      'delta' is the third input slot parameter.";

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public LerpDrawer(FormulaModLerp mod)
        : base(mod) { }

    static LerpDrawer()
    {
        Register(typeof(FormulaModLerp), mod => new LerpDrawer((FormulaModLerp)mod));
    }

    private Vector2 m_space;
    private Vector2 m_fieldSize;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected override void DrawBodyGUI(InputState state)
    {
        base.DrawBodyGUI(state);

        Vector2 textSizeValueMin = GetLabelSize("Min");
        Vector2 textSizeValueMax = GetLabelSize("Max");

        float maxLabelWidth = Mathf.Max(textSizeValueMin.x, textSizeValueMax.x);

        EditorGUIUtility.labelWidth = maxLabelWidth;

        m_fieldSize = new Vector2(BodyArea.width - m_space.x * 2, m_fieldSize.y);

        var m = (mod as FormulaModLerp);

        if (mod.Inputs[1].Link != null)
        {
            GUI.enabled = false;
            EditorGUI.TextField(new Rect(m_space.x, m_space.y * 1 + m_fieldSize.y * 0, m_fieldSize.x, m_fieldSize.y), "Min", "input");
            GUI.enabled = true;
        }
        else
        {
            m.Min = EditorGUI.FloatField(new Rect(m_space.x, m_space.y * 1 + m_fieldSize.y * 0, m_fieldSize.x, m_fieldSize.y), "Min", m.Min);
        }

        if (mod.Inputs[2].Link != null)
        {
            GUI.enabled = false;
            EditorGUI.TextField(new Rect(m_space.x, m_space.y * 2 + m_fieldSize.y * 1, m_fieldSize.x, m_fieldSize.y), "Max", "input");
            GUI.enabled = true;
        }
        else
        {
            m.Max = EditorGUI.FloatField(new Rect(m_space.x, m_space.y * 2 + m_fieldSize.y * 1, m_fieldSize.x, m_fieldSize.y), "Max", m.Max);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected override Vector2 GetBodySize()
    {
        m_space = new Vector2(3, 4);

        m_fieldSize = new Vector2(104, 20);

        return new Vector2(
            m_fieldSize.x + m_space.x * 2,
            m_fieldSize.y * 2 + m_space.y * 3);
    }
}
