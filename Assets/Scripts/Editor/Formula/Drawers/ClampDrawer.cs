using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

using UnityEditor;

using UnityEngine;

public class ClampDrawer : BaseDrawer
{
    public override string HelpMessage => m_helpMessage;

    private const string m_helpMessage = "[<color=orange>Clamp</color>] lets you limit input slot value by the range.\n" +
        "Underlying formula will be like\n" +
        "        res = min(b, max(a, x))\n" +
        "where 'x' is the first input slot parameter,\n" +
        "      'a' and 'b' are configurable field parameters.";

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ClampDrawer(FormulaModClamp mod)
        : base(mod) { }

    static ClampDrawer()
    {
        Register(typeof(FormulaModClamp), mod => new ClampDrawer((FormulaModClamp)mod));
    }

    private Vector2 m_space;
    private Vector2 m_fieldSize;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected override void DrawBodyGUI(ref bool down, ref bool up)
    {
        base.DrawBodyGUI(ref down, ref up);

        Vector2 textSizeValue = GetLabelSize("Value");

        float maxLabelWidth = textSizeValue.x;

        EditorGUIUtility.labelWidth = maxLabelWidth;

        m_fieldSize = new Vector2(BodyArea.width - m_space.x * 2, m_fieldSize.y);

        FormulaModClamp m = (mod as FormulaModClamp);

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
