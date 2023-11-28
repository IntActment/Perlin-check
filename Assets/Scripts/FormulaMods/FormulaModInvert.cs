using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

using UnityEngine;

public class FormulaModInvert : FormulaMod
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
        name = "Invert";
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override float Calculate()
    {
        var res = Inputs[0].CalculateInput();
        if (Mathf.Approximately(0, res))
        {
            return float.NaN;
        }

        return 1 / res;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override string GenerateCode()
    {
        return $"(1 / {Inputs[0].GenerateCode()})";
    }
}
