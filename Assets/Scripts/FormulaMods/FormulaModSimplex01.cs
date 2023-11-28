using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

using UnityEngine;

public class FormulaModSimplex01 : FormulaMod
{
    [SerializeField]
    private float m_offsetX;

    [SerializeField]
    private float m_offsetY;

    [SerializeField]
    private float m_mulX = 1;

    [SerializeField]
    private float m_mulY = 1;

    [SerializeField]
    private int m_octaves = 1;

    public float OffsetX
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => m_offsetX;
#if UNITY_EDITOR
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set => this.ChangeValue(ref m_offsetX, value);
#endif
    }

    public float OffsetY
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => m_offsetY;
#if UNITY_EDITOR
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set => this.ChangeValue(ref m_offsetY, value);
#endif
    }

    public float MulX
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => m_mulX;
#if UNITY_EDITOR
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set => this.ChangeValue(ref m_mulX, value);
#endif
    }

    public float MulY
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => m_mulY;
#if UNITY_EDITOR
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set => this.ChangeValue(ref m_mulY, value);
#endif
    }

    public int Octaves
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => m_octaves;
#if UNITY_EDITOR
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set => this.ChangeValue(ref m_octaves, value);
#endif
    }

#if UNITY_EDITOR
    protected override void OnLateInit()
    {
        if (Inputs.Count == 0)
        {
            AddInput("X");
            AddInput("Y");
        }
    }
#endif

    protected override void OnEnable()
    {
        name = "Simplex01";
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override float Calculate()
    {
        return SimplexNoise.Singleton.MultiNoise01(
            m_octaves,
            m_offsetX + Inputs[0].CalculateInput() * m_mulX,
            m_offsetY + Inputs[1].CalculateInput() * m_mulY);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override string GenerateCode()
    {
        return $"<color=#2b91af>SimplexNoise</color>.Singleton.<color=#74531f>MultiNoise01</color>({m_octaves}, {m_offsetX}f + {Inputs[0].GenerateCode()} * {m_mulX}f, {m_offsetY}f + {Inputs[1].GenerateCode()} * {m_mulY}f)";
    }
}
