using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

using UnityEditor;

using UnityEngine;

public class Simplex01Drawer : BaseDrawer
{
    public override string HelpMessage => mod.Inputs[2].Link == null
        ? m_helpMessage1
        : m_helpMessage2;

    private const string m_helpMessage1 = "[<color=orange>Simplex01</color>] calculates simplex noise value in range [0 .. 1] based\n" +
        "  on two input slots values and multiple parameters.\n" +
        "Underlying formula will be like\n" +
        "        res = simplex01(octaves, offsetX + x * mulX, offsetY + y * mulY)\n" +
        "where 'x' is the first input slot parameter,\n" +
        "      'y' is the second input slot parameter,\n" +
        "      'octaves', 'offxetX/Y' and 'mulX/Y' are configurable field parameters.";

    private const string m_helpMessage2 = "[<color=orange>Simplex01</color>] calculates simplex noise value in range [0 .. 1] based\n" +
        "  on three input slots values and multiple parameters.\n" +
        "Underlying formula will be like\n" +
        "        res = simplex01(octaves, offsetX + x * mulX, offsetY + y * mulY, offsetYZ + z * mulZ)\n" +
        "where 'x' is the first input slot parameter,\n" +
        "      'y' is the second input slot parameter,\n" +
        "      'z' is the third input slot parameter,\n" +
        "      'octaves', 'offxetX/Y/Z' and 'mulX/Y/Z' are configurable field parameters.";

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
    protected override void DrawBodyGUI(InputState state)
    {
        base.DrawBodyGUI(state);

        if (false == mod.IsInitialized)
        {
            // asset not ready yet
            return;
        }

        Vector2 textSizeOffsetX = GetLabelSize("Offset X");
        Vector2 textSizeOffsetY = GetLabelSize("Offset Y");
        Vector2 textSizeMulX = GetLabelSize("Mul X");
        Vector2 textSizeMulY = GetLabelSize("Mul Y");
        Vector2 textSizeOctaves = GetLabelSize("Octaves");

        float maxLabelWidth = Mathf.Max(textSizeOffsetX.x, textSizeOffsetY.x, textSizeMulX.x, textSizeMulY.x, textSizeOctaves.x);

        EditorGUIUtility.labelWidth = maxLabelWidth;

        m_fieldSize = new Vector2(BodyArea.width - m_space.x * 2, m_fieldSize.y);

        FormulaModSimplex01 m = (mod as FormulaModSimplex01);

        int fieldIndex = 0;

        m.OffsetX = EditorGUI.FloatField(new Rect(m_space.x, m_space.y * (fieldIndex + 1) + m_fieldSize.y * fieldIndex++, m_fieldSize.x, m_fieldSize.y), "Offset X", m.OffsetX);
        m.OffsetY = EditorGUI.FloatField(new Rect(m_space.x, m_space.y * (fieldIndex + 1) + m_fieldSize.y * fieldIndex++, m_fieldSize.x, m_fieldSize.y), "Offset Y", m.OffsetY);

        if (mod.Inputs[2].Link != null)
        {
            m.OffsetZ = EditorGUI.FloatField(new Rect(m_space.x, m_space.y * (fieldIndex + 1) + m_fieldSize.y * fieldIndex++, m_fieldSize.x, m_fieldSize.y), "Offset Z", m.OffsetZ);
        }
        m.MulX = EditorGUI.FloatField(new Rect(m_space.x, m_space.y * (fieldIndex + 1) + m_fieldSize.y * fieldIndex++, m_fieldSize.x, m_fieldSize.y), "Mul X", m.MulX);
        m.MulY = EditorGUI.FloatField(new Rect(m_space.x, m_space.y * (fieldIndex + 1) + m_fieldSize.y * fieldIndex++, m_fieldSize.x, m_fieldSize.y), "Mul Y", m.MulY);

        if (mod.Inputs[2].Link != null)
        {
            m.MulZ = EditorGUI.FloatField(new Rect(m_space.x, m_space.y * (fieldIndex + 1) + m_fieldSize.y * fieldIndex++, m_fieldSize.x, m_fieldSize.y), "Mul Z", m.MulZ);
        }

        m.Octaves = EditorGUI.IntSlider(new Rect(m_space.x, m_space.y * (fieldIndex + 1) + m_fieldSize.y * fieldIndex++, m_fieldSize.x, m_fieldSize.y), "Octaves", m.Octaves, 1, 10);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected override Vector2 GetBodySize()
    {
        m_space = new Vector2(3, 4);

        m_fieldSize = new Vector2(164, 20);

        if (mod.Inputs[2].Link == null)
        {
            return new Vector2(
                m_fieldSize.x + m_space.x * 2,
                m_fieldSize.y * 5 + m_space.y * 6);
        }
        else
        {
            return new Vector2(
                m_fieldSize.x + m_space.x * 2,
                m_fieldSize.y * 7 + m_space.y * 8);
        }
    }
}
