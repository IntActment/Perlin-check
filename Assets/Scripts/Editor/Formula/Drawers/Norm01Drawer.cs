using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

using UnityEditor;

using UnityEngine;

public class Norm01Drawer : BaseDrawer
{
    public override string HelpMessage => m_helpMessage;

    private const string m_helpMessage = "[<color=orange>Norm01</color>] normalizes the first input slot value to the provided range\n" +
        "  and returns its representation in the [0 .. 1] range.\n" +
        "Underlying formula will be like\n" +
        "        res = (a - min) / (max - min)\n" +
        "where 'a' is input slot parameter and\n" +
        "      'min/max' are configurable field parameters.";

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Norm01Drawer(FormulaModNorm01 mod)
        : base(mod) { }

    static Norm01Drawer()
    {
        Register(typeof(FormulaModNorm01), mod => new Norm01Drawer((FormulaModNorm01)mod));
    }

    private Vector2 m_space;
    private Vector2 m_fieldSize;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected override void DrawBodyGUI(InputState state)
    {
        base.DrawBodyGUI(state);

        Vector2 textSizeValue = GetLabelSize("Value");

        float maxLabelWidth = textSizeValue.x;

        EditorGUIUtility.labelWidth = maxLabelWidth;

        m_fieldSize = new Vector2(BodyArea.width - m_space.x * 2, m_fieldSize.y);

        FormulaModNorm01 m = (mod as FormulaModNorm01);

        m.Min = EditorGUI.FloatField(new Rect(m_space.x, m_space.y * 1 + m_fieldSize.y * 0, m_fieldSize.x, m_fieldSize.y), "Min", m.Min);
        m.Max = EditorGUI.FloatField(new Rect(m_space.x, m_space.y * 2 + m_fieldSize.y * 1, m_fieldSize.x, m_fieldSize.y), "Max", m.Max);
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
