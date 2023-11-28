using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

using UnityEngine;

public class FormulaModLerp : FormulaMod
{

#if UNITY_EDITOR
    protected override void OnLateInit()
    {
        base.OnLateInit();

        if (Inputs.Count == 0)
        {
            AddInput("Value");
            AddInput("Min");
            AddInput("Max");
        }
    }
#endif

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override float Calculate()
    {
        return Mathf.LerpUnclamped(Inputs[1].CalculateInput(), Inputs[2].CalculateInput(), Inputs[0].CalculateInput());
    }
}
