using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

using UnityEditor;

using UnityEngine;

public class Simplex01Drawer : BaseDrawer
{
    public override string HelpMessage => m_helpMessage;

    private const string m_helpMessage = "[<color=orange>Simplex01</color>] calculates simplex noise value in range [0 .. 1] based\n" +
        "  on two input slots value and multiple parameters.\n" +
        "Underlying formula will be like\n" +
        "        res = simplex01(offsetX + x * mulX, offsetY + y * mulY)\n" +
        "where 'x' is the first input slot parameter,\n" +
        "      'y' is the second input slot parameter,\n" +
        "      'offxetX/Y' and 'mulX/Y' are configurable field parameters.";

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Simplex01Drawer(FormulaModSimplex01 mod)
        : base(mod) { }

    static Simplex01Drawer()
    {
        Register(typeof(FormulaModSimplex01), mod => new Simplex01Drawer((FormulaModSimplex01)mod));
    }

    private Vector2 m_space;
    private Vector2 m_fieldSize;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected override void DrawBodyGUI(ref bool down, ref bool up)
    {
        base.DrawBodyGUI(ref down, ref up);

        Vector2 textSizeOffsetX = GetLabelSize("Offset X");
        Vector2 textSizeOffsetY = GetLabelSize("Offset Y");
        Vector2 textSizeMulX = GetLabelSize("Mul X");
        Vector2 textSizeMulY = GetLabelSize("Mul Y");
        Vector2 textSizeOctaves = GetLabelSize("Octaves");

        float maxLabelWidth = Mathf.Max(textSizeOffsetX.x, textSizeOffsetY.x, textSizeMulX.x, textSizeMulY.x, textSizeOctaves.x);

        EditorGUIUtility.labelWidth = maxLabelWidth;

        m_fieldSize = new Vector2(BodyArea.width - m_space.x * 2, m_fieldSize.y);

        FormulaModSimplex01 m = (mod as FormulaModSimplex01);

        m.OffsetX = EditorGUI.FloatField(new Rect(m_space.x, m_space.y * 1 + m_fieldSize.y * 0, m_fieldSize.x, m_fieldSize.y), "Offset X", m.OffsetX);
        m.OffsetY = EditorGUI.FloatField(new Rect(m_space.x, m_space.y * 2 + m_fieldSize.y * 1, m_fieldSize.x, m_fieldSize.y), "Offset Y", m.OffsetY);
        m.MulX    = EditorGUI.FloatField(new Rect(m_space.x, m_space.y * 3 + m_fieldSize.y * 2, m_fieldSize.x, m_fieldSize.y), "Mul X"   , m.MulX);
        m.MulY    = EditorGUI.FloatField(new Rect(m_space.x, m_space.y * 4 + m_fieldSize.y * 3, m_fieldSize.x, m_fieldSize.y), "Mul Y"   , m.MulY);

        m.Octaves = EditorGUI.IntSlider(new Rect(m_space.x, m_space.y * 5 + m_fieldSize.y * 4, m_fieldSize.x, m_fieldSize.y), "Octaves", m.Octaves, 1, 10);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected override Vector2 GetBodySize()
    {
        m_space = new Vector2(3, 4);

        m_fieldSize = new Vector2(164, 20);

        return new Vector2(
            m_fieldSize.x + m_space.x * 2,
            m_fieldSize.y * 5 + m_space.y * 6);
    }
}
