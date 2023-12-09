using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

using UnityEditor;

using UnityEngine;

public class PowDrawer : BaseDrawer
{
    public override string HelpMessage => mod.Inputs[1].Link == null
        ? m_helpMessage1
        : m_helpMessage2;

    private const string m_helpMessage1 = "[<color=orange>a²</color>] raises the input slot value to a power.\n" +
        "Underlying formula will be like\n" +
        "        res = pow(a, power)\n" +
        "where 'a' is input slot parameter and\n" +
        "      'power' is configurable field parameter.";

    private const string m_helpMessage2 = "[<color=orange>a²</color>] raises the input slot value to a power.\n" +
        "Underlying formula will be like\n" +
        "        res = pow(a, power)\n" +
        "where 'a' is the first input slot parameter and\n" +
        "      'power' is the second input slot parameter.";

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
    protected override void DrawBodyGUI(InputState state)
    {
        base.DrawBodyGUI(state);

        if (false == mod.IsInitialized)
        {
            // asset not ready yet
            return;
        }

        if (mod.Inputs[1].Link != null)
        {
            return;
        }

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
        if (mod.Inputs[1].Link == null)
        {
            m_space = new Vector2(3, 4);

            m_fieldSize = new Vector2(164, 20);

            return new Vector2(
                m_fieldSize.x + m_space.x * 2,
                m_fieldSize.y + m_space.y * 2);
        }

        return base.GetBodySize();
    }
}
