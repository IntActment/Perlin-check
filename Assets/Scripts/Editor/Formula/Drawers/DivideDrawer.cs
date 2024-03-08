using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

using UnityEditor;

using UnityEngine;

public class DivideDrawer : BaseDrawer
{
    public override string HelpMessage => m_helpMessage;

    private const string m_helpMessage = "[<color=orange>a / b</color>] divides the first input slot value by the second input slot value.\n" +
        "Underlying formula will be like\n" +
        "        res = a / b\n" +
        "where 'a' is the first input slot parameter and\n" +
        "      'b' is the second input slot parameter.\n" +
        "If 'b' is zero, it returns PositiveInfinity.";

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public DivideDrawer(FormulaModDivide mod)
        : base(mod) { }

    static DivideDrawer()
    {
        Register(typeof(FormulaModDivide), mod => new DivideDrawer((FormulaModDivide)mod));
    }

    private Vector2 m_space;
    private Vector2 m_fieldSize;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected override void DrawBodyGUI(InputState state)
    {
        base.DrawBodyGUI(state);

        Vector2 textSizeValueMin = GetLabelSize("Dividend");
        Vector2 textSizeValueMax = GetLabelSize("Divisor");

        float maxLabelWidth = Mathf.Max(textSizeValueMin.x, textSizeValueMax.x);

        EditorGUIUtility.labelWidth = maxLabelWidth;

        m_fieldSize = new Vector2(BodyArea.width - m_space.x * 2, m_fieldSize.y);

        var m = (mod as FormulaModDivide);

        if (mod.Inputs[0].Link != null)
        {
            GUI.enabled = false;
            EditorGUI.TextField(new Rect(m_space.x, m_space.y * 1 + m_fieldSize.y * 0, m_fieldSize.x, m_fieldSize.y), "Dividend", "input");
            GUI.enabled = true;
        }
        else
        {
            m.Dividend = EditorGUI.FloatField(new Rect(m_space.x, m_space.y * 1 + m_fieldSize.y * 0, m_fieldSize.x, m_fieldSize.y), "Dividend", m.Dividend);
        }

        if (mod.Inputs[1].Link != null)
        {
            GUI.enabled = false;
            EditorGUI.TextField(new Rect(m_space.x, m_space.y * 2 + m_fieldSize.y * 1, m_fieldSize.x, m_fieldSize.y), "Divisor", "input");
            GUI.enabled = true;
        }
        else
        {
            m.Divisor = EditorGUI.FloatField(new Rect(m_space.x, m_space.y * 2 + m_fieldSize.y * 1, m_fieldSize.x, m_fieldSize.y), "Divisor", m.Divisor);
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
