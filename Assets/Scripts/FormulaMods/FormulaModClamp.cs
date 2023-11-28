using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

using UnityEngine;

public class FormulaModClamp : FormulaMod
{
    [SerializeField]
    private float m_min = 0;

    public float Min
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => m_min;
#if UNITY_EDITOR
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set => this.ChangeValue(ref m_min, value);
#endif
    }

    [SerializeField]
    private float m_max = 1;

    public float Max
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => m_max;
#if UNITY_EDITOR
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set => this.ChangeValue(ref m_max, value);
#endif
    }

    protected override void OnEnable()
    {
        name = "Clamp";
    }

#if UNITY_EDITOR
    protected override void OnLateInit()
    {
        if (Inputs.Count == 0)
        {
            AddInput("Value");
        }
    }
#endif

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override float Calculate()
    {
        return Mathf.Clamp(Inputs[0].CalculateInput(), m_min, m_max);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override string GenerateCode()
    {
        return $"<color=#2b91af>Mathf</color>.<color=#74531f>Clamp</color>({Inputs[0].GenerateCode()}, {m_min}f, {m_max}f)";
    }
}
