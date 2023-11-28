using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

using UnityEngine;

public class FormulaModMultiply : FormulaMod
{
#if UNITY_EDITOR
    protected override void OnLateInit()
    {
        base.OnLateInit();

        if (Inputs.Count == 0)
        {
            AddInput("Multiplicand");
            AddInput("Multiplier");
        }
    }
#endif

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override float Calculate()
    {
        return Inputs[0].CalculateInput() * Inputs[1].CalculateInput();
    }
}
