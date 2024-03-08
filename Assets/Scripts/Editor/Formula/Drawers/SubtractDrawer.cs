using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

using UnityEditor;

using UnityEngine;

public class SubtractDrawer : BaseDrawer
{
    public override string HelpMessage => m_helpMessage;

    private const string m_helpMessage = "[<color=orange>a - b</color>] subtracts the second input slot value from the first input slot value.\n" +
        "Underlying formula will be like\n" +
        "        res = a - b\n" +
        "where 'a' is the first input slot parameter and\n" +
        "      'b' is the second input slot parameter.";

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public SubtractDrawer(FormulaModSubtract mod)
        : base(mod) { }

    static SubtractDrawer()
    {
        Register(typeof(FormulaModSubtract), mod => new SubtractDrawer((FormulaModSubtract)mod));
    }

    private Vector2 m_space;
    private Vector2 m_fieldSize;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected override void DrawBodyGUI(InputState state)
    {
        base.DrawBodyGUI(state);

        Vector2 textSizeValueMin = GetLabelSize("Subtrahend");
        Vector2 textSizeValueMax = GetLabelSize("Minuend");

        float maxLabelWidth = Mathf.Max(textSizeValueMin.x, textSizeValueMax.x);

        EditorGUIUtility.labelWidth = maxLabelWidth;

        m_fieldSize = new Vector2(BodyArea.width - m_space.x * 2, m_fieldSize.y);

        var m = (mod as FormulaModSubtract);

        if (mod.Inputs[0].Link != null)
        {
            GUI.enabled = false;
            EditorGUI.TextField(new Rect(m_space.x, m_space.y * 1 + m_fieldSize.y * 0, m_fieldSize.x, m_fieldSize.y), "Subtrahend", "input");
            GUI.enabled = true;
        }
        else
        {
            m.Subtrahend = EditorGUI.FloatField(new Rect(m_space.x, m_space.y * 1 + m_fieldSize.y * 0, m_fieldSize.x, m_fieldSize.y), "Subtrahend", m.Subtrahend);
        }

        if (mod.Inputs[1].Link != null)
        {
            GUI.enabled = false;
            EditorGUI.TextField(new Rect(m_space.x, m_space.y * 2 + m_fieldSize.y * 1, m_fieldSize.x, m_fieldSize.y), "Minuend", "input");
            GUI.enabled = true;
        }
        else
        {
            m.Minuend = EditorGUI.FloatField(new Rect(m_space.x, m_space.y * 2 + m_fieldSize.y * 1, m_fieldSize.x, m_fieldSize.y), "Minuend", m.Minuend);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected override Vector2 GetBodySize()
    {
        m_space = new Vector2(3, 4);

        m_fieldSize = new Vector2(132, 20);

        return new Vector2(
            m_fieldSize.x + m_space.x * 2,
            m_fieldSize.y * 2 + m_space.y * 3);
    }
}
