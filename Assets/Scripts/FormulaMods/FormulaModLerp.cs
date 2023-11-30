using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

using UnityEngine;

public class FormulaModLerp : FormulaMod
{

#if UNITY_EDITOR
    protected override void Initialize()
    {
        name = "Lerp";

        AddInput("Value");
        AddInput("Min");
        AddInput("Max");
    }
#endif

    private static readonly string m_varPrefix = "lerp";
    public override string VarPrefix => m_varPrefix;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override float Calculate()
    {
        return Mathf.LerpUnclamped(Inputs[1].CalculateInput(), Inputs[2].CalculateInput(), Inputs[0].CalculateInput());
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override string GenerateCode(HashSet<int> vars, StringBuilder builder)
    {
        if (false == vars.Contains(VarIndex))
        {
            vars.Add(VarIndex);

            var val0 = Inputs[0].GenerateCode(vars, builder);
            var val1 = Inputs[1].GenerateCode(vars, builder);
            var val2 = Inputs[2].GenerateCode(vars, builder);

            builder.AppendLine($"        <color=blue>float</color> {VarName} = <color=#2b91af>Mathf</color>.<color=#74531f>Lerp</color>({val1}, {val2}, {val0});");
        }

        return VarName;
    }
}
