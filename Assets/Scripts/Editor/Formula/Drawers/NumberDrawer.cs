using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

using UnityEditor;

using UnityEngine;

public class NumberDrawer : BaseDrawer
{
    public override string HelpMessage => m_helpMessage;

    private const string m_helpMessage = "[<color=orange>Number</color>] provides constant float value.";

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public NumberDrawer(FormulaModNumber mod)
        : base(mod) { }

    static NumberDrawer()
    {
        Register(typeof(FormulaModNumber), mod => new NumberDrawer((FormulaModNumber)mod));
    }

    private Vector2 m_space;
    private Vector2 m_fieldSize;

    public override bool HasInput { get; } = false;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected override void DrawBodyGUI(ref bool down, ref bool up)
    {
        base.DrawBodyGUI(ref down, ref up);

        Vector2 textSizeValue = GetLabelSize("Value");

        float maxLabelWidth = textSizeValue.x;

        EditorGUIUtility.labelWidth = maxLabelWidth;

        m_fieldSize = new Vector2(BodyArea.width - m_space.x * 2, m_fieldSize.y);

        FormulaModNumber m = (mod as FormulaModNumber);

        m.Value = EditorGUI.FloatField(new Rect(m_space.x, m_space.y * 1 + m_fieldSize.y * 0, m_fieldSize.x, m_fieldSize.y), "Value", m.Value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected override Vector2 GetBodySize()
    {
        m_space = new Vector2(3, 4);

        m_fieldSize = new Vector2(84, 20);

        return new Vector2(
            m_fieldSize.x + m_space.x * 2,
            m_fieldSize.y + m_space.y * 2);
    }
}
