using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

using UnityEngine;

public class FormulaModInvert : FormulaMod
{
#if UNITY_EDITOR
    protected override void Initialize()
    {
        AddInput("Value");
    }
#endif

    protected override void OnEnable()
    {
        name = "Invert";
    }

    private static readonly string m_varPrefix = "inv";
    public override string VarPrefix => m_varPrefix;

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
    public override string GenerateCode(HashSet<int> vars, StringBuilder builder)
    {
        if (false == vars.Contains(VarIndex))
        {
            vars.Add(VarIndex);

            var val0 = Inputs[0].GenerateCode(vars, builder);

            builder.AppendLine($"        <color=blue>float</color> {VarName} = 1f / {val0};");
        }

        return VarName;
    }
}