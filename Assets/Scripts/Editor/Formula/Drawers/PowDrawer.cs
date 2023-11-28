using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

using UnityEditor;

using UnityEngine;

public class PowDrawer : BaseDrawer
{
    public override string HelpMessage => m_helpMessage;

    private const string m_helpMessage = "[<color=orange>Pow</color>] raises the input slot value to a power.\n" +
        "Underlying formula will be like\n" +
        "        res = pow(x, power)\n" +
        "where 'x' is input slot parameter and\n" +
        "      'power' is configurable field parameter.";

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public PowDrawer(FormulaModPow mod)
        : base(mod) { }

    static PowDrawer()
    {
        Register(typeof(FormulaModPow), mod => new PowDrawer((FormulaModPow)mod));
    }

    private Vector2 m_space;
    private Vector2 m_fieldSize;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected override void DrawBodyGUI(ref bool down, ref bool up)
    {
        base.DrawBodyGUI(ref down, ref up);

        Vector2 textSizeValue = GetLabelSize("Power");

        float maxLabelWidth = textSizeValue.x;

        EditorGUIUtility.labelWidth = maxLabelWidth;

        m_fieldSize = new Vector2(BodyArea.width - m_space.x * 2, m_fieldSize.y);

        FormulaModPow m = (mod as FormulaModPow);

        m.Power = EditorGUI.Slider(new Rect(m_space.x, m_space.y * 1 + m_fieldSize.y * 0, m_fieldSize.x, m_fieldSize.y), "Power", m.Power, 0, 10);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected override Vector2 GetBodySize()
    {
        m_space = new Vector2(3, 4);

        m_fieldSize = new Vector2(164, 20);

        return new Vector2(
            m_fieldSize.x + m_space.x * 2,
            m_fieldSize.y + m_space.y * 2);
    }
}
