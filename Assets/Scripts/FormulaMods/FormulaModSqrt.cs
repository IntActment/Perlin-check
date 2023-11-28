using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

using UnityEngine;

public class FormulaModSqrt : FormulaMod
{
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
        name = "Sqrt";
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override float Calculate()
    {
        var value = Inputs[0].CalculateInput();
        return value < 0
            ? float.NaN
            : Mathf.Sqrt(value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override string GenerateCode()
    {
        return $"<color=#2b91af>Mathf</color>.<color=#74531f>Sqrt</color>({Inputs[0].GenerateCode()})";
    }
}
