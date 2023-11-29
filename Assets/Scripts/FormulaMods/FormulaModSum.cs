using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

using UnityEngine;

public class FormulaModSum : FormulaMod
{
#if UNITY_EDITOR
    protected override void Initialize()
    {
        AddInput("Augend");
        AddInput("Addend");
    }
#endif

    protected override void OnEnable()
    {
        name = "Sum";
    }

    private static readonly string m_varPrefix = "sum";
    public override string VarPrefix => m_varPrefix;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override float Calculate()
    {
        return Inputs[0].CalculateInput() + Inputs[1].CalculateInput();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override string GenerateCode(HashSet<int> vars, StringBuilder builder)
    {
        if (false == vars.Contains(VarIndex))
        {
            vars.Add(VarIndex);

            var val0 = Inputs[0].GenerateCode(vars, builder);
            var val1 = Inputs[1].GenerateCode(vars, builder);

            builder.AppendLine($"        <color=blue>float</color> {VarName} = {val0} + {val1};");
        }

        return VarName;
    }
}
