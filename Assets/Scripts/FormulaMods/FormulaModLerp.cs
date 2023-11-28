using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

using UnityEngine;

public class FormulaModLerp : FormulaMod
{

#if UNITY_EDITOR
    protected override void OnLateInit()
    {
        if (Inputs.Count == 0)
        {
            AddInput("Value");
            AddInput("Min");
            AddInput("Max");
        }
    }
#endif

    protected override void OnEnable()
    {
        name = "Lerp";
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override float Calculate()
    {
        return Mathf.LerpUnclamped(Inputs[1].CalculateInput(), Inputs[2].CalculateInput(), Inputs[0].CalculateInput());
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override string GenerateCode()
    {
        return $"<color=#2b91af>Mathf</color>.<color=#74531f>Lerp</color>({Inputs[1].GenerateCode()}, {Inputs[2].GenerateCode()}, {Inputs[0].GenerateCode()})";
    }
}
