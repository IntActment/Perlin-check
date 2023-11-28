using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

using UnityEditor;

using UnityEngine;

public class ConstantDrawer : BaseDrawer
{
    public override string HelpMessage => m_helpMessage;

    private const string m_helpMessage = "[<color=orange>Constant</color>] provides constant float value.";

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ConstantDrawer(FormulaModConstant mod)
        : base(mod) { }

    static ConstantDrawer()
    {
        Register(typeof(FormulaModConstant), mod => new ConstantDrawer((FormulaModConstant)mod));
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

        FormulaModConstant m = (mod as FormulaModConstant);

        m.Value = EditorGUI.FloatField(new Rect(m_space.x, m_space.y * 1 + m_fieldSize.y * 0, m_fieldSize.x, m_fieldSize.y), "Value", m.Value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected override Vector2 GetBodySize()
    {
        m_space = new Vector2(3, 4);

        m_fieldSize = new Vector2(64, 20);

        return new Vector2(
            m_fieldSize.x + m_space.x * 2,
            m_fieldSize.y + m_space.y * 2);
    }
}
