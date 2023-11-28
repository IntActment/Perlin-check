using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

using UnityEngine;

public class FormulaModDivide : FormulaMod
{
#if UNITY_EDITOR
    protected override void OnLateInit()
    {
        if (Inputs.Count == 0)
        {
            AddInput("Dividend");
            AddInput("Divisor");
        }
    }
#endif

    protected override void OnEnable()
    {
        name = "Divide";
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override float Calculate()
    {
        float divisor = Inputs[1].CalculateInput();

        return Mathf.Approximately(divisor, 0)
            ? float.NaN
            : Inputs[0].CalculateInput() / divisor;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override string GenerateCode()
    {
        return $"({Inputs[0].GenerateCode()} / {Inputs[1].GenerateCode()})";
    }
}
