using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

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

    private static readonly string m_varPrefix = "div";
    public override string VarPrefix => m_varPrefix;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override float Calculate()
    {
        float divisor = Inputs[1].CalculateInput();

        return Mathf.Approximately(divisor, 0)
            ? float.NaN
            : Inputs[0].CalculateInput() / divisor;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override string GenerateCode(HashSet<int> vars, StringBuilder builder)
    {
        if (false == vars.Contains(VarIndex))
        {
            vars.Add(VarIndex);
            builder.AppendLine($"        <color=blue>float</color> {VarName} = {Inputs[0].GenerateCode(vars, builder)} / {Inputs[1].GenerateCode(vars, builder)};");
        }

        return VarName;
    }
}
