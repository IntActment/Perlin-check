using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

using UnityEngine;

public class FormulaModMultiply : FormulaMod
{
#if UNITY_EDITOR
    protected override void OnLateInit()
    {
        if (Inputs.Count == 0)
        {
            AddInput("Multiplicand");
            AddInput("Multiplier");
        }
    }
#endif

    protected override void OnEnable()
    {
        name = "Multiply";
    }

    private static readonly string m_varPrefix = "mul";
    public override string VarPrefix => m_varPrefix;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override float Calculate()
    {
        return Inputs[0].CalculateInput() * Inputs[1].CalculateInput();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override string GenerateCode(HashSet<int> vars, StringBuilder builder)
    {
        if (false == vars.Contains(VarIndex))
        {
            vars.Add(VarIndex);
            builder.AppendLine($"        <color=blue>float</color> {VarName} = {Inputs[0].GenerateCode(vars, builder)} * {Inputs[1].GenerateCode(vars, builder)};");
        }

        return VarName;
    }
}
