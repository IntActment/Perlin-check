using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

using UnityEngine;

public class FormulaModSqrt : FormulaMod
{
#if UNITY_EDITOR
    protected override void OnLateInit()
    {
        base.OnLateInit();

        if (Inputs.Count == 0)
        {
            AddInput("Value");
        }
    }
#endif

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override float Calculate()
    {
        var value = Inputs[0].CalculateInput();
        return value < 0
            ? float.NaN
            : Mathf.Sqrt(value);
    }
}
