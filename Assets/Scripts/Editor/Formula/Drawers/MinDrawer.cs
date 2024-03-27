using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

using UnityEditor;

using UnityEngine;

public class MinDrawer : BaseDrawer
{
    public override string HelpMessage => mod.Inputs[1].Link == null
        ? m_helpMessage1
        : m_helpMessage2;

    private const string m_helpMessage1 = "[<color=orange>min</color>] returns minimum value.\n" +
        "Underlying formula will be like\n" +
        "        res = min(a, b)\n" +
        "where 'a' is input slot parameter and\n" +
        "      'b' is configurable field parameter.";

    private const string m_helpMessage2 = "[<color=orange>min</color>] returns minimum value.\n" +
        "Underlying formula will be like\n" +
        "        res = min(a, b)\n" +
        "where 'a' is the first input slot parameter and\n" +
        "      'b' is the second input slot parameter.";

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public MinDrawer(FormulaModMin mod)
        : base(mod) { }

    static MinDrawer()
    {
        Register(typeof(FormulaModMin), mod => new MinDrawer((FormulaModMin)mod));
    }

    private Vector2 m_space;
    private Vector2 m_fieldSize;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected override void DrawBodyGUI(InputState state)
    {
        base.DrawBodyGUI(state);

        if (false == mod.IsInitialized || mod.IsLoading)
        {
            // asset not ready yet
            return;
        }

        if (mod.Inputs[1].Link != null)
        {
            return;
        }

        Vector2 textSizeValue = GetLabelSize("Value");

        float maxLabelWidth = textSizeValue.x;

        EditorGUIUtility.labelWidth = maxLabelWidth;

        m_fieldSize = new Vector2(BodyArea.width - m_space.x * 2, m_fieldSize.y);

        FormulaModMin m = (mod as FormulaModMin);

        m.Value2 = EditorGUI.FloatField(new Rect(m_space.x, m_space.y * 1 + m_fieldSize.y * 0, m_fieldSize.x, m_fieldSize.y), "Value", m.Value2);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected override Vector2 GetBodySize()
    {
        if (mod.Inputs[1].Link == null)
        {
            m_space = new Vector2(3, 4);

            m_fieldSize = new Vector2(104, 20);

            return new Vector2(
                m_fieldSize.x + m_space.x * 2,
                m_fieldSize.y + m_space.y * 2);
        }

        return base.GetBodySize();
    }
}
