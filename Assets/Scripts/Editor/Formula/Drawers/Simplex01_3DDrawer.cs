using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

using UnityEditor;

using UnityEngine;

public class Simplex01_3DDrawer : BaseDrawer
{
    public override string HelpMessage => m_helpMessage;

    private const string m_helpMessage = "[<color=orange>Simplex01 3D</color>] calculates simplex noise value in range [0 .. 1] based\n" +
        "  on three input slots values and multiple parameters.\n" +
        "Underlying formula will be like\n" +
        "        res = simplex01(octaves, offsetX + x * mulX, offsetY + y * mulY, offsetYZ + z * mulZ)\n" +
        "where 'x' is the first input slot parameter,\n" +
        "      'y' is the second input slot parameter,\n" +
        "      'z' is the third input slot parameter,\n" +
        "      'octaves', 'offxetX/Y/Z' and 'mulX/Y/Z' are configurable field parameters.";

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Simplex01_3DDrawer(FormulaModSimplex01_3D mod)
        : base(mod) { }

    static Simplex01_3DDrawer()
    {
        Register(typeof(FormulaModSimplex01_3D), mod => new Simplex01_3DDrawer((FormulaModSimplex01_3D)mod));
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
        Vector2 textSizeOffsetZ = GetLabelSize("Offset Z");
        Vector2 textSizeMulX    = GetLabelSize("Mul X");
        Vector2 textSizeMulY    = GetLabelSize("Mul Y");
        Vector2 textSizeMulZ    = GetLabelSize("Mul Z");
        Vector2 textSizeOctaves = GetLabelSize("Octaves");

        float maxLabelWidth = Mathf.Max(textSizeOffsetX.x, textSizeOffsetY.x, textSizeOffsetZ.x, textSizeMulX.x, textSizeMulY.x, textSizeMulZ.x, textSizeOctaves.x);

        EditorGUIUtility.labelWidth = maxLabelWidth;

        m_fieldSize = new Vector2(BodyArea.width - m_space.x * 2, m_fieldSize.y);

        var m = (mod as FormulaModSimplex01_3D);

        int fieldIndex = 0;

        if (mod.Inputs[3].Link != null)
        {
            GUI.enabled = false;
            EditorGUI.TextField(new Rect(m_space.x, m_space.y * (fieldIndex + 1) + m_fieldSize.y * fieldIndex++, m_fieldSize.x, m_fieldSize.y), "Offset X", "input");
            GUI.enabled = true;
        }
        else
        {
            m.OffsetX = EditorGUI.FloatField(new Rect(m_space.x, m_space.y * (fieldIndex + 1) + m_fieldSize.y * fieldIndex++, m_fieldSize.x, m_fieldSize.y), "Offset X", m.OffsetX);
        }

        if (mod.Inputs[4].Link != null)
        {
            GUI.enabled = false;
            EditorGUI.TextField(new Rect(m_space.x, m_space.y * (fieldIndex + 1) + m_fieldSize.y * fieldIndex++, m_fieldSize.x, m_fieldSize.y), "Offset Y", "input");
            GUI.enabled = true;
        }
        else
        {
            m.OffsetY = EditorGUI.FloatField(new Rect(m_space.x, m_space.y * (fieldIndex + 1) + m_fieldSize.y * fieldIndex++, m_fieldSize.x, m_fieldSize.y), "Offset Y", m.OffsetY);
        }

        if (mod.Inputs[5].Link != null)
        {
            GUI.enabled = false;
            EditorGUI.TextField(new Rect(m_space.x, m_space.y * (fieldIndex + 1) + m_fieldSize.y * fieldIndex++, m_fieldSize.x, m_fieldSize.y), "Offset Z", "input");
            GUI.enabled = true;
        }
        else
        {
            m.OffsetZ = EditorGUI.FloatField(new Rect(m_space.x, m_space.y * (fieldIndex + 1) + m_fieldSize.y * fieldIndex++, m_fieldSize.x, m_fieldSize.y), "Offset Z", m.OffsetZ);
        }

        if (mod.Inputs[6].Link != null)
        {
            GUI.enabled = false;
            EditorGUI.TextField(new Rect(m_space.x, m_space.y * (fieldIndex + 1) + m_fieldSize.y * fieldIndex++, m_fieldSize.x, m_fieldSize.y), "Mul X", "input");
            GUI.enabled = true;
        }
        else
        {
            m.MulX = EditorGUI.FloatField(new Rect(m_space.x, m_space.y * (fieldIndex + 1) + m_fieldSize.y * fieldIndex++, m_fieldSize.x, m_fieldSize.y), "Mul X", m.MulX);
        }

        if (mod.Inputs[7].Link != null)
        {
            GUI.enabled = false;
            EditorGUI.TextField(new Rect(m_space.x, m_space.y * (fieldIndex + 1) + m_fieldSize.y * fieldIndex++, m_fieldSize.x, m_fieldSize.y), "Mul Y", "input");
            GUI.enabled = true;
        }
        else
        {
            m.MulY = EditorGUI.FloatField(new Rect(m_space.x, m_space.y * (fieldIndex + 1) + m_fieldSize.y * fieldIndex++, m_fieldSize.x, m_fieldSize.y), "Mul Y", m.MulY);
        }

        if (mod.Inputs[8].Link != null)
        {
            GUI.enabled = false;
            EditorGUI.TextField(new Rect(m_space.x, m_space.y * (fieldIndex + 1) + m_fieldSize.y * fieldIndex++, m_fieldSize.x, m_fieldSize.y), "Mul Z", "input");
            GUI.enabled = true;
        }
        else
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

        return new Vector2(
            m_fieldSize.x + m_space.x * 2,
            m_fieldSize.y * 7 + m_space.y * 8);
    }
}
