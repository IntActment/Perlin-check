using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

using UnityEngine;

public class FormulaModPow : FormulaMod
{
    [SerializeField]
    private int m_power = 2;

    public int Power
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => m_power;
#if UNITY_EDITOR
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set => this.ChangeValue(ref m_power, Mathf.Max(0, value));
#endif
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

    protected override void OnEnable()
    {
        name = "Pow";
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override float Calculate()
    {
        return Mathf.Pow(Inputs[0].CalculateInput(), m_power);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override string GenerateCode()
    {
        return $"<color=#2b91af>Mathf</color>.<color=#74531f>Pow</color>({Inputs[0].GenerateCode()}, {Power})";
    }
}
