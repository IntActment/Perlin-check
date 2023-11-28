using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

using UnityEngine;

public class FormulaModOutput : FormulaMod
{
#if UNITY_EDITOR
    public override bool IsRemovable { get; } = false;

    protected override void OnLateInit()
    {
        if (Inputs.Count == 0)
        {
            AddInput("Result");
        }
    }
#endif

    protected override void OnEnable()
    {
        name = "[Out]";
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override float Calculate()
    {
        return Inputs[0].CalculateInput();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override string GenerateCode()
    {
        return Inputs[0].GenerateCode();
    }
}
